using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityDetail : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI info;
    [SerializeField] TextMeshProUGUI distance;
    [SerializeField] Image cityImage;
    int medicines;
    City _city;
    public void Initialize(City city, float distanceFromSelectedCity, int medicines)
    {
        _city = city;
        distance.text = $"{distanceFromSelectedCity:0.00} Km";
        if (_city.IsSaved)
        {
            info.text = "This city is save and is producing medicines!";
            cityImage.color = _city.Config.SavedColor;
        }
        else
        {
            info.text = "Help! Send medicines now!";
            cityImage.color = _city.Config.NormalColor;
        }

        this.medicines = medicines;
    }

    public void Select()
    {
        UIManager.Instance.SelectDestinationCityForNPC(_city, _city.TakeMedicine(medicines));
    }
}