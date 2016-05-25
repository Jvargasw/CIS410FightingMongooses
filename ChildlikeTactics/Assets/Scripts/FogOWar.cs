using UnityEngine;
using System.Collections.Generic;

public class FogOWar : MonoBehaviour
{
    public GameObject fogOWar;
    public List<int> drawnFog;

    private GameObject[] fog;
    private Transform fogParent;

	void Start ()
    {
        fogParent = new GameObject("Fog").transform;
        fog = new GameObject[Generate.rooms.Count];
        drawnFog = new List<int>();
        drawnFog.Add(0);
        OnPlayerChangeRooms(0);
	}
	
    void OnPlayerChangeRooms(int roomIndex)
    {
        Destroy(fog[roomIndex]);
        foreach (Room room in Generate.rooms)
        {
            int index = Generate.rooms.IndexOf(room);
            if (!drawnFog.Contains(index) && !room.containsPlayer)
            {
                GameObject instance = (GameObject)Instantiate(fogOWar, new Vector3(room.x + room.width / 2, room.y + room.height / 2, 0f), Quaternion.Euler(-90, 0, 0));
                instance.transform.localScale = new Vector3(.9f * (room.width + 2), 3.5f, .9f * (room.height + 2));
                instance.transform.SetParent(fogParent);
                fog[index] = instance;
                drawnFog.Add(index);
            }
        }
    }
}
