using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintedField : MonoBehaviour {

    private Texture2D p_MaskTex;        // Mask texture instance of the shader
    private int p_Dim;                  // Dimension of the sprite
    private int p_Width, p_Height;      // Width and height of the sprite
    private SpriteRenderer p_SpriteRender;

    // Use this for initialization
    void Start()
    {
        setDefaultAlphaTex();
    }

    public Color CalcResult() {

        int Point1=0;
        int Point2=0;


        Color[] color = p_MaskTex.GetPixels(0, 0, p_Width, p_Height);
        for (int x = 0; x < p_Width - 1; x++)
        {
            for (int y = 0; y< p_Height - 1; y++)
            {

                if (color[x * p_Height + y].a == 0) continue;
                       
                if (GameManager.Instance.colors[0].r== color[x * p_Height + y].r)
                {
                    Point1++;
                }
                else if (GameManager.Instance.colors[1].r == color[x * p_Height + y].r) {
                    Point2++;
                }
             }
        }

        if (Point1 > Point2) {
            return GameManager.Instance.colors[0];
        }
        else if (Point1 < Point2){
            return GameManager.Instance.colors[1];
        }
        else{
            return Color.white;
        }
    }

    public void  setDefaultAlphaTex() {
        // Get the main sprite
        p_SpriteRender = GetComponent<SpriteRenderer>();
        Sprite sprite = p_SpriteRender.sprite;

        // Get the dimension of the sprite
        p_Width = (int)sprite.rect.width;
        p_Height = (int)sprite.rect.height;
        p_Dim = p_Width * p_Height;

        // Create the mask texture and bind to the material (shader)
        p_MaskTex = new Texture2D(p_Width, p_Height, TextureFormat.RGBA32, false);
        p_MaskTex.filterMode = FilterMode.Point;
        p_SpriteRender.material.SetTexture("_AlphaTex", p_MaskTex);

        // Create the mask color buffer and set to white (Alpha channels are 1)
        Color[] maskColor = new Color[p_Dim];

        for (int i = 0; i < p_Dim; i++)
        {
            maskColor[i] = new Color(0, 0, 0, 0);
        }
        p_MaskTex.SetPixels(maskColor);
        p_MaskTex.Apply();

    }


    public void paint(Vector3 paint_pos,Texture2D p_brush,Color paintColor)
    {
        int brush_w = p_brush.width/2;
        int brush_h = p_brush.height/2;

        Vector3 relative_pos = paint_pos-transform.position;

        // Vector3(0, 0, z) of the relative position will be the center of the Sprite (SpriteRenderer)

        // Get the boundary of the SpriteRenderer
        Vector2 bound = new Vector2(p_SpriteRender.bounds.size.x / transform.localScale.x, p_SpriteRender.bounds.size.y / transform.localScale.y);

        // Normalize the position between +/- 0.5 and move the origin to the bottom-left corner (coordinate of Texture2D)
        Vector2 uvf = new Vector2(Mathf.Clamp(relative_pos.x, -bound.x / 2, bound.x / 2) / bound.x, Mathf.Clamp(relative_pos.y, -bound.y / 2, bound.y / 2) / bound.y);
        uvf += new Vector2(0.5f, 0.5f);

        // Get (x, y) pixel of the Texture2D
        Vector2 centerPos = new Vector2((uvf.x * p_Width), (uvf.y * p_Height));
        Vector2 startPos = new Vector2(centerPos.x - brush_w, centerPos.y - brush_h); // bottom-left
        Vector2 endPos = new Vector2(centerPos.x + brush_w, centerPos.y + brush_h); // top-right

        // Check boundary to prevent out of bound
        Vector2 offsetX = new Vector2();    // Width
        Vector2 offsetY = new Vector2();    // Height
        if (startPos.x < 0) offsetX.x = Mathf.Abs(startPos.x);
        if (endPos.x > p_Width) offsetX.y = endPos.x - p_Width;
        if (startPos.y < 0) offsetY.x = Mathf.Abs(startPos.y);
        if (endPos.y > p_Height) offsetY.y = endPos.y - p_Height;

        // The real boundary
        startPos = new Vector2((startPos.x + offsetX.x), (startPos.y + offsetY.x));
        Vector2 range = new Vector2(brush_w * 2 - offsetX.y, brush_h * 2 - offsetY.y);

        Color[] p_Circle = p_brush.GetPixels(0,0, p_brush.width, p_brush.height);
        // Get pixels from the mask texture
        Color[] color = p_MaskTex.GetPixels((int)startPos.x, (int)startPos.y, (int)range.x, (int)range.y);
        for (int x = (int)offsetX.x; x < range.x - 1; x++)
        {
            for (int y = (int)offsetY.x; y < range.y - 1; y++)
            {
                int index = x * (int)range.y + y;

                if (index < 0 || index >= p_Circle.Length || index >= color.Length) continue;

                if (p_Circle[index].a != 0)
                {
                    color[index] = paintColor;
                }
            }
        }
        p_MaskTex.SetPixels((int)startPos.x, (int)startPos.y, (int)range.x, (int)range.y, color);
        p_MaskTex.Apply();
    }
}
