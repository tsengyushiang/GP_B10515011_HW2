using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
    public float _threshold = 1.0f;

    public void OnDisable()
    {
        _threshold = 1.0f;
        GetComponent<SpriteRenderer>().material.SetFloat("_Threshold", _threshold);

    }

    public void OnEnable() {
        _threshold = 1.0f;
        GetComponent<SpriteRenderer>().material.SetFloat("_Threshold", _threshold);

    }

    public void setColor(Color color)    {

        GetComponent<SpriteRenderer>().material.SetColor("_Color", color);

    }

    void Update()
    {

        if (_threshold > 0.0)
        {
            _threshold -= 0.5f*Time.deltaTime;
            GetComponent<SpriteRenderer>().material.SetFloat("_Threshold", _threshold);
        }
    }

}
