using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public Color myColor;
    public float speed = 500;
    public float gunPos = 2;
    public GameObject bullet;
    public GameObject RepresentCurrentPlayer;

    public float shootCoolDown = 800;
    private float timeStamp = 0;

    private string playerID;

   

    void Start()
    {
        //隨機分配玩家顏色
        if (isServer)
        {
            RpcsetColor(GameManager.Instance.colors[
           (int.Parse(GetComponent<NetworkIdentity>().netId.ToString()) % GameManager.Instance.colors.Length)
            ]);
        }

        RepresentCurrentPlayer.SetActive(isLocalPlayer);
    }

    public override void OnStartClient()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        renderers[0].color = myColor;
        renderers[1].color = myColor;
    }

    [Command]
    public void CmdsetColor(Color color)
    {
        RpcsetColor(color);
    }

    [ClientRpc]
    public void RpcsetColor(Color color)
    {
        myColor = color;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        renderers[0].color = myColor;
        renderers[1].color = myColor;
    }

    [Command]
    public void CmdExplode(Vector3 origion, Vector3 direction, Color color)
    {
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = origion;
        newBullet.GetComponent<Rigidbody2D>().AddForce(direction * 500);

        newBullet.GetComponent<Bullet>().mycolor = color;

        //設定飛彈朝向的方向
        newBullet.transform.Rotate(0, 0,
            Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x), Space.Self);

        if (direction.x > 0.0f)
            newBullet.transform.Rotate(0, 0, 180, Space.Self);

        //設定飛彈顏色
        NetworkServer.Spawn(newBullet);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        timeStamp += Time.deltaTime;

        // gun direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gunDir = Vector3.Normalize(mousePos - transform.position);

        gunDir.z = 0;
        float m = 1.0f / gunDir.magnitude;
        gunDir.x *= m;
        gunDir.y *= m;

        mousePos.z = 0;
        transform.GetChild(0).position = transform.position + gunDir * gunPos;
        //shoot
        if (Input.GetMouseButton(0) && timeStamp > shootCoolDown)
        {
            Vector3 origion = transform.position + gunDir * gunPos * 3f;
            CmdExplode(origion, gunDir, myColor);
            timeStamp = 0;
        }
        //player Movement
        Rigidbody2D rig = GetComponent<Rigidbody2D>();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        rig.velocity = movement * speed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return;

        GameItem item = collision.gameObject.GetComponent<GameItem>();

        if (item != null)
        {
            CmdsetColor(item.color);
        }

    }
}
