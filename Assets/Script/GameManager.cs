using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public  static PaintedField playGround;

    public void Awake() {
        playGround = GameObject.Find("playGround").GetComponent<PaintedField>();
    }

    public static void Shoot(GameObject bullet,Vector3 origion, Vector3 direction, Color color) {
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = origion;
        newBullet.GetComponent<Rigidbody2D>().AddForce(direction * 1000);
    
        //設定飛彈朝向的方向
        newBullet.transform.Rotate(0, 0,
            Mathf.Rad2Deg * Mathf.Atan(direction.y/ direction.x), Space.Self);

        if(direction.x>0.0f)
            newBullet.transform.Rotate(0, 0,180, Space.Self);

        //設定飛彈顏色
        newBullet.GetComponent<Bullet>().setColor(color);
    }

    public static void Explosion(Vector3 pos,Texture2D footPrint,Color color) {

        Vector3 p = new Vector3(pos.x / playGround.transform.localScale.x,
            pos.y / playGround.transform.localScale.y, 0);

        playGround.paint(p, footPrint, color);
    }


}
