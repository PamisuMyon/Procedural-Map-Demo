using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{

    public enum State
    {
        Idling, Running, Attacking, ShieldHolding
    }

    State state = State.Idling;
    bool canCombo = false;

    Animator animator;
    PlayerInput input;
    Weapon leftWeapon;
    Weapon rightWeapon;

    void Start()
    {
        animator = GetComponent<Animator>();
        input = GetComponentInParent<PlayerInput>();

        leftWeapon = transform.Find("Left").GetComponentInChildren<Weapon>();
        rightWeapon = transform.Find("Right").GetComponentInChildren<Weapon>();
        if (leftWeapon != null) leftWeapon.owner = transform;
        if (rightWeapon != null) rightWeapon.owner = transform;
    }

    void Update()
    {
        if (state == State.Idling)
        {
            if (input.ShieldHeld)
                SetState(State.ShieldHolding);
            else if (input.Attack)
                SetState(State.Attacking);
            else if (input.Movement != Vector3.zero)
                SetState(State.Running);
        }
        else if (state == State.Running)
        {
            if (input.ShieldHeld)
                SetState(State.ShieldHolding);
            else if (input.Attack)
                SetState(State.Attacking);
            else if (input.Movement == Vector3.zero)
                SetState(State.Idling);
        }
        else if (state == State.Attacking)
        {
            if (input.Attack)
            {
                if (canCombo)
                {
                    animator.SetTrigger("Attack");
                }
                input.Attack = false;
            }
        }
        else if (state == State.ShieldHolding)
        {
            if (!input.ShieldHeld)
                SetState(State.Idling);
        }
    }

    void SetState(State newState)
    {
        if (state == newState) return;
        ExitState(state);
        EnterState(newState);
        state = newState;
    }

    void EnterState(State newState)
    {
        if (newState == State.Attacking)
        {
            animator.SetTrigger("Attack");
            canCombo = false;
            input.Attack = false;
        }
        else if (newState == State.Running)
        {
            animator.SetBool("IsRunning", true);
        }
        else if (newState == State.ShieldHolding)
        {
            animator.SetBool("IsHoldingShield", true);
        }
    }

    void ExitState(State oldState)
    {
        if (oldState == State.ShieldHolding)
        {
            animator.SetBool("IsHoldingShield", false);
        }
        else if (oldState == State.Running)
        {
            animator.SetBool("IsRunning", false);
        }
    }

    public void OnIdleAnimBegin()
    {
        SetState(State.Idling);
    }

    public void OnAttackAnimEffectBegin()
    {
        canCombo = true;
        if (leftWeapon != null)
        {
            leftWeapon.IsAttacking = true;
        }
        if (rightWeapon != null)
        {
            rightWeapon.IsAttacking = true;
        }
    }

    public void OnAttackAnimEffectEnd()
    {
        if (leftWeapon != null)
        {
            leftWeapon.IsAttacking = false;
        }
        if (rightWeapon != null)
        {
            rightWeapon.IsAttacking = false;
        }
    }

}
