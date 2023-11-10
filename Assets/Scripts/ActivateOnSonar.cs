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
        if (other.CompareTag("Respawn"))
        {
            FlashHlight = true;
        }
        if (other.CompareTag("Finish") && FlashHlight == false)
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
        if (other.CompareTag("Respawn"))
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
        if (other.CompareTag("Respawn"))
        {
            FlashHlight = false;
        }
        if (other.CompareTag("Finish"))
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
    float duration = 2f;  // 2秒間の点滅
    float elapsed = 0f;

    while (elapsed < duration)
    {
        float intensity = Mathf.PingPong(elapsed * 5f, 1f);
        light2DComponent.intensity = intensity;

        elapsed += Time.deltaTime;
        yield return null;
    }

    light2DComponent.intensity = 0f;  // 点滅が終了したらライトをオフにする
    lightBlinkCoroutine = null;  // 点滅が終了したら変数をリセット
}

}
