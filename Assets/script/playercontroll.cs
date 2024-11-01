using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControl : MonoBehaviour
{
    public float speed = 5.0f;
    public float sprintMultiplier = 1.5f;
    public GameObject dotPrefab; // 도트 프리팹을 연결할 변수
    private Vector3 lastMoveDir;

    private float h, v;
    private float nextAttackTime = 0f; // 다음 공격 시간을 관리하기 위한 변수

    void Start()
    {
        Application.targetFrameRate = 60;
        lastMoveDir = Vector3.left; // 초기 방향 설정
    }

    void FixedUpdate()
    {
        // 이동 관련 코드
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(h, v, 0).normalized;

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        transform.position += moveDir * currentSpeed * Time.deltaTime;

        if (moveDir != Vector3.zero)
        {
            lastMoveDir = moveDir; // 최근 방향으로 업데이트
        }

        // 플레이어 방향 설정
        if (lastMoveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (lastMoveDir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // 도트 발사
        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time >= nextAttackTime)
        {
            FireDot();
            nextAttackTime = Time.time + 1f / GameData.Instance.attackSpeed; // 공격 속도에 따라 다음 공격 시간 설정
        }
    }

    void FireDot()
    {
        // 도트를 인스턴스화하고 발사
        GameObject dot = Instantiate(dotPrefab, transform.position, Quaternion.identity);
        Dot dotScript = dot.GetComponent<Dot>();
        if (dotScript != null)
        {
            dotScript.SetSpeed(10f); // 도트 속도 설정 (필요에 따라 조정 가능)
            dotScript.SetDamage(GameData.Instance.attackPower); // 도트의 데미지를 GameStats에서 가져오기
            dotScript.SetDirection(lastMoveDir); // 최근에 눌린 방향으로 도트 발사
            dotScript.SetRange(GameData.Instance.attackRange);
        }
    }
}
