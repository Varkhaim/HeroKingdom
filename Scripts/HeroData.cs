using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroData
{
    public string HeroName;
    public UnitCharacter unitCharacter;
    public UnitNature unitNature;

    internal string GetTraits()
    {
        return string.Format("{0} - {1}", Capitalize(unitCharacter.ToString()), Capitalize(unitNature.ToString()));
    }

    private string Capitalize(string word)
    {
        return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
    }

    public static HeroData Randomize(List<string> NamesPool)
    {
        HeroData newHeroData = new HeroData();

        int charactersAmount = Enum.GetNames(typeof(UnitCharacter)).Length;
        newHeroData.unitCharacter = (UnitCharacter)UnityEngine.Random.Range(0,charactersAmount);

        int naturesAmount = Enum.GetNames(typeof(UnitNature)).Length;
        newHeroData.unitNature = (UnitNature)UnityEngine.Random.Range(0, naturesAmount);

        newHeroData.HeroName = NamesPool[UnityEngine.Random.Range(0, NamesPool.Count)];

        return newHeroData;
    }
}
