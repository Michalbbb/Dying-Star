using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetSpaceSystems : MonoBehaviour
{
    [SerializeField] RawImage [] alternatives;
    [SerializeField] TextMeshProUGUI [] alternativesText;



    private void Start() {
        if(GlobalVariables.Instance.refreshAvailableSystems){
            GlobalVariables.Instance.refreshAvailableSystems=false;
            GlobalVariables.Instance.chosenSpaceSystem=null;
            GlobalVariables.Instance.availableSpaceSystems.Clear();
            for(int i=0;i<GlobalVariables.Instance.unlockedSystemAlternatives;i++){
                GenerateSystem NewSystem=new GenerateSystem();
                GlobalVariables.Instance.availableSpaceSystems.Add(NewSystem);

            }

        }
        showSystems();
        
        //image.GetComponent<RawImage>().texture=space.getImage();
    }
    private void showSystems(){
        int iterator=0;
        foreach(GenerateSystem sys in GlobalVariables.Instance.availableSpaceSystems){
            alternatives[iterator].GetComponent<RawImage>().texture=sys.getImage();
            alternativesText[iterator].text=sys.getInfo();
            iterator++;
        }
    }
}
