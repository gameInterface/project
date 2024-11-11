using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMover : NetworkBehaviour
{

    private Animator animator;

    public bool isMoveable;

    [SyncVar]
    public float speed = 2f;
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        if(isOwned)
        {
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = 2.5f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
    }

    public void move()
    {
        if(isOwned && isMoveable)
        {
            bool isMove = false;
            Vector3 dir =  Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f), 1f);
            if (dir.x < 0f) transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
            else if (dir.x > 0f) transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            transform.position += dir * speed * Time.deltaTime;
            isMove = dir.magnitude != 0f;
            animator.SetBool("isMove", isMove);
        }
    }
}
