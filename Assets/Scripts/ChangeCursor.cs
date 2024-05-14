using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCursor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CursorManager.CursorType enterCursorType;
    [SerializeField] private CursorManager.CursorType downCursorType;
    private void OnMouseEnter() {
        CursorManager.Instance.SetActiveCursorType(enterCursorType);
    }
    private void OnMouseDown() {
        CursorManager.Instance.SetActiveCursorType(downCursorType);
    }
    private void OnMouseUp() {
        CursorManager.Instance.SetActiveCursorType(enterCursorType);
        
    }
    // Update is called once per frame
    private void OnMouseExit() {
         CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
    }

    public void SwitchToDown(){
        CursorManager.Instance.SetActiveCursorType(downCursorType);
    }
    public void SwitchToDownConditionalButton(Button button){
        if(button.interactable==true)CursorManager.Instance.SetActiveCursorType(downCursorType);
        else CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
    }
    public void SwitchToEnterConditionalButton(Button button){
        if(button.interactable==true)CursorManager.Instance.SetActiveCursorType(enterCursorType);
        else CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
    }
    public void SwitchToEnter(){
        CursorManager.Instance.SetActiveCursorType(enterCursorType);

    }
    public void SwitchToDefault(){
     CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
    }
}
