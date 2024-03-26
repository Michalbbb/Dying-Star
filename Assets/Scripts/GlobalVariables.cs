using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class GlobalVariables {
private static GlobalVariables instance = null;
    private static readonly object padlock = new object();

    GlobalVariables()
    {
    }

    public static GlobalVariables Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GlobalVariables();
                }
                return instance;
            }
        }
    }
    public int currentTechnology=0; // 0 means no technology
    public bool changeTechnology=false;
    public int currentInfo=0;
    public bool changeInfoHighlight=false;

    public bool refreshAvailableSystems=true;

    public const int AVAILABLE_CLASSES=2;
    public  string[] CLASS_NAMES = { "A_class", "K_class" };
    
    public int[] AVAILABLE_VARIANTS={2,3};

    public List<GenerateSystem> availableSpaceSystems= new List<GenerateSystem>();
    public GenerateSystem chosenSpaceSystem;
    public int unlockedSystemAlternatives=3;
}
