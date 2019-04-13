using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance;
    public Color[] colors;

    public PaintedField playGround;
    public Text counter;
    public GameObject WinerTag;

    const int playTime=30;
    [SyncVar]
    private int currentTime;

    public Texture2D bullet;
    public Texture2D ghostFootPrint;

    public Vector3 latestPainted;

    void Awake() {
        Instance = this;
    }

    public void OnEnable() {
        currentTime = playTime;

        
        InvokeRepeating("CmdUpdateCounter", 1f, 1f);
    }

    public void OnDisable() {
        currentTime = playTime;
        playGround.setDefaultAlphaTex();
        WinerTag.SetActive(false);
        CancelInvoke();

    }

    [Command]
    public void CmdUpdateCounter()
    {
        if(isServer)
        RpcUpdateCounter();
    }
    [ClientRpc]
    public void RpcUpdateCounter()
    {
        currentTime--;
        counter.text = currentTime.ToString();
        if (currentTime == 0) {
            counter.text = "";
            CancelInvoke();
            WinerTag.SetActive(true);
            WinerTag.GetComponent<Result>().setColor(playGround.CalcResult());
        }
    }

    [ClientRpc]
    public void RpcReStart()
    {
        currentTime = playTime;
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
