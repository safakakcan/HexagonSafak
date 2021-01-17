using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [Header("Game Statistics")]
    private int score = 0;
    private int moves = 0;
    private int bombSpawned = 0;
    private bool onMoving = false;
    
    [Header("Color Settings")]
    public Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green, Color.magenta };

    [Header("UI Settings")]
    public UnityEngine.UI.Text scoreBoard;
    public UnityEngine.UI.Text moveCounter;
    public GameObject GameOverMessage;

    [Header("Hexagon Settings")]
    public GameObject hexagon;
    public GameObject bomb;
    public GameObject breakingFX;
    public GameObject pointFX;
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


    /// <summary>
    /// Core sınıfı içinde depolanan oyun istatisiklerini sıfırlar.
    /// </summary>
    public void ResetStatistics()
    {
        score = 0;
        moves = 0;
        bombSpawned = 0;
    }

    /// <summary>
    /// Oyunun sona erdiğini bildiren mesajı ekranda görüntüler. GameOverMessage değişkeninde tanımlanan örnek kullanılır.
    /// </summary>
    public void GameOver()
    {
        GameOverMessage.SetActive(true);
    }

    /// <summary>
    /// Kameranın istenilen boyutlara ve pozisyona getirilmesini sağlar.
    /// </summary>
    /// <param name="size">Uygulanacak boyutun yatay genişliğidir.</param>
    /// <param name="position">Uygulanacak pozisyondur.</param>
    public void SetCameraSize(float size, Vector3 position)
    {
        Camera.main.GetComponent<Camera>().orthographicSize = size;

        Camera.main.transform.position = position;
    }

    /// <summary>
    /// Yeni bir hamlenin başladığını bildirmek için kullanılır.
    /// </summary>
    public void StartMoving()
    {
        onMoving = true;
    }

    /// <summary>
    /// Mevcut hamlenin sona erdiğini bildirmek için kullanılır.
    /// </summary>
    public void EndMoving()
    {
        onMoving = false;
    }

    /// <summary>
    /// Devam etmekte olan bir hamle olup olmadığını kontrol eder.
    /// </summary>
    /// <returns>Devam eden hamle varsa 'True' değerini döndürür.</returns>
    public bool IsMoving()
    {
        return onMoving;
    }

    /// <summary>
    /// Oyuncu başarım elde ettiğinde puan kazanabilmesi için kullanılır. Kazanılan puan Core'da tanımlı olan
    /// 'pointFX' örneğine gönderilir ve toplam skora eklenir. Ardından 'scoreBoard' örneğinde tanımlı olan
    /// skor tablosu güncellenir.
    /// </summary>
    /// <param name="points">Kazanılacak puanı ifade eder.</param>
    /// <param name="position">Görsel materyalin Instantiate edileceği pozisyonu ifade eder.</param>
    public void IncreaseScore(int points, Vector2 position)
    {
        score += points;
        GameObject text = Instantiate(pointFX);
        text.GetComponent<Points>().SetPoints(points);
        text.transform.position = position;
        scoreBoard.text = score.ToString();
    }

    /// <summary>
    /// Oyuncu tarafından bir hamle yapıldığında ilgili sayaç arttırılır. 'moveCounter' örneğindeki tablo güncellenir.
    /// </summary>
    /// <param name="count"></param>
    public void IncreaseMoves(int count = 1)
    {
        moves += count;
        moveCounter.text = moves.ToString();
    }

    /// <summary>
    /// Oyunda kaç adet bomba kullanıldığı ile ilgili sayacı arttırmak için kullanılır.
    /// </summary>
    /// <param name="count">Sayacın kaç adet arttırılacağını ifade eder.</param>
    public void IncreaseBombSpawned(int count = 1)
    {
        bombSpawned += count;
    }

    /// <summary>
    /// Oyundaki mevcut skoru verir.
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Oyunda kaç adet hamle yapıldığını verir.
    /// </summary>
    /// <returns></returns>
    public int GetMoves()
    {
        return moves;
    }

    /// <summary>
    /// Oyunda kaç adet bomba kullanıldığını verir.
    /// </summary>
    /// <returns></returns>
    public int GetBombSpawned()
    {
        return bombSpawned;
    }

    /// <summary>
    /// 'colors' değişkeninde tanımlanmış renkler arasından rastgele bir renk verir.
    /// </summary>
    /// <returns></returns>
    public Color GetRandomColor()
    {
        return colors[Random.Range(0, colors.Length)];
    }
}

/// <summary>
/// Her tur ve belirlenen adım geçişlerinde dışarı bilgi verebilen, belli bir nesneyi belli bir yönde döndürebilen
/// çok amaçlı bir "Döndürücü" sınıftır.
/// </summary>
public class Rotator
{
    private RotateDirection direction = RotateDirection.Clockwise;
    private float speed = 360;
    private int maxStep = 3;
    private int step = 1;

    public Rotator(RotateDirection RotationDirection, int MaxStep = 3, float Speed = 500)
    {
        direction = RotationDirection;
        maxStep = MaxStep;
        speed = Speed;
    }

    /// <summary>
    /// Devam eden bir döngü için sürekli olarak çağırılması gereken bir döndürme Method'udur.
    /// </summary>
    /// <param name="currentQuaternion">Döndürülecek nesnenin mevcut dönüş değerini ifade eder.</param>
    /// <returns>Hesaplanan yeni dönüş bilgilerini RotationResult olarak verir.</returns>
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

/// <summary>
/// Dönüş yönlerini ifade eden bir Enumeration yapısıdır.
/// </summary>
public enum RotateDirection
{
    Clockwise,
    CounterClockwise
}

/// <summary>
/// Rotator sınıfı tarafından yapılan hesaplamalardan sonra verilen çıktı değeridir. Dönüş bilgisi ile birlikte
/// tur ve adımların tamamlanıp tamamlanmadığına dair bilgi içerir.
/// </summary>
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