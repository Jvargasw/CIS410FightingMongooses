using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Room
{
	/* A data class for holding room information. */

	public int width = -1;
	public int height = -1; 
	public float x = -1.0f;
	public float y = -1.0f;
	public Vector3 topRightCorner;
	public Vector3 bottomRightCorner;
	public Vector3 bottomLeftCorner;
    
	public Room(int width, int height, float x, float y)
	{
		this.width = width;
		this.height = height;
		this.x = x;
		this.y = y;
	}
}

public class Generate : MonoBehaviour 
{
    public GameObject unwalkablePrefab;
    public GameObject walkablePrefab;

    public int minRoomSize      = 10;
	public int maxRoomSize      = 20;
    public int hallwayHeight    = 4;
    public int numRooms         = 3;

    public float absoluteMinX   = -7.0f;
    public float absoluteMinY   = -3.0f;
	public float spriteSize     = 1;

    private Transform boardHolder;
    private Transform hallwayHolder;
    private int curHallway;
    private int curRoom;

	private List<Room> rooms = new List<Room>();

	void Start() 
	{
		float minX  = absoluteMinX;
		float maxX  = -2.0f;

		float minY  = absoluteMinY;
		float maxY  = 0.0f;

        curRoom     = 1;
        curHallway  = 1;

		for(int i = 0; i < numRooms; i++)
		{
			int width = Random.Range(minRoomSize, maxRoomSize);
			int height = Random.Range(minRoomSize, maxRoomSize);

			float roomX = Random.Range(minX, maxX);
			float roomY = Random.Range(minY, maxY);

			print("Spawning room at x:" + roomX + " y:" + roomY + " With width:" + width + " and height:" + height);

			Room newRoom = generateRoom(width, height, roomX, roomY);
			rooms.Add(newRoom);
			minX = newRoom.topRightCorner.x;
			maxX = minX + Random.Range(1.0f, 5.0f);
		}

		connectRooms(rooms);
	}

	void connectRooms(List<Room> rooms)
	{
		/* Attempt to connect all the rooms. Currently only one room formation is supported. */

		Room prevRoom = null;
        GameObject instance;
        hallwayHolder = new GameObject("Hallway" + curHallway).transform;
        curHallway++;

		foreach(Room room in rooms)
		{
			if(prevRoom != null)
			{
				print("PrevRoom bottomRightCorner: " + prevRoom.bottomRightCorner + " Room bottomRightCorner" + room.bottomRightCorner + " Room topRightCorner: " + room.topRightCorner + " Room bottomLeftCotrner: " + room.bottomLeftCorner);
				if(prevRoom.bottomRightCorner.y >= room.bottomRightCorner.y && prevRoom.bottomRightCorner.y + spriteSize*hallwayHeight <= room.topRightCorner.y)
				{
					float currentX = prevRoom.bottomRightCorner.x - spriteSize;
					float currentY = prevRoom.bottomRightCorner.y;

					for(int i = 0; i < hallwayHeight; i++)
					{

						// Make the hallway from the bottom left corner of the previous room to any part of the right side of the next room. Need to divide by spriteSize to conver from Unity units to
						// the amount of sprites we need to draw. Add one extra sprite to make it actually extend into the room. 
						for(int j = 0; j < ((room.bottomLeftCorner.x + spriteSize)- prevRoom.bottomRightCorner.x)/spriteSize; j++)
						{
							if(i == 0 || i == hallwayHeight - 1)
							{
								instance = (GameObject) Instantiate(unwalkablePrefab, new Vector3(currentX, currentY, -1), transform.rotation);
							}
							else
							{
								instance = (GameObject) Instantiate(walkablePrefab, new Vector3(currentX, currentY, -1), transform.rotation);
							}
                            instance.transform.SetParent(hallwayHolder);
							currentX += spriteSize;
						}

						currentX = prevRoom.bottomRightCorner.x - spriteSize;
						currentY += spriteSize;
					}
				}
				
			}

			prevRoom = room;
		}
	}
		
	Room generateRoom(int roomHeight, int roomWidth, float startX, float startY)
	{
        /* Generate a room given the specifications and return a new Room. 
         * FIXME: We should most likely just be generating a big grid in a list, marking each element in it as a certain tile type, then iterate through them rendering them (similarly to how the rougelike was done). 
         */

        boardHolder = new GameObject("Room" + curRoom).transform;
        curRoom++;
        GameObject instance;

		float currentX = startX;
		float currentY = startY;

		Room room = new Room(roomWidth, roomHeight, startX, startY);

		for(int i = 0; i < roomHeight; i++)
		{
			for(int j = 0; j < roomWidth; j++)
			{
                /* Handle drawing pathable sprites on the interior and unpathable sprites on the exterior. */

                /* FIXME: We should be drawing edges, rather than ignoring them. But. Procedural generation is hard. :(
                 * 
				if(i == 0 || i == roomHeight - 1 || j == 0 || j == roomWidth - 1)
				{
					instance = (GameObject)Instantiate(unwalkablePrefab, new Vector3(currentX, currentY, 0), transform.rotation);
				}	
				else 
				{
					instance = (GameObject)Instantiate(walkablePrefab, new Vector3(currentX, currentY, 0), transform.rotation);	
				}*/

                instance = (GameObject)Instantiate(walkablePrefab, new Vector3(currentX, currentY, 0), transform.rotation);	

				currentX += spriteSize;
                instance.transform.SetParent(boardHolder);

				/* Handle grabbing the top-right most sprite/bottom-right most sprite -- used in hallway construction */
                if (i == roomHeight - 1 && j == roomWidth - 1)
				{
					room.topRightCorner = new Vector3(currentX, currentY);
				}
				else if(i == 0 && j == roomWidth - 1)
				{
					room.bottomRightCorner = new Vector3(currentX, currentY);
				}
				else if(i == 0 && j == 0)
				{
					room.bottomLeftCorner = new Vector3(currentX, currentY);
				}
			}

			currentX = startX;
			currentY += spriteSize;
		}

		return room;
	}
}
