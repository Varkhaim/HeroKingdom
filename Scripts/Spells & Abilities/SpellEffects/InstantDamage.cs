using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamage : SpellEffect
{
    public HKUnit Caster;
    public HKUnit Target;

    public InstantDamage(SpellInfo spellInfo) : base(spellInfo)
    {
        Caster = spellInfo.Caster;
        Target = spellInfo.Target;
    }

    public override void Execute()
    {       
        Target.TakeDamage(Caster.GetBasicAttackDamage(), Caster);
    }
}
