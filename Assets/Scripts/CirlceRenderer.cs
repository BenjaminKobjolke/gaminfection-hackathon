﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirlceRenderer : MonoBehaviour {

	[Range(0,50)]
	public int segments = 50;

	public float xradius = 15;

	public float yradius = 15;
	LineRenderer line;

	void Start ()
	{
		line = gameObject.GetComponent<LineRenderer>();

		line.SetVertexCount (segments + 1);
		line.useWorldSpace = false;
		CreatePoints ();
	}

	void CreatePoints ()
	{
		float x;
		float y;
		float z;

		float angle = 20f;

		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
			z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;

			line.SetPosition (i,new Vector3(x,z,0) );

			angle += (360f / segments);
		}
	}
}
