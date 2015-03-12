using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GhostBehavours
{
	search,
	chase,
	follow,
	flank,
}

public class GhostController : MonoBehaviour
{

	private Camera cam;
	public float speed;
	public string type;
	private Maze maze;
	private MazeManager mazeManager;
	private Directions direction;
	private Vector3 nextCell;
	private Vector3 lastCell;
	private Vector3 currentCell;
	private GhostBehavours state;

	/*
	void Awake () {
	  moveStack = new Stack<Vector3>();
	}
	*/
	// Use this for initialization
	void Start ()
	{
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.LookAt (transform.position + cam.transform.rotation * new Vector3 (0.0f, 0.0f, 1.0f), cam.transform.rotation * Vector3.up);
	}
	/*
	public void pushCellToStack(int x, int y)
	{
	  moveStack.Push(worldPositionOfCell(x,y));
	}
	
	void pushTargetToStack(Vector3 target)
	{
	  moveStack.Push(target);
	}
	
	public void clearStack()
	{
	  moveStack.Clear();
	}
	
	int cellX()
	{
	  return (int) Mathf.Floor(transform.position.x + maze.width/2.0f);
	}
	
	int cellY()
	{
	  return (int) Mathf.Floor(transform.position.z + maze.height/2.0f);
	}
	
	Vector3 worldPositionOfCell(int x, int y)
	{
	  return new Vector3(x-(maze.width/2.0f)+0.5f,0.5f,y-(maze.width/2.0f)+0.5f);
	}
	
	bool isWallInDirection(Directions d)
	{
	  if (d == Directions.N)
	    return isWallNorth();
	  else if (d == Directions.S)
	    return isWallSouth();
	  else if (d == Directions.E)
	    return isWallEast();
	  else if (d == Directions.W)
	    return isWallWest();
	  else
	    return true;
	}
	
	bool isWorldEdgeNorth()
	{
	  return cellY() == 0;
	}
	
	bool isWorldEdgeSouth()
	{
	  return cellY() == maze.height-1;
	}
	
	bool isWorldEdgeEast()
	{
	  return cellX() == maze.width-1;
	}
	
	bool isWorldEdgeWest()
	{
	  return cellX() == 0;
	}
	
	bool isWallNorth()
	{
	  if (isWorldEdgeNorth())
	    return true;
	  return ((maze.hasDirection(cellX(),cellY(),Directions.N)) || (maze.hasDirection(cellX(),cellY()-1,Directions.S)));
	}
	bool isWallSouth()
	{
	  if (isWorldEdgeSouth())
	    return true;
	  return ((maze.hasDirection(cellX(),cellY(),Directions.S)) || (maze.hasDirection(cellX(),cellY()+1,Directions.N)));
	}
	bool isWallEast()
	{
	  if (isWorldEdgeEast())
	    return true;
	  return ((maze.hasDirection(cellX(),cellY(),Directions.E)) || (maze.hasDirection(cellX()+1,cellY(),Directions.W)));
	}
	bool isWallWest()
	{
	  if (isWorldEdgeWest())
	    return true;
	  return ((maze.hasDirection(cellX(),cellY(),Directions.W)) || (maze.hasDirection(cellX()-1,cellY(),Directions.E)));
	}
	
	bool moveInDirection()
	{
	  if (direction == Directions.N)
	  {
	    if (!isWallNorth())
	      pushCellToStack(cellX(),cellY()-1);
	    else
	      return false;
	  }
	  
	  if (direction == Directions.S)
	  {
	    if (!isWallSouth())
	      pushCellToStack(cellX(),cellY()+1);
	    else
	      return false;
	  }
	
	  if (direction == Directions.E)
	  {
	    if (!isWallEast())
	      pushCellToStack(cellX()+1,cellY());
	    else
	      return false;
	  }
	  
	  if (direction == Directions.W)
	  {
	    if (!isWallWest())
	      pushCellToStack(cellX()-1,cellY());
	    else
	      return false;
	  }
	  return true;
	}
	
	void moveRandomOnCollide()
	{
	  if (!moveInDirection())
	  {
	    randomiseDirection();
	  }
	}
	
	void randomiseDirection()
	{
	  int d = (int) Mathf.Floor(Random.Range(0,3.99f));
	  if (d == 0)
	    direction = Directions.N;
	  else if (d == 1)
	    direction = Directions.E;
	  else if (d == 2)
	    direction = Directions.S;
	  else if (d == 3)
	    direction = Directions.W;
	}
	
	void moveRandomly()
	{
	  randomiseDirection();
	  moveInDirection();
	}
	
	Directions getAdjacentDirectionLeft()
	{
	  if (direction == Directions.N)
	    return Directions.W;
	  else if (direction == Directions.S)
	    return Directions.E;
	  else if (direction == Directions.E)
	    return Directions.N;
	  else
	    return Directions.S;
	}
	
	Directions getAdjacentDirectionRight()
	{
	  if (direction == Directions.N)
	    return Directions.E;
	  else if (direction == Directions.S)
	    return Directions.W;
	  else if (direction == Directions.E)
	    return Directions.S;
	  else
	    return Directions.N;
	}
	
	void followLeftWall()
	{
	  if (isWallInDirection(getAdjacentDirectionLeft()))
	  {
	    if (!moveInDirection())
	      direction = getAdjacentDirectionRight();
	  }
	  else
	  {
	    direction = getAdjacentDirectionLeft();
	    moveInDirection();
	  }
	}
	
	void followRightWall()
	{
	  if (isWallInDirection(getAdjacentDirectionRight()))
	  {
	    if (!moveInDirection())
	    {
	      //Debug.Log("Collision change direction from " + (int) direction + " to " + (int)getAdjacentDirectionLeft()+".");
	      direction = getAdjacentDirectionLeft();
	    }
	  }
	  else
	  {
	    //Debug.Log("No wall change direction from " + (int) direction + " to " + (int)getAdjacentDirectionRight()+".");
	    direction = getAdjacentDirectionRight();
	    moveInDirection();
	  }
	}
	
	void onEmptyStack()
	{
	  if (maze == null)
	  {
		mazeManager = GameObject.Find("MazeDrawer").GetComponent<MazeManager>();
	    maze = mazeManager.currentMaze;
	    randomiseDirection();
	  }
	  
	  switch (type)
	  {
	    case "inky":
	      followRightWall();
	      break;
	    case "blinky":
	      followLeftWall();
	      break;
	    case "pinky":
	      followRightWall();
	      break;
	    case "clyde":
	      moveRandomly();
	      break;
	  }
	  
	  //Debug.Log("x:"+cellX()+" y:"+cellY()+" dir:"+(int)direction+" X:"+transform.position.x+" Z:"+transform.position.z+"n:"+isWallNorth()+" e:"+isWallEast()+" s:"+isWallSouth()+" w:"+isWallWest());

	}*/
	public void enableSound (bool isEnabled)
	{
		if (isEnabled) {
			audio.Play ();
		} else {
			audio.Stop ();
		}
	}

