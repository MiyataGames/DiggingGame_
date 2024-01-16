using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FieldEnemy : MonoBehaviour
{

    public int a;
    protected int b;
    bool isBlinking;

    public bool IsBlinking { get => isBlinking; set => isBlinking = value; }

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

    public IEnumerator Blinking()
    {
        IsBlinking = true;
        GameObject enemySymbol = this.gameObject;
        enemySymbol.GetComponent<CapsuleCollider2D>().enabled = false;
        enemySymbol.GetComponent<Rigidbody2D>().simulated = false;
        SpriteRenderer enemySymbolSprite = enemySymbol.GetComponent<SpriteRenderer>();
        for (int i = 0;i < 20;i++)
        {
            if(enemySymbolSprite.enabled == true)
            {
                enemySymbolSprite.enabled = false;
            }
            else
            {
                enemySymbolSprite.enabled = true;

            }
            yield return new WaitForSeconds(0.1f);
        }
        enemySymbol.SetActive(true);
        enemySymbol.GetComponent<CapsuleCollider2D>().enabled = true;
        enemySymbol.GetComponent<Rigidbody2D>().simulated = true;

        IsBlinking = false;
    }
}
