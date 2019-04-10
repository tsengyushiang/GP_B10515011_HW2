using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SyncVar (hook= "setColor")]
    public Color myColor;
    public float speed = 500;
    public float gunPos = 2;
    public GameObject bullet;


    public float shootCoolDown = 500;
    private float timeStamp = 0;

    private string playerID;

    void Start()
    {
        setColor(myColor);
    }

    public void setColor(Color color) {
        myColor = color;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = color;
        }
    }

    [Command]
    public void CmdExplode(Vector3 origion, Vector3 direction)
    {
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = origion;
        newBullet.GetComponent<Rigidbody2D>().AddForce(direction * 1000);

        //設定飛彈朝向的方向
        newBullet.transform.Rotate(0, 0,
            Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x), Space.Self);

        if (direction.x > 0.0f)
            newBullet.transform.Rotate(0, 0, 180, Space.Self);

        //設定飛彈顏色
        newBullet.GetComponent<Bullet>().setColor(myColor);
        NetworkServer.Spawn(newBullet);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        timeStamp+=Time.deltaTime;

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
            Vector3 origion =transform.position+ gunDir * gunPos * 3f;
            CmdExplode(origion,gunDir);
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
}
