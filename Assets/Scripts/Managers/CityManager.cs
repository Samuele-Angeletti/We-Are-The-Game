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

    public static List<City> CitiesHelpedBy(City city)
    {
        if (city.NpcCivilControllers == null || city.NpcCivilControllers.Count < 1) return null;
        return city.NpcCivilControllers.Select(npc => npc.DestinationCity).ToList();
    }

    public static bool IsCityHelpedBy(City targetCity,City sendingCity)
    {
        var helpedCities = CitiesHelpedBy(sendingCity);
        if (helpedCities == null) return false;

        return helpedCities.Contains(targetCity);
    }

    public static NpcCivilController GetNpcHelpingCity(City targetCity, City sendingCity)
    {
        if (sendingCity.NpcCivilControllers == null || sendingCity.NpcCivilControllers.Count < 1) return null;
        return sendingCity.NpcCivilControllers.Find(npc => npc.DestinationCity == targetCity);
    }
}
