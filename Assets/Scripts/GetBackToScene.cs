using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetBackToScene : MonoBehaviour
{
    [SerializeField]public string sceneName;

    [SerializeField] KeyCode key;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key)){
            SceneManager.LoadScene(sceneName);
        }
    }
}
