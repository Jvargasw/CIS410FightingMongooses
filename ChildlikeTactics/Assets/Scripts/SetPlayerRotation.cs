using UnityEngine;
using System.Collections;

public class SetPlayerRotation : MonoBehaviour {

	//this script sets PlayerRenderer's rotation to 270 around x
	void Start () {
		transform.rotation = Quaternion.AngleAxis (270f, new Vector3 (1, 0, 0));
	}
}
