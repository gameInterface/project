using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MeinMenuUI : MonoBehaviour
{
    public void OnClickCreateBtn()
    {
        Debug.Log("�� ���� ��ư�� �������ϴ�");
    }

    public void OnClickJoinBtn()
    {
        Debug.Log("�� ���� ��ư�� �������ϴ�");
    }

    public void CreateRoom()
    {
        var manager = RoomManager.singleton;
        manager.StartHost();
    }

    public void JoinRoom()
    {
        var manager = RoomManager.singleton;
        manager.StartClient();
    }
}
