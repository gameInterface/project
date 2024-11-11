using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private float speed;
    private Vector3 direction;
    private float damage;
    private float range; // 범위 변수를 추가
    private float traveledDistance = 0f; // 이동한 거리 추적

    void Update()
    {
        // 도트 이동
        transform.position += direction * speed * Time.deltaTime;

        // 이동한 거리 업데이트
        traveledDistance += speed * Time.deltaTime;

        // 도트가 설정된 범위를 초과했을 때 파괴
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
        direction = newDirection.normalized; // 방향을 정규화
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage; // 데미지 설정
    }

    public void SetRange(float newRange) // 범위 설정 메서드 추가
    {
        range = newRange;
    }


}
