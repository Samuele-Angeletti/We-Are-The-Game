using DesignPatterns.Generics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ISubscriber
{
    public static UIManager Instance;

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
    [SerializeField] Button acceptEventButton;
    [SerializeField] Button refuseEventButton;

    [Header("Dialogue Vars")]
    [SerializeField] int minDialogueLetters = 10;
    [SerializeField] int maxDialogueLetters = 15;

    //tmp vars
    private GameEventsStats statsToAddPlayer;

    public void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        Publisher.Subscribe(this, typeof(OpenEventUIMessage));
    }
    private void Start()
    {
        playerText.text = RandomDialogueLetters();
        NPCText.text = RandomDialogueLetters();

        refuseEventButton.onClick.AddListener(HidePopoup);
    }
    public void ShowPopup(string _dialogue)
    {
        popUpPanel.SetActive(true);
        popUpText.text = _dialogue;
    }
    public void HidePopoup()
    {
        popUpPanel.SetActive(false);
        Publisher.Publish(new OnOffPlayerMovement(false));
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

        if (maxChars < 1)
            return "";

        const string letters = "1234567890abcdefghijklmnopqrstuvwxyz";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int currentLength = 0;

        while (currentLength < maxChars)
        {
            int wordLength = Random.Range(1, Mathf.Min(8, maxChars - currentLength) + 1);

            for (int i = 0; i < wordLength && currentLength < maxChars; i++)
            {
                char randomLetter = letters[Random.Range(0, letters.Length)];
                sb.Append(randomLetter);
                currentLength++;
            }

            if (currentLength < maxChars && Random.value > 0.3f)
            {
                sb.Append(' ');
                currentLength++;
            }
        }
        return sb.ToString().TrimEnd();
    }
    public void AddStatsToPlayer()
    {
        Publisher.Publish(new AddStatsPlayerMessage(statsToAddPlayer));
        HidePopoup();
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is OpenEventUIMessage eventMessage)
        {
            //mostro il popup con cose dell'evento
            string popupToShow = "";
            switch (eventMessage.GameEvent.GameEventEffect)
            {
                case EGameEventEffect.SpawnResources:
                    popupToShow = "incappi in un imprevisto, ti trovi davanti a delle risorse, cosa fai?";
                    break;
                case EGameEventEffect.HelpCivil:
                    popupToShow = "incappi in un imprevisto, ti trovi davanti a dei civili, cosa fai?";
                    break;
            }
            popupToShow += $"\n in caso di successo: {eventMessage.GameEvent.StatsOnSuccess.Stamina}, {eventMessage.GameEvent.StatsOnSuccess.Medicines}" +
                $"\n in caso di fallimento: {eventMessage.GameEvent.StatsOnFailure.Stamina}, {eventMessage.GameEvent.StatsOnFailure.Medicines}" +
                $"\n perc di successo:  {eventMessage.GameEvent.ChanceSuccessPercentage}" +
                $"\n perc di fallimento:  {eventMessage.GameEvent.ChanceFailurePercentage}";

            statsToAddPlayer = eventMessage.GameEvent.ExecuteEventAndGetStats();

            acceptEventButton.onClick.AddListener(AddStatsToPlayer);
            ShowPopup(popupToShow);

            Publisher.Publish(new OnOffPlayerMovement(true));//fa stare fermo il player
        }
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(OpenEventUIMessage));
    }
    private void OnDestroy()
    {
        OnDisableSubscriber();
    }
}
