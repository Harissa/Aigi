using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossGhost : MonoBehaviour
{

	public float speed;
	public Color colour;
	private float rotation;
	private float posX;
	public float turnSpeed;
	private GameObject gameController;
	private Camera cam;
	private CharacterController cController;
	private Animator animator;

// Use this for initialization
	void Start ()
	{
	
		gameController = GameObject.FindWithTag ("GameController");
		cam = Camera.main;
	}




	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		//Vector3 movement = new Vector3(0.0f,0.0f,moveVertical) * speed;
	
		transform.Translate (Vector3.forward * moveVertical * Time.deltaTime);
		Vector3 rot = transform.localEulerAngles;
		rotation += moveHorizontal * turnSpeed;
		rot.y = rotation;
		transform.localEulerAngles = rot;
	}

}