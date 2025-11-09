using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject projectilesPrefab;
    [SerializeField] float attackSpeed;

    float timerShoot;

    void Start()
    {
    }

    void Update()
    {
        timerShoot += Time.deltaTime;

        if (timerShoot > (attackSpeed/60))
        {
            if (Shoot())
            {
                timerShoot = 0;
            }
        }
    }

    bool Shoot()
    {
        if (GameManager.Instance.enemies.Count > 0)
        {
            Enemy currentTarget = GameManager.Instance.enemies[0];
            float dist = float.MaxValue;
            foreach (Enemy enemy in GameManager.Instance.enemies)
            {
                float newDist = Vector2.Distance(transform.position, enemy.transform.position);
                if(newDist < dist)
                {
                    currentTarget = enemy;
                }
            }
            GameObject go = Instantiate(projectilesPrefab, transform.position, Quaternion.identity);
            go.GetComponent<Projectiles>().target = currentTarget;

            return true;
        }
        return false;
    }
}
