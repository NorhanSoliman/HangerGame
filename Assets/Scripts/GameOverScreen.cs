using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameOverScreen : MonoBehaviour
{
    public void SetUp()
    {
        gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void DisplayMainMenu()
    {
        SceneManager.LoadScene("MainMenu");

    }
    public void ExitGameButton()
    {
        Application.Quit();
        //Debug.Log("Quit Game");
    }
}
