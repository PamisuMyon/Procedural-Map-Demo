using System.Collections;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{

    public float delayTime;
    
    private void Start() 
    {
        StartCoroutine(DestroySelf(delayTime));
    }

    IEnumerator DestroySelf(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

}
