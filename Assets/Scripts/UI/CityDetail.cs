using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityDetail : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI info;
    [SerializeField] TextMeshProUGUI distance;
    [SerializeField] Button selectButton;
    [SerializeField] Image cityImage;
    int medicines;
    City _city;

    bool isReceivingHelp = false;

    public void Initialize(City city, float distanceFromSelectedCity, int medicines)
    {
        _city = city;
        distance.text = $"{distanceFromSelectedCity:0.00} Km";

        City currentCity = UIManager.Instance.currentCity;

        if (CityManager.IsCityHelpedBy(city, currentCity))
        {
            info.text = "This city is already being helped! Click to stop";
            if (_city.IsDestroyed)
            {
                cityImage.color = _city.Config.DestroyedColor;
            }
            else
            {
                cityImage.color = (_city.IsSaved) ? _city.Config.SavedColor : _city.Config.NormalColor;
            }
            isReceivingHelp = true;
        }
        else
        if (!currentCity.CanSendHelp())
        {
            info.text = "Your current city doesn't have available civils to help!";
            if (_city.IsDestroyed)
            {
                cityImage.color = _city.Config.DestroyedColor;
            }
            else
            {
                cityImage.color = (_city.IsSaved) ? _city.Config.SavedColor : _city.Config.NormalColor;
            }
            selectButton.interactable = false;
        }
        else
        if (_city.IsSaved)
        {
            info.text = "This city is save and is producing medicines!";
            cityImage.color = _city.Config.SavedColor;
            isReceivingHelp = false;
        }
        else 
        if (_city.IsDestroyed)
        {
            info.text = "This city is destroyed, helping won't be useful!";
            cityImage.color = _city.Config.DestroyedColor;
            isReceivingHelp = false;
        }
        else
        {
            info.text = "Help! Send medicines now!";
            cityImage.color = _city.Config.NormalColor;
            isReceivingHelp = false;
        }

        this.medicines = medicines;
    }

    public void Select()
    {
        if (isReceivingHelp)
        {
            // stop helping
            UIManager.Instance.StopDestinationCityForNPC(CityManager.GetNpcHelpingCity(_city, UIManager.Instance.currentCity));
            return;
        }

        UIManager.Instance.SelectDestinationCityForNPC(_city, medicines);
    }
}