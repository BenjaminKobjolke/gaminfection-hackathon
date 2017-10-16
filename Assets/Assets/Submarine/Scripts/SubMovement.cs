using UnityEngine;
using System.Collections;

public class SubMovement : MonoBehaviour
{
	// public variables

	public float movementSpeed; // = 100f;
	public float rotationSpeed; // = 5f;
	public float maxSpeed;

	public GameObject[] propellers;
	public float propChangeRate;
	public ParticleSystem[] bubbles;

	public AudioClip hit;


	// private variables
	private float surfaceHeight = 450.2f;


	void Start()
	{
		GetComponent<Rigidbody>().freezeRotation = true;
	}
	

	void FixedUpdate()
	{
	
		if(transform.position.y < surfaceHeight)
		{
			if(Input.GetKey("w"))
			{
				up();
			}
		}

		if(Input.GetKey("s"))
		{
			down();
		}

        if (Input.GetKey("left"))
        {
            slideleft();
        }
        if (Input.GetKey("right"))
        {
            slideright();
        }

        if (Input.GetKey("down"))
		{
			forward();
			foreach(GameObject prop in propellers)
			{	
				prop.GetComponent<Animation>().CrossFade("FastSpin", propChangeRate);
			}

			foreach(ParticleSystem bubble in bubbles)
			{
				bubble.emissionRate = 100f;
			}
		}

		else
		{
			foreach(GameObject prop in propellers)
			{	
				prop.GetComponent<Animation>().CrossFade("SlowSpin", propChangeRate);
			}

			foreach(ParticleSystem bubble in bubbles)
			{
				bubble.emissionRate = 10f;
			}
		}

		if(Input.GetKey("up"))
		{
			backward();
		}
		
		// Rotation
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
		rotate(rotation);


		// Keep in water
		if(transform.position.y >= surfaceHeight)
		{
			Vector3 temp = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
			GetComponent<Rigidbody>().velocity = temp;
		}
		
	}
	

	/***************************
	 Control Functions
	**************************/
	void up()
	{
		GetComponent<Rigidbody>().AddForce(transform.forward * movementSpeed, ForceMode.Force); 
	}

	void down()
	{
		GetComponent<Rigidbody>().AddForce(-1 * transform.forward * movementSpeed, ForceMode.Force); 	
	}
    void slideleft()
    {
        GetComponent<Rigidbody>().AddForce(-1*transform.right   * movementSpeed, ForceMode.Force);
    }

    void slideright()
    {
        GetComponent<Rigidbody>().AddForce( transform.right  * movementSpeed, ForceMode.Force);
    }
    void forward()
	{
		//if(GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
		GetComponent<Rigidbody>().AddForce(-1 * transform.up * movementSpeed, ForceMode.Force);

	}

	void backward()
	{
		GetComponent<Rigidbody>().AddForce(transform.up * movementSpeed, ForceMode.Force);	
	}

	void rotate(float rotation)
	{
		GetComponent<Rigidbody>().freezeRotation = false;
		transform.Rotate(0, rotation, 0);
		GetComponent<Rigidbody>().freezeRotation = true;
	}

	void OnCollisionEnter(Collision collision)
	{
		GetComponent<AudioSource>().PlayOneShot(hit);
	}
	
}


