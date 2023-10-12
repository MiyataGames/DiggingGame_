using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusUIsManager : MonoBehaviour
{
    private int playersCount = 0;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerStatusUIsManager;
    private List<PlayerFieldUI> playerFieldUIs;

    public void SetUpPlayerStatusUI(List<Player> players)
    {
        // �v���C���[�̐l�����ς�����Ƃ������X�V����
        if (players.Count != playersCount)
        {
            // ����m�F�܂�
            for (int i = 0; i < playersCount; i++)
            {
                Destroy(playerFieldUIs[i]);
            }
            playerFieldUIs = new List<PlayerFieldUI>();
            // �X�e�[�^�X��ʂ�Prefab��Instantiate����
            for (int i = 0; i < players.Count; i++)
            {
                PlayerFieldUI playerFieldUI = Instantiate(playerUIPrefab, playerStatusUIsManager.transform).GetComponent<PlayerFieldUI>();
                // �X�e�[�^�XUI�}�l�W���[�ɒǉ�����
                playerFieldUIs.Add(playerFieldUI);
                // �v���C���[��UI�ɒǉ�
                players[i].playerUI = playerFieldUI;
                // �X�e�[�^�X��ʂɏ���ݒ肷��
                playerFieldUI.SetPlayerStatus(players[i]);
            }
        }
        playersCount = players.Count;
    }

    public void selectStatus(int index)
    {
        // �t���[�����I�t
        for (int i = 0; i < playerFieldUIs.Count; i++)
        {
            playerFieldUIs[i].SetActivateSelectedFrame(false);
        }
        playerFieldUIs[index].SetActivateSelectedFrame(true);
    }
}