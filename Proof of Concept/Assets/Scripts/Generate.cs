using UnityEngine;
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

	public float spriteSize     = 1;
 
    private Transform boardHolder;
    private Transform hallwayHolder;
    private int curHallway;
    private int curRoom;


	void Start() 
	{

        int currentX = 0;
        int currentY = 0;

        curRoom     = 1;
        curHallway  = 1;

        Map map = new Map(mapHeight, mapHeight);

        // Generate each room
        for (int i = 0; i < numRooms; i++)
        {
            int randWidth = Random.Range(minRoomSize, maxRoomSize);
            int randHeight = Random.Range(minRoomSize, maxRoomSize);

            generateRoom(map, randWidth, randHeight, currentX, currentY);

            // Randomize if the next room will be above or to the right of the current room.
            // TODO: Add in left or below? 
            if (Random.Range(0, 100) >= 50)
            {
                currentX += randWidth;
            }
            else
            {
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
		
	void generateRoom(Map map, int roomWidth, int roomHeight, int startX, int startY)
	{
        /* Generate a room given the specifications and return a new Room. 
        */

        // Useful values
        int topSide = startY + roomHeight;
        int rightSide = startX + roomWidth; 

        bool spawnedPlayer = false;

        print("Generating a new room at: " + startX + "," + startY + " with WxH: " + roomWidth + "x" + roomHeight);

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
                    else
                    {
                        map.setTileAt(i, j, TileType.WALKABLE);
                    }
				}
			}
		}
	}
}
