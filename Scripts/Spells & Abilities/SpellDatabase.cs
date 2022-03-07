using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellDatabase", menuName = "Hero Kingdom/Database/Database", order = 1)]
public class SpellDatabase : ScriptableObject
{
    public List<Spell> spells;

    public SpellEffect GetSpellEffect(int spellID, SpellInfo spellInfo)
    {
        if (!spells.Exists(x => x.spellID == spellID))
        {
            Debug.LogErrorFormat("Spell [ID={0}] doesn't exist.",spellID);
            return null;
        }

        Spell spell = spells.Find(x => x.spellID == spellID);
        SpellEffectID spellEffectID = spell.spellEffectID;

        spellInfo.MissilePrefab = spell.MissilePrefab;
        spellInfo.ReachEffect = SpellEffect.GetSpellEffect(spell.ReachEffectID, spellInfo);

        SpellEffect spellEffect = SpellEffect.GetSpellEffect(spellEffectID, spellInfo);
        return spellEffect;
    }
}
