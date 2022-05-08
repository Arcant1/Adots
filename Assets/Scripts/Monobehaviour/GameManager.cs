using System;

using Unity.Entities;

using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
	public GameObject titleUI, gameUI, winUI, loseUI;
	public TMPro.TextMeshProUGUI bulletsUI, scoreTextUI;
	public uint level, lives;
	public static GameManager instance;
	private uint points = 0;

	private void Awake()
	{
		instance = this;
		//Reset();
	}

	//public void Reset()
	//{
	//	LoadLevel(0);
	//	SwitchUI(titleUI);
	//	score = 0;
	//	level = 0;
	//	AudioManager.instance.PlayMusicRequest("title");
	//}
	//public void Win()
	//{
	//	SwitchUI(winUI);
	//	AudioManager.instance.PlayMusicRequest("win");
	//}
	//public void GameOver()
	//{
	//	SwitchUI(loseUI);
	//	AudioManager.instance.PlayMusicRequest("lose");
	//}
	//public void InGame()
	//{
	//	SwitchUI(gameUI);
	//	AudioManager.instance.PlayMusicRequest("game");
	//}
	//public void LoseLife()
	//{
	//	lives--;
	//	if (lives < 0) GameOver();
	//}
	//public void SwitchUI(GameObject newUI)
	//{
	//	titleUI.SetActive(false);
	//	gameUI.SetActive(false);
	//	loseUI.SetActive(false);
	//	winUI.SetActive(false);

	//	newUI.SetActive(true);
	//}
	//public void SetTotalPallets(int pallets)
	//{
	//	pelletsUI.SetText($"Pallets: {pallets}");
	//}

	public void SetBulletsPerSecond(int bullets)
	{
		bulletsUI.SetText($"Bullets present in the scene: {bullets}");
	}
	public void AddPoints(uint newPoints)

	{
		points += newPoints;
		scoreTextUI.SetText($"Points {points}");

	}


	//public void LoadLevel(int newLevel)
	//{
	//	UnloadLevel();
	//	level = newLevel;
	//	SceneManager.LoadScene($"Level{level}", LoadSceneMode.Additive);
	//}
	//public void NextLevel()
	//{
	//	InGame();
	//	LoadLevel(level + 1);
	//}
	//public void UnloadLevel()
	//{
	//	EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
	//	foreach (Entity entity in em.GetAllEntities())
	//	{
	//		em.DestroyEntity(entity);
	//	}
	//	if (SceneManager.GetSceneByName($"Level{level}").isLoaded)
	//		SceneManager.UnloadSceneAsync($"Level{level}");
	//}
}
