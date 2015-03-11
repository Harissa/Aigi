using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MazeBuilder
// Randomly generates a different maze of a particular size
// Uses the Recursive division algorithm
// adapted from Ruby code at http://weblog.jamisbuck.org/2011/1/12/maze-generation-recursive-division-algorithm

public class MazeBuilder
{

	private const int _rowDimension = 0;
	private const int _columnDimension = 1;
	private const int HORIZONTAL = 1;
	private const int VERTICAL = 2;

	public Maze maze { get; private set; }

	public MazeBuilder ()
	{

	}

	public int choose_orientation (int width, int height)
	{
		if (width < height) {
			return HORIZONTAL;
		} else {
			if (height < width) {
				return VERTICAL;
			} else {
				return (Random.value > 0.5f) ? HORIZONTAL : VERTICAL;
			}
		}
	}

	private void divide (int[,] grid, int x, int y, int width, int height, int orientation)
	{
		int px2, py2;

		if ((width >= 2) && (height >= 2)) {
			bool horizontal = (orientation == HORIZONTAL);
				
			// where will the wall be drawn from?
			int wx = x + (horizontal ? 0 : Random.Range (0, width - 2));
			int wy = y + (horizontal ? Random.Range (0, height - 2) : 0);
				
			// where will the passage through the wall exist?
			int px = wx + (horizontal ? Random.Range (0, width) : 0);
			int py = wy + (horizontal ? 0 : Random.Range (0, height));

			// second passage through the wall?
			if (Random.Range (0, 1) == 0) {
				px2 = wx + (horizontal ? Random.Range (0, width) : 0);
				py2 = wy + (horizontal ? 0 : Random.Range (0, height));
			} else {
				px2 = px;
				py2 = py;
			}
				
			// what direction will the wall be drawn?
			int dx = horizontal ? 1 : 0;
			int dy = horizontal ? 0 : 1;
				
			// how long will the wall be?
			int length = horizontal ? width : height;
				
			// what direction is perpendicular to the wall?
			Directions dir = horizontal ? Directions.E : Directions.S;
				
			for (int each=0; each<length; each++) {
				if (((wx != px) && (wx != px2)) || ((wy != py) && (wy != py2))) {
					grid [wy, wx] |= (int)dir;
				}

								
				wx += dx;
				wy += dy;
			}
				
			int nx = x;
			int ny = y;
			int w = horizontal ? width : wx - x + 1;
			int h = horizontal ? wy - y + 1 : height;
			divide (grid, nx, ny, w, h, choose_orientation (w, h));
				
			nx = horizontal ? x : wx + 1;
			ny = horizontal ? wy + 1 : y;
			w = horizontal ? width : x + width - wx - 1;
			h = horizontal ? y + height - wy - 1 : height;
			divide (grid, nx, ny, w, h, choose_orientation (w, h));

		}
		
		
	}
			
	public Maze Initialise (int width, int height)
	{
		var maze = new Maze ();
		maze.create (width, height);
		maze.fillValue (0);
		maze.drawPerimeter ();
		return maze;
	}

	public Maze Generate (int width, int height)
	{
		maze = Initialise (width, height);
		divide (maze.cells, 0, 0, maze.width, maze.height, choose_orientation (maze.width, maze.height));
		return maze;
	}







}
