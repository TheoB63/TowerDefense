using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAction
{
    void OnSpawn(Enemy enemy);//création
    void OnDeath(Enemy enemy);//destruction
    void OnUpdate(Enemy enemy);//every frame
    void OnHit(Enemy enemy, int damage);//when touch
    void OnEnterTrigger(Enemy enemy, Collider other);//when is trigger
}

public class Enemy : MonoBehaviour
{
    public string EnemyName;
    public int Hp;
    public int Strength;
    public float Speed;
    public float Xp;

    void Start()
    {

    }

    void Update()
    {

    }

    public void TakeDamage(int _damage)
    {
        Hp -= _damage;
        if (Hp <= 0)
        {
            GameManager.Instance.Enemies.Remove(this);
            Destroy(this.gameObject);
            LevelManager.Instance.AddXp(Xp);
        }
    }
}
