﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerUnitManager : MonoBehaviour {

	private FollowActiveUnit unitIndicator;

	public List<PlayerUnit> units;
	public List<IEnumerator> routines = new List<IEnumerator>();

	private Map map;

	public PlayerUnit activeUnit;
	public int activeUnitIndex = 0;

	// Use this for initialization
	void Awake () {

		units = new List<PlayerUnit>();

		map = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Generate>().map;

		unitIndicator = GameObject.Find ("ActiveUnitIndicator").GetComponent<FollowActiveUnit>();

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
		if ((Input.GetKeyDown (KeyCode.Tab) && map.playerInRoomWithEnemies())) {

            setUnit(activeUnitIndex+1);
		}
	}

    public void setUnit(int index) {
        StopCoroutine(routines[activeUnitIndex]);
        activeUnitIndex = index;
        if (activeUnitIndex > units.Count - 1) {
            activeUnitIndex = 0;
        }
        map.setActivePlayer(activeUnitIndex);
        activeUnit = units[activeUnitIndex];
		unitIndicator.UpdateActiveUnit (activeUnit);
        StartCoroutine(routines[activeUnitIndex]);
    }

    public void updateMap(int index, int x, int y) {
        map.setActivePlayer(index);
        if (!map.movePlayerTo(x, y)) {
            print("Error with following player on grid.");
        }
        map.setActivePlayer(activeUnitIndex);
    }
}
