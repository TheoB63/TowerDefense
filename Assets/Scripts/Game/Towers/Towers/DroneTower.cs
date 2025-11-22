using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneTower : Tower
{
    [Header("Spawner Settings")]
    [SerializeField] int m_baseMaxDrone;
    [SerializeField] float m_baseReloadCooldown;

    //Privates Current values in run time
    int m_maxDrone;
    float m_reloadCooldown;

    int m_nbDroneSpawn; //nb spawn before reload

    //Timer
    float m_timerSpawn;
    float m_timerReload;

    private void Start()
    {
        InitializeTower();
    }

    protected override void InitializeTower()
    {
        base.InitializeTower();
        m_maxDrone = m_baseMaxDrone;
        m_reloadCooldown = m_baseReloadCooldown;
    }

    private void Update()
    {
        MoveLauncher();

        if (m_timerReload <= 0)
        {
            m_timerSpawn -= Time.deltaTime;

            if (m_timerSpawn <= 0)
            {
                Spawn();
                m_nbDroneSpawn++;
                if (m_nbDroneSpawn >= m_maxDrone)
                {
                    m_timerReload = m_reloadCooldown;
                }
            }
        }
        else
        {
            m_timerReload -= Time.deltaTime;
        }

    }

    protected override void MoveLauncher()
    {
        m_launcher.Rotate(0, 0, m_armRotationSpeed * Time.deltaTime);
    }

    void Spawn()
    {
        GameObject go = Instantiate(m_projectilesPrefab, m_barrel.position, Quaternion.identity);
        go.GetComponent<Drone>().SetStats(m_damage, m_nbAttack);
        Debug.Log("spawn drone");
    }

    public override void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseMaxDrones":
                IncreaseMaxDrones();
                break;

            case "ReduceSpawnTime":
                break;

            case "ImproveDroneHealth":
                break;
        }
    }

    private void IncreaseMaxDrones()
    {
        m_maxDrone += 2;
    }
}