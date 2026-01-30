using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] int m_baseDamage;
    [SerializeField] float m_baseRange;
    [SerializeField] float m_baseAttackSpeed;
    [SerializeField] int m_baseNbProjectiles;

    [SerializeField] int m_basePenetration;
    [SerializeField] float m_baseCritRate;
    [SerializeField] float m_baseCritMultiplier;

    [Header("Upgrades")]
    [SerializeField] float m_damageMultiplier;
    [SerializeField] int m_rangeIncrease;
    [SerializeField] float m_attackSpeedMultiplier;
    [SerializeField] int m_nbProjectilesIncrease;

    [SerializeField] int m_penetrationIncrease;
    [SerializeField] float m_critRateIncrease;
    [SerializeField] float m_CritMultiplierIncrease;

    //Privates Current values in run time
    int m_currentDamage;
    float m_currentRange;
    float m_currentAttackSpeed;
    int m_currentNbProjectiles;
    int m_currentPenetration;
    float m_currentCritRate;
    float m_currentCritMultiplier;
    int m_level;

    [Header("Public Values (Get Only)")]
    public int Damage { get => m_currentDamage; set => m_currentDamage = value; }
    public float Range { get => m_currentRange; set => m_currentRange = value; }
    public float AttackSpeed { get => m_currentAttackSpeed; set => m_currentAttackSpeed = value; }
    public int NbProjectiles { get => m_currentNbProjectiles; set => m_currentNbProjectiles = value; }
    public int Penetration { get => m_currentPenetration; set => m_currentPenetration = value; }
    public float CritRate { get => m_currentCritRate; set => m_currentCritRate = value; }
    public float CritMultiplier { get => m_currentCritMultiplier; set => m_currentCritMultiplier = value; }
    public int Level { get => m_level; set => m_level = value; }

    private void Start()
    {
        m_currentDamage = m_baseDamage;
        m_currentRange = m_baseRange;
        m_currentAttackSpeed = m_baseAttackSpeed;
        m_currentNbProjectiles = m_baseNbProjectiles;

        m_currentPenetration = m_basePenetration;
        m_currentCritRate = m_baseCritRate;
        m_currentCritMultiplier = m_baseCritMultiplier;

        m_level = 1;
    }

    public void ApplyUpgrade(string _upgradeName)
    {
        switch (_upgradeName)
        {
            case "BonusDamage":
                BonusDamage();
                break;

            case "MoreBullets":
                MoreBullets();
                break;

            case "Piercing":
                Piercing();
                break;

            case "BulletExplosion":
                BulletExplosion();
                break;

            case "RapidFire":
                RapidFire();
                break;

            case "MadFire":
                MadFire();
                break;

            case "SplittingBullet":
                SplittingBullet();
                break;
        }
    }

    //Upgrades Methods
    private void BonusDamage()
    {
        m_currentDamage = (int)(m_currentDamage * m_damageMultiplier);
    }
    private void MoreBullets()
    {
        m_currentNbProjectiles += m_nbProjectilesIncrease;
    }
    private void Piercing()
    {
        m_currentPenetration += m_penetrationIncrease;
    }

    private void BulletExplosion()
    {
        
    }

    private void RapidFire()
    {
        m_currentAttackSpeed *= m_attackSpeedMultiplier;
    }

    private void MadFire()
    {
        m_level++;
    }

    private void SplittingBullet()
    {
        m_level++;
    }
}