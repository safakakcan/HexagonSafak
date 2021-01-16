using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Core))]
public class GridSystem : MonoBehaviour
{
    [Range(2, 98)]
    public int gridWidth = 8;
    [Range(2, 98)]
    public int gridHeight = 9;

    private GameObject[,] grid;

    [HideInInspector]
    public GameObject pivot;

    private Core core;
    private Rotator rotator;
    private bool fixPositions;

    // Start is called before the first frame update
    void Start()
    {
        core = GetComponent<Core>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (pivot != null && rotator != null)
        {
            RotateGroup();
        }

        if (fixPositions)
        {
            if (FixAllPositions())
            {
                fixPositions = false;
                core.onMoving = false;
            }
        }
    }

    public void Reset()
    {
        ManageCamera();

        if (grid != null)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (grid[x, y] != null)
                        BreakHexagon(grid[x, y]);
                }
            }
        }

        grid = new GameObject[gridWidth, gridHeight];

        FillInBlanks();
        fixPositions = true;
        core.onMoving = true;
    }

    public void ManageCamera()
    {
        Vector3 cameraPosition = (ConvertGridToPosition(gridWidth, gridHeight) - ConvertGridToPosition(0, 0)) / 2 - (core.unitOffset / 2);
        cameraPosition.z = -10;
        float size = gridWidth * core.unitOffset.x;
        core.SetCameraSize(size, cameraPosition);
    }

    public void FillInBlanks()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] == null)
                {
                    Color color = core.GetRandomColor();
                    if ((core.score / (core.bombSpawned + 1) > 1000))
                    {
                        CreateHexagon(x, y, color, true);
                        core.bombSpawned++;
                    }
                    else
                    {
                        CreateHexagon(x, y, color);
                    }
                }
            }
        }
    }

    public Vector2 ConvertGridToPosition(int x, int y)
    {
        float newX = x * core.unitOffset.x;
        float offsetY = x % 2 == 0 ? 0 : (core.unitOffset.y / 2);
        float newY = (y * core.unitOffset.y) + offsetY;

        return new Vector2(newX, newY);
    }

    public Vector2Int ConvertPositionToGrid(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / core.unitOffset.x);
        int y = 0;

        for (int i = 0; i < 99; i++)
        {
            if (Vector2.Distance(position, ConvertGridToPosition(x, i)) < 0.1f)
            {
                y = i;
                break;
            }
        }

        return new Vector2Int(x, y);
    }

    public Vector2 FindAvaiblePivotPosition(Vector2 position)
    {
        Vector2 pivotPosition = new Vector2(0, 0);
        float xPos;
        float yPos = 0;

        for (int y = 0; y < (gridHeight - 1) * 2; y++)
        {
            xPos = y % 2 == 0 ? (core.pivotOffset.x / 2) : core.pivotOffset.x;
            yPos += (core.unitOffset.y / 2);
            for (int x = 0; x < gridWidth - 1; x++)
            {
                if (x != 0)
                    xPos += ((y % 2 == 0) ? (x % 2 == 0) : (x % 2 != 0)) ? core.pivotOffset.x : (core.pivotOffset.x * 2);

                float newDistance = Vector2.Distance(position, new Vector2(xPos, yPos));
                float oldDistance = Vector2.Distance(position, pivotPosition);
                
                if (newDistance < oldDistance)
                {
                    pivotPosition = new Vector2(xPos, yPos);
                }
            }
        }

        return pivotPosition;
    }

    public void CreateGroup(Vector2 pivotPosition)
    {
        if (pivot != null)
            ReleaseGroup();

        List<GameObject> hexList = new List<GameObject>();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (Vector2.Distance(pivotPosition, grid[x, y].transform.position) < core.unitOffset.x)
                {
                    hexList.Add(grid[x, y]);
                }
            }
        }

        if (hexList.Count == 3)
        {
            pivot = Instantiate(core.pivot);
            pivot.transform.position = pivotPosition;

            foreach (GameObject hex in hexList)
            {
                hex.transform.SetParent(pivot.transform);
                hex.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
        }
    }

    public void ReleaseGroup()
    {
        if (pivot == null)
            return;

        for (int i = 0; i < pivot.transform.childCount; i++)
        {
            pivot.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 0;
        }

        pivot.transform.DetachChildren();
        Destroy(pivot);
        pivot = null;
    }

    public bool IsHexagonInGroup(GameObject hexagon)
    {
        bool inGroup = false;

        if (pivot != null)
        {
            for (int i = 0; i < pivot.transform.childCount; i++)
            {
                if (pivot.transform.GetChild(i) == hexagon.transform)
                {
                    inGroup = true;
                    break;
                }
            }
        }

        return inGroup;
    }

    public bool CheckBounds(int x, int y)
    {
        return (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight) ? true : false;
    }

    public GameObject[] CheckGroup()
    {
        List<GameObject> breakList = new List<GameObject>();

        for (int i = 0; i < pivot.transform.childCount; i++)
        {
            Transform hex = pivot.transform.GetChild(i);
            Vector2Int coordinate = ConvertPositionToGrid(hex.position);
            List<GameObject> list = CheckHexagon(coordinate.x, coordinate.y);

            foreach (GameObject h in list)
            {
                if (!breakList.Contains(h))
                    breakList.Add(h);
            }
        }

        return breakList.ToArray();
    }

    public List<GameObject> CheckHexagon(int gridX, int gridY)
    {
        Vector2Int[] oddVectors = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1) };

        Vector2Int[] evenVectors = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, -1),
            new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0) };


        List<GameObject> breakList = new List<GameObject>();
        Vector2Int[] currentVectors = gridX % 2 == 0 ? evenVectors : oddVectors;

        GameObject originHex = grid[gridX, gridY];
        Color originColor = originHex.GetComponent<Unit>().color;

        for (int i = 0; i < 6; i++)
        {
            Color color1;
            Color color2;

            int next = i == 5 ? 0 : i + 1;

            if (!CheckBounds(currentVectors[i].x + gridX, currentVectors[i].y + gridY) ||
                !CheckBounds(currentVectors[next].x + gridX, currentVectors[next].y + gridY))
                continue;

            GameObject hex1 = grid[currentVectors[i].x + gridX, currentVectors[i].y + gridY];
            GameObject hex2 = grid[currentVectors[next].x + gridX, currentVectors[next].y + gridY];

            if (!(IsHexagonInGroup(originHex) && IsHexagonInGroup(hex1) && IsHexagonInGroup(hex2)))
            {
                color1 = hex1.GetComponent<Unit>().color;
                color2 = hex2.GetComponent<Unit>().color;

                if (originColor == color1 && originColor == color2)
                {
                    if (!breakList.Contains(originHex))
                        breakList.Add(originHex);

                    if (!breakList.Contains(hex1))
                        breakList.Add(hex1);

                    if (!breakList.Contains(hex2))
                        breakList.Add(hex2);
                }
            }
        }

        return breakList;
    }

    public GameObject FindBomb()
    {
        GameObject bomb = null;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].TryGetComponent<Bomb>(out _))
                    {
                        bomb = grid[x, y];
                        break;
                    }
                }
            }

            if (bomb != null)
                break;
        }

        return bomb;
    }

    public void CreateHexagon(int x, int y, Color color, bool isBomb = false, float heightOffset = 15)
    {
        GameObject hex = Instantiate(isBomb ? core.bomb : core.hexagon);

        hex.GetComponent<Unit>().Init(color);
        Vector2 position = ConvertGridToPosition(x, y);
        position.y += heightOffset;
        hex.transform.position = position;
        grid[x, y] = hex;
    }

    public void BreakHexagon(GameObject hexagon)
    {
        Vector2Int coordinate = ConvertPositionToGrid(hexagon.transform.position);
        hexagon.GetComponent<Unit>().Break();
        grid[coordinate.x, coordinate.y] = null;
    }

    public void DropHexagonsOnGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            int count = 0;

            for (int i = 0; i < gridHeight; i++)
            {
                if (grid[x, i] == null)
                    count++;
            }

            for (int j = 1; j <= count; j++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (y > 0 && grid[x, y - 1] == null)
                        {
                            grid[x, y - 1] = grid[x, y];
                            grid[x, y] = null;
                        }
                    }
                }
            }
        }
    }

    public bool FixAllPositions(float speed = 10, float delay = 0.001f)
    {
        bool isCompleted = true;
        int counter = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null)
                {
                    Vector2 currentPosition = grid[x, y].transform.position;
                    Vector2 newPosition = Vector2.MoveTowards(currentPosition, ConvertGridToPosition(x, y), 
                        Mathf.Clamp((speed * Time.deltaTime) - (delay * counter), 0, speed * Time.deltaTime));

                    if (currentPosition != newPosition)
                    {
                        grid[x, y].transform.position = newPosition;
                        isCompleted = false;
                        counter++;
                    }
                }
            }
        }

        return isCompleted;
    }

    public void StartGroupRotation(RotateDirection direction)
    {
        if (pivot != null)
        {
            rotator = new Rotator(pivot, direction);
            core.onMoving = true;
        }
    }

    private void RotateGroup()
    {
        Quaternion currentRotation = pivot.transform.rotation;
        RotationResult result = rotator.Rotate(currentRotation);
        pivot.transform.rotation = result.quaternion;

        if (result.isStepCompleted)
        {
            for (int i = 0; i < pivot.transform.childCount; i++)
            {
                Transform hex = pivot.transform.GetChild(i);
                Vector2Int coordinate = ConvertPositionToGrid(hex.position);
                grid[coordinate.x, coordinate.y] = hex.gameObject;
            }

            if (result.isRotationCompleted)
            {
                StopGroupRotation();
                fixPositions = true;
                return;
            }

            GameObject[] breakList = CheckGroup();

            if (breakList.Length > 0)
            {
                ReleaseGroup();

                foreach (GameObject hex in breakList)
                {
                    BreakHexagon(hex);
                }

                DropHexagonsOnGrid();
                FillInBlanks();
                StopGroupRotation();
                fixPositions = true;
                core.IncreaseMoves();

                GameObject bomb = FindBomb();
                if (bomb != null)
                {
                    bomb.GetComponent<Bomb>().counter--;
                    if (bomb.GetComponent<Bomb>().counter == 0)
                    {
                        core.GameOver();
                    }
                }

                return;
            }
        }
    }

    public void StopGroupRotation()
    {
        rotator = null;
    }
}
