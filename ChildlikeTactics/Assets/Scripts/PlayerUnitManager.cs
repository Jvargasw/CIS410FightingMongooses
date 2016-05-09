using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerUnitManager : MonoBehaviour {

	public List<PlayerUnit> units;

	private Map map;

	private Text turnText;
	private Text healthText;

	public PlayerUnit activeUnit;

	// Use this for initialization
	void Start () {
		units = new List<PlayerUnit>();

		map = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Generate>().map;

		turnText = GameObject.Find("TurnText").GetComponent<Text>();
		healthText = GameObject.Find("HealthText").GetComponent<Text>();

		StartCoroutine (ActivateUnit ());
	}
	
	IEnumerator ActivateUnit() {
		//wait for a unit to be added to units, then set activeUnit to that unit and start its PlayerTurn coroutine
		while (activeUnit == null) {
			if (units.Count >= 1) {
				activeUnit = units [0];
				print("activeUnit initialized");
				StartCoroutine (activeUnit.PlayerTurn ());
				print ("PlayerTurn started");
				yield break;
			} else {
				print ("units empty");
				yield return null;
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
