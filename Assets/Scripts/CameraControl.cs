using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Vector3 offset = new Vector3(0, 0, -10f);

    [SerializeField]
    float smoothFactor = 0.05f;

    GameObject target;
    Vector3 velocity = Vector3.zero;

    public GameObject GetTarget()
    {
        return target;
    }

    public void SetTarget(GameObject newTarget)
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
        if (target != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.TransformPoint(offset), ref velocity, smoothFactor);
        }
    }
}
