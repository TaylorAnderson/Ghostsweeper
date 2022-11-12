using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechieFlag : MonoBehaviour
{
    public Vector3 dest = Vector2.zero;
    public Vector3 basePos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      transform.position = Vector3.Lerp(transform.position, (Vector3)dest, 0.2f);   
    }
}
