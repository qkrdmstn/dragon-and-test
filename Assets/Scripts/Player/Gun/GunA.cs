using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunA : GunBase
{
    public override void Reload()
    {
        base.Reload();
    }

    public override void Shoot()
    {
        base.Shoot();
        Debug.Log("A Shoot");
    }
}
