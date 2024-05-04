using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class tooltipManager : MonoBehaviour
{

    public static tooltipManager _instance;
    [SerializeField] public TextMeshProUGUI txt;
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
        vec3.x += 130;
        vec3.y += 40;
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
}
