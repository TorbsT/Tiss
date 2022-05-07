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
    [SerializeField] private GameObject deadMenu;
    [SerializeField] private GameObject COck;
    [SerializeField] private RectTransform tooltipTransform;
    [SerializeField] private RectTransform healthbarTransform;
    private RectCalculator rectCalculator;
    private bool dead;
    private void Awake()
    {
        Instance = this;
        rectCalculator = GetComponent<RectCalculator>();
    }
    private void Update()
    {
        if (!dead && Input.GetKeyDown(KeyCode.Escape))
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
    public void Died()
    {
        dead = true;
        deadMenu.SetActive(true);
        COck.SetActive(true);
        PauseSystem.Instance.Pause();
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
