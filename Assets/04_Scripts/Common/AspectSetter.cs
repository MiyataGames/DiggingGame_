using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectSetter : MonoBehaviour
{
    Camera _camera;   // 目標の解像度
    [SerializeField] bool isUpdate;                 // 毎フレーム画面をそろえるか

    private void Start()
    {
        if (Application.isPlaying)
        {
            _camera = this.GetComponent<Camera>();
            SetAspect();
        }
    }
    void SetAspect()
    {
        // 現在のアスペクト比
        float screenAspect = (float)Screen.width / (float)Screen.height;
        // 目標のアスペクト比
        float targetAspect = GameManager.instance.targetResolution.x / GameManager.instance.targetResolution.y;

        // 現在のアスペクト比と目標のアスペクト比の比率
        float rate = targetAspect / screenAspect;
        Debug.Log("比" + rate);
        Rect rect = new Rect(0, 0, 1, 1);

        // 倍率が小さい場合、横をそろえる
        if(rate < 1)
        {
            rect.width = rate;
            rect.x = 0.5f - rect.width * 0.5f;
            /*
            GameManager.instance.currentResolution.x = rate * Screen.width;
            GameManager.instance.currentResolution.y =
            */
        }// 盾をそろえる
        else
        {
            rect.height = 1 / rate;
            rect.y = 0.5f - rect.height * 0.5f;
        }
        Debug.Log("x" + rect.x+"y"+rect.y);


        _camera.rect = rect;

    }

    void OnApplicationQuit()
    {
        _camera.rect = new Rect(0, 0, 1, 1);  // 正しいビューポートにリセット
    }
}
