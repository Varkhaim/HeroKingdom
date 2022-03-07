using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : SpellEffect
{
    public HKUnit Caster;
    public HKUnit Target;
    public GameObject MissilePrefab;
    public SpellEffect ReachEffect;
    public Vector3 MissileSpawnPoint;

    public SpawnProjectile(SpellInfo spellInfo) : base(spellInfo)
    {
        Caster = spellInfo.Caster;
        Target = spellInfo.Target;
        MissilePrefab = spellInfo.MissilePrefab;
        ReachEffect = spellInfo.ReachEffect;
        MissileSpawnPoint = spellInfo.MissileSpawnPoint;
    }

    public override void Execute()
    {
        GameObject projectileGO = GameObject.Instantiate(MissilePrefab, MissileSpawnPoint, Quaternion.identity, GameCore.GetMissileParent());
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        projectile.Setup(Target.transform, MissileSpawnPoint, ReachEffect);
    }
}
