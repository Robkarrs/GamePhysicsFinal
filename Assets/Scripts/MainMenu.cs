using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayCup()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayFlow()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitSim()
    {
        Application.Quit();
    }
}
