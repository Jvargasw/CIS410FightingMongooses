using UnityEngine;
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

public class Position {
    public int x { get; set; }
    public int y { get; set; }

    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public class Map {
    public int width { get; private set; }
    public int height { get; private set; }

    public int renderHeight { get; set; }
    public int renderWidth { get; set; }

    private int activePlayer = 0;
    private List<Position> enemyPositions = new List<Position>();
    private List<Position> itemPositions = new List<Position>();
    private List<Position> playerPositions = new List<Position>();
    private int currentRoomIndex = 0;

    public TileType[,] grid;

    public Map(int width, int height) {
        this.width = width;
        this.height = height;
        grid = new TileType[width, height];
    }

    public void setActivePlayer(int index) {
        activePlayer = index;
    }

    public void incrementActivePlayer() {
        activePlayer++;
    }

    public bool playerInRoomWithEnemies() {
        return Generate.rooms[currentRoomIndex].hasEnemies();
    }

    private void updatePlayerRoom() {
        if (Generate.rooms.Count == 0 || currentRoomIndex == Generate.rooms.Count) {
            // Assume that rooms haven't been generated yet.
            return;
        }

        Room currentRoom = Generate.rooms[currentRoomIndex];
        if (!(currentRoom.x <= playerPositions[activePlayer].x && currentRoom.x + currentRoom.width >= playerPositions[activePlayer].x && currentRoom.y <= playerPositions[activePlayer].y && currentRoom.y + currentRoom.height >= playerPositions[activePlayer].y)) {
            currentRoomIndex += 1;
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

    public Room(int x, int y, int width, int height, int enemyCount) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.enemyCount = enemyCount;
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

    public int minRoomSize = 10;
    public int maxRoomSize = 20;
    public int hallwayHeight = 4;
    public int numRooms = 3;
    public int mapHeight = 64;
    public int mapWidth = 64;
    public int wallHeight = 1;

    public float spriteSize = 1;
    public Map map;
    public static List<Room> rooms;

    private Transform boardHolder;
    private Transform hallwayHolder;
    private int curHallway;
    private int curRoom;
    private int itemCount;

    void Awake() {
        int currentX = 0;
        int currentY = 0;
        int randWidth;
        int randHeight;
        int width;
        int height;

        curRoom = 1;
        curHallway = 1;
        itemCount = 2;

        map = new Map(mapHeight, mapHeight);
        int hallwayDirection = -1;
        rooms = new List<Room>();

        // Generate each room
        for (int i = 0; i < numRooms; i++) {

            randWidth = Random.Range(minRoomSize, maxRoomSize);
            randHeight = Random.Range(minRoomSize, maxRoomSize);

            width = Mathf.Min(63 - currentX, randWidth);
            height = Mathf.Min(63 - currentY, randHeight);
            print("randWidth: " + randWidth + " randHeight: " + randHeight);
            print("width: " + width + " height: " + height);
            if (width != randWidth) {
                print("Width overide: " + width);
                if (width <= 0) {
                    width = Mathf.Max(63 - currentX, randWidth);
                    print("Double width override: " + width);
                }
            }
            if (height != randHeight) {
                print("Height overide: " + height);
                if (height <= 0) {
                    height = Mathf.Max(63 - currentY, randHeight);
                    print("Double height override: " + height);
                }
            }

            rooms.Add(generateRoom(map, width, height, currentX, currentY, hallwayDirection));

            // Randomize if the next room will be above or to the right of the current room.
            // TODO: Add in left or below? 
            if (Random.Range(0, 100) >= 50) {
                hallwayDirection = 0;
                currentX += randWidth;
            }
            else {
                hallwayDirection = 1;
                currentY += randHeight;
            }
            curRoom++;
        }

        // Render the map, starting at (0.0, 0.0)
        renderMap(map, 0.0f, 0.0f);
    }

    void renderMap(Map map, float startX, float startY) {
        boardHolder = new GameObject("Board").transform;


        float currentX = startX;
        float currentY = startY;

        print("Render Height: " + map.renderHeight);
        print("Render Width: " + map.renderWidth);

        /* Fit the plane properly to the map size. Now a much better implementation (Thanks Ryan!). 
         */
        GameObject instance = (GameObject)Instantiate(walkablePrefab, new Vector3((map.renderHeight - 1) / 2.0f, (map.renderWidth - 1) / 2.0f, 1.0f), Quaternion.Euler(-90, 0, 0));
        instance.transform.localScale = new Vector3(0.1f * map.renderHeight, 1, 0.1f * map.renderWidth);

        GameObject enemy;
        GameObject item;

        for (int i = 0; i < map.renderWidth; i++) {
            for (int j = 0; j < map.renderHeight; j++) {
                TileType curTile = map.getTileAt(i, j);
                switch (curTile) {
                    case TileType.NONE:
                    // Woooo. Abusing logical fall throughs!

                    case TileType.UNWALKABLE:
                        instance = (GameObject)Instantiate(unwalkablePrefab, new Vector3((float)currentX, (float)currentY, 0f), transform.rotation);

                        // Make the wall 2 units high. 
                        // FIXME: This currently only increases it's height by 0.5 (I believe), because unity scales both ends.
                        Vector3 unwalkableScale = instance.transform.localScale = new Vector3(1, 1, 2);
                        break;

                    case TileType.PLAYER:
                        print("RENDERING PLAYER");
                        Instantiate(playerPrefab, new Vector3((float)currentX, (float)currentY, 0f), transform.rotation);
                        break;

                    case TileType.ENEMY:
                        enemy = (GameObject)Instantiate(enemyPrefab, new Vector3((float)currentX, (float)currentY, 0f), transform.rotation);
                        enemy.GetComponent<EnemyController>().index = map.getPlayerCollidedWith(i, j);
                        break;

                    case TileType.BOSS:
                        enemy = (GameObject)Instantiate(bossPrefab, new Vector3((float)currentX, (float)currentY, 0f), transform.rotation);
                        enemy.GetComponent<EnemyController>().index = map.getPlayerCollidedWith(i, j);
                        break;

                    case TileType.ITEM:
                        int type = Random.Range(0, itemCount);
                        if (type == 0) {
                            item = (GameObject)Instantiate(itemHPPrefab, new Vector3((float)currentX, (float)currentY, 0f), transform.rotation);
                        }
                        else if (type == 1) {
                            item = (GameObject)Instantiate(itemDMGPrefab, new Vector3((float)currentX, (float)currentY, 0f), transform.rotation);
                        }
                        else {
                            print("Invalid item type");
                            break;
                        }
                        item.GetComponent<ItemController>().index = map.getPlayerCollidedWith(i, j);
                        break;
                }
                currentX += spriteSize;
                instance.transform.SetParent(boardHolder);
            }
            currentX = startX;
            currentY += spriteSize;
        }
    }

    Room generateRoom(Map map, int roomWidth, int roomHeight, int startX, int startY, int hallwayDirection) {
        /* Generate a room given the specifications and return a new Room. 
        */

        // Useful values

        if (startX >= map.width || startY >= map.height) {
            return null;
        }

        int topSide = startY + roomHeight;
        int rightSide = startX + roomWidth;

        bool spawnedPlayer = false;
        bool spawnedBoss = false;
        int numEnemies = (int)Mathf.Floor(Mathf.Log(curRoom));
        int numItems = 1;
        Room room = new Room(startX, startY, roomWidth, roomHeight, numEnemies);

        print("StartX: " + startX + " StartY: " + startY);

        // FIXME: Apparently our x and y are getting swapped somewhere. We should fix that. 
        // Create a hallway on the top of the room previous room to the bottom of the current room.
        if (hallwayDirection == 0) {
            for (int i = startX - 5; i < startX + 5; i++) {
                for (int j = startY + 1; j < startY + hallwayHeight; j++) {
                    if (i < map.width && j < map.height) {
                        map.setTileAt(i, j, TileType.WALKABLE);
                    }
                    else {
                        print("We overwrote on downwards hallway generation!");
                    }
                }
            }
        }

        // Create a hallway on the right side of the previous room to the left side of the current room.
        else if (hallwayDirection == 1) {
            for (int i = startX + 1; i < startX + hallwayHeight; i++) {
                for (int j = startY - 5; j < startY + 5; j++) {
                    if (i < map.width && j < map.height) {
                        map.setTileAt(i, j, TileType.WALKABLE);
                    }
                    else {
                        print("We overwote on rightwards (leftwards?) hallway generation!");
                    }
                }
            }
        }

        for (int i = startX; i < startX + roomWidth; i++) {
            for (int j = startY; j < startY + roomHeight; j++) {
                /* Handle drawing pathable sprites on the interior and unpathable sprites on the exterior. */
                if (i != startX && i != rightSide - 1 && j != startY && j != topSide - 1 && i < map.width && j < map.height) {
                    if (curRoom != 1 && curRoom != numRooms && numEnemies != 0) {
                        map.addEnemy(new Position(i, j), TileType.ENEMY);
                        numEnemies--;

                    }
                    else if (curRoom == numRooms && !spawnedBoss) {
                        map.addEnemy(new Position(i, j), TileType.BOSS);
                        spawnedBoss = true;
                    }
                    else {
                        map.renderHeight = Mathf.Max(map.renderHeight, j + 2);
                        map.setTileAt(i, j, TileType.WALKABLE);
                    }
                }
            }
            map.renderWidth = Mathf.Max(map.renderWidth, i + 2);
        }

        if (curRoom == 1) {
            Position pos = RandomPosition(roomWidth, roomHeight, startX, startY);
            map.setPlayerPosition(pos.x, pos.y);

            //spawn a second player (for testing)
            map.setActivePlayer(1);
            map.setPlayerPosition(pos.x + 1, pos.y + 1);
            map.setActivePlayer(0);
            //

        }
        else {
            if (numItems != 0) {
                numItems -= 1;
                Position pos = RandomPosition(roomWidth, roomHeight, startX, startY);
                while (map.getTileAt(pos.x, pos.y) != TileType.WALKABLE) {
                    pos = RandomPosition(roomWidth, roomHeight, startX, startY);
                }
                map.addItem(pos, TileType.ITEM);

            }
        }

        return room;
    }

    public Position RandomPosition(int width, int height, int x, int y) {
        int topSide = x + height;
        int rightSide = x + width;
        int endX = Random.Range(x + 2, rightSide - 2);
        int endY = Random.Range(y + 2, topSide - 2);

        return new Position(endX, endY);
    }

    void OnDrawGizmos() {
        if (drawGizmos) {
            Gizmos.DrawWireCube(transform.position, new Vector3(map.width, 1, map.height));

            if (map.grid != null) {
                for (int i = 0; i < mapWidth; i++) {
                    for (int j = 0; j < mapHeight; j++) {
                        Gizmos.color = (map.grid[i, j] == TileType.WALKABLE) ? Color.white : Color.red;
                        Gizmos.DrawCube(new Vector3(j, i, -3f), Vector3.one * (.9f));
                    }
                }
            }
        }
    }
}
