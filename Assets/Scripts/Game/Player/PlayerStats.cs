using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] float m_baseRange;
    [SerializeField] float m_baseCritRate;
    [SerializeField] float m_baseCritMultiplier;

    [SerializeField] int m_baseDamage;
    [SerializeField] float m_baseFireRate;
    [SerializeField] int m_baseNbBullets;
    [SerializeField] int m_basePenetration;

    [SerializeField] float m_baseExlosionRange;
    [SerializeField] int m_baseExlosionDamage;

    //Run Values
    int m_damage;
    float m_fireRate;
    int m_nbBullets;
    int m_nbSubBullets;
    int m_penetration;
    bool m_splitOnHit;

    float m_explosionChance;
    float m_explosionRange;
    float m_explosionDamage;

    bool m_ToggleFireDamage;
    bool m_ToggleLightningDamage;

    float m_bouncingChance;

    float m_knockback;

    int m_level;


    [Header("Public Values (Get Only)")]
    public float GetRange { get => m_baseRange; set => m_baseRange = value; }
    public float GetCritRate { get => m_baseCritRate; set => m_baseCritRate = value; }
    public float GetCritMultiplier { get => m_baseCritMultiplier; set => m_baseCritMultiplier = value; }
    public int GetDamage { get => m_damage; }
    public float GetFireRate { get => m_fireRate; }
    public int GetNbBullets { get => m_nbBullets; }
    public int GetNbSubBullets { get => m_nbSubBullets; }
    public int GetPenetration { get => m_penetration; }
    public bool GetSplitOnHit { get => m_splitOnHit; }
    public float GetExplosionChance { get => m_explosionChance; }
    public float GetExplosionRange { get => m_explosionRange; }
    public float GetExplosionDamage { get => m_explosionDamage; }
    public bool GetToggleFireDamage { get => m_ToggleFireDamage; }
    public bool GetToggleLightningDamage { get => m_ToggleLightningDamage; }
    public float GetBouncingChance { get => m_bouncingChance; }
    public float GetKnockback { get => m_knockback; }
    public int GetLevel { get => m_level; }
    

    private void Start()
    {
        m_damage = m_baseDamage;
        m_fireRate = m_baseFireRate;
        m_nbBullets = m_baseNbBullets;
        m_penetration = m_basePenetration;

        m_explosionRange = m_baseExlosionRange;
        m_explosionDamage = m_baseExlosionDamage;

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

            case "SplittingBullets":
                SplittingBullet();
                break;

            case "ExplosionExpansion":
                ExplosionExpansion();
                break;

            case "ExplosionDamage":
                ExplosionDamage();
                break;

            case "FlameBullets":
                FlameBullets();
                break;

            case "ElectricBullets":
                ElectricBullets();
                break;

            case "BouncingBullets":
                BouncingBullets();
                break;

            case "ExplosionBoost":
                ExplosionBoost();
                break;

            case "Knockback":
                Knockback();
                break;

            case "ImpactBullets":
                ImpactBullets();
                break;
        }
    }

    //Upgrades Methods
    private void BonusDamage()
    {
        m_damage = (int)(m_damage * 1.5);
    }
    private void MoreBullets()
    {
        m_nbBullets += 1;
    }
    private void Piercing()
    {
        m_penetration += 1;
    }

    private void BulletExplosion()
    {
        m_explosionChance += 0.1f;
    }

    private void RapidFire()
    {
        m_fireRate *= 1.25f;
    }

    private void MadFire()
    {
        m_nbSubBullets += 2;
        m_level++;
    }

    private void SplittingBullet()
    {
        m_splitOnHit = true;
        m_level++;
    }

    private void ExplosionExpansion()
    {
        m_explosionRange *= 1.5f;
    }

    private void ExplosionDamage()
    {
        m_explosionDamage += 0.5f;
    }

    private void FlameBullets()
    {
        m_ToggleFireDamage = true;
    }

    private void ElectricBullets()
    {
        m_ToggleLightningDamage = true;
    }

    private void BouncingBullets()
    {
        m_bouncingChance += 0.2f;
        m_level++;
    }

    private void ExplosionBoost()
    {
        m_explosionChance += 0.2f;
        m_explosionDamage += 0.5f;
    }
    private void Knockback()
    {
        m_knockback += 1f;
    }

    private void ImpactBullets()
    {
        m_damage = (int)(m_damage * 1.5);
        m_knockback += 1f;
        m_level++;
    }
}