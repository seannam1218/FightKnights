using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrosshairController : MonoBehaviourPunCallbacks
{
    Vector3 mousePos;
    Vector3 mouseWorldPos;

    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) {
            return;
        } 
        
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mousePos.z+10));

        this.transform.position = mouseWorldPos;
    }
}
