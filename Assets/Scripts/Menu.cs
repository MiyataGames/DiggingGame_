using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuCommand
{
    ITEM,
    STATUS,
    EQUIPMENT,
    SETTING,
    END
}

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject[] menuSelectArrow;

    // ���C���p�l�����I���I�t����֐�
    public void ActivateMenuPanel(bool activate)
    {
        menuPanel.SetActive(activate);
    }

    // �A�C�e���p�l�����I���I�t����
    public void ActivateItemPanel(bool activate)
    {
        itemPanel.SetActive(activate);
    }

    // �X�e�[�^�X�p�l�����I���I�t����
    public void ActivateStatusPanel(bool activate)
    {
        statusPanel.SetActive(activate);
    }

    // ���C���p�l���̖����I���I�t����
    public void ActivateMenuSelectArrow(MenuCommand menuCommand)
    {
        for (int i = 0; i < menuSelectArrow.Length; i++)
        {
            menuSelectArrow[i].SetActive(false);
        }
        menuSelectArrow[((int)menuCommand)].SetActive(true);
    }
}