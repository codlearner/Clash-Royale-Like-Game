using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊�? 
//功能说明�?
//***************************************** 
public class Death : Character
{
    public AudioClip callSkeletonClip;
    protected override void Start()
    {
        base.Start();
        InvokeRepeating("CreateSkeletons",1,10);
    }
    private void CreateSkeletons()
    {
        GameManager.Instance.PlaySound(callSkeletonClip);
        GameController.Instance.CreateUnit(11, transform.position + new Vector3(1, 0, 1), isPurple);
        GameController.Instance.CreateUnit(11, transform.position + new Vector3(1, 0, -1), isPurple);
        GameController.Instance.CreateUnit(11, transform.position + new Vector3(-1, 0, 1), isPurple);
        GameController.Instance.CreateUnit(11, transform.position + new Vector3(-1, 0, -1), isPurple);
    }
}
