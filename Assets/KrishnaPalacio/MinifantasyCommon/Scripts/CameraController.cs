using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minifantasy
{
    public class CameraController : MonoBehaviour
    {
        public Transform player;
        public float smoothTime = 0.3f;
        public Vector2 offset = new Vector2(0, 1);

        private Vector3 velocity = Vector3.zero;

        private void LateUpdate()
        {
            // ��ǥ ��ġ ���
            Vector3 targetPosition = new Vector3(
                player.position.x + offset.x,
                player.position.y + offset.y,
                transform.position.z);

            // SmoothDamp�� ����� ī�޶� ��ǥ ��ġ�� �ε巴�� �̵�
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // ī�޶� ��ġ�� �ȼ� ����Ʈ�ϰ� ���� (�ݿø��Ͽ� ����ȭ)
            transform.position = RoundToPixel(smoothedPosition);
        }

        private Vector3 RoundToPixel(Vector3 position)
        {
            float pixelPerUnit = 8f; // ����: 16 PPU
            position.x = Mathf.Round(position.x * pixelPerUnit) / pixelPerUnit;
            position.y = Mathf.Round(position.y * pixelPerUnit) / pixelPerUnit;
            return position;
        }

        public void ResetCamera()
        {
            Vector3 targetPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);
            transform.position = RoundToPixel(targetPosition);
            velocity = Vector3.zero;
        }
    }
}
