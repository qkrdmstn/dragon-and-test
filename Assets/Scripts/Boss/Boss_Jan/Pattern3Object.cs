using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern3Object : MonoBehaviour
{
    public GameObject dangerZone;
    public GameObject safeZone;
    public PolygonCollider2D safeZoneCollider;
    public List<Collider2D> inRangeSafeZone;

    public void ObjectSetActive(bool flag)
    {
        dangerZone.SetActive(flag);
        safeZone.SetActive(flag);
    }

    public bool IsInSafeZone()
    {
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.SetLayerMask(LayerMask.GetMask("Player", "PlayerOnDamaged"));
        if (Physics2D.OverlapCollider(safeZoneCollider, filter, inRangeSafeZone)!=0)
            return true;
        else
            return false;
    }

    public void SetSafeZone(float degree)
    {
        safeZone.transform.rotation = Quaternion.EulerAngles(0, 0, degree);
    }
}
