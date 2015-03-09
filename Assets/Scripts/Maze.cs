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

		public bool validHasDirection(int x,int y, Directions flag) {
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
	public List<Vector2> getRoutes(Vector2 centreCell) {
		Debug.Log ("get routes centre=" + centreCell.ToString ());
		int cellX = Mathf.RoundToInt (centreCell.x);
		int cellY = Mathf.RoundToInt (centreCell.y);
		List<Vector2> routes = new List<Vector2> ();
		if ((!hasDirection (cellX, cellY, Directions.N)) && (!validHasDirection (cellX, cellY - 1, Directions.S))) {
			routes.Add (new Vector2 (cellX, cellY - 1));
		}
		if ((!hasDirection (cellX, cellY, Directions.S)) && (!validHasDirection (cellX, cellY + 1, Directions.N))) {
			routes.Add (new Vector2 (cellX, cellY + 1));
		}
		if ((!hasDirection (cellX, cellY, Directions.W)) && (!validHasDirection (cellX - 1, cellY, Directions.E))) {
			routes.Add (new Vector2 (cellX - 1, cellY));
		}
		if ((!hasDirection (cellX, cellY, Directions.E)) && (!validHasDirection (cellX + 1, cellY, Directions.W))) {
			routes.Add (new Vector2 (cellX, cellY + 1));
		}
		return routes;
	}

}
