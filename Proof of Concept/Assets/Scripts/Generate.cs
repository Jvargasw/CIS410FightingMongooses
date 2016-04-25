using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum TileType
{
    UNWALKABLE,
    WALKABLE,
    PLAYER,
    ENEMY,
    BOSS,
    NONE
};

public class Map
{
    private int width;
    private int height;
    private TileType[,] grid;

    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new TileType[width, height];
    }

    public TileType getTileAt(int height, int width)
    {
        return grid[height, width];
    }

    public void setTileAt(int height, int width, TileType type)
    {
        grid[height, width] = type;
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }
}

public class Room
{
	/* A data class for holding room information. */

	public int width = -1;
	public int height = -1; 
	public int x = -1;
	public int y = -1;
	public Vector3 topRightCorner;
	public Vector3 bottomRightCorner;
	public Vector3 bottomLeftCorner;
    
	public Room(int width, int height, int x, int y)
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
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    public int minRoomSize      = 10;
	public int maxRoomSize      = 20;
    public int hallwayHeight    = 4;
    public int numRooms         = 3;
    public int mapHeight        = 64;
    public int mapWidth         = 64;

    public int absoluteMinX   = 0;
    public int absoluteMinY   = 0;
	public float spriteSize     = 1;
 
    private Transform boardHolder;
    private Transform hallwayHolder;
    private int curHallway;
    private int curRoom;

	private List<Room> rooms = new List<Room>();

	void Start() 
	{
		int minX  = absoluteMinX;
		int maxX  = 5;

		int minY  = absoluteMinY;
		int maxY  = 20;

        curRoom     = 1;
        curHallway  = 1;

        Map map = new Map(mapHeight, mapHeight);

        // Generate each room
        for (int i = 0; i < numRooms; i++)
        {
            print("Min X: " + minX + " Max X: " + maxX);
            int randX = Random.Range(minX, maxX);
            int randY = Random.Range(minY, maxY);

            int randWidth = Random.Range(minRoomSize, maxRoomSize);
            int randHeight = Random.Range(minRoomSize, maxRoomSize);

            // Try to make sure that no rooms ever collide. There's probably a better way to do this. 
            minX = randX + randWidth;
            maxX += randWidth;

            generateRoom(map, randWidth, randHeight, randX, randY);
            curRoom++;
        }

        // Render the map, starting at (0.0, 0.0)
        renderMap(map, 0.0f, 0.0f);
    }

	/* void connectRooms(List<Room> rooms)
	{
		// Attempt to connect all the rooms. Currently only one room formation is supported. 

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
	} */

    void renderMap(Map map, float startX, float startY)
    {
        boardHolder = new GameObject("Board").transform;
        GameObject instance;

        float currentX = startX;
        float currentY = startY;

        for (int i = 0; i < map.getWidth(); i++)
        {
            for (int j = 0; j < map.getHeight(); j++)
            {
                TileType curTile = map.getTileAt(i, j);
                if(curTile == TileType.NONE || curTile== TileType.UNWALKABLE)
                {
                    instance = (GameObject)Instantiate(unwalkablePrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                }
                else if(curTile == TileType.WALKABLE)
                {
                    instance = (GameObject)Instantiate(walkablePrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                }
                else if(curTile == TileType.PLAYER)
                {
                    instance = (GameObject)Instantiate(walkablePrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                    Instantiate(playerPrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                }
                else if(curTile == TileType.ENEMY)
                {
                    instance = (GameObject)Instantiate(enemyPrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                }
                else if(curTile == TileType.BOSS)
                {
                    instance = (GameObject)Instantiate(bossPrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                }
                else
                {
                    instance = null;
                }

                currentX += spriteSize;
                instance.transform.SetParent(boardHolder);
            }

            currentX = startX;
            currentY += spriteSize;
        }
    }
		
	Room generateRoom(Map map, int roomHeight, int roomWidth, int startX, int startY)
	{
        /* Generate a room given the specifications and return a new Room. 
        */
        
		int currentX = startX;
		int currentY = startY;
        bool spawnedPlayer = false;

        print("Generating a new room at: " + startX + "," + startY + " with WxH: " + roomWidth + "x" + roomHeight);

		Room room = new Room(roomWidth, roomHeight, startX, startY);

		for(int i = startX; i < startX + roomHeight; i++)
		{
			for(int j = startY; j < startY + roomWidth; j++)
			{
                /* Handle drawing pathable sprites on the interior and unpathable sprites on the exterior. */
				if(i != startX && i != (startX + roomHeight) - 1 && j != startY && j != (startY + roomWidth) - 1)
				{
                    if (!spawnedPlayer && curRoom == 1)
                    {
                        spawnedPlayer = true;
                        map.setTileAt(i, j, TileType.PLAYER);
                    }
                    else
                    {
                        map.setTileAt(i, j, TileType.WALKABLE);
                    }
				}

                currentX += 1;
               

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
            currentY += 1;
		}

		return room;
	}
}
