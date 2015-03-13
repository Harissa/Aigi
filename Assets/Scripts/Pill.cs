using UnityEngine;
using System.Collections;

public class Pill : MonoBehaviour {


	private Camera cam;
	public bool collected=false;
	public int index;
	
	public Light light;

	// Use this for initialization
	void Start () {
	  cam = Camera.main;
	  light.intensity = 0.7f;
	  transform.localScale -= new Vector3(0.2f,0.2f,0.2f);
	}
	
	public void onPickup()
	{
	      collected = true;
	      transform.Translate(new Vector3(0,0.92f,0));
	      light.intensity = 0.1f;
	      transform.localScale = new Vector3(0.15f,0.15f,0.15f);
	}
	
	// Update is called once per frame
	void Update () {
	  transform.LookAt(transform.position + cam.transform.rotation * new Vector3(0.0f,0.0f,1.0f),cam.transform.rotation * Vector3.up);
	}
}
