using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public enum TileType {
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

public class Position {
    public int x { get; set; }
    public int y { get; set; }

    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public class Map {
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

    public Map(int width, int height) {
        this.width = width;
        this.height = height;
        grid = new TileType[width, height];
        initializeGrid();
    }

    private void initializeGrid()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                setTileAt(i, j, TileType.UNWALKABLE);
            }
        }
        return;
    }

    public void setActivePlayer(int index) {
        activePlayer = index;
    }

    public void incrementActivePlayer() {
        activePlayer++;
    }

    public bool playerInRoomWithEnemies() {
        foreach (Room room in Generate.rooms)
        {
            if(room.containsPlayer)
            {
                return room.hasEnemies();
            }
        }

        return false;
    }

    private void updatePlayerRoom() {
        if (Generate.rooms.Count == 0 || currentRoomIndex == Generate.rooms.Count) {
            // Assume that rooms haven't been generated yet.
            return;
        }

        Room currentRoom = Generate.rooms[currentRoomIndex];
        if (!(currentRoom.x <= playerPositions[0].x && currentRoom.x + currentRoom.width >= playerPositions[0].x && currentRoom.y <= playerPositions[0].y && currentRoom.y + currentRoom.height >= playerPositions[0].y)) {
            if (currentRoom.x > playerPositions[0].x || currentRoom.y > playerPositions[0].y) {
                currentRoomIndex -= 1;
            }
            else {
                currentRoomIndex += 1;
            }
        }
    }

    public int getPlayerCollidedWith(int x, int y) {
        /* Returns the index in the enemy list of the enemy the player would collide with if the 
         * move to coordinates (x, y) is performed.
         * Args:
         *      int x - The x coordinate of the move to perform.
         *      int y - The y coordinate of the move to perform.
         * Returns:
         *      The position in the enemy list of the enemy the player would collide with, -1 if the player
         *      wouldn't collide with any enemy.
         */

        for (int i = 0; i < enemyPositions.Count; i++) {
            if (enemyPositions[i].x == x && enemyPositions[i].y == y) {
                return i;
            }
        }
        for (int i = 0; i < itemPositions.Count; i++) {
            if (itemPositions[i].x == x && itemPositions[i].y == y) {
                return i;
            }
        }

        return -1;
    }

    public bool moveEnemyTo(int enemyIndex, int x, int y) {
        /* Moves the enemy at the given enemyIndex to the supplied x and y in the grid, 
         * if possible.
         * Args:
         *      int x - The x coordinate to move to
         *      int y - The y coordinate to move to
         * Returns:
         *      true if the move was successful, false if it wasn't.
         */

        if (canMoveTo(x, y)) {
            setEnemyPosition(enemyIndex, new Position(x, y));
            return true;
        }

        return false;
    }

    public bool movePlayerTo(int x, int y) {
        /* Moves the player at the to the supplied x and y in the grid if possible.
         * Args:
         *      int x - The x coordinate to move to
         *      int y - The y coordinate to move to
         * Returns:
         *      true if the move was successful, false if it wasn't.
         */

        if (canMoveTo(x, y)) {
            setPlayerPosition(x, y);
            return true;
        }

        return false;
    }

    public bool canMoveTo(int x, int y) {
        /* Checks to see if the tile at grid position (x, y) is WALKABLE.
         * Args:
         *      int x - The x coordinate to check
         *      int y - The y coordinate to check
         * Returns:
         *      true if the tile at grid position (x, y) is WALKABLE, false if it isn't.
         */

        if (grid[x, y] == TileType.WALKABLE) {
            return true;
        }

        return false;
    }

    public void setPlayerPosition(Position position) {
        setPlayerPosition(position.x, position.y);
    }

    public void setPlayerPosition(int x, int y) {
        // Update the grid
        if (playerPositions.Count <= activePlayer) {
            playerPositions.Add(new Position(x, y));
        }
        else {
            grid[playerPositions[activePlayer].x, playerPositions[activePlayer].y] = TileType.WALKABLE;
        }
        grid[x, y] = TileType.PLAYER;

        playerPositions[activePlayer].x = x;
        playerPositions[activePlayer].y = y;

        if (activePlayer == 0) {
            updatePlayerRoom();
        }
    }

    public List<Position> getPlayerPositions() {
        return playerPositions;
    }

    public void addEnemy(Position enemyPosition, TileType type) {
        enemyPositions.Add(enemyPosition);
        grid[enemyPosition.x, enemyPosition.y] = type;
    }

    public void addItem(Position pos, TileType type) {
        itemPositions.Add(pos);
        grid[pos.x, pos.y] = type;
    }

    public List<Position> getAllEnemyPositions() {
        return enemyPositions;
    }

    public Position getEnemyPosition(int index) {
        return enemyPositions[index];
    }

    public Position getItemPosition(int index) {
        return itemPositions[index];
    }

    public void setEnemyPosition(int index, Position position) {
        // Update the grid
        grid[enemyPositions[index].x, enemyPositions[index].y] = TileType.WALKABLE;
        grid[position.x, position.y] = TileType.ENEMY;

        enemyPositions[index] = position;
    }

    public TileType getTileAt(int x, int y) {
        try {
            TileType toReturn = grid[x, y];
            return toReturn;
        }
        catch {
            MonoBehaviour.print("Invalid getTileAt array access using: X = " + x + " Y = " + y);
            throw;
        }
    }

    public void setTileAt(int x, int y, TileType type) {

        try {
            grid[x, y] = type;
        }
        catch {
            MonoBehaviour.print("Invalid setTileAt array access using: X = " + x + " Y = " + y);
            throw;
        }
    }

    public void destroyEnemy(int index) {
        Position pos = getEnemyPosition(index);
        Generate.rooms[currentRoomIndex].enemyCount--;
        destroyThing(pos.x, pos.y);
    }

    public void destroyThing(int x, int y) {
        setTileAt(x, y, TileType.WALKABLE);
    }

    public void pickupItem(int index) {
        Position pos = getItemPosition(index);
        destroyThing(pos.x, pos.y);
    }

    public void pickupItem(int x, int y) {
        setTileAt(x, y, TileType.WALKABLE);
    }
}

