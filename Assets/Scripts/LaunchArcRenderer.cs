using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float velocity;
    public float angle;
    public int resolution = 10;

    float gravity;
    float radAngle;
    public float y;
    public bool drawRenderer;
    public float bowAngle;
    bool lrEnabled;
    bool fading;

    public bool canFade;
    [Range(0.5f, 3f)]
    public float fadingTime = 1;

    void Awake()
    {
        gravity = Mathf.Abs(Physics.gravity.y);
    }

    void Update()
    {
        bowAngle = GameManager.instance.player.GetComponent<PlayerScript>().arrowPoint.transform.rotation.eulerAngles.x;

        angle = 90 - bowAngle;
        if (drawRenderer)
        {
            RenderArc();
            drawRenderer = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        bowAngle = GameManager.instance.player.GetComponent<PlayerScript>().arrowPoint.transform.rotation.eulerAngles.x;
        angle = 90 - bowAngle;
        RenderArc();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            RenderArc();
        }
    }


    //Populate linerenderer
    void RenderArc()
    {

        List<Vector3> pointsList = new List<Vector3>();
        pointsList = CalculateArcArray();
        Vector3[] points = new Vector3[pointsList.Count];
        points = pointsList.ToArray();
        lrEnabled = true;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    List<Vector3> CalculateArcArray()
    {
        List<Vector3> arr = new List<Vector3>();
        radAngle = Mathf.Deg2Rad * angle;
        //For y0 starting at ground level (0,0)
        // d = (v² * sin(2 * radAngle)) / gravity
        //For y0 with height (0 , y)
        // d = (v² * (1 + sqrt(1+[(2*gravity*y0) / (v²*sin²(radAngle))]) * sin(2 * radAngle)) / (2 * gravity)
        //float maxDistance = (velocity * velocity * Mathf.Sin(2 * radAngle)) / gravity;
        float maxDistanceYOffset = 20 + (velocity * Mathf.Cos(radAngle) / gravity) * (velocity * Mathf.Sin(radAngle) + Mathf.Sqrt((velocity * Mathf.Sin(radAngle)) * (velocity * Mathf.Sin(radAngle) + (2 * gravity * y))));
        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            //arr[i] = CalculateArcPoint(t, maxDistanceYOffset);
            arr.Add(CalculateArcPointY(y, t, maxDistanceYOffset));
            if (arr[i].y <= 1)
                break;
        }
        return arr;
    }



    Vector3 CalculateArcPoint(float t, float maxDistance)
    {

        //The height y of the projectile at distance x is given by
        //y = y_{ 0} +x\tan \theta -{\frac  { gx ^{ 2} } { 2(v\cos \theta) ^{ 2} } }.

        float x = t * maxDistance;
        float y = (x * Mathf.Tan(radAngle)) - ((gravity * x * x) / (2 * (velocity * Mathf.Cos(radAngle)) * (velocity * Mathf.Cos(radAngle))));
        return new Vector3(x, y);
    }

    Vector3 CalculateArcPointY(float yInit, float t, float maxDistanceYOffset)
    {
        float x = t * maxDistanceYOffset;
        float y = yInit + (x * Mathf.Tan(radAngle)) - ((gravity * x * x) / (2 * (velocity * Mathf.Cos(radAngle)) * (velocity * Mathf.Cos(radAngle))));

        return new Vector3(x, y);
    }

    public IEnumerator FadeAfterTime()
    {
        if (lrEnabled && !fading && canFade)
        {
            fading = true;
            yield return new WaitForSeconds(fadingTime);
            lineRenderer.enabled = false;
            fading = false;
        }
    }
    public void Hide()
    {
        StopCoroutine(FadeAfterTime());
        lineRenderer.enabled = false;
        lrEnabled = false;
    }
}
