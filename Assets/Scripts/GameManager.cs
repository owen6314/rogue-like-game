using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float gameOverDelay = 2f;
	public float turnDelay = .1f;
	//单例模式
	public static GameManager instance = null;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;
	private GameObject levelImage;
	private int level = 0;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;
	private BoardManager boardScript;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		enemies = new List<Enemy> ();
		DontDestroyOnLoad (gameObject);
		boardScript = GetComponent<BoardManager> ();
	}
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		level++;
		InitGame ();
	}
	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}
	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
	public void InitGame()
	{
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);
		enemies.Clear ();
		boardScript.SetupScene (level);
	}

	private void HideLevelImage()
	{
		levelImage.SetActive (false);
		doingSetup = false;
	}
	private void EnableLevelImage()
	{
		levelImage.SetActive (true);
	}
	public void GameOver()
	{
		levelText.text = "After " + level + " days, you starved!";
		Invoke ("EnableLevelImage", gameOverDelay);
		enabled = false;
	}


	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0)
		{
			yield return new WaitForSeconds (turnDelay);
		}
		for (int i = 0; i < enemies.Count; i++) 
		{
			enemies [i].MoveEnemy ();
			yield return new WaitForSeconds (enemies [i].moveTime);
		}
		enemiesMoving = false;
		playersTurn = true;
	}

	void Update()
	{
		if (playersTurn || enemiesMoving || doingSetup)
			return;
		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add (script);
	}
}
