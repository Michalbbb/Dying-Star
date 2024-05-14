using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class tooltipManager : MonoBehaviour
{

    public static tooltipManager _instance;
    [SerializeField] public TextMeshProUGUI txt;
    [SerializeField] public TMP_FontAsset globalFont;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vec3 = Input.mousePosition;
        if(vec3.x+gameObject.GetComponent<RectTransform>().sizeDelta.x*1 < Screen.width )vec3.x+= gameObject.GetComponent<RectTransform>().sizeDelta.x/2+5;
        else vec3.x-= gameObject.GetComponent<RectTransform>().sizeDelta.x/2+5;
        if(vec3.y+gameObject.GetComponent<RectTransform>().sizeDelta.y*1 < Screen.height )vec3.y+= gameObject.GetComponent<RectTransform>().sizeDelta.y/2+5;
        else vec3.y-= gameObject.GetComponent<RectTransform>().sizeDelta.y/2+5;
        transform.position = vec3;
    }
    public void setAndShowToolTip(string text)
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().SetAsLastSibling();
        txt.text = text;
    }
    public void hideToolTip()
    {
        gameObject.SetActive(false);
        txt.text = string.Empty;
    }
    public void refresh(string text){
        hideToolTip();
        setAndShowToolTip(text);
    }
}
