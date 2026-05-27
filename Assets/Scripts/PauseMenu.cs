using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
   public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           TogglePause();
        }
    }

 public void TogglePause()
        {
        if (!GameIsPaused )
        Pause();
       else Resume();
    }

public void Resume()
    {
    pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Resume clicked");
    }
    void Pause()
    {
       pauseMenuUI.SetActive(true);
    Time.timeScale = 0f;
    GameIsPaused = true;
    }


   }
