using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorFunction : MonoBehaviour
{
    public virtual void ExecuteCommand(string functionName,string animFuncName){

    }

    protected void CharactorChangeVec(GameObject charactor, string vec){

        Animator anim = charactor.GetComponent<Animator>();

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

    protected GameObject SpawnCharactor(GameObject gameObject, Vector3 Position, Transform parent){
        GameObject charactor;
        charactor = Instantiate(gameObject,Position,Quaternion.identity,parent);
        return charactor;
    }

    protected void WarpCharactor(GameObject gameObject, Vector3 position){
        gameObject.transform.position = position;
    }

    protected void SideView2TopDown(){
        GameManager.instance.CurrentSceneIndex = (int)GameMode.TOWN_SCENE;
    }

    
}
