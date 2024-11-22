using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SontyoHouseExitPoint : MonoBehaviour
{
    [SerializeField] FadeController fadeController;
    [SerializeField] GameObject sontyoHouse;
    [SerializeField] GameObject event1_1map;
    [SerializeField] GameObject player_Story;
    [SerializeField] Transform playerPos;
    // Start is called before the first frame update

    private void OnCollisionEnter2D(Collision2D collision)
    {
        fadeController.StartFadeOutIn();
        sontyoHouse.SetActive(false);
        event1_1map.SetActive(true);
        player_Story.transform.position = playerPos.position;
    }

}
