using DesignPatterns.Generics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityManager : Singleton<CityManager>
{
    List<City> cityList;

    public override void Awake()
    {
        base.Awake();
        cityList = FindObjectsByType<City>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    public List<City> CityList => cityList;
}
