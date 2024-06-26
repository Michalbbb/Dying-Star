using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Play(){
        GlobalVariables.Instance.forceRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public void ReturnToMainScene(){
        SceneManager.LoadScene("MainScene");
    }
    public void Quit(){
        Application.Quit();
        Debug.Log("Player has left.");
    }
    
}
