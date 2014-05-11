using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotentialTower : MonoBehaviour
{
    public Vector2 Location;
    public float TotalDistance;
    public Dictionary<LineSegment, float> SegmentDistances = new Dictionary<LineSegment, float>();

    public void ComputeTotal()
    {
        TotalDistance = 0;

        foreach (var kvp in SegmentDistances)
            TotalDistance += kvp.Value;

        if (SegmentDistances.Count == 0)
            TotalDistance = (int)AIPLACEMENTERRORS.PathOutsideRange;
    }
}