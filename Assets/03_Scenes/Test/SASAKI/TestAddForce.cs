using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddForce : MonoBehaviour
{
    [SerializeField] private float xpower = 9;
    [SerializeField] private float ypower = 9;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space")){
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

            rb.AddForce(new Vector2(xpower, ypower), ForceMode2D.Impulse);
        }
    }
}
