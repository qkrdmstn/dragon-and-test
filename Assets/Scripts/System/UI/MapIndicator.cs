using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapType
{
    A, B, C
}

public class MapIndicator : MonoBehaviour
{
    [SerializeField] MapType myType;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMiniMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeMiniMap()
    {
        
    }
}
