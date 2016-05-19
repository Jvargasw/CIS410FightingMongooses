using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public enum TileType
{
    UNWALKABLE,
    WALKABLE,
    PLAYER,
    ENEMY,
    BOSS,
    ITEM,
    NONE
};

public enum Direction
{
    NORTH,
    EAST,
    WEST,
    SOUTH
};

public class Position
{
    /* A dataclass to make holding/passing Positions easier and more
     * succinct.
     * 
     * Args:
     *      int x - The x part of the position
     *      int y - The y part of the position
     */

    public int x { get; set; }
    public int y { get; set; }

    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public class Map
{
    //temp variable for current level. may delete later
    public int currLevel = 1;

    public int width { get; private set; }
    public int height { get; private set; }

    public int renderHeight { get; set; }
    public int renderWidth { get; set; }

    private int activePlayer = 0;
    private List<Position> enemyPositions = new List<Position>();
    private List<Position> itemPositions = new List<Position>();
    private List<Position> playerPositions = new List<Position>();
    public int currentRoomIndex = 0;

    public TileType[,] grid;

    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new TileType[width, height];
        initializeGrid();
    }

    private void initializeGrid()
    {
        /* Initializes the grid to all UNWALKABLE's.
         * 
         * Args:
         *      None
         * 
         * Returns:
         *      Nothing.
         */

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                setTileAt(i, j, TileType.UNWALKABLE);
            }
        }
        return;
    }

    public void setActivePlayer(int index)
    {
        activePlayer = index;
    }

    public void incrementActivePlayer()
    {
        activePlayer++;
    }

    public bool playerInRoomWithEnemies()
    {
        /* Returns whether the romo the player is currently in has
         * any enemies in it, so that we can swap from movement/battle mode.
         * 
         * Args:
         *      None
         *      
         * Returns:
         *      Nothing.
         */

        foreach (Room room in Generate.rooms)
        {
            if(room.containsPlayer)
            {
                return room.hasEnemies();
            }
        }

        return false;
    }

    private void updatePlayerRoom()
    {
        /* Updates which room the player is currently in, so that
         * playerInRoomWithEnemies actually returns an accurate value.
         * 
         * Args:
         *      None
         *      
         * Returns:
         *      Nothing
         */

        if (Generate.rooms.Count == 0 || currentRoomIndex == Generate.rooms.Count)
        {
            // Assume that rooms haven't been generated yet.
            return;
        }

        foreach (Room room in Generate.rooms)
        {
            if(room.containsPlayer)
            {
                // Set the previous room we were in to not containing the player
                room.containsPlayer = false;
            }

            if (room.x <= playerPositions[0].x && room.x + room.width >= playerPositions[0].x && room.y <= playerPositions[0].y && room.y + room.height >= playerPositions[0].y)
            {
                MonoBehaviour.print("Assuming the player is in room: " + Generate.rooms.IndexOf(room) + " has enemies?: " + room.hasEnemies());
                // Set .containsPlayer to true. Break because once we found the room, we don't need to keep searching
                room.containsPlayer = true;
                break;
            }
        }
    }

    public int getPlayerCollidedWith(int x, int y)
    {
        /* Returns the index in the enemy list of the enemy the player would collide with if the 
         * move to coordinates (x, y) is performed.
         * 
         * Args:
         *      int x - The x coordinate of the move to perform.
         *      int y - The y coordinate of the move to perform.
         *      
         * Returns:
         *      The position in the enemy list of the enemy the player would collide with, -1 if the player
         *      wouldn't collide with any enemy.
         */

        for (int i = 0; i < enemyPositions.Count; i++)
        {
            if (enemyPositions[i].x == x && enemyPositions[i].y == y)
            {
                return i;
            }
        }
        for (int i = 0; i < itemPositions.Count; i++)
        {
            if (itemPositions[i].x == x && itemPositions[i].y == y)
            {
                return i;
            }
        }

        return -1;
    }

    public bool moveEnemyTo(int enemyIndex, int x, int y)
    {
        /* Moves the enemy at the given enemyIndex to the supplied x and y in the grid, 
         * if possible.
         * Args:
         *      int x - The x coordinate to move to
         *      int y - The y coordinate to move to
         * Returns:
         *      true if the move was successful, false if it wasn't.
         */

        if (canMoveTo(x, y))
        {
            setEnemyPosition(enemyIndex, new Position(x, y));
            return true;
        }

        return false;
    }

    public bool movePlayerTo(int x, int y)
    {
        /* Moves the player at the to the supplied x and y in the grid if possible.
         * Args:
         *      int x - The x coordinate to move to
         *      int y - The y coordinate to move to
         * Returns:
         *      true if the move was successful, false if it wasn't.
         */

        if (canMoveTo(x, y))
        {
            setPlayerPosition(x, y);
            return true;
        }

        return false;
    }

    public bool canMoveTo(int x, int y)
    {
        /* Checks to see if the tile at grid position (x, y) is WALKABLE.
         * Args:
         *      int x - The x coordinate to check
         *      int y - The y coordinate to check
         * Returns:
         *      true if the tile at grid position (x, y) is WALKABLE, false if it isn't.
         */

        if (grid[x, y] == TileType.WALKABLE)
        {
            return true;
        }

        return false;
    }

    public void setPlayerPosition(Position position)
    {
        setPlayerPosition(position.x, position.y);
    }

    public void setPlayerPosition(int x, int y)
    {
        // Update the grid
        if (playerPositions.Count <= activePlayer)
        {
            playerPositions.Add(new Position(x, y));
        }
        else
        {
            grid[playerPositions[activePlayer].x, playerPositions[activePlayer].y] = TileType.WALKABLE;
        }
        grid[x, y] = TileType.PLAYER;

        playerPositions[activePlayer].x = x;
        playerPositions[activePlayer].y = y;

        if (activePlayer == 0)
        {
            updatePlayerRoom();
        }
    }

    public List<Position> getPlayerPositions()
    {
        return playerPositions;
    }

    public void addEnemy(Position enemyPosition, TileType type)
    {
        enemyPositions.Add(enemyPosition);
        grid[enemyPosition.x, enemyPosition.y] = type;
    }

    public void addItem(Position pos, TileType type)
    {
        itemPositions.Add(pos);
        grid[pos.x, pos.y] = type;
    }

    public List<Position> getAllEnemyPositions()
    {
        return enemyPositions;
    }

    public Position getEnemyPosition(int index)
    {
        return enemyPositions[index];
    }

    public Position getItemPosition(int index)
    {
        return itemPositions[index];
    }

    public void setEnemyPosition(int index, Position position)
    {
        // Update the grid
        grid[enemyPositions[index].x, enemyPositions[index].y] = TileType.WALKABLE;
        grid[position.x, position.y] = TileType.ENEMY;

        enemyPositions[index] = position;
    }

    public TileType getTileAt(int x, int y)
    {
        /* Gets the tile (kinda safely) at the given x, y in the grid.
         * 
         * Args:
         *      int x - The x to set
         *      int y - The y to set
         *      
         * Returns:
         *      The tile at the given x, y
         */

        try
        {
            TileType toReturn = grid[x, y];
            return toReturn;
        }
        catch
        {
            MonoBehaviour.print("Invalid getTileAt array access using: X = " + x + " Y = " + y);
            throw;
        }
    }

    public void setTileAt(int x, int y, TileType type)
    {
        /* Sets the tile (kinda safely) at the given x, y in the grid to the passed in TileType.
         * 
         * Args:
         *      int x - The x to set
         *      int y - The y to set
         *      TileType type - The type to set the given tile to
         *      
         * Returns:
         *      Nothing
         */

        try
        {
            grid[x, y] = type;
        }
        catch
        {
            MonoBehaviour.print("Invalid setTileAt array access using: X = " + x + " Y = " + y);
            throw;
        }
    }

    public void destroyEnemy(int index)
    {
        Position pos = getEnemyPosition(index);
        Generate.rooms[currentRoomIndex].enemyCount--;
        destroyThing(pos.x, pos.y);
    }

    public void destroyThing(int x, int y)
    {
        setTileAt(x, y, TileType.WALKABLE);
    }

    public void pickupItem(int index)
    {
        Position pos = getItemPosition(index);
        destroyThing(pos.x, pos.y);
    }

    public void pickupItem(int x, int y)
    {
        setTileAt(x, y, TileType.WALKABLE);
    }
}

