using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SettingUI : MonoBehaviour
{
    [SerializeField]
    private Button MouseControlButton;
    [SerializeField]
    private Button KeyboardMouseControlButton;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    public void Close()
    {
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        animator.SetTrigger("close");
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
        animator.ResetTrigger("close");
    }


}

