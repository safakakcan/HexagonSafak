using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Core core;
    private GridSystem gridSystem;

    private Vector2 DragBeginPos = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.FindGameObjectWithTag("Settings").GetComponent<Core>();
        gridSystem = GameObject.FindGameObjectWithTag("Settings").GetComponent<GridSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!core.onMoving)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                DragBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Vector2.Distance(DragBeginPos, mousePosition) <= 0.5f)
                {
                    Vector2 pivotPosition = gridSystem.FindAvaiblePivotPosition(mousePosition);
                    gridSystem.CreateGroup(pivotPosition);
                    DragBeginPos = Vector2.zero;
                }
            }

            if (gridSystem.pivot != null && DragBeginPos != Vector2.zero 
                && Vector2.Distance(DragBeginPos, gridSystem.pivot.transform.position) < 2
                && Vector2.Distance(DragBeginPos, mousePosition) > 0.5f)
            {
                RotateDirection direction = CalculateRotationDirection(mousePosition);
                gridSystem.StartGroupRotation(direction);
                DragBeginPos = Vector2.zero;
            }
        }
    }

    public RotateDirection CalculateRotationDirection(Vector2 pullDirection)
    {
        RotateDirection direction = RotateDirection.Clockwise;
        Vector2 pivotPosition = gridSystem.pivot.transform.position;
        Vector2 V1 = DragBeginPos - pivotPosition;
        Vector2 V2 = pullDirection - pivotPosition;

        float angle = Vector2.SignedAngle(V1, V2);
        if (angle < 0)
            direction = RotateDirection.CounterClockwise;

        return direction;
    }
}
