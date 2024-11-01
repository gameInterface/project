using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed = 5.0f;
    public float sprintMultiplier = 1.5f;
    private Vector3 lastMoveDir;

    float h, v;

    void Start()
    {
        Application.targetFrameRate = 60;
        lastMoveDir = Vector3.left;
    }

    void FixedUpdate()
    {
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
            lastMoveDir = moveDir;
        }

        if (lastMoveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (lastMoveDir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
