using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color color = GetComponent<TextMesh>().color;

        if (color.a <= 0)
            Destroy(this.gameObject);

        GetComponent<TextMesh>().color = new Color(1, 1, 1, color.a - Time.deltaTime);
        transform.Translate(new Vector2(0, Time.deltaTime * 2));
    }

    public void SetPoints(int points)
    {
        GetComponent<TextMesh>().text = points.ToString();
    }
}
