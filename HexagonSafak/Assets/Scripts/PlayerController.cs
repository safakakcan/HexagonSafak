using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Core core;
    public GridSystem gridSystem;

    private Vector2 DragBeginPos = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!core.IsMoving())
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
                    // Kaydırma hareketi yapılmamışsa uygun olan en yakın noktada bir grup oluştur.
                    Vector2 pivotPosition = gridSystem.FindAvaiblePivotPosition(mousePosition);
                    gridSystem.CreateGroup(pivotPosition);
                    DragBeginPos = Vector2.zero;
                }
            }

            if (gridSystem.GetPivot() != null && DragBeginPos != Vector2.zero 
                && Vector2.Distance(DragBeginPos, gridSystem.GetPivot().transform.position) < 2
                && Vector2.Distance(DragBeginPos, mousePosition) > 0.5f)
            {
                // Kaydırma hareketi yapılmışsa kaydırılan yönde bir döndürme işlemi başlat.
                RotateDirection direction = CalculateRotationDirection(mousePosition);
                gridSystem.StartGroupRotation(direction);
                DragBeginPos = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// Mevcut işaretçi konumunu kullanarak bloğun hangi yönde döndürüldüğünü hesaplar.
    /// </summary>
    /// <param name="pullDirection">Mevcut işaretçi konumunu ifade eder.</param>
    /// <returns></returns>
    public RotateDirection CalculateRotationDirection(Vector2 pullDirection)
    {
        RotateDirection direction = RotateDirection.Clockwise;
        Vector2 pivotPosition = gridSystem.GetPivot().transform.position;
        Vector2 V1 = DragBeginPos - pivotPosition;
        Vector2 V2 = pullDirection - pivotPosition;

        float angle = Vector2.SignedAngle(V1, V2);
        if (angle < 0)
            direction = RotateDirection.CounterClockwise;

        return direction;
    }

    /// <summary>
    /// Tüm istatistikleri ve oyunu sıfırlayarak yeni bir oyun başlatır.
    /// </summary>
    public void ResetGame()
    {
        core.ResetStatistics();
        gridSystem.Reset();
    }
}
