using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private float speed;
    private Vector3 direction;
    private float damage;
    private float range; // ���� ������ �߰�
    private float traveledDistance = 0f; // �̵��� �Ÿ� ����

    void Update()
    {
        // ��Ʈ �̵�
        transform.position += direction * speed * Time.deltaTime;

        // �̵��� �Ÿ� ������Ʈ
        traveledDistance += speed * Time.deltaTime;

        // ��Ʈ�� ������ ������ �ʰ����� �� �ı�
        if (traveledDistance > range)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized; // ������ ����ȭ
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage; // ������ ����
    }

    public void SetRange(float newRange) // ���� ���� �޼��� �߰�
    {
        range = newRange;
    }


}
