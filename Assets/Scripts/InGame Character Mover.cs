using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class InGameCharacterMover : NetworkBehaviour
{
    private Animator animator;
    public bool isMoveable;

    [SyncVar]
    public float speed = 2f;

    public GameObject dotPrefab;

    [SyncVar(hook = nameof(OnDirectionChanged))]
    private Vector3 lastMoveDir = Vector3.right;

    private float nextAttackTime = 0f; // 다음 공격 시간 기반 쿨다운

    [SyncVar(hook = nameof(OnHPChanged))]
    private int hp = 100;

    [SerializeField]
    private Text hpText; // HP 텍스트 변수 (Inspector에서 할당하거나 자식 오브젝트에서 자동 할당)

    [SerializeField]
    private GameObject pauseOverlay; // PauseOverlay 참조

    // 게임 일시정지 상태를 관리하는 정적 변수
    private static bool isGamePaused = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (isLocalPlayer)
        {
            // 로컬 플레이어의 hpText 설정
            hpText = GetComponentInChildren<Text>();
            if (hpText != null)
            {
                hpText.text = hp.ToString();
            }
            else
            {
                Debug.LogError("hpText가 할당되지 않았습니다: " + gameObject.name);
            }

            // 로컬 플레이어의 카메라 설정
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = 2.5f;
        }
        else
        {
            // 다른 플레이어의 hpText 비활성화
            hpText = GetComponentInChildren<Text>();
            if (hpText != null)
            {
                hpText.gameObject.SetActive(false);
            }
        }

        // PauseOverlay 초기 상태 설정
        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(false);
        }
        else
        {
            if (isLocalPlayer)
            {
                Debug.LogError("PauseOverlay가 할당되지 않았습니다: " + gameObject.name);
            }
        }
    }

    void FixedUpdate()
    {
        if (isLocalPlayer && !isGamePaused)
        {
            Move();
            HandleDotFire();
        }
    }

    void Update()
    {
        if (isGamePaused && isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키로 게임 재개
            {
                CmdResumeGame();
            }
        }
    }

    public void Move()
    {
        if (isMoveable)
        {
            bool isMove = false;

            // WASD 키 입력에 따라 이동 방향 설정
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir.y += 1f;
            if (Input.GetKey(KeyCode.S)) dir.y -= 1f;
            if (Input.GetKey(KeyCode.A)) dir.x -= 1f;
            if (Input.GetKey(KeyCode.D)) dir.x += 1f;

            dir = Vector3.ClampMagnitude(dir, 1f); // 대각선 이동 속도 일정 유지

            if (dir != Vector3.zero)
            {
                // 서버로 방향 업데이트 요청
                CmdUpdateDirection(dir);
            }

            transform.position += dir * speed * Time.deltaTime;
            isMove = dir.magnitude != 0f;
            animator.SetBool("isMove", isMove);
        }
    }

    [Command] // 서버에서 방향 업데이트 처리
    private void CmdUpdateDirection(Vector3 dir)
    {
        lastMoveDir = dir; // SyncVar 업데이트
    }

    private void OnDirectionChanged(Vector3 oldDir, Vector3 newDir)
    {
        if (newDir.x < 0f)
            transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
        else if (newDir.x > 0f)
            transform.localScale = new Vector3(0.3f, 0.3f, 1f);
    }

    private void HandleDotFire()
    {
        // Right Shift로만 발사
        if (Input.GetKeyDown(KeyCode.RightShift) && Time.time >= nextAttackTime && !isGamePaused)
        {
            FireDot();
            nextAttackTime = Time.time + 1f / GameData.Instance.attackSpeed; // 공격 속도에 따른 쿨다운 설정
        }
    }

    private void FireDot()
    {
        CmdFireDot();
    }

    [Command]
    private void CmdFireDot()
    {
        if (dotPrefab == null)
        {
            Debug.LogError("Dot Prefab이 할당되지 않았습니다.");
            return;
        }

        GameObject dot = Instantiate(dotPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(dot, connectionToClient); // 네트워크에 도트 스폰

        Dot dotScript = dot.GetComponent<Dot>();
        if (dotScript != null)
        {
            dotScript.SetStats(10f, GameData.Instance.attackPower, lastMoveDir, GameData.Instance.attackRange, netIdentity);
        }
        else
        {
            Debug.LogError("Dot prefab에 Dot 스크립트가 없습니다.");
        }
    }

    [Server]
    public void TakeDamage(int damage)
    {
        if (hp <= 0) return; // 이미 사망한 경우 무시
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            // 플레이어 사망 처리 (예: 사망 애니메이션, 리스폰 등)
            Debug.Log($"{gameObject.name}이(가) 사망했습니다.");

            // 게임 일시정지 요청
            if (!isGamePaused)
            {
                isGamePaused = true;
                RpcPauseGame();
            }
        }
        Debug.Log($"{gameObject.name}이(가) 피해를 입었습니다. 현재 HP: {hp}");
        // SyncVar가 자동으로 hp를 동기화하고 OnHPChanged를 호출함
    }

    private void OnHPChanged(int oldHP, int newHP)
    {
        Debug.Log($"OnHPChanged가 호출되었습니다: {gameObject.name}, 이전 HP: {oldHP}, 새로운 HP: {newHP}");
        if (isLocalPlayer)
        {
            UpdateHPText(newHP);
        }

        // HP가 0으로 변경되면 추가적인 로직을 원할 경우 여기에 추가
    }

    private void UpdateHPText(int currentHP)
    {
        if (hpText != null)
        {
            hpText.text = currentHP.ToString(); // HP 값을 업데이트
            Debug.Log($"{gameObject.name}의 HP 텍스트가 {currentHP}로 업데이트되었습니다.");
        }
        else
        {
            Debug.LogError("hpText가 null입니다: " + gameObject.name);
        }
    }

    // 모든 클라이언트에게 게임을 일시정지하도록 명령
    [ClientRpc]
    private void RpcPauseGame()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            Time.timeScale = 0f; // 게임 시간 정지
            Debug.Log("게임이 일시정지되었습니다.");

            if (pauseOverlay != null)
            {
                pauseOverlay.SetActive(true); // 오버레이 활성화
            }
            else
            {
                Debug.LogError("PauseOverlay가 할당되지 않았습니다.");
            }

            // 일시정지 UI를 표시하거나 추가적인 처리 로직을 여기에 추가할 수 있습니다.
            // 예를 들어, PauseMenu를 활성화하는 코드:
            // PauseMenu.Instance.Show();
        }
    }

    [Command]
    private void CmdResumeGame()
    {
        if (isGamePaused)
        {
            isGamePaused = false;
            RpcResumeGame();
        }
    }

    [ClientRpc]
    private void RpcResumeGame()
    {
        Time.timeScale = 1f; // 게임 시간 재개
        Debug.Log("게임이 재개되었습니다.");

        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(false); // 오버레이 비활성화
        }
    }
}
