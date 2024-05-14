using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManagement : MonoBehaviour
{
    [SerializeField] RawImage  [] slots;
    [SerializeField] TextMeshProUGUI  [] descriptions;
    [SerializeField] RawImage equipmentSpace;
    [SerializeField] GameObject grid;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject area;
    [SerializeField] GameObject scroll;

    public void refreshEquipment(Pilot p=null){
            int childCount = equipmentSpace.transform.childCount;

        
            for (int i = childCount - 1; i >= 0; i--)
            {
            Transform child = equipmentSpace.transform.GetChild(i);
            if (child.name == "Equipment") Destroy(child.gameObject);
            }
            float width=area.GetComponent<RectTransform>().sizeDelta.x;
            float height=Screen.height / 9;
            float SPACE_BETWEEN=Screen.height/36;
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(width,panel.GetComponent<RectTransform>().sizeDelta.y);
            area.GetComponent<RectTransform>().sizeDelta = new Vector2(width,area.GetComponent<RectTransform>().sizeDelta.y);
            scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(width,scroll.GetComponent<RectTransform>().sizeDelta.y);
            grid.GetComponent<GridLayoutGroup>().cellSize=new Vector2(width,height);
            grid.GetComponent<GridLayoutGroup>().spacing=new Vector2(0,SPACE_BETWEEN);

            equipmentSpace.GetComponent<RectTransform>().sizeDelta=new Vector2(width,(height+SPACE_BETWEEN)*GlobalVariables.Instance.equipment.Count-SPACE_BETWEEN); 
            int x=5;
            float y=0;  
            GlobalVariables.Instance.equipment.Sort(new EquipmentComparer());
            foreach(Equipment eq in GlobalVariables.Instance.equipment){
                generateEquipmentButton(equipmentSpace,x,y,width,height,eq,p);
                y-=width+SPACE_BETWEEN;
            }
    }
    public void generateEquipmentButton(RawImage equipmentSpace,float posX,float posY,float width,float height,Equipment eq,Pilot p=null){


        //Base
        GameObject equipment = new GameObject("Equipment");
        RawImage imageComponent = equipment.AddComponent<RawImage>();
        imageComponent.GetComponent<RectTransform>().SetParent(equipmentSpace.transform);
        imageComponent.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0); // Set position
        imageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height); // Set size
        imageComponent.color = Color.black;
        //Icon
        GameObject icon = new GameObject("Portrait");
        RawImage iconComponent = icon.AddComponent<RawImage>();
        iconComponent.GetComponent<RectTransform>().SetParent(equipment.transform);
        iconComponent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        iconComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        iconComponent.texture=eq.getIcon();
        iconComponent.color = new Color(1f,1f,1f,0.3f);
        //Text
        GameObject eqTxt=new GameObject("equipmentTxt");
        TextMeshProUGUI eTxt=eqTxt.AddComponent<TextMeshProUGUI>();
        eTxt.GetComponent<RectTransform>().SetParent(equipment.transform);
        eTxt.GetComponent<RectTransform>().localPosition = new Vector3(0,0, 0);
        eTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        eTxt.alignment=TextAlignmentOptions.Center;
        eTxt.enableAutoSizing=true;
        eTxt.fontSizeMin=8;
        eTxt.font=tooltipManager._instance.globalFont;
        eTxt.SetText(eq.getDescriptionLite());

        if(p==null) return;
        Button buttonComponent = equipment.AddComponent<Button>();
        buttonComponent.onClick.AddListener(() => {p.equipItem(eq);refreshSlots(p);refreshEquipment(p);});
        

    }
    public void refreshSlots(Pilot p){
        if(p==null) return;
            string message = GlobalVariables.Instance.equipment.Count>0 ? null : "In order to build equipment go to factory.";
            for(int i=0;i<slots.Length;i++){

                if(message==null)message= !p.isEquipmentAssigned(i) ? "You can click on piece of equipment(list on the right) to equip it." : "You can unequip item by clicking on it.";
                slots[i].GetComponent<toolTip>().message=message;
                
                slots[i].texture=p.getEquipmentIcon(i);
                descriptions[i].SetText(p.getEquipmentDesc(i));

            }
    }
}
public class EquipmentComparer : IComparer<Equipment>
{
    public int Compare(Equipment x, Equipment y)
    {
        if(x.isAttached()&&!y.isAttached()) return 1;
        if(x.isAttached()&&y.isAttached()||!x.isAttached()&&!y.isAttached()){
            return y.getCompareValue()-x.getCompareValue();
        }
        return -1;
    }
}
