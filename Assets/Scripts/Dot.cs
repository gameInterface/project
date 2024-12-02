using UnityEngine;
using Mirror;

public class Dot : NetworkBehaviour
{
    [SyncVar]
    private float duration;
    [SyncVar]
    private int damage;
    [SyncVar]
    private Vector3 direction;
    [SyncVar]
    private float range;
    [SyncVar]
    private NetworkIdentity shooter;

    private Vector3 startPosition;
    private float speed = 5f;
    private float startTime;

    // 도트의 속성 설정
    public void SetStats(float duration, int damage, Vector3 direction, float range, NetworkIdentity shooter)
    {
        this.duration = duration;
        this.damage = damage;
        this.direction = direction.normalized;
        this.range = range;
        this.shooter = shooter;
        startPosition = transform.position;
        startTime = Time.time;
    }

    void Update()
    {

        // 도트 이동
        float step = speed * Time.deltaTime;
        transform.position += direction * step;

        // 이동한 거리 또는 지속 시간이 초과되면 도트 파괴
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    // 도트가 충돌했을 때 처리
    private void OnTriggerEnter2D(Collider2D other)
    {

        InGameCharacterMover target = other.GetComponent<InGameCharacterMover>();
        if (target != null && target.netIdentity != shooter)
        {
            target.TakeDamage(damage);
            NetworkServer.Destroy(gameObject);
        }
    }
}
