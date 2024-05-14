using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchToCanvas : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject != null && canvas != null){
            gameObject.GetComponent<RectTransform>().localPosition=new Vector3(canvas.transform.localPosition.x,canvas.transform.localPosition.y,-canvas.GetComponent<RectTransform>().sizeDelta.x/2);
            gameObject.GetComponent<RectTransform>().sizeDelta=new Vector2(canvas.GetComponent<RectTransform>().sizeDelta.x,canvas.GetComponent<RectTransform>().sizeDelta.y);
        
        }
    }

}
