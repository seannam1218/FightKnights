using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour {

	public static CameraController Instance;

	private float yOffset = -0.5f;
	public GameObject Target;
	public int smoothValue;
	public float pullFactor;
	public float zoomFactor;
	private Vector3 mousePos;
	private Vector3 curCamPos;
	private float distMouseTarget;
	private float defaultCameraZ = -10;
	private float targetViewSize;
	private float defaultCameraViewSize = 4.6f;

	// Use this for initialization
	public Coroutine my_co;

	void Start()
	{
	}


	void Update()
	{
		mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10);
		distMouseTarget = (float) Math.Sqrt(Math.Pow(mousePos.x/Screen.width - 0.5f, 2) + Math.Pow(mousePos.y/Screen.height - 0.5f, 2));

		Vector3 targetPos = new Vector3(
			Target.transform.position.x + (mousePos.x/Screen.width - 0.5f) * pullFactor, 
			Target.transform.position.y + yOffset + (mousePos.y/Screen.height - 0.5f) * pullFactor/1.5f, 
			defaultCameraZ);

		targetViewSize = defaultCameraViewSize + (float)Math.Pow(distMouseTarget, 2)*zoomFactor;

		transform.position += (targetPos - transform.position)/smoothValue;
		GetComponent<Camera>().orthographicSize += (targetViewSize - GetComponent<Camera>().orthographicSize)/smoothValue;
		// transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*smoothValue);
	}



}
