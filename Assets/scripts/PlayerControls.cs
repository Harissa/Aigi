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
	public AudioClip dotSound;
	public Maze maze;
	
	public KeyCode run;

	private GameObject gameController;
	private bool eatenDot=false;
	
	private CharacterController cController;
	private Animator animator;

	private Vector2 currentCell;
	
	// Use this for initialization
	void Start () {
	  cController = GetComponent<CharacterController>();
	  gameController = GameObject.FindWithTag("GameController");
	  animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	  
	}
	
	void FixedUpdate ()
	{
		currentCell.x = Mathf.Round (transform.position.x);
		currentCell.y = Mathf.Round (transform.position.z);

	  float moveHorizontal = Input.GetAxis("Horizontal");
	  float moveVertical = Input.GetAxis("Vertical");
	  Vector3 movement = new Vector3(0.0f,0.0f,moveVertical) * speed;
	  movement = transform.TransformDirection(movement);
	  movement.y += yVelocity;
	  //cController.Move(movement * Time.deltaTime);
	  animator.SetFloat("speed",moveVertical*speed);
	  
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
	  rotation += moveHorizontal * turnSpeed;
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
	int mazeCost(Vector2 current, Vector2 next) {
		return 1; // TODO fix cost
	}
	int heuristic(Vector2 next) {
		return 1; // TODO fix heuristic
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

	void findRoute(Vector2 startCell) {

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
					int priority = newCost + heuristic(neighbours[i]);
					frontier.Enqueue(neighbours[i],priority); 
					cameFrom.Add (neighbours[i],current);
				}
			}
		}
	}
}
