//using UnityEngine;
//using TMPro;

//public class CityUI : MonoBehaviour
//{
//    [Header("Source")]
//    public City city;
//    [Header("Binding")]
//    public TMP_Text medicineCount;
//    [Header("Visual")]
//    public Color savedColor;
//    public Color riskColor;

//    void Start()
//    {
        
//    }

//    void Update()
//    {
//        if (!city.isDestroyed)
//        {
//            medicineCount.gameObject.SetActive(true);

//            if (city.isSaved)
//            {
//                medicineCount.color = savedColor;
//            }
//            else
//            {
//                medicineCount.color = riskColor;
//            }

//        }
//        else
//        {
//            medicineCount.gameObject.SetActive(false);
//        }
        
//    }
//}
