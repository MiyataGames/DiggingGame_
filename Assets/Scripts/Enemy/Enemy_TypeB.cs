using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Enemy_TypeB : FieldEnemy
{

#if UNITY_EDITOR
    [CustomEditor(typeof(FieldEnemy))]
#endif

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void FieldMove()
    {
        base.FieldMove();
        Debug.Log(a);
    }

    protected override void Search()
    {
        base.Search();
    }

}
