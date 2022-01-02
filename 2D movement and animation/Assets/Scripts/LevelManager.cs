using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private GameObject player;

    public void Start()
    {
        player = GameObject.FindWithTag("Player");
    }




}
