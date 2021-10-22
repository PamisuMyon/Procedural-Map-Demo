using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{

    public void OnStartButton()
    {
        GameManager.Instance.GameStart();
    }

    public void OnQuitButton()
    {
        GameManager.Instance.Quit();
    }
}
