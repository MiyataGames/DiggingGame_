using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FieldEnemy : MonoBehaviour
{

    public int a;
    protected int b;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate(){
        FieldMove();
    }

    public virtual void FieldMove(){

    }

    protected virtual void Search(){

    }
}
