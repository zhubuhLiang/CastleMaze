using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public static class StoreyBackTracker
{
    private static int North = 1, South = 2, East = 4, West = 8, Up = 16, Down = 32;
    private static int seed = 17;
    private static Random rng = new Random(seed);

    private static int[] DIRS = new int[] { North, South, East, West, Up, Down};
    private static int[] DIRS2D = new int[] { North, South, East, West};
    private static Dictionary<int, int> DirInX = new Dictionary<int, int> { { East, 1 }, { West, -1 }, { North, 0 }, { South, 0 }, {Up, 0 },{Down, 0} };
    private static Dictionary<int, int> DirInY = new Dictionary<int, int> { { East, 0 }, { West, 0 }, { North, -1 }, { South, 1 } , { Up, 0 }, { Down, 0 } };
    private static Dictionary<int, int> DirInZ = new Dictionary<int, int> { { East, 0 }, { West, 0 }, { North, 0 }, { South, 0 }, { Up, 1 }, { Down, -1 } };
    private static Dictionary<int, int> OPPOSITE = new Dictionary<int, int> { { East, West }, { West, East }, { North, South }, { South, North }, {Up, Down },{Down, Up} };
    private static int[,,] directionGrid;


    public static void Run(IRoom[,,] roomGrid, int width, int height, int storeys, bool unEvenSized)
    {
        // record the sum of all opened directions for a indexed room.
        directionGrid = new int[width,height, storeys];
        int[] DirsToSearch = storeys == 1 ? DIRS2D : DIRS;

        //Check if the maze is set as unevenly shaped.
        List<Vector2Int> unevenMap;
        if (unEvenSized)
            unevenMap = UnEvenMap(width, height);
        else
            unevenMap = null;
        FillRoomGrid(roomGrid, width, height, storeys, unEvenSized, unevenMap);

        // build a stack whose element contains a room and the stack of available directions to its neighbors.
        Stack<(Stack<int>, IRoom)> stack = new Stack<(Stack<int>, IRoom)>();
        int startPosX = unEvenSized ? unevenMap[0].x : 0;
        stack.Push((new Stack<int>(DirsToSearch.OrderBy(a => rng.Next())), roomGrid[startPosX, 0, 0]));

        while (stack.Count > 0)
        {
            (Stack<int> directions, IRoom currentRoom) = stack.Peek();
            (int x, int y, int z) = (currentRoom.Index3D.x, currentRoom.Index3D.y, currentRoom.Index3D.z);

            // Visit neighbors of current room from all unvisited directions.
            while (directions.Count > 0)
            {
                int direction = directions.Pop();
                (int nx, int ny, int nz) = (x + DirInX[direction], y + DirInY[direction], z + DirInZ[direction]);

                // When the neighboring room meet the condition, make it the current room.
                if (nx >= 0 && ny >= 0 && nz >= 0 && nx < width && ny < height && nz < storeys && roomGrid[nx,ny,nz].IsActive && directionGrid[nx,ny,nz] == 0)
                {
                    directionGrid[x,y,z] |= direction;
                    directionGrid[nx,ny,nz] |= OPPOSITE[direction];
                    JoinWithNeighbour(roomGrid[x,y,z], roomGrid[nx,ny,nz], roomGrid);
                    stack.Push((directions = new(DirsToSearch.OrderBy(a => rng.Next())), roomGrid[nx,ny,nz]));
                    (x, y, z) = (nx, ny, nz);
                }
            }
            stack.Pop();
        }
    }


    private static void FillRoomGrid(IRoom[,,] roomGrid, int width, int height, int storeys, bool unEvenSized,List<Vector2Int> unevenMap)
    {
        for (int z = 0; z < storeys; z++)
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (IsRoomInMap(unEvenSized, x, y, z, width, height, storeys, unevenMap))
                    {
                        roomGrid[x,y,z].Reset();
                    }
                    else
                        roomGrid[x,y,z].Disable();
                }
    }


    private static void JoinWithNeighbour(IRoom currentRoom, IRoom neighbourRoom, IRoom[,,] grid)
    {
        Vector3Int cIndex = currentRoom.Index3D;
        Vector3Int nIndex = neighbourRoom.Index3D;
        currentRoom.RemoveWallWithNeighbour(nIndex.x, nIndex.y, nIndex.z);
        neighbourRoom.RemoveWallWithNeighbour(cIndex.x, cIndex.y, cIndex.z);

    }


    private static List<Vector2Int> UnEvenMap(int width, int height)
    {
        List<Vector2Int> roomRows = new List<Vector2Int>();
        int left = 0;
        int right = width;
        for (int i = 0; i < height; i++)
        {
            int previousLeft = left;
            left = RandomPointInRow(0, right-1);
            int maxLeft = Mathf.Max(previousLeft, left);
            right = RandomPointInRow(maxLeft, width);
            roomRows.Add(new Vector2Int(left, right));
        }

        return roomRows;
    }


    private static bool IsRoomInMap(bool uneven ,int x, int y, int z, int width , int height, int storys, List<Vector2Int>map)
    {
        if (!uneven && y < height && x < width && z < storys)
            return true;

        if ( uneven && y < height && x >= map[y].x && x <= map[y].y)
            return true;

        return false;
    }


    private static int RandomPointInRow(int start, int end)
    {
        return UnityEngine.Random.Range(start, end);
    }
}