public class Room
{
    /* A dataclass used to hold important information about each room.
     * Things like the amount of enemies in the room, the x, the y, the 
     * height, the width, etc.
     * 
     * Args:
     *      int x - The x position of the room
     *      int y - The y position of the room
     *      int width - The width of the room
     *      int height - The hieght of the room
     *      int enemycount - The amount of enemies in the room
     *      bool containsPlayer - True if the player is in the current room, false otherwise.
     */

    public int enemyCount { get; set; }
    public int x { get; private set; }
    public int y { get; private set; }
    public int height { get; private set; }
    public int width { get; private set; }
    public bool containsPlayer { get; set; }

    public Room(int x, int y, int width, int height, int enemyCount, bool containsPlayer)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.enemyCount = enemyCount;
        this.containsPlayer = containsPlayer;
    }

    public bool hasEnemies()
    {
        return enemyCount != 0;
    }
}

public class Generate : MonoBehaviour
{
    public bool drawGizmos; //determines whether or not to draw the grid gizmos

    // All the game objects that we need to instantiate (at some point)
    public GameObject unwalkablePrefab;
    public GameObject walkablePrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject itemHPPrefab;
    public GameObject itemDMGPrefab;

    // Public values useful for generation
    public int minRoomWidth = 10;
    public int maxRoomWidth = 20;
    public int minRoomHeight = 10;
    public int maxRoomHeight = 20;
    public int hallwayHeight = 4;
    public int numRooms = 3;
    public int mapHeight = 64;
    public int mapWidth = 64;
    public int wallHeight = 1;
    public float spriteSize = 1;
    public Map map;
    public static List<Room> rooms;
    public int seed;

