using UnityEngine;
using System.Collections;

public class GhostIcon : MonoBehaviour {

	public GameObject ghost= null;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (ghost != null) {
			transform.localPosition = ghost.transform.position+offset;
				}
	}
	public void setOffset(Vector3 newOffset) {
		offset = newOffset;
	}

}
