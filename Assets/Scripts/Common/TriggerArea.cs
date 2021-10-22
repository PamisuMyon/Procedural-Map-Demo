using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{

    public event System.Action<Collider> TriggerEnter;
    public event System.Action<Collider> TriggerStay;
    public event System.Action<Collider> TriggerExit;

    private void OnTriggerEnter(Collider other) 
    {
        TriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other) 
    {
        TriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other) 
    {
        TriggerExit?.Invoke(other);
    }
}
