using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Maze Manager
// Manages drawing the maze and
// putting the ghosts and dots into it
public class MazeManager : MonoBehaviour
{

	public GameObject wallPrefab;
	public GameObject dotPrefab;
	public GameObject blinkyPrefab;
	public GameObject pinkyPrefab;
	public GameObject inkyPrefab;
	public GameObject clydePrefab;
	public GameObject floorTilePrefab;
	public GameObject ghostPrefab;
	public int width;
	public int height;
	public int cellHeight;
	public int cellWidth;
	private float halfCellWidth, halfCellHeight;
	public Maze currentMaze;
	private MazeBuilder builder;
	private List<GameObject> ghosts;
	private GameObject boss;

	void Start ()
	{
		halfCellWidth = cellWidth / 2.0f;
		halfCellHeight = cellHeight / 2.0f;
		builder = new MazeBuilder ();

	}

	public int createNewMaze (int newLevel)
	{
		Debug.Log ("creating new maze " + newLevel);
		width = getMazeSize (newLevel);
		height = width;
			
		removeCurrentMaze ();
		// generates maze shape
		currentMaze = builder.Generate (width, height);
		// adds maze prefabs to the scene
		drawMaze (currentMaze);
		createDots ();
		createGhosts (newLevel);
		return width * height;
	}

	public List<GameObject> getGhosts ()
	{
		return ghosts;
	}

	public  List<Vector3> getRoutes (Vector3 centreCell)
	{
		return currentMaze.getRoutes (centreCell);
	}

	public bool hasVisited (int x, int y)
	{
		return currentMaze.hasVisited (x, y);
	}

	public void eatenPill (Vector3 pillPosition)
	{
		currentMaze.setVisited (Mathf.RoundToInt (pillPosition.x), Mathf.RoundToInt (pillPosition.z), true);
	}

	void removeCurrentMaze ()
	{
		GameObject[] allWalls;
		allWalls = GameObject.FindGameObjectsWithTag ("wall");
		foreach (GameObject wall in allWalls) {
			Destroy (wall);
		}
	}

	int getMazeSize (int level)
	{
		int size = Mathf.Clamp (level + 3, 4, 10);
		if (level == 1) {
			size = 3;
		}
		return size;
	}

