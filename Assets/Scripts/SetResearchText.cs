using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetResearchText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI researchText;
    // Start is called before the first frame update
    void Start()
    {
        researchText.SetText("Research speed: " + GlobalVariables.Instance.researchRate*100 + "%");
    }

    
}
