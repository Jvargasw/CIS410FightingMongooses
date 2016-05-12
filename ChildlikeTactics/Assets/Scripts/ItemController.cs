using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType {
    HP,
    DMG
};

public class ItemController : MonoBehaviour {

    public int setType = -1;//-1 = unset, 0 = HP, 1 = DMG, add as needed
    public int index;
    public List<int> stats = new List<int>();


    private ItemType type;

	void Awake () {
        if (setType == -1) {
            print("ITEM PREFAB ERROR, please set setType on your item prefab");
        }
        if (setType == 0) {
            type = ItemType.HP;
        }
        else if(setType == 1){
            type = ItemType.DMG;
        }
        //stats[0] = 30; //will vary with item
    }
	
	void Update () {
	
	}

    public ItemType GetItemType() {
        return type;
    }
}
