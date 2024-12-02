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

    private float nextAttackTime = 0f; // ���� ���� �ð� ��� ��ٿ�

    [SyncVar(hook = nameof(OnHPChanged))]
    private int hp = 100;

    [SerializeField]
    private Text hpText; // HP �ؽ�Ʈ ���� (Inspector���� �Ҵ��ϰų� �ڽ� ������Ʈ���� �ڵ� �Ҵ�)

    [SerializeField]
    private GameObject pauseOverlay; // PauseOverlay ����

    // ���� �Ͻ����� ���¸� �����ϴ� ���� ����
    private static bool isGamePaused = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (isLocalPlayer)
        {
            // ���� �÷��̾��� hpText ����
            hpText = GetComponentInChildren<Text>();
            if (hpText != null)
            {
                hpText.text = hp.ToString();
            }
            else
            {
                Debug.LogError("hpText�� �Ҵ���� �ʾҽ��ϴ�: " + gameObject.name);
            }

            // ���� �÷��̾��� ī�޶� ����
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = 2.5f;
        }
        else
        {
            // �ٸ� �÷��̾��� hpText ��Ȱ��ȭ
            hpText = GetComponentInChildren<Text>();
            if (hpText != null)
            {
                hpText.gameObject.SetActive(false);
            }
        }

        // PauseOverlay �ʱ� ���� ����
        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(false);
        }
        else
        {
            if (isLocalPlayer)
            {
                Debug.LogError("PauseOverlay�� �Ҵ���� �ʾҽ��ϴ�: " + gameObject.name);
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
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ���� �簳
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

            // WASD Ű �Է¿� ���� �̵� ���� ����
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir.y += 1f;
            if (Input.GetKey(KeyCode.S)) dir.y -= 1f;
            if (Input.GetKey(KeyCode.A)) dir.x -= 1f;
            if (Input.GetKey(KeyCode.D)) dir.x += 1f;

            dir = Vector3.ClampMagnitude(dir, 1f); // �밢�� �̵� �ӵ� ���� ����

            if (dir != Vector3.zero)
            {
                // ������ ���� ������Ʈ ��û
                CmdUpdateDirection(dir);
            }

            transform.position += dir * speed * Time.deltaTime;
            isMove = dir.magnitude != 0f;
            animator.SetBool("isMove", isMove);
        }
    }

    [Command] // �������� ���� ������Ʈ ó��
    private void CmdUpdateDirection(Vector3 dir)
    {
        lastMoveDir = dir; // SyncVar ������Ʈ
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
        // Right Shift�θ� �߻�
        if (Input.GetKeyDown(KeyCode.RightShift) && Time.time >= nextAttackTime && !isGamePaused)
        {
            FireDot();
            nextAttackTime = Time.time + 1f / GameData.Instance.attackSpeed; // ���� �ӵ��� ���� ��ٿ� ����
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
            Debug.LogError("Dot Prefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        GameObject dot = Instantiate(dotPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(dot, connectionToClient); // ��Ʈ��ũ�� ��Ʈ ����

        Dot dotScript = dot.GetComponent<Dot>();
        if (dotScript != null)
        {
            dotScript.SetStats(10f, GameData.Instance.attackPower, lastMoveDir, GameData.Instance.attackRange, netIdentity);
        }
        else
        {
            Debug.LogError("Dot prefab�� Dot ��ũ��Ʈ�� �����ϴ�.");
        }
    }

    [Server]
    public void TakeDamage(int damage)
    {
        if (hp <= 0) return; // �̹� ����� ��� ����
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            // �÷��̾� ��� ó�� (��: ��� �ִϸ��̼�, ������ ��)
            Debug.Log($"{gameObject.name}��(��) ����߽��ϴ�.");

            // ���� �Ͻ����� ��û
            if (!isGamePaused)
            {
                isGamePaused = true;
                RpcPauseGame();
            }
        }
        Debug.Log($"{gameObject.name}��(��) ���ظ� �Ծ����ϴ�. ���� HP: {hp}");
        // SyncVar�� �ڵ����� hp�� ����ȭ�ϰ� OnHPChanged�� ȣ����
    }

    private void OnHPChanged(int oldHP, int newHP)
    {
        Debug.Log($"OnHPChanged�� ȣ��Ǿ����ϴ�: {gameObject.name}, ���� HP: {oldHP}, ���ο� HP: {newHP}");
        if (isLocalPlayer)
        {
            UpdateHPText(newHP);
        }

        // HP�� 0���� ����Ǹ� �߰����� ������ ���� ��� ���⿡ �߰�
    }

    private void UpdateHPText(int currentHP)
    {
        if (hpText != null)
        {
            hpText.text = currentHP.ToString(); // HP ���� ������Ʈ
            Debug.Log($"{gameObject.name}�� HP �ؽ�Ʈ�� {currentHP}�� ������Ʈ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("hpText�� null�Դϴ�: " + gameObject.name);
        }
    }

    // ��� Ŭ���̾�Ʈ���� ������ �Ͻ������ϵ��� ���
    [ClientRpc]
    private void RpcPauseGame()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            Time.timeScale = 0f; // ���� �ð� ����
            Debug.Log("������ �Ͻ������Ǿ����ϴ�.");

            if (pauseOverlay != null)
            {
                pauseOverlay.SetActive(true); // �������� Ȱ��ȭ
            }
            else
            {
                Debug.LogError("PauseOverlay�� �Ҵ���� �ʾҽ��ϴ�.");
            }

            // �Ͻ����� UI�� ǥ���ϰų� �߰����� ó�� ������ ���⿡ �߰��� �� �ֽ��ϴ�.
            // ���� ���, PauseMenu�� Ȱ��ȭ�ϴ� �ڵ�:
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
        Time.timeScale = 1f; // ���� �ð� �簳
        Debug.Log("������ �簳�Ǿ����ϴ�.");

        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(false); // �������� ��Ȱ��ȭ
        }
    }
}
