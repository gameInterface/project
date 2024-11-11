using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MeinMenuUI : MonoBehaviour
{
    public void OnClickCreateBtn()
    {
        Debug.Log("룸 생성 버튼을 눌렀습니다");
    }

    public void OnClickJoinBtn()
    {
        Debug.Log("룸 참가 버튼을 눌렀습니다");
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
