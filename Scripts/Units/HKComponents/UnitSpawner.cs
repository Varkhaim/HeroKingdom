using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Spawner")]
    public Transform UnitSpawnPoint;
    public GameObject SpawnedUnit;
    public float SpawnCooldown = 10f;
    public int UnitLimit = 4;

    // PRIVATE
    private float spawnTimer = 0.0f;
    private List<HKUnit> OwnedUnits = new List<HKUnit>();
    private HKEntity entity;

    private void Awake()
    {
        entity = GetComponent<HKEntity>();
        if (!entity)
            Destroy(this);
    }

    private void Update()
    {
        HandleSpawn();
    }

    private void HandleSpawn()
    {
        if (entity.IsDead) return;

        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            SpawnUnit();
            spawnTimer = SpawnCooldown;
        }
    }

    private void SpawnUnit()
    {
        if (OwnedUnits.Count + 1 > UnitLimit) return;
        GameObject newUnit = Instantiate(SpawnedUnit, UnitSpawnPoint.position, Quaternion.identity);
        HKUnit hkUnit = newUnit.GetComponent<HKUnit>();
        OwnedUnits.Add(hkUnit);
        hkUnit.SetSpawnSickness();
    }
}
