using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceInCornerOfCanvas : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Canvas target;
    // Start is called before the first frame update
    void Start()
    {
        if(button!=null && target!=null){
            button.GetComponent<RectTransform>().sizeDelta=new Vector2(target.GetComponent<RectTransform>().sizeDelta.x/5, target.GetComponent<RectTransform>().sizeDelta.y/7);
            button.GetComponent<RectTransform>().localPosition=new Vector3(target.GetComponent<RectTransform>().sizeDelta.x/2-button.GetComponent<RectTransform>().sizeDelta.x/2,target.GetComponent<RectTransform>().sizeDelta.y/2-button.GetComponent<RectTransform>().sizeDelta.y/2,0);
        }
    }

}
