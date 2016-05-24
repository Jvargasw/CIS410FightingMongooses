using System.Collections.Generic;
using UnityEngine;

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

    //Generate generator = GameObject.Find("TileManager").GetComponent<Generate>();

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

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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
            if (room.containsPlayer)
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
            if (room.containsPlayer)
            {
                // Set the previous room we were in to not containing the player
                room.containsPlayer = false;
            }

            if (room.x <= playerPositions[0].x && room.x + room.width >= playerPositions[0].x && room.y <= playerPositions[0].y && room.y + room.height >= playerPositions[0].y)
            {
                //MonoBehaviour.print("Assuming the player is in room: " + Generate.rooms.IndexOf(room) + " has enemies?: " + room.hasEnemies());
                //MonoBehaviour.print("Assuming the player is in room: " + Generate.rooms.IndexOf(room) + " has enemies?: " + room.hasEnemies());
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
            if (enemyPositions[i] != null) {
                if (enemyPositions[i].x == x && enemyPositions[i].y == y) {
                    return i;
                }
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

    public void swapPlayerPositions() {
        int temp = activePlayer;
        activePlayer = 0;
        Position temp1 = playerPositions[activePlayer];
        activePlayer++;
        Position temp2 = playerPositions[activePlayer];
        setPlayerPosition(temp1);
        activePlayer = 0;
        setPlayerPosition(temp2);
        activePlayer = temp;
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
        enemyPositions[index] = null;
        foreach (Room room in Generate.rooms)
        {
            if (room.x <= pos.x && room.x + room.width >= pos.x && room.y <= pos.y && room.y + room.height >= pos.y)
            {
                // Break because once we found the room, we don't need to keep searching
                room.enemyCount--;
                break;
            }
        }
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