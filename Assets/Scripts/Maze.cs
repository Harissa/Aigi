using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Maze
// Data structure class which holds the Maze data

//Flags for the wall directions for each cell
public enum Directions
{
	N = 1,
	S = 2,
	E = 4,
	W = 8
}

public class Maze
{

	public int[,] cells;
	public bool[,] visited;
	public int width;
	public int height;

	public Maze ()
	{
	}

	public void create (int newWidth, int newHeight)
	{
		width = newWidth;
		height = newHeight;
		cells = new int[width, height];
		visited = new bool[width, height];
	}

	public void fillValue (int newValue)
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				cells [x, y] = newValue;
				visited [x, y] = false;
			}				
		}
	}

	public void drawPerimeter ()
	{
		for (int x = 0; x < width; x++) {
			cells [x, 0] |= (int)Directions.N;
			cells [x, height - 1] |= (int)Directions.S;
		}
		for (int y = 0; y < height; y++) {
			cells [0, y] |= (int)Directions.W;
			cells [width - 1, y] |= (int)Directions.E;

		}				
	}

	public bool isValidCell (int x, int y)
	{
		return ((x >= 0 && x < width) && (y >= 0 && y < height));
	}
	
	public void fill (int width, int height, int[,] newCells)
	{
		cells = newCells;
	}

	public int contents (int x, int y)
	{
		return cells [x, y];
	}

	public bool hasDirection (int x, int y, Directions flag)
	{
		return ((cells [x, y] & (int)flag) > 0);
	}

	public bool validHasDirection (int x, int y, Directions flag)
	{
		if (isValidCell (x, y)) {
			return hasDirection (x, y, flag);
		} else {
			return false;
		}
	}

	public bool hasVisited (int x, int y)
	{		
		return visited [x, y];
	}

	public void setVisited (int x, int y, bool hasVisited)
	{
		visited [x, y] = hasVisited;
	}

	public bool canSee (Vector3 viewer, Vector3 target,int maxDistance)
	{
		bool canSee = false;
		int increment;
		int difference;
		Directions firstDirection, secondDirection;
		if (Vector3.Distance (viewer, target) < maxDistance) {
			Vector3 roundViewer = new Vector3 (Mathf.Round (viewer.x), Mathf.Round (viewer.y), Mathf.Round (viewer.z));
			Vector3 roundTarget = new Vector3 (Mathf.Round (target.x), Mathf.Round (target.y), Mathf.Round (target.z));
			if (roundViewer.x == roundTarget.x) {
				// x the same 
				canSee = true;
				difference = (int)(roundTarget.y - roundViewer.y);
				if (difference < 0) {
					increment = -1;
					firstDirection = Directions.S;
					secondDirection = Directions.N;
				} else {
					increment = 1;
					firstDirection = Directions.N;
					secondDirection = Directions.S;
				}
				while ((canSee) && (roundViewer.y!=roundTarget.y)) {
					if (hasDirection ((int)roundViewer.x, (int)roundViewer.y, firstDirection) || validHasDirection ((int)roundViewer.x, (int)roundViewer.y + increment, secondDirection)) {
						canSee = false;
					}
					roundViewer.y += increment;
				}
				return canSee;
			}
			if (roundViewer.y == roundTarget.y) {
				// y the same 
				canSee = true;
				difference = (int)(roundTarget.x - roundViewer.x);
				if (difference < 0) {
					increment = -1;
					firstDirection = Directions.W;
					secondDirection = Directions.E;
				} else {
					increment = 1;
					firstDirection = Directions.E;
					secondDirection = Directions.W;
				}
				while ((canSee) && (roundViewer.x!=roundTarget.x)) {
					if (hasDirection ((int)roundViewer.x, (int)roundViewer.y, firstDirection) || validHasDirection ((int)roundViewer.x - 1, (int)roundViewer.y, secondDirection)) {
						canSee = false;
					}
					roundViewer.x += increment;
				}
				return canSee;
			}
		}

		return canSee;

	}

	public List<Vector3> getRoutes (Vector3 centreCell)
	{
		//	Debug.Log ("get routes centre=" + centreCell.ToString ());
		int cellX = Mathf.RoundToInt (centreCell.x);
		int cellY = Mathf.RoundToInt (centreCell.z);
		List<Vector3> routes = new List<Vector3> ();
		if ((!hasDirection (cellX, cellY, Directions.N)) && (!validHasDirection (cellX, cellY - 1, Directions.S))) {
			routes.Add (new Vector3 (cellX, 0, cellY - 1));
		}
		if ((!hasDirection (cellX, cellY, Directions.S)) && (!validHasDirection (cellX, cellY + 1, Directions.N))) {
			routes.Add (new Vector3 (cellX, 0, cellY + 1));
		}
		if ((!hasDirection (cellX, cellY, Directions.W)) && (!validHasDirection (cellX - 1, cellY, Directions.E))) {
			routes.Add (new Vector3 (cellX - 1, 0, cellY));
		}
		if ((!hasDirection (cellX, cellY, Directions.E)) && (!validHasDirection (cellX + 1, cellY, Directions.W))) {
			routes.Add (new Vector3 (cellX + 1, 0, cellY));
		}
		return routes;
	}

}
