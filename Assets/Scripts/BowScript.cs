using UnityEngine;

public class BowScript : MonoBehaviour
{


    public float maxAngle;
    public float minAngle;
    // Update is called once per frame
    void Update()
    {
        Mathf.Clamp(transform.localEulerAngles.z, minAngle, maxAngle);
    }
}
