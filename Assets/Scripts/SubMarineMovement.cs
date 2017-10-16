using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMarineMovement : MonoBehaviour {

	public float speed = 1.5f;
	public float spacing = 1.0f;
	private Vector3 pos;

	void Start() {
		pos = transform.position;
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.W)) {
			pos.y += spacing;
		} 

		if (Input.GetKeyDown (KeyCode.S)) {
			pos.y -= spacing;
		} 

		if (Input.GetKeyDown (KeyCode.A)) {
			pos.x -= spacing;
			transform.Rotate(Vector3.up, 90);
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			pos.x += spacing;
		}

		//transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
	}

}