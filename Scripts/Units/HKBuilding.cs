using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class HKBuilding : HKEntity
{
    public NavMeshObstacle obstacle;
    public List<HKUnit> Visitors = new List<HKUnit>();

    protected override void InitEntity()
    {
        if (!obstacle)
            obstacle = GetComponent<NavMeshObstacle>();
        RefillHealth();
    }

    public override float GetHitboxRadius()
    {
        if (obstacle)
            return obstacle.radius;
        return 0f;
    }

    protected override void RecalculateMaxHealth()
    {
        MaxHealth = Mathf.Floor(BaseMaxHealth + (HealthPerLevel * Level));
    }

    protected override void UpdateAlive()
    {
        CheckTargeting();
    }

    private void CheckTargeting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID()) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                selectManager.SetupPanel(this);
            }
        }
    }

    protected override void HandleDeath()
    {
        
    }

    public void AddGuest(HKUnit guest)
    {
        Visitors.Add(guest);
    }
}
