using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class AspectSetter : MonoBehaviour
{
    Camera _camera;
    [SerializeField] Vector2 targetResolution;      // 目標の解像度
    [SerializeField] bool isUpdate;                 // 毎フレーム画面をそろえるか

    private void Awake()
    {
        _camera = this.GetComponent<Camera>();
        SetAspect();
    }
    void SetAspect()
    {
        // 現在のアスペクト比
        float screenAspect = (float)Screen.width / (float)Screen.height;
        // 目標のアスペクト比
        float targetAspect = targetResolution.x / targetResolution.y;

        // 現在のアスペクト比と目標のアスペクト比の比率
        float rate = targetAspect / screenAspect;
        Rect rect = new Rect(0, 0, 1, 1);

        // 倍率が小さい場合、横をそろえる
        if(rate < 1)
        {
            rect.width = rate;
            rect.x = 0.5f - rect.width * 0.5f;
        }// 盾をそろえる
        else
        {
            rect.height = 1 / rate;
            rect.y = 0.5f - rect.height * 0.5f;
        }

        _camera.rect = rect;

    }
}
