using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType {
    HP,
    DMG,
	DEF
};

public class ItemController : MonoBehaviour {

    public int setType = -1;//-1 = unset, 0 = HP, 1 = DMG, 2 = DEF, add as needed
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
		else if(setType == 2){
			type = ItemType.DEF;
		}
        //stats[0] = 30; //will vary with item
    }
	
	void Update () {
	
	}

    public ItemType GetItemType() {
        return type;
    }
}
