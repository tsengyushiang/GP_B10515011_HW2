using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paintingField : MonoBehaviour {

    public int m_radius = 100;          // Radius for the drawing circle  
    private Texture2D p_MaskTex;        // Mask texture instance of the shader

    private int p_Dim;                  // Dimension of the sprite
    private int p_Width, p_Height;      // Width and height of the sprite

    private Color[] p_Circle;           // Brush buffer
    private SpriteRenderer p_SpriteRender;

    // Use this for initialization
    void Start()
    {
        // Get the main sprite
        p_SpriteRender = GetComponent<SpriteRenderer>();
        Sprite sprite = p_SpriteRender.sprite;

        // Get the dimension of the sprite
        p_Width = (int)sprite.rect.width;
        p_Height = (int)sprite.rect.height;
        p_Dim = p_Width * p_Height;

        // Create the mask texture and bind to the material (shader)
        p_MaskTex = new Texture2D(p_Width, p_Height, TextureFormat.RGBA32, false);
        p_SpriteRender.material.SetTexture("_AlphaTex", p_MaskTex);

        // Create the mask color buffer and set to white (Alpha channels are 1)
        Color[] maskColor = new Color[p_Dim];
        for (int i = 0; i < p_Dim; i++)
        {
            maskColor[i] = Color.white;
        }
        p_MaskTex.SetPixels(maskColor);
        p_MaskTex.Apply();

        // Create the circle mask by radius
        p_Circle = new Color[m_radius * m_radius * 4];
        for (int i = 0; i < m_radius * 2; i++)
        {
            for (int j = 0; j < m_radius * 2; j++)
            {
                float dis = Mathf.Sqrt(Mathf.Pow(j - m_radius, 2) + Mathf.Pow(i - m_radius, 2));
                if (dis < m_radius)
                {
                    // Smoothed outline
                    p_Circle[i * m_radius * 2 + j] = Color.white - Color.white * dis / m_radius;
                }
                else
                    p_Circle[i * m_radius * 2 + j] = Color.black;
            }
        }

        UpdateMask(Vector3.zero);
        UpdateMask(Vector3.zero);
        UpdateMask(Vector3.zero);
        UpdateMask(Vector3.zero);
        UpdateMask(Vector3.zero);
        UpdateMask(Vector3.zero);
    }


    private void UpdateMask(Vector3 relative_pos)
    {
        // Vector3(0, 0, z) of the relative position will be the center of the Sprite (SpriteRenderer)

        // Get the boundary of the SpriteRenderer
        Vector2 bound = new Vector2(p_SpriteRender.bounds.size.x / transform.localScale.x, p_SpriteRender.bounds.size.y / transform.localScale.y);

        // Normalize the position between +/- 0.5 and move the origin to the bottom-left corner (coordinate of Texture2D)
        Vector2 uvf = new Vector2(Mathf.Clamp(relative_pos.x, -bound.x / 2, bound.x / 2) / bound.x, Mathf.Clamp(relative_pos.y, -bound.y / 2, bound.y / 2) / bound.y);
        uvf += new Vector2(0.5f, 0.5f);

        // Get (x, y) pixel of the Texture2D
        Vector2 centerPos = new Vector2((uvf.x * p_Width), (uvf.y * p_Height));
        Vector2 startPos = new Vector2(centerPos.x - m_radius, centerPos.y - m_radius); // bottom-left
        Vector2 endPos = new Vector2(centerPos.x + m_radius, centerPos.y + m_radius); // top-right

        // Check boundary to prevent out of bound
        Vector2 offsetX = new Vector2();    // Width
        Vector2 offsetY = new Vector2();    // Height
        if (startPos.x < 0) offsetX.x = Mathf.Abs(startPos.x);
        if (endPos.x > p_Width) offsetX.y = endPos.x - p_Width;
        if (startPos.y < 0) offsetY.x = Mathf.Abs(startPos.y);
        if (endPos.y > p_Height) offsetY.y = endPos.y - p_Height;

        // The real boundary
        startPos = new Vector2((startPos.x + offsetX.x), (startPos.y + offsetY.x));
        Vector2 range = new Vector2(m_radius * 2 - offsetX.y, m_radius * 2 - offsetY.y);

        // Get pixels from the mask texture
        Color[] color = p_MaskTex.GetPixels((int)startPos.x, (int)startPos.y, (int)range.x, (int)range.y);
        for (int x = (int)offsetX.x; x < m_radius * 2 - (int)offsetX.y - 1; x++)
        {
            for (int y = (int)offsetY.x; y < m_radius * 2 - (int)offsetY.y - 1; y++)
            {
                // Subtract the circle color
                color[x * m_radius * 2 + y] = color[x * m_radius * 2 + y] - p_Circle[x * m_radius * 2 + y] / 4;
            }
        }
        p_MaskTex.SetPixels((int)startPos.x, (int)startPos.y, (int)range.x, (int)range.y, color);
        p_MaskTex.Apply();
    }
}
