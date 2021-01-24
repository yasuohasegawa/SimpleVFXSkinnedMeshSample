using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private float t = 0f;
    [SerializeField] private Vector3 target = Vector3.zero;
    [SerializeField] private float dist = 3.5f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float height = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime* speed;
        float rad = Mathf.Deg2Rad * t;
        float x = dist*Mathf.Cos(rad);
        float z = dist * Mathf.Sin(rad);

        Vector3 pos = transform.position;
        pos.x = x;
        pos.y = height;
        pos.z = z;
        transform.position = pos;
        transform.LookAt(target);
    }
}
