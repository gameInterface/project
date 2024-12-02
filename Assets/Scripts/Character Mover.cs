using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMover : NetworkBehaviour
{
    private Animator animator;
    public bool isMoveable;
    [SyncVar] public float speed = 2f;

    public GameObject dotPrefab;

    [SyncVar(hook = nameof(OnDirectionChanged))]
    private Vector3 lastMoveDir = Vector3.right;

    private float nextAttackTime = 0f; // Next attack time based on cooldown

    void Start()
    {
        animator = GetComponent<Animator>();

        if (isOwned)
        {
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = 2.5f;
        }
    }

    void FixedUpdate()
    {
        Move();
        HandleDotFire();
    }

    public void Move()
    {
        if (isOwned && isMoveable)
        {
            bool isMove = false;

            // WASD 키 입력에 따라 이동 방향 설정
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir.y += 1f;
            if (Input.GetKey(KeyCode.S)) dir.y -= 1f;
            if (Input.GetKey(KeyCode.A)) dir.x -= 1f;
            if (Input.GetKey(KeyCode.D)) dir.x += 1f;

            dir = Vector3.ClampMagnitude(dir, 1f); // 방향 벡터를 정규화하여 대각선 이동 속도 일정 유지

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
        if (newDir.x < 0f) transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
        else if (newDir.x > 0f) transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    private void HandleDotFire()
    {
        if (isOwned && Input.GetKeyDown(KeyCode.RightShift) && Time.time >= nextAttackTime)
        {
            FireDot();
            nextAttackTime = Time.time + 1f / GameData.Instance.attackSpeed; // Sets cooldown based on attack speed
        }
    }

    private void FireDot()
    {
        GameObject dot = Instantiate(dotPrefab, transform.position, Quaternion.identity);
        Dot dotScript = dot.GetComponent<Dot>();

        if (dotScript != null)
        {
            dotScript.SetStats(10f, GameData.Instance.attackPower, lastMoveDir, GameData.Instance.attackRange, netIdentity);
        }
    }
}
