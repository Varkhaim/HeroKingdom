using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform Target;
    private Vector3 StartPoint;

    bool setupDone = false;
    SpellEffect ReachEffect;

    public void Setup(Transform target, Vector3 startPoint, SpellEffect reachEffect)
    {
        Target = target;
        StartPoint = startPoint;
        ReachEffect = reachEffect;
        setupDone = true;
    }

    private void Update()
    {
        if (!setupDone) return;

        transform.position = Vector3.MoveTowards(transform.position, Target.position, 0.1f);
        transform.LookAt(Target.position);
        if (Vector3.Distance(transform.position, Target.position) < 1f)
        {
            ReachTarget();
        }
    }

    private void ReachTarget()
    {
        ReachEffect.Execute();
        Destroy(gameObject);
    }
}
