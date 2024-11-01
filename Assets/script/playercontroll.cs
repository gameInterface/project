using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControl : MonoBehaviour
{
    public float speed = 5.0f;
    public float sprintMultiplier = 1.5f;
    public GameObject dotPrefab; // ��Ʈ �������� ������ ����
    private Vector3 lastMoveDir;

    private float h, v;
    private float nextAttackTime = 0f; // ���� ���� �ð��� �����ϱ� ���� ����

    void Start()
    {
        Application.targetFrameRate = 60;
        lastMoveDir = Vector3.left; // �ʱ� ���� ����
    }

    void FixedUpdate()
    {
        // �̵� ���� �ڵ�
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
            lastMoveDir = moveDir; // �ֱ� �������� ������Ʈ
        }

        // �÷��̾� ���� ����
        if (lastMoveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (lastMoveDir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // ��Ʈ �߻�
        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time >= nextAttackTime)
        {
            FireDot();
            nextAttackTime = Time.time + 1f / GameData.Instance.attackSpeed; // ���� �ӵ��� ���� ���� ���� �ð� ����
        }
    }

    void FireDot()
    {
        // ��Ʈ�� �ν��Ͻ�ȭ�ϰ� �߻�
        GameObject dot = Instantiate(dotPrefab, transform.position, Quaternion.identity);
        Dot dotScript = dot.GetComponent<Dot>();
        if (dotScript != null)
        {
            dotScript.SetSpeed(10f); // ��Ʈ �ӵ� ���� (�ʿ信 ���� ���� ����)
            dotScript.SetDamage(GameData.Instance.attackPower); // ��Ʈ�� �������� GameStats���� ��������
            dotScript.SetDirection(lastMoveDir); // �ֱٿ� ���� �������� ��Ʈ �߻�
            dotScript.SetRange(GameData.Instance.attackRange);
        }
    }
}
