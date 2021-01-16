using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [Header("Game Statistics")]
    public int score = 0;
    public int moves = 0;
    public int bombSpawned = 0;
    public bool onMoving = false;
    
    [Header("Color Settings")]
    public Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green, Color.magenta };

    [Header("Hexagon Settings")]
    public GameObject hexagon;
    public GameObject bomb;
    public GameObject pivot;
    public Vector2 unitOffset = new Vector2(0.75f, 0.86f);
    public Vector2 pivotOffset = new Vector2(0.5f, 0.42f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCameraSize(float size, Vector3 position)
    {
        Camera.main.GetComponent<Camera>().orthographicSize = size;

        Camera.main.transform.position = position;
    }

    public void IncreaseScore(int points)
    {
        score += points;
    }

    public void IncreaseMoves(int count = 1)
    {
        moves += count;
    }

    public Color GetRandomColor()
    {
        return colors[Random.Range(0, colors.Length)];
    }

    public void GameOver()
    {

    }
}

public enum RotateDirection
{
    Clockwise,
    CounterClockwise
}

public class Rotator
{
    private GameObject pivot;
    private RotateDirection direction = RotateDirection.Clockwise;
    private float speed = 360;
    private int maxStep = 3;
    private int step = 1;

    public Rotator(GameObject Pivot, RotateDirection RotationDirection, int MaxStep = 3, float Speed = 500)
    {
        pivot = Pivot;
        direction = RotationDirection;
        maxStep = MaxStep;
        speed = Speed;
    }

    public RotationResult Rotate(Quaternion currentQuaternion)
    {
        float targetZ = direction == RotateDirection.Clockwise ? ((360 / maxStep) * step) : -((360 / maxStep) * step);
        Quaternion targetQuaternion = Quaternion.Euler(new Vector3(0, 0, targetZ));
        Quaternion newQuaternion = Quaternion.RotateTowards(currentQuaternion, targetQuaternion, Time.deltaTime * speed);

        bool stepCompleted = (newQuaternion == targetQuaternion);
        bool rotationCompleted = false;

        if (stepCompleted)
        {
            if (step == maxStep)
            {
                step = 1;
                rotationCompleted = true;
            }
            else
            {
                step++;
            }
        }

        return new RotationResult(newQuaternion, stepCompleted, rotationCompleted);
    }
}

public struct RotationResult
{
    public Quaternion quaternion;
    public bool isStepCompleted;
    public bool isRotationCompleted;

    public RotationResult(Quaternion Quaternion, bool IsStepCompleted, bool IsRotationCompleted)
    {
        quaternion = Quaternion;
        isStepCompleted = IsStepCompleted;
        isRotationCompleted = IsRotationCompleted;
    }
}