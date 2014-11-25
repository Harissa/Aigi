﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject startGUI;
	public GameObject pauseGUI;
	public GameObject gameGUI;
	public GameObject completeGUI;
	public GameObject failGUI;
	
	public GUIText scoreText;
	public GUIText levelText;
	
	public bool started = false;
	public bool gameOver = false;
	public bool paused = false;
	public bool levelComplete = false;
	
	public int score = 0;
	public int level = 1;
	public int targetScore;
	
	public string levelToLoad;

	
	void Awake ()
	{
	  DontDestroyOnLoad(this);
	  DontDestroyOnLoad(startGUI);
	  DontDestroyOnLoad(pauseGUI);
	  DontDestroyOnLoad(gameGUI);
	  DontDestroyOnLoad(completeGUI);
	  DontDestroyOnLoad(failGUI);
	  DontDestroyOnLoad(scoreText);
	  DontDestroyOnLoad(levelText);
	  Application.LoadLevel(levelToLoad);
	}

	// Use this for initialization
	void Start ()
	{
	    pauseGUI.SetActive(false);
	    startGUI.SetActive(true);
	    gameGUI.SetActive(false);
	    failGUI.SetActive(false);
	    completeGUI.SetActive(false);
	    Time.timeScale = 0;
	    updateLevel();
	}
	
	void nextLevel()
	{
	    score = 0;
	    level++;
	    started = false;
	    gameOver = false;
	    levelComplete = false;
	    loadLevel();
	    updateLevel();
	    startGUI.SetActive(true);
	    failGUI.SetActive(false);
	    completeGUI.SetActive(false);
	}
	
	void restartLevel()
	{
	    score = 0;
	    started = false;
	    gameOver = false;
	    levelComplete = false;
	    loadLevel();
	    startGUI.SetActive(true);
	    failGUI.SetActive(false);
	    completeGUI.SetActive(false);
	}
	
	void loadLevel()
	{
	    Application.LoadLevel(Application.loadedLevel);
	}
	
	void startLevel()
	{
	    Time.timeScale = 1;
	    started = true;
	    startGUI.SetActive(false);
	    gameGUI.SetActive(true);
	}
	
	void pause()
	{
	    paused = true;
	    Time.timeScale = 0;
	    pauseGUI.SetActive(true);
	    gameGUI.SetActive(false);
	}
	
	void unpause()
	{
	    Time.timeScale = 1;
	    paused = false;
	    pauseGUI.SetActive(false);
	    gameGUI.SetActive(true);
	}
	
	void levelCompleted()
	{	
	    levelComplete = true;
	    completeGUI.SetActive(true);
	    Time.timeScale = 0;
	}
	
	public void levelFailed()
	{
	    gameOver = true;
	    failGUI.SetActive(true);
	    Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	  if (Input.GetKeyDown (KeyCode.Space))
	  {
	    if (levelComplete)
	      nextLevel();
	    else if (gameOver)
	      restartLevel();
	    else if (!started)
	      startLevel();
	    else if (paused)
	      unpause();
	    else
	      pause();
	  }
	  
	  if (score >= targetScore)
	    levelCompleted();
	}
	
	public void updateScore()
	{
	  scoreText.text = "Score: " + score.ToString();
	}
	
	public void updateLevel()
	{
	  levelText.text = "Level: " + level.ToString();
	}
}