using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolTip : MonoBehaviour
{
    public string message="empty";
    private void OnMouseEnter()
    {
        tooltipManager._instance.setAndShowToolTip(message);
    }
    public void forceRefersh(){
        tooltipManager._instance.refresh(message);
    }
    private void OnMouseExit()
    {
        tooltipManager._instance.hideToolTip();

    }
}
