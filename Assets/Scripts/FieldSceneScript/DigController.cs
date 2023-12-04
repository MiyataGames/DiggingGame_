using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigController : MonoBehaviour
{

    private bool isDigging = false;
    
    [SerializeField] private Animator playerAnimController;
    [SerializeField] private GameObject digCollider; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDigging == false){
            if(Input.GetMouseButtonDown(0)){
                isDigging = true;
                digCollider.SetActive(true);
                playerAnimController.SetBool("isDigging",true);
            }
        }
    }

    public void endDiggingAnim(){
        isDigging = false; 
        digCollider.SetActive(false);
        playerAnimController.SetBool("isDigging",false);
    }
}
