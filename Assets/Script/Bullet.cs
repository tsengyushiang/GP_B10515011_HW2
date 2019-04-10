using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    public Color mycolor;
    public Texture2D explode;

    public void setColor(Color color) {
        mycolor = color;
        GetComponent<SpriteRenderer>().color = color;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Explosion(transform.position, explode, mycolor);
        Destroy(gameObject);
    }
}
