using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public Vector3 force;
    public Transform owner;

    public virtual bool IsAttacking
    {
        get => isAttacking;
        set
        {
            isAttacking = value;
            attacked.Clear();
        }
    }
    [SerializeField]
    protected bool isAttacking;


    protected List<GameObject> attacked;
    
}
