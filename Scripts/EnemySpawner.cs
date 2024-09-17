using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊航 
//功能说明：
//***************************************** 
public class EnemySpawner : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("CreateUnits",5,10); // first time spawn in 5 seconds, then spawn in 10 seconds
    }
    private void CreateUnits()
    {
        // true means purple team
        GameController.Instance.CreateUnit(Random.Range(1,9),transform.position,true);
    }
}