	void createDots ()
	{
		//Vector3 offset = new Vector3 (halfCellWidth, 0, halfCellWidth);
		Vector3 offset = new Vector3 (0, 0, 0);
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GameObject newPill = makePill (x, y, offset, Vector3.zero);
				newPill.GetComponent<Pill> ().index = x * height + y;
			}
		}
	}

	void createGhosts (int newLevel)
	{
		int noOfGhosts;
		ghosts = new List<GameObject> ();
		if (newLevel == 1) {
			noOfGhosts = 0;
		} else {
			noOfGhosts = newLevel - 1;
		}
		// Todo
		//noOfGhosts = 0;

		for (int i=0; i<noOfGhosts; i++) {
			ghosts.Add (makeGhost (i+1));
		}

		setGhostPositions ();
	}
	// Not very efficient
	public bool isGhostPosition(int x,int y) {
		Vector2 bossPosition = new Vector2 (Mathf.Round (boss.transform.position.x), Mathf.Round (boss.transform.position.z));
		if ((bossPosition.x == x) && (bossPosition.y == y)) {
			return true;
		} else {
			for(int i=0;i<ghosts.Count;i++) {
				if ((Mathf.Round (ghosts[i].transform.position.x)==x) && (Mathf.Round (ghosts[i].transform.position.z)==y)) {
					return true;
				}
			}
			return false;
		}
	}
	public List<Vector3> getAllGhostPositions() {
		List<Vector3> positions = new List<Vector3> ();
		positions.Add (new Vector3 (Mathf.Round (boss.transform.position.x), 0, Mathf.Round (boss.transform.position.z)));
		for (int i=0; i<ghosts.Count; i++) {
			positions.Add (new Vector3 (Mathf.Round (ghosts [i].transform.position.x), 0, Mathf.Round (ghosts [i].transform.position.z)));
		}
		return positions;
	}
		             



	// Goes through the Maze data structure and creates prefab blocks in the right places and orientation
	void drawMaze (Maze drawMaze)
	{
		Vector3 wallSize = wallPrefab.renderer.bounds.size;
		//Debug.Log ("wall size =" + wallSize);
		// offsets for each side of the cell
		Vector3 Npos = new Vector3 (0, 0, -halfCellHeight + wallSize.z / 2);
		Vector3 Spos = new Vector3 (0, 0, halfCellHeight - wallSize.z / 2);
		Vector3 Wpos = new Vector3 (-halfCellWidth + wallSize.z / 2, 0, 0);
		Vector3 Epos = new Vector3 (halfCellWidth - wallSize.z / 2, 0, 0);
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				makeFloor (x, y);
				if (drawMaze.hasDirection (x, y, Directions.N)) {
					makeWall (x, y, Npos, Vector3.zero);
				}
				if (drawMaze.hasDirection (x, y, Directions.S)) {
					makeWall (x, y, Spos, Vector3.zero);
				}
				if (drawMaze.hasDirection (x, y, Directions.W)) {
					makeWall (x, y, Wpos, new Vector3 (0, 90f, 0));
				}
				if (drawMaze.hasDirection (x, y, Directions.E)) {
					makeWall (x, y, Epos, new Vector3 (0, 90f, 0));
				}
			}
		}
	}

	void makeFloor (int x, int y)
	{
		Vector3 offset = Vector3.zero;
		Vector3 rotate = Vector3.zero;
		Vector3 position = new Vector3 (cellWidth * x, -0.5f, cellHeight * y) + transform.position;
		GameObject newFloor = (GameObject)Instantiate (floorTilePrefab, position + offset, Quaternion.Euler (rotate));
		GameObject newCeiling = (GameObject)Instantiate (floorTilePrefab, position + offset + new Vector3 (0, 1.5f, 0), Quaternion.Euler (rotate));
		newCeiling.transform.parent = transform;
		newFloor.transform.parent = transform;

	}
		
	// Adds the wall prefab to the scene
	void makeWall (int x, int y, Vector3 offset, Vector3 rotate)
	{
				
		Vector3 position = new Vector3 (cellWidth * x, 0, cellHeight * y) + transform.position;

		GameObject newWall = (GameObject)Instantiate (wallPrefab, position + offset, Quaternion.Euler (rotate));
		newWall.transform.parent = transform;
		//Debug.Log ("Make wall at " + (position + offset).ToString ());
	}

	GameObject makePill (int x, int y, Vector3 offset, Vector3 rotate)
	{
		//Vector3 position = new Vector3 (cellWidth * (x - (width / 2.0f)), 0, cellHeight * (y - (height / 2.0f))) + transform.position;
		Vector3 position = new Vector3 (cellWidth * x, 0, cellHeight * y) + transform.position;

		GameObject newDot = (GameObject)Instantiate (dotPrefab, position + offset, Quaternion.Euler (rotate));
		newDot.transform.parent = transform;
		return newDot;
	}

	GameObject makeGhost (int i)
	{
	
		GameObject prefab = pinkyPrefab;
		if (i == 0)
			prefab = blinkyPrefab;
		else if (i == 1)
			prefab = pinkyPrefab;
		else if (i == 2)
			prefab = inkyPrefab;
		else if (i == 3)
			prefab = clydePrefab;
		else {
			int r = Random.Range (1, 3);
			if (r == 0)
				prefab = blinkyPrefab;
			else if (r == 1)
				prefab = pinkyPrefab;
			else if (r == 2)
				prefab = inkyPrefab;
			else if (r == 3)
				prefab = clydePrefab;
		}
				
				  
		GameObject newGhost = (GameObject)Instantiate (prefab, Vector3.zero, Quaternion.identity);
		newGhost.transform.parent = transform;
		return newGhost;
	}

	public void setGhostPositions ()
	{
		for (int i=0; i<ghosts.Count; i++) {
			setGhostPosition (ghosts[i]);
			ghosts [i].GetComponent<GhostBillboard> ().clearStack ();
		}
		boss = (GameObject)GameObject.Find ("BossGhost");
		setGhostPosition(boss);
	}
	void setGhostPosition(GameObject theGhost) {
		Vector2 pos = Vector2.zero;
		switch (Random.Range (0, 3)) {
		case 0:
			pos = new Vector2 (Random.Range (0, width - 1), 0);
			break;
		case 1:
			pos = new Vector2 (Random.Range (0, width - 1), height - 1);
			break;
		case 2:
			pos = new Vector2 (0, Random.Range (0, height - 1));
			break;
		case 3:
			pos = new Vector2 (width - 1, Random.Range (0, height - 1));
			break;
		}
		
		Vector3 position = new Vector3 (cellWidth * pos.x, 0, cellHeight * pos.y) + transform.position;
		theGhost.transform.position = position;// + offset;
	}
		
		public void enableGhostSounds (bool areEnabled)
	{
		for (int i=0; i<ghosts.Count; i++) {
			ghosts [i].GetComponent<GhostBillboard> ().enableSound (areEnabled);
		}
	}


	

}
