using DesignPatterns.Generics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Player/NPC Dialogue")]
    public GameObject playerDialogueContainer;
    public TMP_Text playerText;
    public Image NPCImage;
    public GameObject NPCDialogueContainer;
    public TMP_Text NPCText;

    [Header("Total Cities")]
    public GameObject[] totalCities;

    [Header("Current City")]
    public GameObject currentCityOptionsPanel;
    public Slider currentCityHealthBar;
    public Slider currentCityMedicinesBar;
    public TMP_Text currentCityHealthText;
    public TMP_Text currentCityMedicinesText;
    public TMP_Text currentCityDialogueText;

    [Header("PopUp")]
    public GameObject popUpPanel;
    public TMP_Text popUpText;

    public override void Awake()
    {
        base.Awake();
    }

    public void ShowPopup(string _dialogue)
    {
        popUpPanel.SetActive(true);
        popUpText.text = _dialogue;
    }
    public void HidePopoup()
    {
        popUpPanel.SetActive(false);
    }

    public void ShowCityPanelOptions()
    {
        playerDialogueContainer.SetActive(true);
        NPCDialogueContainer.SetActive(true);
        NPCImage.gameObject.SetActive(true);

        currentCityOptionsPanel.SetActive(true);
    }
    public void HideCityPanelOptions()
    {
        playerDialogueContainer.SetActive(false);
        NPCDialogueContainer.SetActive(false);
        NPCImage.gameObject.SetActive(false);

        currentCityOptionsPanel.SetActive(false);
    }
}
