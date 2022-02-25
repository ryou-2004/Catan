using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/ResourceData",fileName =("ResourceData"))]
public class ResourceData : ScriptableObject
{
    public List<Resource> sheet;
    [System.Serializable]
    public class Resource
    {
        public string tileName;
        public string resourceName;
        public Material material;
        public int id;
    }
}
