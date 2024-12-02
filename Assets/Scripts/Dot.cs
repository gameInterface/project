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

    // ��Ʈ�� �Ӽ� ����
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

        // ��Ʈ �̵�
        float step = speed * Time.deltaTime;
        transform.position += direction * step;

        // �̵��� �Ÿ� �Ǵ� ���� �ð��� �ʰ��Ǹ� ��Ʈ �ı�
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    // ��Ʈ�� �浹���� �� ó��
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
