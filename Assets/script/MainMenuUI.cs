using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeinMenuUI : MonoBehaviour
{
    public void OnClickQuitButton()
    {   
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
