using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class SceneChangeOnClickTMP : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI targetText;  // Ŭ���� TextMeshPro �ؽ�Ʈ
    public string nextSceneName;        // �̵��� ���� �̸�

    public void OnPointerClick(PointerEventData eventData)
    {
        // ���� ��ȯ�մϴ�.
        SceneManager.LoadScene(nextSceneName);
    }
}
