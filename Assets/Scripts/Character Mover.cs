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
        move();
        HandleDotFire();
    }

    public void move()
    {
        if (isOwned && isMoveable)
        {
            bool isMove = false;
            Vector3 dir = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f), 1f);

            if (dir != Vector3.zero) lastMoveDir = dir;

            transform.position += dir * speed * Time.deltaTime;
            isMove = dir.magnitude != 0f;
            animator.SetBool("isMove", isMove);
        }
    }

    private void OnDirectionChanged(Vector3 oldDir, Vector3 newDir)
    {
        if (newDir.x < 0f) transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
        else if (newDir.x > 0f) transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    private void HandleDotFire()
    {
        if (isOwned && Input.GetKeyDown(KeyCode.LeftControl) && Time.time >= nextAttackTime)
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
            dotScript.SetSpeed(10f);
            dotScript.SetDamage(GameData.Instance.attackPower);
            dotScript.SetDirection(lastMoveDir);
            dotScript.SetRange(GameData.Instance.attackRange);
        }
    }
}

