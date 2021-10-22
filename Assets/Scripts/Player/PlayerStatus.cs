using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamagable
{

    public float maxHp;
    public float maxStamina;
    
    [Header("View Only")]
    [SerializeField] float hp;
    [SerializeField] float stamina;

    PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();

        hp = maxHp;
        stamina = maxStamina;
    }

    public void CostStamina(float value)
    {
        stamina -= value;
    }

    public void TakeDamage(float value, Transform damageSource = null, Vector3 force = default(Vector3))
    {
        hp -= value;
        if (hp <= 0)
        {
            controller.Die();
        }
        else
        {
            controller.TakeDamage(value);
        }

        UIManager.Instance.ShowHurtIndicator(transform, damageSource);
    }
    
}
