using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	Transform cameraTransform;

	void Start(){
		cameraTransform = Camera.main.transform;
	}

	// Update is called once per frame
	void Update () {
		transform.rotation = cameraTransform.rotation;
	}
}
