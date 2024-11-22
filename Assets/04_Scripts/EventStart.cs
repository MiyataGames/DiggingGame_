using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStart : MonoBehaviour
{
    [SerializeField] private string storyPrefabPath;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other){

        if(other.gameObject.tag == "FieldPlayer")
        {
            Debug.Log("startEvent");
            GameManager.instance.StartEvent(storyPrefabPath);
            Destroy(this);
        }
        
    }
}
