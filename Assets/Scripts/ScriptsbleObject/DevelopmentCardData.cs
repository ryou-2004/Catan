using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/DevelopmentCardData", fileName = ("DevelopmentCardData"))]
public class DevelopmentCardData : ScriptableObject
{
    public List<DevelopmentCard> sheet;
    [System.Serializable]
    public class DevelopmentCard
    {
        public string cardName;
        public GameObject obj;
        public int id;
    }
}
