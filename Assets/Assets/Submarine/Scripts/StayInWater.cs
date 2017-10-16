using UnityEngine;
using System.Collections;

public class StayInWater : MonoBehaviour {

	public float waterLevel;
	public float safeDistance;
	
	// Update is called once per frame
	void Update ()
	{
		if(GetComponent<Rigidbody>().position.y > waterLevel - safeDistance)
		{

			GetComponent<Rigidbody>().position = new Vector3(
				GetComponent<Rigidbody>().position.x,
				waterLevel - safeDistance,
				GetComponent<Rigidbody>().position.z);
				
		}
	}
}
