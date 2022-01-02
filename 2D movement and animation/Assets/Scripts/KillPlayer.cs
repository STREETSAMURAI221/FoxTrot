using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class KillPlayer : MonoBehaviour
{



   
    public int Respawn;

    Vector3 originalPos;
    void Start()
    {
        originalPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameObject.transform.position = originalPos;

            SceneManager.LoadScene(Respawn);
        }
    }



}


 
