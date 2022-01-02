using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] private float slopeCheckDistance;
	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	//for adding hangtime
	public float hangTime = 10f;
	private float hangCounter;

	//for adding jump buffer
	public float jumpBufferLength = 10f;
	private float jumpBufferCount;

	//dust trail effect.
	public ParticleSystem footsteps;
	private ParticleSystem.EmissionModule footEmission;

	//adding impact effect.
	public ParticleSystem impactEffect;
	private bool wasOnGround;
	public GameObject platform;

	//Teleport

	float distance = 7f;


	// Teleport
	private bool CanTeleport;
	


	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}




	private void Start()
	{
		footEmission = footsteps.emission;
	}


	private void FixedUpdate()
	{
		int layerUse = (1 << 10);
	
		bool wasGrounded = m_Grounded;
		m_Grounded = false;


		if (m_FacingRight)
		{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 7f, layerUse);
			if (hit)
			{
				distance = hit.distance;
			} else
            {
				distance = 7f;
            }

		}

		if (!m_FacingRight)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), 7f, layerUse);
			if (hit)
			{
				distance = hit.distance;
            }
            else
            {
				distance = 7f;
            }

		}
















		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject) 
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
			//while (colliders[i].gameObject == platform && m_Grounded)
            {
			//	gameObject.transform.parent = platform.transform; 
            }
		}
	}

    //if (_canDash) StartCoroutine(Dash(_horizontalDirection, _verticalDirection)); 
    //if (!_isDashing)



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gems"))
        {
			Destroy(other.gameObject);
        }
    }


    public void Move(float move, bool crouch, bool jump, bool teleport)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}


	

		if (teleport && CanTeleport && m_FacingRight )
		{
			transform.position = new Vector2(transform.position.x + distance, transform.position.y);
			CanTeleport = false;
		} 
		else if (teleport && CanTeleport && !m_FacingRight)
		{
			transform.position = new Vector2(transform.position.x - distance, transform.position.y);
			CanTeleport = false;

		}

		if (m_Grounded)
        {
			CanTeleport = true;
        }


		

		
		



		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}





		}
		
       


		// to Manage hangtime...
		if (m_Grounded)
		{
			hangCounter = hangTime; //	 hangTime = .2f;
		
		}
		else
		{
			hangCounter -= Time.deltaTime;

		}
		//manage jump buffer
		if (jump)
		{
			jumpBufferCount = jumpBufferLength; //jumpBufferLength = .05f;
		}
		else
		{
			jumpBufferCount -= Time.deltaTime;
		}
		
		

		// If the player should jump/prevent the player from jumping mid jump.
		if (jumpBufferCount >= 0 && hangCounter > 0f && m_Rigidbody2D.velocity.y <= 4f)// this works but prevents jumping on slopes, is there another way?
		{
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			jumpBufferCount = 0;
			
		}  //basically the way my jump works is that "jumpBufferCount" and "hangCounter" are timers that start counting down once the jump is pressed, if jump is pressed after the timers hit zero jump wont be allowed.

		
	

		//small jumps
		//this will need to be adjusted for the double jump
		if (Input.GetButtonUp("Jump") && m_Rigidbody2D.velocity.y > 0 ) // might cause issues on slopes, should take a look into.
		{
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_Rigidbody2D.velocity.y * 0.5f);


		}

		if (Input.GetButtonUp("Jump"))
		{
			hangCounter = -0f;


		}
		//show footstep dust effect
		if (Input.GetAxisRaw("Horizontal") != 0 && m_Grounded)
		{
			footEmission.rateOverTime = 35f;
		}
		else
		{
			footEmission.rateOverTime = 0f;
		}

		//show Impact Effect.
		if (!wasOnGround && m_Grounded)
		{
			impactEffect.gameObject.SetActive(true);
			impactEffect.Stop();
			//impactEffect.transform.position = footEmission.transform.position;
			impactEffect.Play();
		}

		wasOnGround = m_Grounded;


	}







	void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}















}