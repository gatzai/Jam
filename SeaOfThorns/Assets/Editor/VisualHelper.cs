using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class VisualHelper
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(BoatMove boat, GizmoType gizmotype)
    {
        if((gizmotype & GizmoType.Selected) != 0)
            Gizmos.color = Color.yellow;
        else
        {
            Gizmos.color = Color.yellow * .5f;
        }

        Gizmos.DrawSphere(boat.transform.position, boat.PickRange);
        foreach(Transform p in boat.transform.Find("Place"))
        {
            Gizmos.color = Color.green * .5f;
            Gizmos.DrawSphere(p.position, .2f);
        }
    }
}
