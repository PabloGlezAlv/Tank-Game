using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("TankBattle");
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
