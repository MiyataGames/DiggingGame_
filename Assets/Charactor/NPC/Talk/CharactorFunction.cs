using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorFunction : MonoBehaviour
{

    
    protected void CharactorChangeVec(GameObject charactor, string vec){

        Animator anim = GetComponent<Animator>();

        switch(vec){
            case "Up":
                anim.SetFloat("x",0);
                anim.SetFloat("y",1);
            break;
            case "down":
                anim.SetFloat("x",0);
                anim.SetFloat("y",-1);
            break;
            case "Right":
                anim.SetFloat("x",1);
                anim.SetFloat("y",0);
            break;
            case "Left":
                anim.SetFloat("x",-1);
                anim.SetFloat("y",0);
            break;
        }
    }

    protected void SpawnCharactor(GameObject gameObject, Vector3 Position){
        Instantiate(gameObject,Position,Quaternion.identity);
    }

    protected void WarpCharactor(GameObject gameObject, Vector3 position){
        gameObject.transform.position = position;
    }

    
}
