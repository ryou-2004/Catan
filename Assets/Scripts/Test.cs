using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void OnClick(GameObject obj)
    {
        print(obj.gameObject.name);
    }
}
