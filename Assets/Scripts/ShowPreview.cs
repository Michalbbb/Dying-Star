using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowPreview : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    public static RawImage rw;
 
    public void switchToBlack(){
        if(rw!=rawImage){
            rawImage.color=Color.black;
        }
        
    }
    public void switchToTransparent(){
        rawImage.color=Color.white;
        
    }
}
