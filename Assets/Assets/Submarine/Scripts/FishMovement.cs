using UnityEngine;
using System.Collections;

public class FishMovement : MonoBehaviour {
	
	void Update ()
	{
		UpdateFacingDirection();
	}

	void UpdateFacingDirection()
	{
		this.transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity, Vector3.up);
	}
}
