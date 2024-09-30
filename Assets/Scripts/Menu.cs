using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Play(){
        GlobalVariables.Instance.forceRestart();
        SceneManager.LoadScene("Intro");
    }
    public void ReturnToMainScene(){
        SceneManager.LoadScene("MainScene");
    }
    public void Credits(){
        SceneManager.LoadScene("Credits");
    }
    public void Quit(){
        Application.Quit();
        Debug.Log("Player has left.");
    }
    
}
