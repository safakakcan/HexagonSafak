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
        transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshPro>().text = counter.ToString();
        transform.rotation = Quaternion.Euler(Vector2.zero);

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

    public bool TickTock()
    {
        counter--;

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
