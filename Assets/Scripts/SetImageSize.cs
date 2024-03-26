using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImageSize : MonoBehaviour
{

    [SerializeField] RawImage rawImage;
    [SerializeField] Canvas targetSize;

    // Start is called before the first frame update
    void Start()
    {
        float x=targetSize.GetComponent<RectTransform>().sizeDelta.x;
        float y=targetSize.GetComponent<RectTransform>().sizeDelta.y;
        if(rawImage!=null && targetSize !=null){
            rawImage.GetComponent<RectTransform>().sizeDelta=new Vector2(x,y);
        }
    }

    
}
