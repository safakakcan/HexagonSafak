using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Color color;
    private GameObject breakingFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Color Color, GameObject BreakingFX)
    {
        color = Color;
        GetComponent<SpriteRenderer>().color = color;
        breakingFX = BreakingFX;
    }

    public void Break()
    {
        GameObject fx = Instantiate(breakingFX);
        fx.transform.position = transform.position;
        ParticleSystem.MainModule main = fx.GetComponent<ParticleSystem>().main;
        main.startColor = color;
        Destroy(this.gameObject);
    }
}
