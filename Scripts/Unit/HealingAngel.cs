using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊航
//功能说明:
//***************************************** 
public class HealingAngel : Character
{
    private float healNum=2;
    public Bullet ball;

    public override void AttackAnimationEvent()
    {
        // look at enemy and make them hurt
        base.AttackAnimationEvent();
        // instantiate a bullet
        Bullet bullet = Instantiate(ball, transform.position, Quaternion.identity);
        if (hasTarget)
        {
            if (targetUnit)
            {
                // set the target pos of this bullet to be the target unit position
                bullet.targetPos = targetUnit.transform.position;
            }          
        }
        else
        {
            bullet.targetPos = defaultTarget.transform.position;
        }     
        healNum++; // heal every 2 times of attack
        if (healNum>=2)
        {
            GameController.Instance.CreateUnit(12, transform.position, !isPurple);
            healNum = 0;
        }     
    }
}