public class Room {
    public int enemyCount { get; set; }
    public int x { get; private set; }
    public int y { get; private set; }
    public int height { get; private set; }
    public int width { get; private set; }
    public bool containsPlayer { get; set; }

    public Room(int x, int y, int width, int height, int enemyCount, bool containsPlayer) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.enemyCount = enemyCount;
        this.containsPlayer = containsPlayer;
    }

    public bool hasEnemies() {
        return enemyCount != 0;
    }
}

public class Generate : MonoBehaviour {
    public bool drawGizmos; //determines whether or not to draw the grid gizmos
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

    private Transform boardHolder;
    private int curRoom;
    private int itemCount;
    private bool spawnedPlayer;

    void Awake() {
        // Bugged seed.
        Random.seed = 1463179365;

        curRoom = 1;
        itemCount = 2;
        spawnedPlayer = false;

        map = new Map(mapHeight, mapHeight);
        rooms = new List<Room>();

        generateRooms(1, 1, new List<Direction> { Direction.EAST, Direction.WEST, Direction.NORTH, Direction.SOUTH }, Random.Range(minRoomWidth, maxRoomWidth), Random.Range(minRoomHeight, maxRoomHeight));
        generateHallways();
        populateRooms();

        // Render the map, starting at (0.0, 0.0)
        renderMap(map, 0.0f, 0.0f);
    }

    void renderMap(Map map, float startX, float startY) {
        boardHolder = new GameObject("Board").transform;

        print("Render Height: " + map.renderHeight);
        print("Render Width: " + map.renderWidth);

        /* Fit the plane properly to the map size. Now a much better implementation (Thanks Ryan!). 
         */
        GameObject instance = (GameObject)Instantiate(walkablePrefab, new Vector3((map.renderWidth - 1) / 2.0f, (map.renderHeight - 1) / 2.0f, 1.0f), Quaternion.Euler(-90, 0, 0));
        instance.transform.localScale = new Vector3(0.1f * map.renderWidth, 1, 0.1f * map.renderHeight);

        GameObject enemy;
        GameObject item;

        for (int i = 0; i < map.renderWidth; i++) {
            for (int j = 0; j < map.renderHeight; j++) {
                TileType curTile = map.getTileAt(i, j);
                switch (curTile) {
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
                        if (type == 0) {
                            item = (GameObject)Instantiate(itemHPPrefab, new Vector3(i, j, 0f), transform.rotation);
                        }
                        else if (type == 1) {
                            item = (GameObject)Instantiate(itemDMGPrefab, new Vector3(i, j, 0f), transform.rotation);
                        }
                        else {
                            print("Invalid item type");
                            break;
                        }
                        item.GetComponent<ItemController>().index = map.getPlayerCollidedWith(i, j);
                        break;
                }
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    bool canPlaceRoom(int startX, int startY, int roomWidth, int roomHeight)
    {
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
        for(int i = startX; i < startX + roomWidth; i++)
        {
            for(int j = startY; j < startY + roomHeight; j++)
            {
                map.setTileAt(i, j, TileType.WALKABLE);
            }
        }

        curRoom++;

        // Create a new room with a random amount of enemies and no player (by default)
        return new Room(startX, startY, roomWidth, roomHeight, (int)Mathf.Floor(Mathf.Log(curRoom)), false);
    }

    void populateRooms()
    {
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
                    map.addEnemy(RandomPosition(room.width, room.height, room.x, room.y), TileType.ENEMY);
                }
            }
        }
    }

    void generateHallways()
    {
        foreach(Room room in rooms)
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
        /* Generate a room given the specifications and return a new Room. 
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

    public Position RandomPosition(int width, int height, int x, int y) {
        int topSide = x + height;
        int rightSide = x + width;
        int endX = Random.Range(x + 2, rightSide - 2);
        int endY = Random.Range(y + 2, topSide - 2);

        return new Position(endX, endY);
    }


	void OnDrawGizmos()
    {
		if (drawGizmos) {
			Gizmos.DrawWireCube (transform.position, new Vector3 (map.width, 1, map.height));

			if (map.grid != null) {
				for (int i = 0; i < mapWidth; i++) {
					for (int j = 0; j < mapHeight; j++) {
						TileType tile = map.grid [i, j];
						if (tile == TileType.WALKABLE)
							Gizmos.color = Color.white;
						else if (tile == TileType.PLAYER)
							Gizmos.color = Color.blue;
						else
							Gizmos.color = Color.red;
						Gizmos.DrawCube (new Vector3 (j, i, -3f), Vector3.one * (.9f));
					}
				}
			}
		}
	}
}