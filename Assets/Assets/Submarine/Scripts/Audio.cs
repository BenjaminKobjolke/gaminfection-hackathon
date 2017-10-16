using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour
{

	public GUITexture button;
	public Texture2D mutedTexture;
	public Texture2D unmutedTexture;

	void Start()
	{
		GetComponent<AudioSource>().pitch = -0.8f;
	}
	void Update()
	{
		if(Input.GetKey(KeyCode.UpArrow))
		{
			GetComponent<AudioSource>().pitch += 0.01f;
			if(GetComponent<AudioSource>().pitch >= 1.2f)
			{
				GetComponent<AudioSource>().pitch = 1.2f;
			}
		}
		
		if(Input.GetKey(KeyCode.UpArrow) == false)
		{
			GetComponent<AudioSource>().pitch -= 0.01f;
			if(GetComponent<AudioSource>().pitch <= 0.8f)
			{
				GetComponent<AudioSource>().pitch = 0.8f;
			}
		}

	}
}
