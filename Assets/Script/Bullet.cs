using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SyncVar(hook ="setColor")]
    public Color mycolor;
    public Vector3 StartPoint;

    public override void OnStartClient() {
        setColor(mycolor);
        StartPoint = transform.position;
    }

    public void setColor(Color color)
    {
        mycolor = color;
        GetComponent<SpriteRenderer>().material.SetColor("_Color",color);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return;
        GameManager.Instance.RpcBulletHit(transform.position, mycolor);
        Destroy(gameObject);
    }
}
