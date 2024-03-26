using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] PlayImageSoundAndText[] scenes;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] AudioSource audioSource;

    private float timeToNext=0f;
    [SerializeField] public int numberOfScenes;
    [SerializeField] public string loadSceneAfter;
    private int currentScene=0;
    private bool skip=false;
    public void Skip() {
        skip=true;
        SceneManager.LoadScene(loadSceneAfter);
    }
    // Update is called once per frame
    void Update()
    {
        timeToNext-=Time.deltaTime;
        if(timeToNext<0f){
            currentScene++;
            if(currentScene>numberOfScenes || skip){
                SceneManager.LoadScene(loadSceneAfter);
                
            }
            else{
            timeToNext=scenes[currentScene-1].clip.length;
            audioSource.PlayOneShot(scenes[currentScene-1].clip);
            text.SetText(scenes[currentScene-1].text);
            rawImage.GetComponent<RawImage>().texture=scenes[currentScene-1].texture;
            }
        }
    }
    [System.Serializable]
    public class PlayImageSoundAndText{
        public Texture texture;
        public string text;
        public AudioClip clip;
    }
}
