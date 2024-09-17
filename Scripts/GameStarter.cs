using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//*****************************************
//创建人： 马俊�? 
//功能说明：游戏启动器
//***************************************** 
public class GameStarter : MonoBehaviour
{
    public AudioClip audioClip;
    void Start()
    {
        GameManager.Instance.PlayMusic(audioClip);
        Invoke("LoadChoiceCardScene",2);
    }
    private void LoadChoiceCardScene()
    {
        SceneManager.LoadScene(2);
    }
}
