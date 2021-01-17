using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Unit
{
    private int counter = 7;
    private bool flipFlop = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector2.zero);

        // Bomba ikonunun hareketli olarak yerinini bildiriyoruz. Pinpon tekniği ile boyutunu bir miktar arttırıp azaltıyoruz.
        if (flipFlop)
        {
            float scale = transform.GetChild(0).localScale.x + (Time.deltaTime * 2);
            transform.GetChild(0).localScale = new Vector2(scale, scale);
            if (scale > 1.5f)
                flipFlop = false;
        }
        else
        {
            float scale = transform.GetChild(0).localScale.x - (Time.deltaTime * 2);
            transform.GetChild(0).localScale = new Vector2(scale, scale);
            if (scale < 1.0f)
                flipFlop = true;
        }
    }

    /// <summary>
    /// Bombanın sayacını çalıştırır.
    /// </summary>
    /// <returns></returns>
    public bool TickTock()
    {
        // Sayacı '1' azaltıyoruz.
        counter--;
        transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshPro>().text = counter.ToString();

        if (counter == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
