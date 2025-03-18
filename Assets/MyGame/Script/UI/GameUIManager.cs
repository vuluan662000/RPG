using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    CharacterController characterController;

    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameIsFinish;

    private enum GameUI_State
    {
        GamePlay, GamePause, GameOver, GameIsFinished
    }
    GameUI_State currentState;

    private void Start()
    {
        SwitchUIState(GameUI_State.GamePlay);
    }
    private void Update()
    {
        HandleUI();   
    }

    private void SwitchUIState(GameUI_State newState)
    {
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinish.SetActive(false);

        Time.timeScale = 1;

        switch (newState)
        {
            case GameUI_State.GamePlay:
                break;
            case GameUI_State.GamePause:
                Time.timeScale = 0;
                UI_Pause.SetActive(true);
                break;
            case GameUI_State.GameOver:
                UI_GameOver.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameUI_State.GameIsFinished:
                UI_GameIsFinish.SetActive(true);
                Time.timeScale = 0;
                break;
        }

        currentState = newState;
    }

    private void HandleUI()
    {
        if (StateManager.Instance._currentState is DeadState)
        {
            StartCoroutine(GameOver());
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

    }
    private IEnumerator GameOver()
    {
        WaitForSeconds wait = new WaitForSeconds(2);
        yield return wait;
        SwitchUIState(GameUI_State.GameOver);
    }

    private void TogglePause()
    {
        if (currentState == GameUI_State.GamePlay)
        {
            SwitchUIState(GameUI_State.GamePause);

        }   
        else if (currentState == GameUI_State.GamePause)
        {
            SwitchUIState(GameUI_State.GamePlay);
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeGame()
    {
        SwitchUIState(GameUI_State.GamePlay);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameMenu"); 
    }
}
