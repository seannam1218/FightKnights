using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CrosshairController : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10);
        transform.position = mousePos;
    }
}
