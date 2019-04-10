using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class EraserAI : NetworkBehaviour
{

    [SerializeField]
    private Vector3 dst;
    public float moveSpeed;
    private string latestSpriteName;

    void Start() {
        InvokeRepeating("getDis", 2f, 1f);
    }

    void getDis() {
        dst=GameManager.Instance.latestPainted;
    }

	// Update is called once per frame
	void Update () {

        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
       
        //按動畫frame數去清除畫布
        string spriteName = GetComponent<SpriteRenderer>().sprite.name;

        if (spriteName == "ghostSheet_6" && latestSpriteName!=spriteName) {
            GameManager.Instance.RpcGhostWalk(transform.position, new Color(0,0,0,0));
        }
        latestSpriteName = spriteName;


        //自動追蹤
        Vector3 move = Vector3.zero;
      
        move.x += Mathf.Sign(dst.x-transform.position.x) * moveSpeed*Time.deltaTime;
        move.y += Mathf.Sign(dst.y-transform.position.y  ) * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, dst) < moveSpeed * Time.deltaTime * 1.414f)
            transform.position = dst;
        else
            transform.position += move;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return;
        //Destroy(gameObject);
    }

}
