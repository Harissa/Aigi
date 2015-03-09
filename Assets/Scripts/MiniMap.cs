using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Minimap
// Manages the Mini map and draws walls, the player and ghosts on it as appropriate

public class MiniMap : MonoBehaviour
{

		public GameObject wallPrefab;
		public GameObject player;
		public GameObject playerIcon;
		public GameObject ghostIcon;
		public GameObject mainMap;
		private MazeManager mazeManager;
		private bool visible;
		public int cellHeight;
		public int cellWidth;
		private float halfCellWidth, halfCellHeight;
		private Vector3 offset;
		private List<GameObject> ghostIcons;
		// Use this for initialization
		void Start ()
		{
				mazeManager = mainMap.GetComponent<MazeManager> ();
				setVisible (false);
				halfCellWidth = cellWidth / 2.0f;
				halfCellHeight = cellHeight / 2.0f;
		}

		public void setVisible (bool isVisible)
		{
				visible = isVisible;
				// make all children invisible or visible
				foreach (Transform aaa in transform)
						if (aaa.gameObject.renderer)
								aaa.gameObject.renderer.enabled = visible; 

				if (visible) {
						makeGhostIcons ();
				}
		}
		public void centreMap() {
		offset = new Vector3 (-(mazeManager.width / 2.0f), 0, -(mazeManager.height / 2.0f));
		for (int i=0; i<ghostIcons.Count; i++) {
			ghostIcons [i].GetComponent<GhostIcon> ().setOffset (offset);
		}
	}

		void makeGhostIcons ()
		{
				List<GameObject> ghosts = mazeManager.getGhosts ();
				ghostIcons = new List<GameObject> ();
				for (int i=0; i<ghosts.Count; i++) {
						GameObject newIcon = (GameObject)Instantiate (ghostIcon, Vector3.zero, Quaternion.identity);
						newIcon.transform.parent = transform;
						newIcon.GetComponent<GhostIcon> ().ghost = ghosts [i];
						ghostIcons.Add (newIcon);
						
				}
		}

	
		// Update is called once per frame
		void Update ()
		{
				if (visible) {
						Vector3 playerPos = player.transform.position;
						playerIcon.transform.localPosition = player.transform.position+offset;
						//Vector2 cellPos = new Vector2 (Mathf.Round (playerPos.x + (mazeManager.width / 2.0f) - 0.5f), Mathf.Round (playerPos.z + (mazeManager.height / 2.0f) - 0.5f));
						Vector2 cellPos = new Vector2 (Mathf.Round (playerPos.x), Mathf.Round (playerPos.z));

						if (!mazeManager.currentMaze.hasVisited ((int)cellPos.x, (int)cellPos.y)) {
								drawCell ((int)cellPos.x, (int)cellPos.y);
								mazeManager.currentMaze.setVisited ((int)cellPos.x, (int)cellPos.y, true);
						}
						
				}
		}

		void drawCell (int x, int y)
		{
				Maze cm = mazeManager.currentMaze;
				// offsets for each side of the cell
	
				Vector3 wallSize = wallPrefab.renderer.bounds.size;
				Vector3 Npos = new Vector3 (0, 0, -halfCellHeight + wallSize.z / 2);
				Vector3 Spos = new Vector3 (0, 0, halfCellHeight - wallSize.z / 2);
				Vector3 Wpos = new Vector3 (-halfCellWidth + wallSize.z / 2, 0, 0);
				Vector3 Epos = new Vector3 (halfCellWidth - wallSize.z / 2, 0, 0);
				if (cm.hasDirection (x, y, Directions.N)) {
						makeWall (x, y, Npos, Vector3.zero);
				}
				if (cm.isValidCell (x, y - 1)) {
						if (cm.hasDirection (x, y - 1, Directions.S)) {
								makeWall (x, y - 1, Spos, Vector3.zero);
						}
				}

				if (cm.hasDirection (x, y, Directions.S)) {
						makeWall (x, y, Spos, Vector3.zero);
				}
				if (cm.isValidCell (x, y + 1)) {
						if (cm.hasDirection (x, y + 1, Directions.N)) {
								makeWall (x, y + 1, Npos, Vector3.zero);
						}
				}

				if (cm.hasDirection (x, y, Directions.W)) {
						makeWall (x, y, Wpos, new Vector3 (0, 90f, 0));
				}
				if (cm.isValidCell (x - 1, y)) {
						if (cm.hasDirection (x - 1, y, Directions.E)) {
								makeWall (x - 1, y, Epos, new Vector3 (0, 90f, 0));
						}
				}

				if (cm.hasDirection (x, y, Directions.E)) {
						makeWall (x, y, Epos, new Vector3 (0, 90f, 0));
				}
				if (cm.isValidCell (x + 1, y)) {
						if (cm.hasDirection (x + 1, y, Directions.W)) {
								makeWall (x + 1, y, Wpos, new Vector3 (0, 90f, 0));
						}
				}


		}


// Adds the wall prefab to the scene
		void makeWall (int x, int y, Vector3 wallOffset, Vector3 rotate)
		{
				//Vector3 position = new Vector3 (cellWidth * (0.5f + x - (mazeManager.width / 2.0f)), -1.5f, cellHeight * (0.5f + y - (mazeManager.height / 2.0f))) + transform.position;
		Vector3 position = new Vector3 (cellWidth * x, -1.5f, cellHeight * y) + transform.position + offset;

		GameObject newWall = (GameObject)Instantiate (wallPrefab, position + wallOffset, Quaternion.Euler (rotate));
				
				newWall.transform.parent = transform;
		}
}
