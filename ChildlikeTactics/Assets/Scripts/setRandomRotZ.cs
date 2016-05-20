using UnityEngine;
using System.Collections;

public class setRandomRotZ : MonoBehaviour {

	void Start () {
		transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 0, 1));
		transform.rotation *= Quaternion.AngleAxis(-90f, new Vector3(1, 0, 0));
	}
}
