using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField]private List<CursorAnimation> cursorAnimationList;

    private CursorAnimation currentCursorAnimation;
    private int currentFrame;
    private float frameTimer;
    private int frameCount;
    public enum CursorType{
        Default,
        Clickable,
        Down
    }

    public static CursorManager Instance {get; private set;}
    void Start()
    {
        SetActiveCursorType(CursorType.Default);
    }
    private void Awake() {
        Instance=this;
    }
    private void Update() {
        frameTimer-=Time.deltaTime;
        if(frameTimer<= 0f){
            frameTimer+=currentCursorAnimation.frameRate;
            currentFrame=(currentFrame+1)%frameCount;
            Cursor.SetCursor(currentCursorAnimation.textureArray[currentFrame],new Vector2(0,0),CursorMode.Auto);
        }


    }
    private void SetActiveCursorAnimation(CursorAnimation cursorAnimation){
        currentCursorAnimation=cursorAnimation;
        currentFrame=0;
        frameTimer=cursorAnimation.frameRate;
        frameCount=cursorAnimation.textureArray.Length;
    }
    public void SetActiveCursorType(CursorType cursorType){
        SetActiveCursorAnimation(GetCursorAnimation(cursorType));
    }
    private CursorAnimation GetCursorAnimation(CursorType cursorType){
        foreach(CursorAnimation cs in cursorAnimationList){
            if(cs.cursorType==cursorType){
                return cs;
            }
        }
        return null;
    }
    [System.Serializable]
    public class CursorAnimation{
        public CursorType cursorType;
        public Texture2D[] textureArray;
        public float frameRate;
        public Vector2 offset;

    }
}
