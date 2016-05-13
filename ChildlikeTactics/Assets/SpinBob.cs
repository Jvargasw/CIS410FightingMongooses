using UnityEngine;
using System.Collections;

public class SpinBob : MonoBehaviour {

	float bounce = 0f;

	public Vector3 rotateRate;
	public float bounceRate;
	public float bounceDamping = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (rotateRate);

		transform.position += new Vector3 (0, 0, Mathf.Sin(Mathf.Deg2Rad*bounce)*(1/bounceDamping) );

		bounce += bounceRate;
		if (bounce > 360) {
			bounce = 0;
		}
	}
}
