using UnityEngine;
using System.Collections;

public class StatsPanelManager : MonoBehaviour {

	private PlayerUnit unit;

	// Use this for initialization
	void Start () {
		unit = GameObject.Find ("GameManager").GetComponent<PlayerUnitManager> ().activeUnit;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
