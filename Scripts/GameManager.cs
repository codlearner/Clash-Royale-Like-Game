using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊�?
//功能说明: 游戏管理�?
//***************************************** 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioSource audioSource;
    public AudioClip btnSound;
    public AudioClip mainMusic;
    public int golds;
    public int diamands;
    public int experience;
    public List<int> battleCardsList=new List<int>() { 1,2,3,4,5,6,7,8};
    public List<int> allCardsList=new List<int>() { 9};
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        PlayMusic(mainMusic);
    }
    /// <summary>
    /// play music
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlayMusic(AudioClip audioClip)
    {
        if (audioSource.clip!= audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
    public void PlaySound(AudioClip audioClip)
    {
        if (audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(btnSound);
    }
}

