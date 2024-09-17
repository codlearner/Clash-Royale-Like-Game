using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： 马俊航 
//功能说明：
//***************************************** 
public class HPSlider : MonoBehaviour
{
    public Slider slider;
    private Camera cam;
    public float yOffset; //血条与人物间的垂直偏移
    private Transform target; //人物Transform
    private void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        Vector3 worldPos = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        transform.position = worldPos;

        // 让血条面向摄像机
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }

    public void SetHPColorSlider(bool isPurple)
    {
        cam = Camera.main;
        target = transform.parent;
        if (isPurple)
        {
            slider = transform.Find("Slider_Blue").GetComponent<Slider>();
        }
        else
        {
            slider = transform.Find("Slider_Red").GetComponent<Slider>();
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            //when not damaged we don't show xuetiao
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetHPValue(float value)
    {
        if (!slider.gameObject.activeSelf)
        {
            slider.gameObject.SetActive(true);
        }
        slider.value = value;
    }
}
