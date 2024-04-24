using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource 
{
    int id;
    string name;
    string description;
    int amount;
    int passiveIncome;
    Texture2D image;
    public Resource(int id,string name,string desc,int amount,string img,int passiveIncome=0){
        this.id = id;
        this.name = name;
        this.description = desc;
        this.amount = amount;
        this.passiveIncome=passiveIncome;
        image=Resources.Load<Texture2D>($"Icons/{img}");
    }
    public bool check(int amountToSpend){
        if(amount<amountToSpend) return false;
        return true;
    }
    public void spend(int amountToSpend){
        amount-=amountToSpend;
    }
    public void setAmount(int amount){this.amount=amount; }
    public void skipTime(int months){
        amount+=months*passiveIncome;
    }
    public Texture2D getImage(){
        if(image!=null) return image;
        return Resources.Load<Texture2D>("ph");
    }
    public int getAmount(){
        return amount;
    }
    public string getAmountAsString(){
        string suffix="";
        int amountCopy=amount;
        string [] suffixes={"K","M","B"}; 
        for(int i=0;i<3;i++){
            if(amountCopy/1000>=1){
                suffix=suffixes[i];
                amountCopy=amountCopy/1000;
            }
            else break;
            

        }
        return amountCopy.ToString()+suffix;
    }
    public string getIncomeAsString(){
        string suffix="";
        int incomeCopy=passiveIncome;
        string [] suffixes={"K","M","B"}; 
        for(int i=0;i<3;i++){
            if(incomeCopy/1000>=1){
                suffix=suffixes[i];
                incomeCopy=incomeCopy/1000;
            }
            else break;
            

        }
        return incomeCopy.ToString()+suffix;
    }
    public string getName(){
        return name;
    }

}
