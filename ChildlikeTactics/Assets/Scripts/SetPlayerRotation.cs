using UnityEngine;
using System.Collections;

public class SetPlayerRotation : MonoBehaviour {

	void Start () {
		transform.rotation *= Quaternion.AngleAxis (270f, new Vector3 (1, 0, 0));
	}
}
