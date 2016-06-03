using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageDealtTextScript : MonoBehaviour {

	private Vector3 startPosition;
	private GameObject canvas;

	// Use this for initialization
	void Start () {
		canvas = GameObject.Find ("Canvas");
		transform.SetParent (canvas.transform, false);
		StartCoroutine (DisableAfterSeconds ());
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(new Vector3(0, 15 * Time.deltaTime, 0));
	}

	private IEnumerator DisableAfterSeconds() {
		yield return new WaitForSeconds (2.5f);
		Destroy (gameObject);
	}

	public void SetText(int damage) {
		GetComponent<Text>().text = "-" + damage.ToString () + "!";
	}


}
