using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerUnitManager : MonoBehaviour {

	public List<PlayerUnit> units;
	public List<IEnumerator> routines = new List<IEnumerator>();

	private Map map;

	private Text turnText;
	private Text healthText;

	public PlayerUnit activeUnit;
	public int activeUnitIndex = 0;

	// Use this for initialization
	void Awake () {
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
				activeUnit = units [activeUnitIndex];
				print("activeUnit initialized");
                StartCoroutine (routines[activeUnitIndex]);
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
		if (Input.GetKeyDown (KeyCode.Tab)) {
			StopCoroutine (routines[activeUnitIndex]);
			activeUnitIndex += 1;
			if (activeUnitIndex > units.Count - 1) {
				activeUnitIndex = 0;
			}
            map.setActivePlayer(activeUnitIndex);
			activeUnit = units [activeUnitIndex];
			StartCoroutine (routines[activeUnitIndex]);
		}
	}
}
