using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： 马俊航 
//功能说明：单位基类
//***************************************** 
public class Unit : MonoBehaviour
{
    // basic attributes
    public UnitInfo unitInfo;
    public bool isPurple; 
    public bool hasTarget; // if there is target in range
    private int currentHP;
    public bool isDead;
    // component properties
    public Animator animator;
    protected NavMeshAgent meshAgent;
    protected Collider[] colliders;
    protected HPSlider hpslider;
    // others
    public Unit defaultTarget; // default target is king
    public Unit targetUnit; // attacking this target unit
    public List<Unit> targetsList=new List<Unit>(); // all targets in range
    public List<Unit> attackerList = new List<Unit>(); // all attackers attacking us
    public AudioClip attackClip;
    public AudioClip dieClip;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>(); // animator is on child of script
        meshAgent = GetComponent<NavMeshAgent>();
        GetComponentInChildren<SphereCollider>().radius = unitInfo.attackArea; // set this unit's attack area
        colliders = GetComponentsInChildren<Collider>(); // get all colliders
        currentHP = unitInfo.hp;
        if (unitInfo.hp>0) // magic don't have hp (0)
        {
            hpslider = transform.Find("Canvas_HP").GetComponent<HPSlider>();
            hpslider.SetHPColorSlider(isPurple);
        }
    }
    protected virtual void UnitMove()
    {
        // has target in range
        if(hasTarget)
        {
            // target is not null and alive then attack it
            // because there will be animation when unit die
            // so even though it's not null we still need to judge if it's dead, for it may be dying
            if (targetUnit != null && !targetUnit.isDead) 
            {
                // move to target unit
                meshAgent.SetDestination(targetUnit.transform.position);
                PlayLocomotionAnimation(transform.position, targetUnit.transform.position);
            }
            // target is dead
            else
            {
                // reset target to attack
                ResetTarget(targetUnit);
            }
        }
        // no target in range
        else
        {
            // reset target to be default target
            GameController.Instance.UnitGetTargetPos(this,isPurple);
            if (defaultTarget!=null) // prince or king(when that side prince is dead)
            {
                // move to default target unit
                meshAgent.SetDestination(defaultTarget.transform.position);
                PlayLocomotionAnimation(transform.position, defaultTarget.transform.position);
            }
        }
    }
    protected virtual void UnitBehaviour()
    {
        if (meshAgent.enabled)
        {
            meshAgent.isStopped = false;
        }
        animator.SetBool("IsMoving", true);
        animator.SetBool("IsAttacking", false);
    }
    private void PlayLocomotionAnimation(Vector3 currentPos, Vector3 targetPos)
    {
        // not in attack range
        if(Vector3.Distance(currentPos,targetPos)>=unitInfo.attackArea)
        {
            UnitBehaviour();
        }
        // in attack range
        else
        {
            // show attack animation
            meshAgent.isStopped = true;
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsMoving", false);
        }
    }
    public virtual void AttackAnimationEvent()
    {
        if (hasTarget) // if has target in attack range, look at it
        {
            transform.LookAt(new Vector3(targetUnit.transform.position.x, transform.position.y, targetUnit.transform.position.z));
        }
        else // no target in attack range, look at default target
        {
            transform.LookAt(new Vector3(defaultTarget.transform.position.x, transform.position.y, defaultTarget.transform.position.z));
        }
        if (targetUnit != null)
        {
            targetUnit.TakeDamage(unitInfo.attackValue, this); // enemy target take damage from us with value of this unit's attackValue
        }
        GameManager.Instance.PlaySound(attackClip); // play attack clip
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponentInParent<Unit>(); // script "unit" is on "attack area" parent object
            if (isPurple != unit.isPurple)// not the same team
            {
                targetsList.Add(unit); // add this enemy to our targets list
                unit.AddAttackerToList(this); // add ourselves to this enemy's attacker list
                ClearDeadUnitInList(); // clear dead unit in our target list
                SetTarget();
            }
        }
    }
    private void AddAttackerToList(Unit unit)
    {
        attackerList.Add(unit);
    }
    private void ClearDeadUnitInList() // clear dead unit in our target list
    {
        List<int> clearList = new List<int>();
        for (int i = 0; i < targetsList.Count; i++)
        {
            if (targetsList[i]==null)
            {
                clearList.Add(i);
            }
        }
        for (int i = 0; i < clearList.Count; i++)
        {
            targetsList.RemoveAt(clearList[i]);
        }
    }
    private void SetTarget()
    {
        float closestDistance = Mathf.Infinity; // 最短距离，默认设置为无限大
        Unit u = null;
        for (int i = 0; i < targetsList.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, targetsList[i].transform.position); // 计算自己和当前游戏物体之间的距离
            if (distance < closestDistance)
            {   //如果当前距离比最短距离还要短
                closestDistance = distance; // 更新最短距离
                u = targetsList[i]; // 更新最近的游戏物体
            }
        }
        targetUnit = u;
        hasTarget = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponentInParent<Unit>();
        if (unit!=null && unit.isPurple != isPurple && targetUnit == unit)
        {
            ResetTarget(unit);
        }
    }
    public void ResetTarget(Unit unit)
    {
        targetsList.Remove(unit);
        ClearDeadUnitInList();
        // has a target set it 
        if (targetsList.Count>0)
        {
            SetTarget();
        }
        // no target in sight
        else
        {
            hasTarget = false;
            targetUnit = null;
            GameController.Instance.UnitGetTargetPos(this, isPurple);
        }
    }
    public void TakeDamage(int damageValue, Unit attacker)
    {
        currentHP -= damageValue;
        Mathf.Clamp(currentHP,0,unitInfo.hp);
        hpslider.SetHPValue((float)currentHP / unitInfo.hp); // set xuetiao's value
        if (currentHP <= 0)
        {
            //GameManager.Instance.PlaySound(dieClip);
            Die(attacker);
        }
    }
    protected virtual void Die(Unit attacker)
    {
        isDead = true;
        GameManager.Instance.PlaySound(dieClip); // play die clip
        animator.SetTrigger("Die");
        SetColliders(false); // set all our colliders to false
        attacker.ResetTarget(this); // let attacker know we're dead
        RemoveSelfFromOtherAttack();
        hpslider.gameObject.SetActive(false); // 血条隐藏
        if (meshAgent)
        {
            meshAgent.enabled = false;
        }
        Invoke("DestoryGame", 5); // delay 5s then destroy
    }
    public void SetColliders(bool state)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = state;
        }
    }
    private void DestoryGame()
    {
        Destroy(gameObject);
    }
    private void RemoveSelfFromOtherAttack()
    {
        for (int i = 0; i < attackerList.Count; i++)
        {
            // let all our attacker know we're dead and remove us from their target list
            attackerList[i].targetsList.Remove(this);
        }
    }
}
public struct UnitInfo // infos about unit
{
    public int id;
    public string unitName;
    public int cost;
    public int hp;
    public float attackArea;
    public int speed;
    public int attackValue;
    public bool canCreateAnywhere;
}