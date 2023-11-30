using UnityEngine;
using System.Collections;


public class ActivateOnSonar : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;  // ここで spriteRenderer を宣言
    private UnityEngine.Rendering.Universal.Light2D light2DComponent;
    private Coroutine lightBlinkCoroutine;
    private bool FlashHlight = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer コンポーネントが見つかりません。");
        }

        Transform lightChild = transform.Find("Light");
        if (lightChild != null)
        {
            light2DComponent = lightChild.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            if (light2DComponent == null)
            {
                Debug.LogError("Light 2D コンポーネントが見つかりません。");
            }
        }
        else
        {
            Debug.LogError("Lightという名前の子オブジェクトが見つかりません。");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("flashlight"))
        {
            FlashHlight = true;
        }
        if (other.CompareTag("sonar") && FlashHlight == false)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false; // SpriteRenderer を非アクティベートにする
            }
            if (light2DComponent != null && lightBlinkCoroutine == null)
            {
                lightBlinkCoroutine = StartCoroutine(BlinkingLight());
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("flashlight"))
        {
            FlashHlight = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true; // SpriteRenderer を非アクティベートにする
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("flashlight"))
        {
            FlashHlight = false;
        }
        if (other.CompareTag("sonar"))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true; // SpriteRenderer を非アクティベートにする
            }
            if (light2DComponent != null && lightBlinkCoroutine != null)
            {
                StopCoroutine(lightBlinkCoroutine);
                lightBlinkCoroutine = null;
                light2DComponent.intensity = 0f;
            }
        }
    }
private IEnumerator BlinkingLight()
{
    while (true) // 無限ループを作成して点滅を繰り返す
    {
        float duration = 0.4f;  // 点滅時間を短く設定（例：0.5秒）
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Mathf.PingPongを使って点滅させる
            float intensity = Mathf.PingPong(elapsed, 3f); // 明るさを0から3まで変化
            light2DComponent.intensity = 3;

            elapsed += Time.deltaTime;
            yield return null;
        }

        light2DComponent.intensity = 0f;  // 点滅をオフにする
        yield return new WaitForSeconds(0.1f);  // 待機時間も短く設定（例：0.1秒）
    }
}
}

