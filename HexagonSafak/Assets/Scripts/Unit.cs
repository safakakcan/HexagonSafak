using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hexagon, Bomba veya türevlerini üretmek için soyut bir birim sınıfıdır.
/// </summary>
public abstract class Unit : MonoBehaviour
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Color"></param>
    /// <param name="BreakingFX"></param>
    public void Init(Color Color, GameObject BreakingFX)
    {
        color = Color;
        GetComponent<SpriteRenderer>().color = color;
        breakingFX = BreakingFX;
    }

    /// <summary>
    /// İlgili birimin, obje kısmının sahneden kaldırılmasını sağlar.
    /// </summary>
    public void Break()
    {
        GameObject fx = Instantiate(breakingFX);
        fx.transform.position = transform.position;
        ParticleSystem.MainModule main = fx.GetComponent<ParticleSystem>().main;
        main.startColor = color;
        Destroy(this.gameObject);
    }
}
