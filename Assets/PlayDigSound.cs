using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDigSound : MonoBehaviour
{
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioClip diggingSE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision2D){
       if(this.gameObject.GetComponentInParent<PlayerController>().IsDigging == true){
        if(seAudioSource.isPlaying == false){
            seAudioSource.PlayOneShot(diggingSE);
        }
        
       }
    }
}
