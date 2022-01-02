using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    
    // Start is called before the first frame update
    

    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;

    float HorizontalMove = 0f;

    bool jump = false;

    bool crouch = false;

    bool teleport = false;

    //bool dash = false;

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    // Update is called once per frame
    void Update()
    {
     
      HorizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(HorizontalMove));

        if (Input.GetButtonDown("teleport"))
        {
            teleport = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;  
        } else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    }



   public void OnLanding()
    {
        animator.SetBool("isJumping", false);

    }

    public void OnCrouching (bool isCrouching)
    {
        animator.SetBool("isCrouching", isCrouching);
    }

    public void FixedUpdate()
    {
      
        {   
            controller.Move(HorizontalMove * Time.fixedDeltaTime, crouch, jump, teleport);
         
        }

        jump = false;

        teleport = false;
    }
}
