using UnityEngine;

public interface IEnemyAction
{
    void OnSpawn(Enemy enemy);//création
    void OnDeath(Enemy enemy);//destruction
    void OnUpdate(Enemy enemy);//every frame
    void OnHit(Enemy enemy, int damage);//when touch
    void OnEnterTrigger(Enemy enemy, Collider other);//when is trigger
}

public enum EnemyType
{
    Mob,
    boss,
}

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] GameObject deathEffectsPrefab;
    public Color previewColor;

    public string EnemyName;
    public float Hp;
    public int Damage;
    public float Speed;
    public float Xp;

    void Start()
    {

    }

    void Update()
    {
        if (Hp <= 0)
        {
            Kill();
        }


    }

    public void TakeDamage(float _damage)
    {
        Hp -= _damage;
        GameObject go = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        go.GetComponent<DamageText>().Setup(_damage);
    }

    public void Kill()
    {
        LevelManager.Instance.AddXp(Xp);

        GameObject particle = Instantiate(deathEffectsPrefab);
        particle.transform.position = transform.position;

        ParticleSystem.Burst newBurst = new ParticleSystem.Burst();
        newBurst.count = 50;
        particle.GetComponent<ParticleSystem>().emission.SetBurst(0, newBurst);


        EnemyManager.Instance.Enemies.Remove(this);
        Destroy(gameObject);
    }
}
