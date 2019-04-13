using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance;
    public Color[] colors;

    public PaintedField playGround;

    public Texture2D bullet;
    public Texture2D ghostFootPrint;

    public Vector3 latestPainted;

    void Awake() {
        Instance = this;
    }

    public void OnDisable() {
        playGround.setDefaultAlphaTex();
    }

    [ClientRpc]
    public void RpcReStart()
    {
        playGround.setDefaultAlphaTex();
    }

    [ClientRpc]
    public void RpcBulletHit(Vector3 pos,Color color) {

        PaintByBrush(pos, color, bullet);
        latestPainted = pos;

    }

    [ClientRpc]
    public void RpcGhostWalk(Vector3 pos, Color color)
    {
        PaintByBrush(pos, color, ghostFootPrint);

    }

    public void PaintByBrush(Vector3 pos, Color color, Texture2D texture)
    {
        Vector3 p = new Vector3(pos.x / playGround.transform.localScale.x,
            pos.y / playGround.transform.localScale.y, 0);

        playGround.paint(p, texture, color);
    }

}
