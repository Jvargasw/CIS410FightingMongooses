using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeftRightSlidingText : MonoBehaviour {
	
	private RectTransform textTransform;
	public Text text;


	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		textTransform = GetComponent<RectTransform> ();
		StartCoroutine (scroll ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator scroll() {
		while (textTransform.position.x < 900) {
			print (textTransform.position.x.ToString ());
			textTransform.position += new Vector3(400, 0, 0) * Time.deltaTime;
			yield break;
		}
		Destroy (gameObject);
		yield return null;
	}
}
