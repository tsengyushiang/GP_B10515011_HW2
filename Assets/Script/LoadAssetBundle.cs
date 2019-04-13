using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundle : MonoBehaviour {

    private AssetBundle p_AssetBundle;

    IEnumerator Load()
    {
        WWW www = new WWW("C:/Unity/assetbundle");
        yield return www;

        p_AssetBundle = www.assetBundle;

        if (p_AssetBundle != null)
        {

            Texture2D obj_Texture = p_AssetBundle.LoadAsset("Monster") as Texture2D;

            if (obj_Texture != null)
            {
                Sprite s = Sprite.Create(obj_Texture, new Rect(0, 0, obj_Texture.width, obj_Texture.height), Vector2.zero);
                GetComponent<SpriteRenderer>().sprite = s;
            }

            AudioClip obj_bgm = p_AssetBundle.LoadAsset("bgm") as AudioClip;
            if (obj_bgm != null)
            {
                GetComponent<AudioSource>().clip = obj_bgm;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    // Use this for initialization
    void Start () {

        StartCoroutine("Load");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
