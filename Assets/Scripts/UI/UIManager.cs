using DesignPatterns.Generics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Player/NPC Dialogue")]
    [SerializeField] GameObject playerDialogueContainer;
    [SerializeField] TMP_Text playerText;
    [SerializeField] Image NPCImage;
    [SerializeField] GameObject NPCDialogueContainer;
    [SerializeField] TMP_Text NPCText;

    [Header("Total Cities")]
    public GameObject[] totalCities;

    [Header("Current City")]
    [SerializeField] GameObject currentCityOptionsPanel;
    [SerializeField] Slider currentCityHealthBar;
    [SerializeField] Slider currentCityMedicinesBar;
    [SerializeField] TMP_Text currentCityHealthText;
    [SerializeField] TMP_Text currentCityMedicinesText;
    [SerializeField] TMP_Text currentCityDialogueText;

    [Header("PopUp")]
    [SerializeField] GameObject popUpPanel;
    [SerializeField] TMP_Text popUpText;

    [Header("Dialogue Vars")]
    [SerializeField] int minDialogueLetters = 10;
    [SerializeField] int maxDialogueLetters = 15;

    public override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        playerText.text = RandomDialogueLetters();
        NPCText.text = RandomDialogueLetters();
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

        playerText.text = RandomDialogueLetters();
        NPCText.text = RandomDialogueLetters();
    }
    public void HideCityPanelOptions()
    {
        playerDialogueContainer.SetActive(false);
        NPCDialogueContainer.SetActive(false);
        NPCImage.gameObject.SetActive(false);

        currentCityOptionsPanel.SetActive(false);
    }
    public string RandomDialogueLetters()
    {
        int maxChars = Random.Range(minDialogueLetters, maxDialogueLetters + 1);

        // Se maxChars è minore di 1, restituiamo stringa vuota
        if (maxChars < 1)
            return "";

        // Definisce i caratteri possibili (puoi aggiungere numeri o simboli se vuoi)
        const string letters = "abcdefghijklmnopqrstuvwxyz";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int currentLength = 0;

        while (currentLength < maxChars)
        {
            // Decidi casualmente la lunghezza della parola (da 1 a 8 lettere, ma non oltre maxChars)
            int wordLength = Random.Range(1, Mathf.Min(8, maxChars - currentLength) + 1);

            // Aggiungi lettere
            for (int i = 0; i < wordLength && currentLength < maxChars; i++)
            {
                char randomLetter = letters[Random.Range(0, letters.Length)];
                sb.Append(randomLetter);
                currentLength++;
            }

            // Aggiungi spazio casualmente, solo se non stai per superare maxChars
            if (currentLength < maxChars && Random.value > 0.3f) // 70% di probabilità di aggiungere spazio
            {
                sb.Append(' ');
                currentLength++;
            }
        }

        // Rimuove eventuale spazio finale
        return sb.ToString().TrimEnd();
    }
}
