﻿using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
    public bool containsBoss { get; set; }

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
        return enemyCount != 0 /*|| containsBoss*/;
    }
}

public class Generate : MonoBehaviour
{
    public bool drawGizmos; //determines whether or not to draw the grid gizmos

    // All the game objects that we need to instantiate (at some point)
    public GameObject unwalkablePrefab;
    public GameObject walkablePrefab;
    public GameObject playerPrefab;
    public GameObject playerPrefab2;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject itemHPPrefab;
    public GameObject itemDMGPrefab;
    public GameObject itemDEFPrefab;

    // Public values useful for generation
    public int minRoomWidth = 10;
    public int maxRoomWidth = 20;
    public int minRoomHeight = 10;
    public int maxRoomHeight = 20;
    public int hallwayHeight = 4;
    public int mapHeight = 64;
    public int mapWidth = 64;
    public Map map;
    public static List<Room> rooms;
    public int seed;
    public bool useSeed = false;

    private Transform boardHolder;
    private int curRoom;
    private int itemCount;
    private bool spawnedPlayer;

    void Awake()
    {
        /* Called when the script first "wakes up" */

        // Random seed bruh
        seed = PersistentStorage.seed;
        Random.seed = PersistentStorage.seed;

        curRoom = 0;
        itemCount = 3;
        spawnedPlayer = false;

        map = new Map(mapHeight, mapHeight);
        rooms = new List<Room>();

        // Generate all the rooms, then generate all the hallways for the rooms, then populate the rooms with enemies/items/players/etc, then generate the obstacles for the rooms, .
        generateRooms(1, 1, new List<Direction> { Direction.EAST, Direction.WEST, Direction.NORTH, Direction.SOUTH }, Random.Range(minRoomWidth, maxRoomWidth), Random.Range(minRoomHeight, maxRoomHeight));
        generateHallways();
        shuffleRooms();
        populateRooms();
        generateObstacles();

        sendMessageToAllObjects("OnGeneratedMap");
        

        // Render the map, starting at (0.0, 0.0)
        renderMap(map, 0, 0);
        sendMessageToAllObjects("OnRenderedMap");
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

        EnemyController enemy;
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

                        // Make the wall 2 units high. "JK DON'T !!!" -jalan
                        instance.transform.localScale = new Vector3(1, 1, 1);
                        break;

                    case TileType.PLAYER:
                        Instantiate(playerPrefab, new Vector3(i, j, 0f), transform.rotation);
                        playerPrefab = playerPrefab2;
                        break;

                    case TileType.ENEMY:
                        enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(i, j, 0f), transform.rotation)).GetComponent<EnemyController>();
                        enemy.index = map.getPlayerCollidedWith(i, j);

                        foreach (Room room in Generate.rooms) {
                            if (room.x <= i && room.x + room.width >= i && room.y <= j && room.y + room.height >= j) {
                                // Break because once we found the room, we don't need to keep searching
                                enemy.room = room;
                                break;
                            }
                        }

                        break;

                    case TileType.BOSS:
                        enemy = ((GameObject)Instantiate(bossPrefab, new Vector3(i, j, 0f), transform.rotation)).GetComponent<EnemyController>();
                        enemy.index = map.getPlayerCollidedWith(i, j);

                        foreach (Room room in Generate.rooms) {
                            if (room.x <= i && room.x + room.width >= i && room.y <= j && room.y + room.height >= j) {
                                // Break because once we found the room, we don't need to keep searching
                                enemy.room = room;
                                break;
                            }
                        }

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
                        else if (type == 2) {
                            item = (GameObject)Instantiate(itemDEFPrefab, new Vector3(i, j, 0f), transform.rotation);
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

    void fillUnwalkable(int startX, int startY, int roomWidth, int roomHeight)
    {
        /* Fills in a structure of the given dimensions at the given coordinates with UNWALKABLE's.
         *
         * Args:
         *      int startX - The starting x position 
         *      int startY - The starting y position
         *      int roomWidth - The width of the room
         *      int roomheight - The height of the room
         *      
         * Returns:
         *      Nothing
         */

        for (int i = startX; i < startX + roomWidth; i++)
        {
            for (int j = startY; j < startY + roomHeight; j++)
            {
                map.setTileAt(i, j, TileType.UNWALKABLE);
            }
        }
    }

    bool canPlaceObstacle(int startX, int startY, int obstacleWidth, int obstacleHeight)
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

        if (startX + obstacleWidth < map.width && startY + obstacleHeight < map.height && startX >= 0 && startY >= 0)
        {
            for (int i = startX; i < startX + obstacleWidth; i++)
            {
                for (int j = startY; j < startY + obstacleHeight; j++)
                {
                    if (map.getTileAt(i, j) != TileType.WALKABLE)
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
            else if(rooms.IndexOf(room) == rooms.Count - 1)
            {
                room.enemyCount = 1;
                room.containsBoss = true;
                Position random = RandomPosition(room.width, room.height, room.x, room.y);
                map.addEnemy(random, TileType.BOSS);
            }
            else
            {
                bool generateItems = Random.Range(0, 100) > 75;
                while(generateItems)
                { 
                    Position random = RandomPosition(room.width, room.height, room.x, room.y);
                    map.addItem(random, TileType.ITEM);
                    generateItems = Random.Range(0, 100) > 75;
                }

                for(int i = 0; i < room.enemyCount; i++)
                {
                    Position random = RandomPosition(room.width, room.height, room.x, room.y);
                    map.addEnemy(random, TileType.ENEMY);
                }
            }
        }
    }

    void generateObstacles()
    {
        foreach(Room room in rooms)
        {
            bool createObstacle = Random.Range(0, 100) > 50;
            while(createObstacle)
            {
                int obstacleHeight = Random.Range(1, room.height);
                int obstacleWidth = Random.Range(1, room.width);
                Position obstaclePosition = RandomPosition(obstacleWidth, obstacleHeight, room.x, room.y);
                if (canPlaceObstacle(obstaclePosition.x, obstaclePosition.y, obstacleWidth, obstacleHeight))
                {
                    print("Creating obstacle in room: " + rooms.IndexOf(room));
                    fillUnwalkable(obstaclePosition.x, obstaclePosition.y, obstacleWidth, obstacleHeight);
                }
                createObstacle = Random.Range(0, 100) > 25;
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
        /* Warning Removal
        int top = startY + roomHeight;
        int right = startX + roomWidth;
        int bottom = startY;
        int left = startX;
        */

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

    public void shuffleRooms()
    {
        int n = rooms.Count;
        System.Random rng = new System.Random();
        while(n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Room room = rooms[k];
            rooms[k] = rooms[n];
            rooms[n] = room;
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

    void sendMessageToAllObjects(string message)
    {
        foreach(GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
        {
            gameObject.BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
        }
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