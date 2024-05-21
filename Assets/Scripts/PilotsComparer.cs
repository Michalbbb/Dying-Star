using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotComparer : IComparer<Pilot>
{
    public int Compare(Pilot x, Pilot y)
    {
        if(x.isAnyPointUnspent()&&!y.isAnyPointUnspent()) return -1;
        if(x.isAnyPointUnspent()&&y.isAnyPointUnspent()||!x.isAnyPointUnspent()&&!y.isAnyPointUnspent()){
            return y.getLevel()-x.getLevel();
        }
        return 1;
    }
}