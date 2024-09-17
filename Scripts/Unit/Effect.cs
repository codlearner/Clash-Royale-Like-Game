using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊航
//功能说明:
//***************************************** 
public class Effect : MonoBehaviour
{
    public float destoryTime=2;
    void Start()
    {
        Destroy(gameObject, destoryTime);
    }
}
