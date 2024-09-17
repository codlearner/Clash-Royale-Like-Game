using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊航 
//功能说明：
//***************************************** 
public class MagicFire : Unit
{
    public GameObject explosion;
    private Vector3 moveSpeed;
    public Vector3 targetPos;
    public Vector3 startPos;
    public AudioClip useClip;
    public AudioClip hitClip;
    protected override void Start()
    {
        base.Start();
        // start from our king tower
        startPos = GameController.Instance.orangeBuildings[0].transform.position;
        transform.position = startPos;
        // speed at target position
        moveSpeed = (targetPos - startPos).normalized * unitInfo.speed;
        // so it won't hurt enemy when flying
        SetColliders(false);
        GameManager.Instance.PlaySound(useClip);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position,targetPos)<=0.3f)
        {
            SetColliders(true);
            Invoke("DamageAllUnits",0.2f);
        }
        else
        {
            // use Lerp() to move smoothly
            transform.position += moveSpeed * Time.deltaTime;
        }
    }

    private void DamageAllUnits()
    {
        for (int i = 0; i < targetsList.Count; i++)
        {
            targetsList[i].TakeDamage(unitInfo.attackValue, this);
        }
        Instantiate(explosion,transform.position,Quaternion.identity);
        GameManager.Instance.PlaySound(hitClip);
        Destroy(gameObject);
    }
}
