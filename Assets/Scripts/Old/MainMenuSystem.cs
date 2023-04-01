using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSystem : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        // IF GETTING NULLREFERENCEEXCEPTIONS:
        // apparently this is a thing with EventSystem according to google.
        // In my case, every error was related to the observer-observed pattern.
        // All of them were fixed by unsubscribing on OnDestroy().
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
