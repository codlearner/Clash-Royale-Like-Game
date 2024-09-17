using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊航 
//功能说明：
//***************************************** 
public class Building : Unit
{
    public bool isKing;
    private float attackCD=1.4f;
    private float attackTimer;
    public GameObject[] towerGos;
    public Transform archerTrans;
    public AudioClip destoryClip;
    protected override void Start()
    {
        unitInfo = GameController.Instance.unitInfos[12];
        base.Start();
        if (isKing)
        {
            SetColliders(false); // king is not active until it's attacked
            unitInfo.attackArea -= 1;
            unitInfo.attackValue += 2;
            unitInfo.hp *= 2;   
        }

    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        Attack();
    }
    private void Attack()
    {
        if (Time.time - attackTimer >= attackCD)
        {
            attackTimer = Time.time;
            if (hasTarget && targetUnit != null)
            {
                // ignore target's y pos for not to look down at the enemy
                archerTrans.LookAt(new Vector3(targetUnit.transform.position.x, archerTrans.position.y, targetUnit.transform.position.z)); 
                targetUnit.TakeDamage(unitInfo.attackValue, this);
                animator.SetBool("IsAttacking", true);
            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }
        }
    }
    protected override void Die(Unit attacker)
    {
        base.Die(attacker);
        towerGos[0].SetActive(false);
        towerGos[1].SetActive(true);
        GameManager.Instance.PlaySound(destoryClip);
        if (isKing) // if king die, game over
        {
            // if purple king die, you win
            UIManager.Instance.GameOver(isPurple);
        }
        else // activate king tower
        {
            GameController.Instance.EnableKing(isPurple);
        }
    }
    public override void AttackAnimationEvent()
    {
        // do nothing so building will not turn like characters
        // otherwise it will use the func in unit, turn the tower toward the enemy
    }
}
