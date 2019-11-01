using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Vector3 offset = new Vector3(0, 0, -10f);

    [SerializeField]
    float smoothFactor = 0.05f;

    Transform target;
    Vector3 velocity = Vector3.zero;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.TransformPoint(offset), ref velocity, smoothFactor);
    }
}
