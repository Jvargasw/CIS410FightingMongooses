using UnityEngine;
using System.Collections;

public class SetPlayerRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.rotation = Quaternion.AngleAxis (270f, new Vector3 (1, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
