using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEventManager : MonoBehaviour
{
    //イベント1のすべてのフラグ
    public List<int> story1Flag = new List<int>() {0};

    void Start() {
        Debug.Log(story1Flag[0]);
    }

}
