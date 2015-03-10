﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControls : MonoBehaviour
{


	public float speed;
	public float jumpSpeed;
	public float gravity;
	private float yVelocity;
	private float rotation;
	private float posX;
	public float turnSpeed;
	public GameObject mainMap;
	public AudioClip dotSound;
	public KeyCode run;
	private MazeManager mazeManager;
	private GameObject gameController;
	private bool eatenDot = false;
	private CharacterController cController;
	private Animator animator;
	private bool paused = true;
	private Vector3 currentCell;
	private Vector3 nextCell;
	private Vector3 lastCell;
	private Hashtable hasVisited = new Hashtable ();
	
	// Use this for initialization
	void Start ()
	{
		cController = GetComponent<CharacterController> ();
		gameController = GameObject.FindWithTag ("GameController");
		animator = GetComponent<Animator> ();
		mazeManager = mainMap.GetComponent<MazeManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	  
	}

	public void setStartPos (float x, float y)
	{
		transform.position = new Vector3 (x, 0, y);
		currentCell.x = Mathf.Round (x);
		currentCell.y = 0;
		currentCell.z = Mathf.Round (y);
		lastCell = currentCell;
		nextCell = findRoute (currentCell, lastCell);
		//Debug.Log ("Current cell=" + currentCell.ToString () + "next=" + nextCell.ToString ());
		paused = false;

	}
	
	void FixedUpdate ()
	{
		if (!paused) {

		
			if ((Vector3.Distance (nextCell,transform.position)<0.2)) {
				currentCell.x = Mathf.Round (transform.position.x);
				currentCell.z = Mathf.Round (transform.position.z);
				nextCell = findRoute (currentCell, lastCell);
				lastCell = currentCell;
				//Debug.Log ("current cell=" + currentCell.ToString () + "new next cell=" + nextCell.ToString ());
			}

			//float moveHorizontal = Input.GetAxis("Horizontal");
			//float moveVertical = Input.GetAxis("Vertical");
			//Vector3 movement = new Vector3(0.0f,0.0f,1.0f) * speed;
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, nextCell, step);
			transform.LookAt(nextCell);

			//movement = transform.TransformDirection(movement);
			//movement.y += yVelocity;
			//cController.Move(movement * Time.deltaTime);
	  
			// TODO set animation to reflect motion
			//animator.SetFloat("speed",moveVertical*speed);
	  
			if (Input.GetKey ("escape")) {
				Application.Quit ();
			}
	  
			if (Input.GetKey (run)) {
				animator.SetBool ("running", true);
			} else {
				animator.SetBool ("running", false);
			}
	  
			//Vector3 rot = transform.localEulerAngles;
			// TODO set rotation to reflect motio 
			//rotation += moveHorizontal * turnSpeed;
			//rot.y = rotation;
			//transform.localEulerAngles = rot;
			if (eatenDot) {
				eatenDot = false;
				AudioSource.PlayClipAtPoint (dotSound, transform.position);
				gameController.GetComponent<GameController> ().pillsInWorld--;
				gameController.GetComponent<GameController> ().updateScore ();
						
			}
		}

	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Monster") {
			Debug.Log ("Hit a ghost");
			gameController.GetComponent<GameController> ().playerDied ();
		}
	  
		if (other.gameObject.tag == "Pill") {
			Pill thePill = other.gameObject.GetComponent<Pill> ();
			Debug.Log ("eat pill " + thePill.index+"pos="+thePill.transform.position);

			if (!thePill.collected) {
				eatenDot = true;
				thePill.onPickup ();
				//mazeManager.eatenPill(thePill.transform.position);

			}
		   		

		}
	}

	int mazeScore (Vector3 cell)
	{
		if (mazeManager.hasVisited (Mathf.RoundToInt (cell.x), Mathf.RoundToInt (cell.z))) {
			return 0; // no pill
		} else {
			return 10; // is pill
		}
	}
	// Manhattan distance on a square grid
	int heuristic (Vector2 first, Vector2 second)
	{
		return Mathf.RoundToInt (Mathf.Abs (first.x - second.x) + Mathf.Abs (first.y - second.y));
	}

	// TODO
	// How does a* react to negative maze costs?
	// how do we turn something designed to find a goal into something which finds the best route?
	// priority needs to have dots added to it
	// if dot cost = -1
	// if ghost cost = 10
	// Can we add in a priority queue ***
	// How do we know what is the best route ***
	// hmm, should I have used a tree weighing algorithm?

	/*void findRoute(Vector2 startCell) {

		PriorityQueue<int,Vector2> frontier = new PriorityQueue<int,Vector2> ();
		frontier.Enqueue (startCell,0); 
		Hashtable cameFrom = new Hashtable ();
		Hashtable costSoFar = new Hashtable ();
		cameFrom.Add (startCell, null);
		//costSoFar.Add (Start, 0);
		costSoFar [startCell] = 0;

		while (!frontier.IsEmpty) {
			Vector2 current = frontier.Dequeue ();
			List<Vector2> neighbours = maze.getRoutes (current);
			for (int i=0; i<neighbours.Count; i++) {
				int newCost = costSoFar.GetObjectData(current) + mazeCost(current,neighbours[i]);
				// if we haven't come here before or have a better way then add
				if (!costSoFar.Contains (neighbours[i]) || (newCost < costSoFar.GetObjectData(neighbours[i]))) {
					costSoFar.Add(neighbours[i],newCost);
					int priority = newCost + heuristic(startCell,neighbours[i]);
					frontier.Enqueue(neighbours[i],priority); 
					cameFrom.Add (neighbours[i],current);
				}
			}
			current = goal
				path = [current]
				while current != start:
					current = came_from[current] 
					path.append(current)
		}
	}*/
	Vector3 findRoute (Vector3 current, Vector3 lastCell)
	{
		hasVisited.Clear ();
		int maxScore = -99;
		Vector3 nextCell = currentCell;
		hasVisited [current] = true;
		List<Vector3> neighbours = mazeManager.getRoutes (current);

		for (int i=0; i<neighbours.Count; i++) {
			int neighbourScore = getScore (neighbours [i],1);
			if (neighbours [i] == lastCell) {
				neighbourScore--;
			}
			if (neighbourScore > maxScore) {
				nextCell = neighbours [i];
				maxScore = neighbourScore;
			}
		}
		return nextCell;
	}

	int getScore (Vector3 current,int distance)
	{
		hasVisited [current] = true;
		List<Vector3> neighbours = mazeManager.getRoutes (current);
		int maxScore = -99;
		int thisScore = mazeScore (current) / (distance * distance);
		for (int i=0; i<neighbours.Count; i++) {
			if (!hasVisited.Contains (neighbours [i])) {
				int neighbourScore = getScore (neighbours [i],distance+1);
				if (neighbourScore > maxScore) {
					maxScore = neighbourScore;
				}
			}
		}
		return (thisScore + Mathf.Max (maxScore,0));
	}

}

