using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SpellEffect
{
    public SpellInfo spellInfo;
    public SpellEffectID spellEffectID;

    public SpellEffect(SpellInfo spellInfo)
    {
        this.spellInfo = spellInfo;
    }

    public abstract void Execute();

    public static SpellEffect GetSpellEffect(SpellEffectID spellEffectID, SpellInfo spellInfo)
    {
        switch (spellEffectID)
        {
            case SpellEffectID.INSTANT_DAMAGE: return new InstantDamage(spellInfo);
            case SpellEffectID.SPAWN_PROJECTILE: return new SpawnProjectile(spellInfo);
            default: return null;
        }
    }
}
public enum SpellEffectID
{
    INSTANT_DAMAGE,
    SPAWN_PROJECTILE
}