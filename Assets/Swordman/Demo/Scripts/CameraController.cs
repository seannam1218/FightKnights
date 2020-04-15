using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


    public static CameraController Instance;

    public GameObject Target;
    public int smoothValue =2;
    public float posY = (float) -0.5;
    public Vector2 mousePos;
    public float pullValue = 10;

    // Use this for initialization
    public Coroutine my_co;

    void Start()
    {
     
    }


    void Update()
    {
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10);
  
        Vector3 targetPos = new Vector3(
            Target.transform.position.x + (mousePos.x/Screen.width - (float)0.5) * pullValue, 
            Target.transform.position.y + posY + (mousePos.y/Screen.height - (float)0.5) * pullValue, 
            -10);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*smoothValue);
    }



}
