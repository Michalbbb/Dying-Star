using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSystem 
{
    Texture2D image;
    string info;


    public GenerateSystem(){
        int classType= UnityEngine.Random.Range(0,GlobalVariables.AVAILABLE_CLASSES);
        int variant=UnityEngine.Random.Range(1,GlobalVariables.Instance.AVAILABLE_VARIANTS[classType]+1);
        image=Resources.Load<Texture2D>($"Space systems/{GlobalVariables.Instance.CLASS_NAMES[classType]}{variant}");
        int firstLetter=UnityEngine.Random.Range(65,91);
        int secondLetter=UnityEngine.Random.Range(65,91);
        int number=UnityEngine.Random.Range(0,100);
        string name="";
        name+=(char)firstLetter;
        name+=(char)secondLetter;
        name+='-';
        name+=number;
        info=$"{name}({GlobalVariables.Instance.CLASS_NAMES[classType]})";
    }
    public Texture2D getImage(){
        return image;
    }
    public string getInfo(){
        return info;
    }
   
}
