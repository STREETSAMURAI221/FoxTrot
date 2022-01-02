using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public int gemValue = 1;
    

    private bool destroyed = false;
    private void OnDestroy()
    {
        gemManager.instance.ChangeScore(gemValue);
        destroyed = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
       if(other.gameObject.CompareTag("Player") && destroyed == false )
        {
            destroyed = true;
            Debug.Log("this should only happen once");
            Destroy(gameObject);
        }
    }
    
}
    

