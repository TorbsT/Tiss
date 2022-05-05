using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public RectCalculator RectCalculator => rectCalculator;
    public RectTransform TooltipTransform => tooltipTransform;
    public RectTransform HealthbarTransform => healthbarTransform;
    public static UI Instance { get; private set; }

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private RectTransform tooltipTransform;
    [SerializeField] private RectTransform healthbarTransform;
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
            if (startPause)
            {
                PauseSystem.Instance.Pause();
            } else
            {
                PauseSystem.Instance.Resume();
            }
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
