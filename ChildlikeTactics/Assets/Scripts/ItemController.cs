using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType {
    HP
};

public class ItemController : MonoBehaviour {

	public int index;
    public List<int> stats = new List<int>();


    private ItemType type = ItemType.HP;

	void Awake () {
        //stats[0] = 30; //will vary with item
    }
	
	void Update () {
	
	}

    public ItemType GetItemType() {
        return type;
    }
}
