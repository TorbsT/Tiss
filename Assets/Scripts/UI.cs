using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public RectCalculator RectCalculator => rectCalculator;
    public static UI Instance { get; private set; }

    [SerializeField] private GameObject pauseMenu;
    private RectCalculator rectCalculator;
    private void Awake()
    {
        Instance = this;
        rectCalculator = GetComponent<RectCalculator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool startPause = !pauseMenu.activeSelf;
            pauseMenu.SetActive(startPause);
        }
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync("Game");
    }
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
