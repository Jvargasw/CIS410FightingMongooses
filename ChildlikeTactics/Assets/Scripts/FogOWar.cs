using UnityEngine;
using System.Collections.Generic;

public class FogOWar : MonoBehaviour
{
	public GameObject fogOWar, fadedFogOWar;
    public List<int> drawnFog;

    private GameObject[] fog;
    private Transform fogParent;

	void Start ()
    {
        fogParent = new GameObject("Fog").transform;
		fog = new GameObject[Generate.rooms.Count * 2];
        drawnFog = new List<int>();
        drawnFog.Add(0);
        OnPlayerChangeRooms(0);
	}
	

	void OnPlayerChangeRooms(int roomIndex)
    {
        Destroy(fog[roomIndex]);
		Destroy(fog[Generate.rooms.Count + roomIndex]);
        foreach (Room room in Generate.rooms)
        {
            int index = Generate.rooms.IndexOf(room);
            if (!drawnFog.Contains(index) && !room.containsPlayer)
            {
				/*for (int i = 0; i < room.width; i++) {
					for (int j = 0; j < room.height; j++) {
						GameObject instance = (GameObject)Instantiate(fogOWar, new Vector3(room.x + i, room.y - j, 0f), Quaternion.Euler(-90, 0, 0));
						instance.transform.SetParent(fogParent);
						fog[index] = instance;
						drawnFog.Add(index);
					}
				}*/

				GameObject fadedFog = (GameObject)Instantiate(fadedFogOWar, new Vector3(room.x + room.width / 2, room.y + room.height / 2, 0f), Quaternion.Euler(-90, 0, 0));
				GameObject solidFog = (GameObject)Instantiate(fogOWar, new Vector3(room.x + room.width / 2, room.y + room.height / 2, 0f), Quaternion.Euler(-90, 0, 0));
				fadedFog.transform.localScale = new Vector3(.9f * (room.width + 2), 2.8f, .9f * (room.height + 2));
				fadedFog.transform.SetParent(fogParent);
				solidFog.transform.localScale = new Vector3(.9f * (room.width - 1.5f), 3.3f, .9f * (room.height - 2.5f));
				solidFog.transform.SetParent(fogParent);
				fog[index] = fadedFog;
				fog [Generate.rooms.Count + index] = solidFog;
                drawnFog.Add(index);
                
            }
        }
    }
}
