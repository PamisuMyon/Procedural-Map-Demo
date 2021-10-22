using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    
    public List<string> targetTags;
    public LayerMask targetLayerMask;
    public GameObject hitEffect;
    public TrailRenderer slashTrail;

    BoxCollider boxCollider;

    public override bool IsAttacking
    {
        get => isAttacking;
        set
        {
            isAttacking = value;
            attacked.Clear();
            ToggleSlashTrail(value);
        }
    }

    private void Start() 
    {
        boxCollider = GetComponent<BoxCollider>();

        attacked = new List<GameObject>();   
    }

    private void Update() 
    {
        if (!IsAttacking) return;
        var center = transform.TransformPoint(boxCollider.center);
        var cols = Physics.OverlapBox(center, boxCollider.size / 2, transform.rotation, targetLayerMask, QueryTriggerInteraction.Ignore);
        foreach (var item in cols)
        {
            if (targetTags.Contains(item.tag) && !attacked.Contains(item.gameObject))
            {
                var iDamagable = item.GetComponent<IDamagable>();
                if (iDamagable != null)
                {
                    iDamagable.TakeDamage(damage, owner);
                    attacked.Add(item.gameObject);
                    if (hitEffect)
                    {
                        var hitPoint = GetColliderCenter(item);
                        Instantiate(hitEffect, hitPoint, Random.rotation);
                    }
                }
                Debug.Log(gameObject + " Attacked: " + item.gameObject);
                break;
            }
        }

        // var hits = Physics.BoxCastAll(center, boxCollider.size / 2, transform.forward, transform.rotation, .001f, targetLayerMask);
        // foreach (var item in hits)
        // {
        //     if (!item.collider) continue;
        //     if (targetTags.Contains(item.transform.tag) && !attacked.Contains(item.transform.gameObject))
        //     {
        //         var iDamagable = item.transform.GetComponent<IDamagable>();
        //         if (iDamagable != null)
        //         {
        //             if (hitEffect)
        //             {
        //                 Instantiate(hitEffect, item.point, Random.rotation);
        //                 Debug.Log("Hit Point: " + item.point);
        //             }
        //             iDamagable.TakeDamage(damage);
        //             attacked.Add(item.transform.gameObject);

        //         }
        //         Debug.Log(gameObject + " Attacked: " + item.transform.gameObject);
        //         break;
        //     }
        // }
    }

    Vector3 GetColliderCenter(Collider col)
    {
        var center = col.transform.localPosition;
        if (col is CapsuleCollider)
        {
            center = ((CapsuleCollider) col).center;
        }
        else if (col is BoxCollider)
        {
            center = ((BoxCollider) col).center;
        }
        return col.transform.TransformPoint(center);
    }

    void ToggleSlashTrail(bool isOn)
    {
        if (!slashTrail) return;
        slashTrail.enabled = isOn;
    }

    // private void OnTriggerStay(Collider other) 
    // {
    //     if (!IsAttacking) return;
    //     foreach (var item in targetTags)
    //     {
    //         if (item == other.gameObject.tag && !attacked.Contains(other.gameObject))
    //         {
    //             var iDamagable = other.GetComponent<IDamagable>();
    //             iDamagable.TakeDamage(damage);
    //             attacked.Add(other.gameObject);
    //             Debug.Log(gameObject + " Attacked: " + other.gameObject);
    //             break;
    //         }
    //     }
    // }
}
