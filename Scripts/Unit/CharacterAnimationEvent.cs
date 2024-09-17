using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊航 
//功能说明：
//***************************************** 
public class CharacterAnimationEvent : MonoBehaviour
{
    private Unit unit;
    void Start()
    {
        unit = GetComponentInParent<Unit>();
    }

    void Update()
    {

    }
    private void AttackAnimationEvent()
    {
        unit.AttackAnimationEvent();
    }
}
