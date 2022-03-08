using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : BordObject
{
    [HideInInspector] public bool isDesert;
    [HideInInspector] public int resourceId;
    [HideInInspector] public int token;
    public List<GameObject> aroundCity;
}
