using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerControls : MonoBehaviour {


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
	private bool eatenDot=false;
	
	private CharacterController cController;
	private Animator animator;

	private Vector2 currentCell;
	private Vector2 nextCell;
	private Hashtable hasVisited = new Hashtable ();
	
	// Use this for initialization
	void Start () {
	  cController = GetComponent<CharacterController>();
	  gameController = GameObject.FindWithTag("GameController");
	  animator = GetComponent<Animator>();
	mazeManager = mainMap.GetComponent<MazeManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	  
	}
	public void setStartPos(float x, float y) {
		transform.position = new Vector3 (x, 0, y);
		currentCell.x = Mathf.Round (transform.position.x);
		currentCell.y = Mathf.Round (transform.position.z);
		nextCell = findRoute (currentCell);
		Debug.Log ("Current cell="+currentCell.ToString ()+"next="+nextCell.ToString ());

	}
	
	void FixedUpdate ()
	{
		currentCell.x = Mathf.Round (transform.position.x);
		currentCell.y = Mathf.Round (transform.position.z);
		//if (currentCell.Equals(nextCell)) {
		//	nextCell = findRoute(currentCell);
		//}

	  //float moveHorizontal = Input.GetAxis("Horizontal");
	  //float moveVertical = Input.GetAxis("Vertical");
	  //Vector3 movement = new Vector3(0.0f,0.0f,moveVertical) * speed;
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, nextCell, step);
	  //movement = transform.TransformDirection(movement);
	  //movement.y += yVelocity;
	  //cController.Move(movement * Time.deltaTime);
	  
		// TODO set animation to reflect motion
	//animator.SetFloat("speed",moveVertical*speed);
	  
	  if (Input.GetKey ("escape")) {
	     Application.Quit();
	  }
	  
	  if (Input.GetKey(run))
	  {
	    animator.SetBool("running",true);
	  }
	  else
	  {
	    animator.SetBool("running",false);
	  }
	  
	  Vector3 rot = transform.localEulerAngles;
		// TODO set rotation to reflect motio 
	  //rotation += moveHorizontal * turnSpeed;
	  rot.y = rotation;
	  transform.localEulerAngles = rot;
		if (eatenDot) {
			eatenDot=false;
			AudioSource.PlayClipAtPoint(dotSound, transform.position);
			gameController.GetComponent<GameController> ().pillsInWorld--;
			gameController.GetComponent<GameController> ().updateScore ();
						
		}

	}
	
	void OnTriggerEnter(Collider other)
	{
	  if (other.gameObject.tag == "Monster")
	  {
		  Debug.Log ("Hit a ghost");
		  gameController.GetComponent<GameController>().playerDied();
	  }
	  
	  if (other.gameObject.tag == "Pill")
	  {
	      Debug.Log ("eat pill "+other.gameObject.GetComponent<Pill>().index);

	      if (!other.gameObject.GetComponent<Pill>().collected)
	      {
		  eatenDot=true;
		  other.gameObject.GetComponent<Pill>().onPickup();
	      }
		   		

	  }
    }
	int mazeScore(Vector2 cell) {
		if (mazeManager.hasVisited (Mathf.RoundToInt(cell.x), Mathf.RoundToInt(cell.y))) {
			return 1; // no pill
		} else {
			return -10; // is pill
		}
	}
	// Manhattan distance on a square grid
	int heuristic(Vector2 first, Vector2 second) {
		return Mathf.RoundToInt(Mathf.Abs (first.x - second.x) + Mathf.Abs (first.y - second.y));
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
	Vector3 findRoute(Vector2 current) {
		int maxScore = 0;
		Vector2 nextCell = current;
		List<Vector2> neighbours = mazeManager.getRoutes (current);

		for (int i=0; i<neighbours.Count; i++) {
			int neighbourScore = getScore (neighbours[i]);
			if (neighbourScore>maxScore) {
				nextCell = neighbours[i];
				maxScore = neighbourScore;
			}
		}
		return new Vector3 (nextCell.x, 0, nextCell.y);
	}
	int getScore(Vector2 current) {
		hasVisited [current] = true;
		List<Vector2> neighbours = mazeManager.getRoutes (current);
		int maxScore = 0;
		int thisScore = mazeScore (current);
		for (int i=0; i<neighbours.Count; i++) {
			if (!hasVisited.Contains (neighbours[i])) {
				float neighbourScore = getScore (neighbours [i]);
				if (neighbourScore > maxScore) {
					maxScore = thisScore;
				}
			}
		}
		return (thisScore+maxScore);
	}

}

