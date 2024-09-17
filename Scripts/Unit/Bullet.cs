using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//*****************************************
//创建人： 马俊航 
//功能说明：
//***************************************** 
public class Bullet : MonoBehaviour
{
    public Vector3 targetPos;
    private Vector3 moveSpeed;
    private int speed = 10;
    public GameObject hitEffectGo;
    //public AudioClip hitClip;
    private void Start()
    {
        if (targetPos==Vector3.zero) // when no target position
        {
            targetPos = transform.position; // releaset at angel position
        }
        moveSpeed = (targetPos - transform.position).normalized * speed;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) <= 0.3f)
        {
            if (hitEffectGo) // if hitEffect not null
            {
                // hitEffect
                Instantiate(hitEffectGo, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else
        {
            // use Lerp() to move smoothly
            transform.position += moveSpeed * Time.deltaTime;
        }
    }
}
