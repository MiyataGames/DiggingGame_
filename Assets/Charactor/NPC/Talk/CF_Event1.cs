using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactor_Event1 : CharactorFunction
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject syu;

    public void SpawanSyo(){
        SpawnCharactor(syu, player.transform.position + new Vector3(0,3));
    }
}