    private Transform boardHolder;
    private int curRoom;
    private int itemCount;
    private bool spawnedPlayer;

    void Awake()
    {
        /* Called when the script first "wakes up" */

        // Random seed bruh
        Random.seed = -774579528;//(int)DateTime.Now.Ticks;
        seed = Random.seed;

        curRoom = 0;
        itemCount = 2;
        spawnedPlayer = false;

        map = new Map(mapHeight, mapHeight);
        rooms = new List<Room>();

        // Generate all the rooms, then generate all the hallways for the rooms, then populate the rooms with enemies/items/players/etc.
        generateRooms(1, 1, new List<Direction> { Direction.EAST, Direction.WEST, Direction.NORTH, Direction.SOUTH }, Random.Range(minRoomWidth, maxRoomWidth), Random.Range(minRoomHeight, maxRoomHeight));
        generateHallways();
        populateRooms();

        // Render the map, starting at (0.0, 0.0)
        renderMap(map, 0, 0);
    }

    void renderMap(Map map, int startX, int startY)
    {
        /* This method iterates through the grid (starting at startX and going to map.renderWidth and startY to map.renderHeight) and instantiates everything it finds.
         * Args:
         *      Map map - The map to read instantiate from.
         *      int startX - The x position to start instantiating things 
         *      int startY - The y position to start instantiating things
         *      
         * Returns:
         *      Nothing
         */

        // The object to hold our "board"
        boardHolder = new GameObject("Board").transform;

        print("Render Height: " + map.renderHeight);
        print("Render Width: " + map.renderWidth);

        // Fit the plane properly to the map size. Now a much better implementation (Thanks Ryan!). 
        GameObject instance = (GameObject)Instantiate(walkablePrefab, new Vector3((map.renderWidth - 1) / 2.0f, (map.renderHeight - 1) / 2.0f, 1.0f), Quaternion.Euler(-90, 0, 0));
        instance.transform.localScale = new Vector3(0.1f * map.renderWidth, 1, 0.1f * map.renderHeight);

        GameObject enemy;
        GameObject item;

        for (int i = 0; i < map.renderWidth; i++)
        {
            for (int j = 0; j < map.renderHeight; j++)
            {
                TileType curTile = map.getTileAt(i, j);
                switch (curTile)
                {
                    case TileType.NONE:
                    // Woooo. Abusing logical fall throughs!

                    case TileType.UNWALKABLE:
                        instance = (GameObject)Instantiate(unwalkablePrefab, new Vector3(i, j, 0f), transform.rotation);

                        // Make the wall 2 units high. 
                        instance.transform.localScale = new Vector3(1, 1, 2);
                        break;

                    case TileType.PLAYER:
                        Instantiate(playerPrefab, new Vector3(i, j, 0f), transform.rotation);
                        break;

                    case TileType.ENEMY:
                        enemy = (GameObject)Instantiate(enemyPrefab, new Vector3(i, j, 0f), transform.rotation);
                        enemy.GetComponent<EnemyController>().index = map.getPlayerCollidedWith(i, j);
                        break;

                    case TileType.BOSS:
                        enemy = (GameObject)Instantiate(bossPrefab, new Vector3(i, j, 0f), transform.rotation);
                        enemy.GetComponent<EnemyController>().index = map.getPlayerCollidedWith(i, j);
                        break;

                    case TileType.ITEM:
                        int type = Random.Range(0, itemCount);
                        if (type == 0)
                        {
                            item = (GameObject)Instantiate(itemHPPrefab, new Vector3(i, j, 0f), transform.rotation);
                        }
                        else if (type == 1)
                        {
                            item = (GameObject)Instantiate(itemDMGPrefab, new Vector3(i, j, 0f), transform.rotation);
                        }
                        else
                        {
                            print("Invalid item type");
                            break;
                        }
                        item.GetComponent<ItemController>().index = map.getPlayerCollidedWith(i, j);
                        break;
                }

                // Set the object to be a child of "boardHolder" so that we have a kinda-clear hierarchy. 
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    bool canPlaceRoom(int startX, int startY, int roomWidth, int roomHeight)
    {
        /* Checks to see if it can place a room with the given dimensions, without
         * running out of the grid, or without colliding with a preexisting structure.
         * 
         * Args:
         *      int startX - The starting x position (lower left corner) of the structure
         *      int startY - The starting y position (lower left corner) of the structure
         *      int roomWidth - The width of the room
         *      int roomHeight - The height of the room
         * 
         * Returns:
         *      True if you can place the room with the given dimensions, false if not.
         */

        if (startX + roomWidth < map.width && startY + roomHeight < map.height && startX >= 0 && startY >= 0)
        {
            for (int i = startX; i < startX + roomWidth; i++)
            {
                for (int j = startY; j < startY + roomHeight; j++)
                {
                    if (map.getTileAt(i, j) != TileType.UNWALKABLE)
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    Room fillRoom(int startX, int startY, int roomWidth, int roomHeight)
    {
        /* Fills in a structure of the given dimensions at the given coordinates with WALKABLE's.
         * It then adds one to the current room total, and returns a new Room object with the given
         * dimensions and default values for enemies/players.
         *
         * Args:
         *      int startX - The starting x position 
         *      int startY - The starting y position
         *      int roomWidth - The width of the room
         *      int roomheight - The height of the room
         *      
         * Returns:
         *      A new room filled out with the given dimensions, and default values for players/enemies.
         */

        for(int i = startX; i < startX + roomWidth; i++)
        {
            for(int j = startY; j < startY + roomHeight; j++)
            {
                map.setTileAt(i, j, TileType.WALKABLE);
            }
        }

        curRoom++;

        return new Room(startX, startY, roomWidth, roomHeight, (int)Mathf.Floor(Mathf.Log(curRoom)), false);
    }

    void populateRooms()
    {
        /* Attempts to populate each room with players/enemies. FIXME: This should
         * also be filling in items and the like (which it is currently not doing -- unfortunately)
         * 
         * Args:
         *      None
         * 
         * Returns:
         *      Nothing
         */

        foreach(Room room in rooms)
        {
            if(!spawnedPlayer)
            {
                spawnedPlayer = true;
                map.setPlayerPosition(RandomPosition(room.width, room.height, room.x, room.y));
                map.setActivePlayer(1);
                map.setPlayerPosition(RandomPosition(room.width, room.height, room.x, room.y));
                map.setActivePlayer(0);

                room.enemyCount = 0;
                room.containsPlayer = true;
            }
            else
            { 
                for(int i = 0; i < room.enemyCount; i++)
                {
                    Position random = RandomPosition(room.width, room.height, room.x, room.y);
                    map.addEnemy(random, TileType.ENEMY);
                }
            }
        }
    }

    void generateHallways()
    {
        /* Attempts to generate hallways between all rooms. It does this by iterating over all known rooms and
         * first seeing if it can even place a hallway where it wants to for the room (and by making sure the given room 
         * isn't on the border, so given room isn't on the border, so we don't get hallways to nothing), then 
         * if it can make a hallway there, it does.
         * 
         * Args:
         *      None
         *      
         * Returns:
         *      Nothing
         */

        foreach (Room room in rooms)
        {
            int top = room.y + room.height;
            int right = room.x + room.width;
            int bottom = room.y;
            int left = room.x;

            // Draw hallway to the right if possible
            if(canPlaceRoom(right, (top + bottom) / 2, 1, hallwayHeight) && (right + 1) != map.renderWidth)
            {
                fillRoom(right, (top + bottom) / 2, 1, hallwayHeight);
            }

            // Draw hallway to the left if possible
            if (canPlaceRoom(left, (top + bottom) / 2, 1, hallwayHeight))
            {
                fillRoom(left, (top + bottom) / 2, 1, hallwayHeight);
            }

            // Draw hallway above if possible (don't draw hallways on the top of the map)
            if (canPlaceRoom((right + left) / 2, top, hallwayHeight, 1) && (top + 1) != map.renderHeight)
            {
                fillRoom((right + left) / 2, top, hallwayHeight, 1);
            }
        }
    }

    void generateRooms(int startX, int startY, List<Direction> directions, int roomWidth, int roomHeight)
    {
        /* Generates all rooms uniformly using a recursive algorithm that I don't really feel
         * like describing in a docstring (seriously, I made the code super easy to read, just read it).
         * 
         * Args:
         *      int startX - The starting x position of the current room
         *      int startY - The starting y position of the current room
         *      List<Direction> directions - A list of directions for the algorithm to probe.
         *      int roomWidth - The width of the current room
         *      int roomHeight - The height of the current room
         * 
         * Returns:
         *      Nothing (yay side effects! :( )
        */
        
        // Useful values
        int top = startY + roomHeight;
        int right = startX + roomWidth;
        int bottom = startY;
        int left = startX;

        if(canPlaceRoom(startX, startY, roomWidth, roomHeight))
        {
            // Update the render height/width
            map.renderHeight = Mathf.Max(map.renderHeight, (startY + roomHeight) + 1);
            map.renderWidth = Mathf.Max(map.renderWidth, (startX + roomWidth) + 1);

            // Fill the area with walkables and add the room generated to the list of rooms
            rooms.Add(fillRoom(startX, startY, roomWidth, roomHeight));
        }
        else
        {
            // If you can't place a room here, we don't want to be here. 
            return;
        }

        // Only generate a fixed width room, so that we keep the 1 tile border.
        foreach (Direction direction in directions)
        {
            if (direction == Direction.NORTH)
            {
                generateRooms(startX, startY + (roomHeight + 1), directions, roomWidth, roomHeight);
            }
            
            if(direction == Direction.EAST)
            {
                generateRooms(startX + (roomWidth + 1), startY, directions, roomWidth, roomHeight);    
            }

            if (direction == Direction.WEST)
            {
                generateRooms(startX - (roomWidth + 1), startY, directions, roomWidth, roomHeight);
            }

            if (direction == Direction.SOUTH)
            {
                generateRooms(startX, startY - (roomHeight + 1), directions, roomWidth, roomHeight);
            }
        }
    }

    public Position RandomPosition(int width, int height, int x, int y)
    {
        /* Returns a random Position object for a room with the given parameters.
         * 
         * Args:
         *      int width - The width of the room 
         *      int height - The height of the room
         *      int x - The x position of the room (lower left corner)
         *      int y - The y position of the room (lower left corner)
         * 
         * Returns:
         *      A Position object that has a random x and y
         */

        int topSide = y + height;
        int rightSide = x + width;
        int endX = Random.Range(x + 2, rightSide - 2);
        int endY = Random.Range(y + 2, topSide - 2);

        return new Position(endX, endY);
    }


    void OnDrawGizmos() {
        /* Dunno. Tegan wrote this. */
        if (drawGizmos) {
            if (map != null) {
                Gizmos.DrawWireCube(transform.position, new Vector3(map.width, 1, map.height));

                if (map.grid != null) {
                    for (int i = 0; i < mapWidth; i++) {
                        for (int j = 0; j < mapHeight; j++) {
                            TileType tile = map.grid[i, j];
                            if (tile == TileType.WALKABLE)
                                Gizmos.color = Color.white;
                            else if (tile == TileType.PLAYER)
                                Gizmos.color = Color.blue;
                            else
                                Gizmos.color = Color.red;
                            Gizmos.DrawCube(new Vector3(i, j, -3f), Vector3.one * (.9f));
                        }
                    }
                }
            }
        }
    }
}