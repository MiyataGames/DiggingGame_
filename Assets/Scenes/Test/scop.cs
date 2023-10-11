using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scop : MonoBehaviour
{
    public float rotationSpeed = 45.0f;  // 回転速度 (度/秒)

    private bool isRotating = false;

    void Update()
    {
        if (isRotating)
        {
            // スペースキーが押されたら回転を停止
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isRotating = false;
            }
            else
            {
                // 親オブジェクトの位置を中心に回転
                transform.RotateAround(transform.parent.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            // スペースキーが押されたら回転を開始
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isRotating = true;
            }
        }
    }
}
