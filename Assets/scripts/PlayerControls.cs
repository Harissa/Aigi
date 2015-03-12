using UnityEngine;
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
	public Vector3 lastCell;
//	private Hashtable hasVisited = new Hashtable ();
	
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
		Debug.Log ("Current cell=" + currentCell.ToString () + "next=" + nextCell.ToString ());
		paused = false;

	}
	
	void FixedUpdate ()
	{
		if (!paused) {

		
			if ((Vector3.Distance (nextCell, transform.position) < 0.4)) {
				currentCell.x = Mathf.Round (transform.position.x);
				currentCell.z = Mathf.Round (transform.position.z);
				nextCell = findRoute (currentCell, lastCell);
				lastCell = currentCell;
				Debug.Log ("current cell=" + currentCell.ToString () + "new next cell=" + nextCell.ToString ());
			}


		
			float step = speed * Time.deltaTime;
	
			animator.SetFloat ("speed", speed);



	  
			// TODO set animation to reflect motion

	  
			if (Input.GetKey ("escape")) {
				Application.Quit ();
			}
	  
			//if (Input.GetKey (run)) {
			//	animator.SetBool ("running", true);
			//} else {
			animator.SetBool ("running", false);
			//}

			//find the vector pointing from our position to the target
			Vector3 _direction = (nextCell - transform.position).normalized;
			
			//create the rotation we need to be in to look at the target
			Quaternion _lookRotation = Quaternion.LookRotation (_direction);
			
			//rotate us over time according to speed until we are in the required rotation
			transform.rotation = Quaternion.Slerp (transform.rotation, _lookRotation, Time.deltaTime * turnSpeed);


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
			//Debug.Log ("eat pill " + thePill.index + "pos=" + thePill.transform.position);

			if (!thePill.collected) {
				eatenDot = true;
				thePill.onPickup ();
				//mazeManager.eatenPill(thePill.transform.position);

			}
		   		

		}
	}

	int mazeScore (Vector3 cell)
	{
		int x = Mathf.RoundToInt (cell.x);
		int y = Mathf.RoundToInt (cell.z);
		if (mazeManager.isGhostPosition (x, y)) {
			return -100; // ghost don't go there
		} else {
			if (mazeManager.hasVisited (x, y)) {
				return 1; // no pill
			} else {
				return 10; // is pill
			}
		} 

	}

	int mazeCost (Vector3 cell)
	{
		int x = Mathf.RoundToInt (cell.x);
		int y = Mathf.RoundToInt (cell.z);
		if (mazeManager.isGhostPosition (x, y)) {
			return 100;//-100; // ghost don't go there
		} else {
			if (mazeManager.hasVisited (x, y)) {
				return 10; // no pill
			} else {
				return 1; // is pill
			}
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

	Vector3 findRoute (Vector3 startCell, Vector3 lastCell)
	{

		PriorityQueue<float,Vector3> frontier = new PriorityQueue<float,Vector3> ();
		frontier.Enqueue (startCell, 0); 
		Hashtable cameFrom = new Hashtable ();
		Hashtable costSoFar = new Hashtable ();
		Hashtable scoreSoFar = new Hashtable ();
		cameFrom [startCell] = null;
		costSoFar [startCell] = 0.0f;
		scoreSoFar [startCell] = 0.0f;
		float highestScore = 0.0f;
		float score;
		Vector3 bestDestination = startCell;
		Vector3 next;
		List<Vector3> neighbours;

		while (!frontier.IsEmpty) {
			Vector3 current = frontier.Dequeue ();
			neighbours = mazeManager.getRoutes (current);
			for (int i=0; i<neighbours.Count; i++) {
				next = neighbours[i];
				// Calculate cost
				float newCost = (float)costSoFar [current] + mazeCost (next);
				// if going backwards then add additional cost
				if (next.Equals (lastCell)) {
					newCost+=10;
				}
				// if we haven't come here before or have a better way then add
				if (!costSoFar.Contains (next) || (newCost < (float)costSoFar [next])) {

					costSoFar [next] = newCost;
					// Also calculate a score for each path
					score = mazeScore (next)/newCost;
					scoreSoFar [next] = (float)scoreSoFar [current] + score;
					// If this path has the highest score then remember it
					if ((float)scoreSoFar [next] > highestScore) {
						highestScore = (float)scoreSoFar [next];
						bestDestination = next;
					}
					float priority = newCost;
					frontier.Enqueue (next, priority); 
					cameFrom [next] = current;
				}
			}
		}
		Vector3 nextCell = bestDestination;
		//if (cameFrom.ContainsKey (bestDestination)) {
		bool atEnd = false;
		while (!atEnd) {
			if (bestDestination.Equals (startCell)) {
				atEnd = true;
			} else {
				nextCell = bestDestination;
				bestDestination = (Vector3)cameFrom [bestDestination];
				/*if (cameFrom.ContainsKey (bestDestination) && (bestDestination != null)) {
						bestDestination = (Vector3)cameFrom [bestDestination];
					} else {
						Debug.Log ("came from does not contain " + bestDestination);
						atEnd = true;
					}*/
			}
		}
		//} else {
		//		Debug.Log ("choosing random route");
		//		neighbours = mazeManager.getRoutes (startCell);
		//		nextCell = neighbours [Random.Range (0, neighbours.Count - 1)];
		//	}
		return nextCell;

	}
	/*Vector3 findRoute (Vector3 current, Vector3 lastCell)
	{
		hasVisited.Clear ();
		float maxScore = -99;
		Vector3 nextCell = currentCell;
		hasVisited [current] = true;
		List<Vector3> neighbours = mazeManager.getRoutes (current);

		for (int i=0; i<neighbours.Count; i++) {
			float neighbourScore = getScore (neighbours [i], 1);
			if (neighbours [i] == lastCell) {
				neighbourScore /= 2;
			}
			if (neighbourScore > maxScore) {
				nextCell = neighbours [i];
				maxScore = neighbourScore;
			}
		}
		return nextCell;
	}

	float getScore (Vector3 current, int distance)
	{
		hasVisited [current] = true;
		List<Vector3> neighbours = mazeManager.getRoutes (current);
		float maxScore = 0;
		float thisScore = mazeScore (current) / distance;
		for (int i=0; i<neighbours.Count; i++) {
			if (!hasVisited.Contains (neighbours [i])) {
				float neighbourScore = getScore (neighbours [i], distance + 1);
				if (neighbourScore > Mathf.Abs (maxScore)) {
					maxScore = neighbourScore;
				}
			}
		}
		return (thisScore + maxScore);
	}
*/
}

