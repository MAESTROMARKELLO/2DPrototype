using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{

    public GameObject thisObject;

    public void DestroySelf() 
    {
        StartCoroutine(Die());
    }
    
    IEnumerator Die() 
    {
        yield return new WaitForSeconds(0.45f);
        Destroy(thisObject); 
    }
}
