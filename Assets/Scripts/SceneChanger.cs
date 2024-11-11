using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using Mirror;


public class SceneChangeOnClickTMP : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI targetText;  // Ŭ���� TextMeshPro �ؽ�Ʈ
    public string nextSceneName;        // �̵��� ���� �̸�

    public void OnPointerClick(PointerEventData eventData)
    {
        // ���� ��ȯ�մϴ�.
        SceneManager.LoadScene(nextSceneName);
    }

    public void CreateRoom()
    {
        var manager = RoomManager.singleton;

        manager.StartHost();
    }
}
