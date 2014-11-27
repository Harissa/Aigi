﻿using UnityEngine;
using System.Collections;

//Flags
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
		// Use this for initialization
		public Maze ()
		{
		}
		public void create(int newWidth,int newHeight) {
			width = newWidth;
			height = newHeight;
			cells = new int[width,height];
			visited = new bool[width, height];
		}
		public void fillValue(int newValue) {
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					cells[x, y] = newValue;
					visited[x,y]=false;
				}				
			}
		}
		public void drawPerimeter() {
			for (int x = 0; x < width; x++) {
				cells[x, 0] |= (int)Directions.N;
				cells[x, height-1] |= (int)Directions.S;
			}
			for (int y = 0; y < height; y++)
			{
				cells[0, y] |= (int)Directions.W;
				cells[width-1, y] |= (int)Directions.E;

			}				
	}
	
	
	public void fill (int width, int height,int[,] newCells)
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
	public bool hasVisited(int x,int y) {
				return visited [x, y];
		}
	public void setVisited(int x,int y,bool hasVisited) {
				visited [x, y] = hasVisited;
		}
}
