using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public int gemValue = 1;
    

    private bool destroyed = false;
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.CompareTag("Player") && destroyed == false )
        {
            Debug.Log("OnTriggerEnter2d");
            destroyed = true;
            Debug.Log("this should only happen once");
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("destroyed");
        gemManager.instance.ChangeScore(gemValue);
        destroyed = false;
    }
    void FixedUpdate()
    {
        if (destroyed)
        {
            Debug.Log("destroyed == true");
        } else if (destroyed == false)
        {
            Debug.Log("destroyed == false");
        }
    }
}
    

