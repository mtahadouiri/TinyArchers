using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoinbtScript : MonoBehaviour
{
    Transform target;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.gameOver)
        {
            target = GameManager.instance.activeRoad.transform;

            Vector3 targetPostition = new Vector3(target.position.x,
                                            transform.position.y,
                                            target.position.z);
            transform.LookAt(targetPostition);
            transform.Rotate(Vector3.up, -90);
        }
    }
}