	float getRouteScore (Vector3 testCell)
	{
		float score = 0;
		List<Vector3> allGhosts;
		switch (state) {
		case GhostBehavours.search:
			// find distance from each ghost
			// add squares and take square root
			allGhosts = mazeManager.getAllGhostPositions ();
			for (int i=0; i<allGhosts.Count; i++) {
				score += Mathf.Pow (Vector3.Distance (testCell, allGhosts [i]), 2);
			}
			score = Mathf.Sqrt (score);
			break;
		}
		return score;
	}

	Vector3 findRoute (Vector3 startCell, Vector3 lastCell)
	{
		List<Vector3> neighbours = maze.getRoutes (startCell);
		float bestRouteScore = 0.0f;
		Vector3 routeCell = startCell;
		float eachScore;
		for (int i=0; i<neighbours.Count; i++) {
			if (!neighbours [i].Equals (lastCell)) {
				eachScore = 0;
			} else {
				eachScore = getRouteScore (neighbours [i]);
			}
			if (eachScore >= bestRouteScore) {
				routeCell = neighbours [i];
				bestRouteScore = eachScore;
			}
		}
		return routeCell;

	}
	
	void FixedUpdate ()
	{
	  
		if ((Vector3.Distance (nextCell, transform.position) < 0.4)) {
			currentCell.x = Mathf.Round (transform.position.x);
			currentCell.z = Mathf.Round (transform.position.z);
			nextCell = findRoute (currentCell, lastCell);
			lastCell = currentCell;
			//Debug.Log ("current cell=" + currentCell.ToString () + "new next cell=" + nextCell.ToString ());
		}
	  
		Vector3 direction = nextCell - transform.position;
		float distanceToTarget = direction.magnitude;
		direction.Normalize ();
		float moveAmount = Mathf.Clamp (distanceToTarget, 0, speed * Time.deltaTime);
	 
		transform.Translate (direction * moveAmount, Space.World);
	    
	    
	    
	}
}