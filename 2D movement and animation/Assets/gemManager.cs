using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class gemManager : MonoBehaviour
{
    public static gemManager instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI toWin;
    int score;
    public int winAmount = 4;
  
    // Start is called before the first frame update
    void Start()
    {
        score = winAmount;
        toWin.text = "/" + winAmount.ToString();
       

       if (instance == null)
        {
            instance = this;
        } 
    }


    public void ChangeScore(int gemValue)
    {
        score += gemValue;
        scoreText.text = score.ToString();

        if (score >= winAmount)
       {
           Debug.Log("Finish!!");
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
       }

    
      
    

    }

  
   
}
