using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBG : MonoBehaviour
{
    public PolygonCollider2D [] bgColliders;
    public void InitializeBlockBG()
    {
        bgColliders = GetComponentsInChildren<PolygonCollider2D>();
    }
}
