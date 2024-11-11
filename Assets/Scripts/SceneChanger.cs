using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using Mirror;


public class SceneChangeOnClickTMP : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI targetText;  // 클릭할 TextMeshPro 텍스트
    public string nextSceneName;        // 이동할 씬의 이름

    public void OnPointerClick(PointerEventData eventData)
    {
        // 씬을 전환합니다.
        SceneManager.LoadScene(nextSceneName);
    }

    public void CreateRoom()
    {
        var manager = RoomManager.singleton;

        manager.StartHost();
    }
}
