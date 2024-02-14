using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLine : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    public LineRenderer lineRenderer { get { return _lineRenderer ? _lineRenderer : _lineRenderer = GetComponent<LineRenderer>(); } }

    public Color aimColor = Color.red;
    public Color flickerColor = Color.white;

    private void Start()
    {
        ToggleFlicker(false);
    }

    public void UpdateLine(Vector3 origin, Vector3 destination)
    {
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, destination);
    }

    public void ToggleFlicker(bool flicker)
    {
        lineRenderer.startColor = flicker ? flickerColor : aimColor;
        lineRenderer.endColor = flicker ? flickerColor : aimColor;
    }
}
