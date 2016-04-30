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

    public int renderHeight { get; set; }
    public int renderWidth { get; set; }

    private TileType[,] grid;

    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new TileType[width, height];
    }

    public TileType getTileAt(int x, int y)
    {
        return grid[x, y];
    }

    public void setTileAt(int x, int y, TileType type)
    {
        grid[x, y] = type;
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
    private int numEnemies;
    private int x;
    private int y;
    private int height;
    private int width;

    public Room(int x, int y, int width, int height, int numEnemies)
    {
        this.x = x;
        this.y = y;
        this.numEnemies = numEnemies;
        this.width = width;
        this.height = height;
    }

    public bool hasEnemies()
    {
        return numEnemies == 0;
    }

    public int getNumEnemies()
    {
        return numEnemies;
    }

    public int getHeight()
    {
        return height;
    }

    public int getWidth()
    {
        return width;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
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
    public int wallHeight       = 1;

	public float spriteSize     = 1;
    public static Map map;
    public static List<Room> rooms;
 
    private Transform boardHolder;
    private Transform hallwayHolder;
    private int curHallway;
    private int curRoom;

	void Awake() 
	{
        int currentX = 0;
        int currentY = 0;
        int randWidth;
        int randHeight;
        int width;
        int height;

        curRoom     = 1;
        curHallway  = 1;

        map = new Map(mapHeight, mapHeight);
        int hallwayDirection = -1;
        rooms = new List<Room>();

        // Generate each room
        for (int i = 0; i < numRooms; i++)
        {

            randWidth = Random.Range(minRoomSize, maxRoomSize);
            randHeight = Random.Range(minRoomSize, maxRoomSize);

            width = Mathf.Min(63 - currentX, randWidth);
            height = Mathf.Min(63 - currentY, randHeight);
            if(width != randWidth)
            {
                print("Width overide: " + width);
            }
            if (height != randHeight)
            {
                print("Height overide: " + height);
            }

            rooms.Add(generateRoom(map, width, height, currentX, currentY, hallwayDirection));

            // Randomize if the next room will be above or to the right of the current room.
            // TODO: Add in left or below? 
            if (Random.Range(0, 100) >= 50)
            {
                hallwayDirection = 0;
                currentX += randWidth;
            }
            else
            {
                hallwayDirection = 1;
                currentY += randHeight;
            }
            curRoom++;
        }

        // Render the map, starting at (0.0, 0.0)
        renderMap(map, 0.0f, 0.0f);
    }

    void renderMap(Map map, float startX, float startY)
    {
        boardHolder = new GameObject("Board").transform;
        

        float currentX = startX;
        float currentY = startY;

        print("Render Height: " + map.renderHeight);
        print("Render Width: " + map.renderWidth);

        /* We got some magic numbers up the ass here. Basically, when divided by 9, the floor *almost* fits, but then we have to multiply that by 5 because Unity planes are 5 units long, 
         * so if we want the bottom left corner to be aligned correctly, it needs to be moved by 5 * length and 5 * height. This is a shitty implementation, but it works for now. 
         */
        GameObject instance = (GameObject)Instantiate(walkablePrefab, new Vector3((map.renderHeight / 9) * 5, (map.renderWidth / 9) * 5, 1), Quaternion.Euler(-90, 0, 0));
        instance.transform.localScale = new Vector3(map.renderHeight / 9, 1, map.renderWidth / 9);

        for (int i = 0; i < map.renderWidth; i++)
        {
            for (int j = 0; j < map.renderHeight; j++)
            {
                TileType curTile = map.getTileAt(i, j);
                switch(curTile)
                {
                    case TileType.NONE:
                        // Woooo. Abusing logical fall throughs!

                    case TileType.UNWALKABLE:
                        instance = (GameObject)Instantiate(unwalkablePrefab, new Vector3(currentX, currentY, 0), transform.rotation);

                        // Make the wall 2 units high. 
                        // FIXME: This currently only increases it's height by 0.5 (I believe), because unity scales both ends.
                        Vector3 unwalkableScale = instance.transform.localScale = new Vector3(1, 1, 2);
                        break;

                    case TileType.PLAYER:
                        Instantiate(playerPrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                        break;

                    case TileType.ENEMY:
                        Instantiate(enemyPrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                        break;

                    case TileType.BOSS:
                        Instantiate(bossPrefab, new Vector3(currentX, currentY, 0), transform.rotation);
                        break;
                }
                currentX += spriteSize;
                instance.transform.SetParent(boardHolder);
            }
            currentX = startX;
            currentY += spriteSize;
        }
    }
		
	Room generateRoom(Map map, int roomWidth, int roomHeight, int startX, int startY, int hallwayDirection)
	{
        /* Generate a room given the specifications and return a new Room. 
        */

        // Useful values
        int topSide = startY + roomHeight;
        int rightSide = startX + roomWidth; 

        bool spawnedPlayer = false;
        bool spawnedBoss = false;
        int numEnemies = (int)Mathf.Floor(Mathf.Log(numRooms));


        // FIXME: Apparently our x and y are getting swapped somewhere. We should fix that. 
        // Create a hallway on the top of the room previous room to the bottom of the current room.
        if(hallwayDirection == 0)
        {
            for(int i = startX - 5; i < startX + 5; i++)
            {
                for (int j = startY + 1; j < startY + hallwayHeight; j++)
                {
                    map.setTileAt(i, j, TileType.WALKABLE);
                }
            }          
        }
  
        // Create a hallway on the right side of the previous room to the left side of the current room.
        else if(hallwayDirection == 1)
        {
            for(int i = startX + 1; i < startX + hallwayHeight; i++)
            {
                for (int j = startY - 5; j < startY + 5; j++)
                {
                    map.setTileAt(i, j, TileType.WALKABLE);
                }
            }
        }

		for(int i = startX; i < startX + roomWidth; i++)
		{
			for(int j = startY; j < startY + roomHeight; j++)
			{
                /* Handle drawing pathable sprites on the interior and unpathable sprites on the exterior. */
				if(i != startX && i != rightSide - 1 && j != startY && j != topSide- 1)
				{
                    if (!spawnedPlayer && curRoom == 1)
                    {
                        spawnedPlayer = true;
                        map.setTileAt(i, j, TileType.PLAYER);
                    }
                    else if (curRoom != 1 && curRoom != numRooms && numEnemies != 0)
                    {
                        map.setTileAt(i, j, TileType.ENEMY);
                        numEnemies -= 1;
                    }
                    else if (curRoom == numRooms && !spawnedBoss)
                    {
                        map.setTileAt(i, j, TileType.BOSS);
                        spawnedBoss = true;
                    }
                    else
                    {
                        map.renderHeight = Mathf.Max(map.renderHeight, j + 2);
                        map.setTileAt(i, j, TileType.WALKABLE);
                    }
				}
			}
            map.renderWidth = Mathf.Max(map.renderWidth, i + 2);
		}
        return new Room(startX, startY, roomWidth, roomHeight, numEnemies);
    }
}
