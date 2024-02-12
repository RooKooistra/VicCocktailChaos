using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGizmos : MonoBehaviour
{
    [Tooltip("Colour of solid Gizmo shape")]
    [SerializeField, Range(0, 1)] private float GizmoRedValue;
    [SerializeField, Range(0, 1)] private float GizmoGreenValue;
    [SerializeField, Range(0, 1)] private float GizmoBlueValue;

    [SerializeField, Range(0.1f, 10)] private float GizmoSize = 1f;
    [SerializeField, Range(0f, 1f)] private float GizmoAlpha = 0.33f;
    [SerializeField, Range(0f, 1f)] private float GizmoWireFrameAlpha = 1f;

    [Tooltip("All Gizmos are cubes by default")]
    public bool changeGizmoToSphere = false;

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(GizmoRedValue, GizmoGreenValue, GizmoBlueValue, GizmoAlpha);
        if (changeGizmoToSphere)
        {
            Gizmos.DrawSphere(transform.position, GizmoSize);
        }
        else
        {
            Gizmos.DrawCube(transform.position, new Vector3(GizmoSize, GizmoSize, GizmoSize));
        }

        Gizmos.color = new Color(GizmoRedValue, GizmoGreenValue, GizmoBlueValue, GizmoWireFrameAlpha);
        if (changeGizmoToSphere)
        {
            Gizmos.DrawWireSphere(transform.position, GizmoSize);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(GizmoSize, GizmoSize, GizmoSize));
        }
    }
}
