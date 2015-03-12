using UnityEngine;
using System.Collections;

// Different states of the game
public enum GameStates
{
	intro,
	playing,
	died,
	gameover,
	levelComplete,
	paused
}
// Main game controller
// Controls the states of the game and shows the user interface
public class GameController : MonoBehaviour
{

	public GameObject startGUI;
	public GameObject pauseGUI;
	public GameObject gameGUI;
	public GameObject completeGUI;
	public GameObject failGUI;
	public GameObject dieGUI;
	public MazeManager maze;
	public GUIText scoreText;
	public GUIText levelText;
	public GUIText livesText;
	public GUIText arghText;
	public GUIText completeText;
	public GUIText reachedText;
	public GameStates state;
	public bool started = false;
	public bool paused = false;
	public int lives;
	public int pillsInWorld;
	public int level;
	public AudioClip dieSound;
	public AudioClip successSound;
	//public int targetScore;
	
	public string levelToLoad;
	
	void Awake ()
	{
		DontDestroyOnLoad (this);
		DontDestroyOnLoad (startGUI);
		DontDestroyOnLoad (pauseGUI);
		DontDestroyOnLoad (gameGUI);
		DontDestroyOnLoad (completeGUI);
		DontDestroyOnLoad (failGUI);
		DontDestroyOnLoad (dieGUI);
		DontDestroyOnLoad (scoreText);
		DontDestroyOnLoad (levelText);
		DontDestroyOnLoad (livesText);
		DontDestroyOnLoad (arghText);
		DontDestroyOnLoad (successSound);
		Application.LoadLevel (levelToLoad);
	}

	// Use this for initialization
	void Start ()
	{
		pauseGUI.SetActive (false);
		startGUI.SetActive (true);
		gameGUI.SetActive (false);
		dieGUI.SetActive (false);
		failGUI.SetActive (false);
		completeGUI.SetActive (false);
		pillsInWorld = 999;
		Time.timeScale = 0;
		Screen.showCursor = false;
		Random.seed = 42; // TODO make sure that levels are the same each time
		startGame ();
	}

	void startGame ()
	{
		level = 2;//1;
		lives = 3;
		updateLevel ();
		updateLives ();
		started = false;
		state = GameStates.intro;
		failGUI.SetActive (false);
		startGUI.SetActive (true);


		loadLevel ();
	
	}
	
	void nextLevel ()
	{
		level++;
		updateLevel ();
		startLevel ();
	}
	
	void loadLevel ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	void prepareLevel ()
	{
		Debug.Log ("prepare level");		
		maze = (MazeManager)GameObject.Find ("MazeDrawer").GetComponent (typeof(MazeManager));
		pillsInWorld = maze.createNewMaze (level);
		setPlayerPos ();
		setMiniMapActive (true);
	}

	void setMiniMapActive (bool isActive)
	{
		MiniMap miniMap = (MiniMap)GameObject.Find ("Minimap").GetComponent (typeof(MiniMap));
		miniMap.setVisible (isActive);
		miniMap.centreMap ();
		miniMap.drawWholeMap ();
	}

	void setPlayerPos ()
	{
		PlayerControls player = (PlayerControls)GameObject.Find ("Player").GetComponent (typeof(PlayerControls));
		//Player.transform.position = new Vector3 (-0.5f + (maze.width % 2) / 2.0f, 0, -0.5f + (maze.height % 2) / 2.0f);
		player.setStartPos (Mathf.Round (maze.width / 2.0f), Mathf.Round (maze.height / 2.0f));
	
	}
	
	void startLevel ()
	{
		Time.timeScale = 1;
		prepareLevel ();

		started = true;
		state = GameStates.playing;
		completeGUI.SetActive (false);
		startGUI.SetActive (false);
		gameGUI.SetActive (true);
	}
	
	void pause ()
	{
		paused = true;
		Time.timeScale = 0;
		pauseGUI.SetActive (true);
		gameGUI.SetActive (false);
	}
	
	void unpause ()
	{
		Time.timeScale = 1;
		paused = false;
		pauseGUI.SetActive (false);
		gameGUI.SetActive (true);
	}
	
	void levelCompleted ()
	{	
		//loadLevel ();

		if (state != GameStates.levelComplete) {
			Debug.Log ("play success sound");
			GameObject player = GameObject.Find ("Player");
			AudioSource.PlayClipAtPoint (successSound, player.transform.position);
		}
		state = GameStates.levelComplete;
		pillsInWorld = 999;
		completeText.text = "Level " + level.ToString () + " Complete";
		completeGUI.SetActive (true);
				
		Invoke ("doLoadLevel", 1);

				
	}

	void doLoadLevel ()
	{
		loadLevel ();

		Time.timeScale = 0;
	}

	public void playerDied ()
	{
		GameObject player = GameObject.Find ("Player");
		AudioSource.PlayClipAtPoint (dieSound, player.transform.position);
		MazeManager mazeManager = (MazeManager)GameObject.Find ("MazeDrawer").GetComponent (typeof(MazeManager));
		mazeManager.enableGhostSounds (false);
		gameGUI.SetActive (false);
		lives--;
		updateLives ();
		if (lives == 0) {
			gameOver ();
		} else {
			dieGUI.SetActive (true);
			Time.timeScale = 0;
			state = GameStates.died;
		}
	}

	public void resumeLevel ()
	{
		// reset player position
		setPlayerPos ();
		// reset ghost positions
		maze = (MazeManager)GameObject.Find ("MazeDrawer").GetComponent (typeof(MazeManager));
		maze.setGhostPositions ();
		maze.enableGhostSounds (true);
		dieGUI.SetActive (false);
		gameGUI.SetActive (true);

		state = GameStates.playing;
		Time.timeScale = 1;
	}

	public void gameOver ()
	{
		state = GameStates.gameover;
		reachedText.text = "You reached level " + level.ToString ();
		failGUI.SetActive (true);
		Time.timeScale = 0;
		state = GameStates.gameover;
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Input.GetKeyDown (KeyCode.Space)) {
			switch (state) {
			case GameStates.intro:
				startLevel ();
				break;
			case GameStates.died:
				resumeLevel ();
				break;
			case GameStates.gameover:
				startGame ();
				break;
			case GameStates.levelComplete:
				nextLevel ();
				break;
			}
	   
		}
	  
		if (pillsInWorld <= 0)
			levelCompleted ();
	}
	
	public void updateScore ()
	{
		//Debug.Log ("update score");
		scoreText.text = "Dots left: " + pillsInWorld.ToString ();
	}

	public void updateLives ()
	{
		livesText.text = "Lives: " + lives.ToString ();
	}
	
	public void updateLevel ()
	{
		levelText.text = "Level: " + level.ToString ();
	}
}
