using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer middleGroundSpriteRenderer;
    [SerializeField] private SpriteRenderer frontGroundSpriteRenderer;
    private Texture2D frontOriginalTexture;
    private Texture2D middleOriginalTexture;
    private Color[] frontGroundInitialPixels;
    private Color[] middleGroundInitialPixels;
    private Texture2D frontGroundTexture;
    private Texture2D middleGroundTexture;

    // 真ん中のテクスチャの掘る穴の大きさ
    [SerializeField] private int digHoleRadius = 20;

    // フロントのテクスチャを掘る
    [SerializeField] private int digHoleMiddleTexture = 20;

    // ずれてほる分
    [SerializeField] private float digHoleOffset = 0.5f;

    private bool sarch;// サーチ中かどうか

    private void Awake()
    {
        frontOriginalTexture = frontGroundSpriteRenderer.sprite.texture;
        frontGroundInitialPixels = frontOriginalTexture.GetPixels();
        middleOriginalTexture = middleGroundSpriteRenderer.sprite.texture;
        middleGroundInitialPixels = middleOriginalTexture.GetPixels();
        frontGroundTexture = (Texture2D)frontGroundSpriteRenderer.sprite.texture;
        middleGroundTexture = (Texture2D)middleGroundSpriteRenderer.sprite.texture;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    /*
    private IEnumerator SarchAround(Vector2 position, int radius)
    {
        sarch = true;
        Color[] tmpGroundPixels = groundTexture.GetPixels();
        Color[] tmpMiddleGroundPixels = middleGroundTexture.GetPixels();
        //groundTexture
        //middleGroundTexture
        // テクスチャ座標に変換
        Vector2Int pixelPos = WorldToTextureCoord(position);
        // 穴の半径にもどついてテクスチャを操作
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (!IsPixelInsideTexture(pixelPos, groundTexture))
                {
                    continue;
                }
                // ピクセル座標を計算
                Vector2Int pixelOffset = new Vector2Int(x, y);
                Vector2Int pixel = pixelPos + pixelOffset;

                if (!IsPixelInsideTexture(pixel, groundTexture))
                {
                    continue;
                }
                if (pixelOffset.magnitude <= radius)
                {
                    Color transparentColor = groundTexture.GetPixel(pixel.x, pixel.y);
                    transparentColor.a = 0.3f;
                    groundTexture.SetPixel(pixel.x, pixel.y, transparentColor);
                }
            }
        }

        groundTexture.Apply();
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (!IsPixelInsideTexture(pixelPos, middleGroundTexture))
                {
                    continue;
                }
                // ピクセル座標を計算
                Vector2Int pixelOffset = new Vector2Int(x, y);
                Vector2Int pixel = pixelPos + pixelOffset;

                if (!IsPixelInsideTexture(pixel, middleGroundTexture))
                {
                    continue;
                }
                if (pixelOffset.magnitude <= radius)
                {
                    Color transparentColor = middleGroundTexture.GetPixel(pixel.x, pixel.y);
                    transparentColor.a = 0.3f;
                    middleGroundTexture.SetPixel(pixel.x, pixel.y, transparentColor);
                }
            }
        }
        middleGroundTexture.Apply();
        yield return new WaitForSeconds(1);
        groundTexture.SetPixels(tmpGroundPixels);
        middleGroundTexture.SetPixels(tmpMiddleGroundPixels);
        groundTexture.Apply();
        middleGroundTexture.Apply();
        sarch = false;
    }*/

    public void DigHoleAllTexture(Vector2 position, Define.DirectionNumber directionNum)
    {
        Vector2 digPosition = position + Define.directions[((int)directionNum)].normalized * 0.2f;
        Debug.Log(Define.directions[((int)directionNum)].normalized);
        DigHole(middleGroundTexture, digPosition, digHoleRadius);
        DigHole(frontGroundTexture, digPosition, digHoleRadius + digHoleMiddleTexture);
    }

    private void DigHole(Texture2D texture, Vector2 position, int radius)
    {
        // テクスチャ座標に変換
        Vector2Int pixelPos = WorldToTextureCoord(position);

        // 穴の半径にもどついてテクスチャを操作
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (!IsPixelInsideTexture(pixelPos, texture))
                {
                    continue;
                }
                // ピクセル座標を計算
                Vector2Int pixelOffset = new Vector2Int(x, y);
                Vector2Int pixel = pixelPos + pixelOffset;

                if (pixelOffset.magnitude <= radius)
                {
                    Color transparentColor = new Color(0, 0, 0, 0);
                    texture.SetPixel(pixel.x, pixel.y, transparentColor);
                }
            }
        }
        texture.Apply();
    }

    private bool IsPixelInsideTexture(Vector2Int pixel, Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        return pixel.x >= 0 && pixel.x < width && pixel.y >= 0 && pixel.y < height;
    }

    private Vector2Int WorldToTextureCoord(Vector2 worldPos)
    {
        Vector2 textureSize = new Vector2(frontGroundSpriteRenderer.sprite.texture.width, frontGroundSpriteRenderer.sprite.texture.height);
        float pixelPerUnit = frontGroundSpriteRenderer.sprite.pixelsPerUnit;

        // ワールド座標をスプライトの中心を原点とした相対座標に変換
        Vector2 localPos = worldPos - (Vector2)frontGroundSpriteRenderer.transform.position;
        int texturX = (int)(textureSize.x / 2 + localPos.x * pixelPerUnit);
        int textureY = (int)(textureSize.y / 2 + localPos.y * pixelPerUnit);

        Vector2Int pixelCoord = new Vector2Int(
            texturX, textureY
        );

        return pixelCoord;
    }

    public void ResetTexture()
    {
        Texture2D texture = (Texture2D)frontGroundSpriteRenderer.sprite.texture;
        texture.SetPixels(frontGroundInitialPixels);
        texture.Apply();
        texture = (Texture2D)middleGroundSpriteRenderer.sprite.texture;
        texture.SetPixels(middleGroundInitialPixels);
        texture.Apply();
        //groundSpriteRenderer.sprite = Sprite.Create(originalTexture, groundSpriteRenderer.sprite.rect, groundSpriteRenderer.sprite.pivot);
    }

    private void OnApplicationQuit()
    {
        ResetTexture();
    }

    /*
    // 中間の地面
    [SerializeField] private SpriteRenderer middleGroundSpriteRenderer;

    // 一番表の地面
    [SerializeField] private SpriteRenderer frontGroundSpriteRenderer;

    // 中間の地面の元のテクスチャ
    private Texture2D middleGroundOriginalTexture;

    // 一番表の地面の元のテクスチャ
    private Texture2D frontGroundOriginalTexture;

    // 中間の地面の元のピクセル値
    private Color[] middleGroundInitialPixels;

    // 一番表の地面の元のピクセル値
    private Color[] frontGroundInitialPixels;

    // 掘る中間の地面のテクスチャ
    private Texture2D middleGroundTexture;

    // 掘る一番表の地面のテクスチャ
    private Texture2D frontGroundTexture;

    // 穴の半径
    private int digHoleRadius = 20;

    private void Awake()
    {
        middleGroundOriginalTexture = middleGroundSpriteRenderer.sprite.texture;
        middleGroundInitialPixels = middleGroundOriginalTexture.GetPixels();
        frontGroundOriginalTexture = frontGroundSpriteRenderer.sprite.texture;
        frontGroundInitialPixels = frontGroundOriginalTexture.GetPixels();
        middleGroundTexture = (Texture2D)middleGroundSpriteRenderer.sprite.texture;
        frontGroundTexture = (Texture2D)frontGroundSpriteRenderer.sprite.texture;
    }

    // Start is called before the first frame update
    private void Start()
    {
        DigHole(frontGroundTexture, new Vector2(0, 0), 20, 0);
    }

    /// <summary>
    /// middleGroundとfrontGroundを掘る関数
    /// </summary>
    public void DigHoleAllTexture(Vector2 position)
    {
        DigHole(middleGroundTexture, position, digHoleRadius, 0);
        DigHole(frontGroundTexture, position, digHoleRadius + 10, 0);
    }

    /// <summary>
    /// 穴を掘る関数
    /// </summary>
    /// <param name="texture"> 掘るテクスチャ</param>
    /// <param name="position">掘る中心となる場所</param>
    /// <param name="direction">掘る方向</param>
    public void DigHole(Texture2D texture, Vector2 position, int radius, int direction)
    {
        // テクスチャ座標に変換
        Vector2Int pixelPos = WorldToTextureCoord(position);

        // 穴の半径と方向に基づいてテクスチャを操作する
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (!IsPixelInsideTexture(pixelPos, texture))
                {
                    continue;
                }
                // ピクセル座標を変換
                Vector2Int pixelOffset = new Vector2Int(x, y);
                Vector2Int pixel = pixelPos + pixelOffset;

                if (pixelOffset.magnitude <= radius)
                {
                    Debug.Log(x + ";" + y);
                    Color transparentColor = new Color(0, 0, 0, 0);
                    texture.SetPixel(pixel.x, pixel.y, transparentColor);
                }
            }
        }
    }

    /// <summary>
    /// ピクセルがテクスチャの中にあるか判定する
    /// </summary>
    /// <param name="pixel">対象となるピクセル値</param>
    /// <param name="texture">テクスチャ</param>
    /// <returns></returns>
    private bool IsPixelInsideTexture(Vector2Int pixel, Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        return pixel.x >= 0 && pixel.x < width && pixel.y >= 0 && pixel.y < height;
    }

    /// <summary>
    /// ワールド座標をテクスチャ座標に変換する
    /// </summary>
    /// <param name="worldPos">変換するワールド座標</param>
    /// <returns></returns>
    private Vector2Int WorldToTextureCoord(Vector2 worldPos)
    {
        Vector2 textureSize = new Vector2(middleGroundSpriteRenderer.sprite.texture.width, middleGroundSpriteRenderer.sprite.texture.height);
        float pixelPerUnit = middleGroundSpriteRenderer.sprite.pixelsPerUnit;

        // ワールド座標をスプライトの中心を原点とした相対座標に変換
        Vector2 localPos = worldPos - (Vector2)middleGroundSpriteRenderer.transform.position;
        int texturX = (int)(textureSize.x / 2 + localPos.x * pixelPerUnit);
        int textureY = (int)(textureSize.y / 2 + localPos.y * pixelPerUnit);

        Vector2Int pixelCoord = new Vector2Int(
            texturX, textureY
        );

        return pixelCoord;
    }

    private void ResetTexture()
    {
        Texture2D texture = (Texture2D)middleGroundSpriteRenderer.sprite.texture;
        texture.SetPixels(middleGroundInitialPixels);
        texture.Apply();
        texture = (Texture2D)frontGroundSpriteRenderer.sprite.texture;
        texture.SetPixels(frontGroundInitialPixels);
        texture.Apply();
    }

    // ゲームが終了したら
    private void OnApplicationQuit()
    {
        ResetTexture();
    }
    */
}