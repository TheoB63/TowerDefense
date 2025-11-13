using System.Collections.Generic;
using UnityEngine;

public class LaserTower : MonoBehaviour, ITower
{
    [Header("Laser Base Stats")]
    [SerializeField] int m_baseDamage;
    [SerializeField] float m_baseRange;
    [SerializeField] float m_baseReloadTime;
    [SerializeField] float m_baseDuration;
    [SerializeField] float m_baseTickRate;
    [SerializeField] int m_baseNbTarget;

    [Header("Upgrades")]
    [SerializeField] int m_damageMultiplier;
    [SerializeField] float m_rangeIncrease;
    [SerializeField] float m_reloadTimeDecrease;
    [SerializeField] float m_durationIncrease;
    [SerializeField] float m_tickRateIncrease;
    [SerializeField] int m_nbTargetIncrease;

    [Header("Laser Visual Settings")]
    [SerializeField] private float laserWidth = 0.1f;
    [SerializeField] private Color laserStartColor = Color.red;
    [SerializeField] private Color laserEndColor = new Color(1f, 0.5f, 0f, 0.8f);
    [SerializeField] private Material laserMaterial;
    [SerializeField] private int laserSortingOrder = 5;

    [Header("Curve Settings")]
    [SerializeField] private int curveResolution = 20;
    [SerializeField] private float curvature = 0.5f;
    [SerializeField] private float curveHeight = 1f;

    [Header("Muzzle Effect Settings")]
    [SerializeField] private float muzzleLifetime = 0.3f;
    [SerializeField] private float muzzleSpeed = 1f;
    [SerializeField] private float muzzleSize = 0.3f;
    [SerializeField] private Color muzzleColor = new Color(0f, 1f, 1f, 0.8f);
    [SerializeField] private float muzzleEmissionRate = 50f;
    [SerializeField] private float muzzleRadius = 0.2f;

    [Header("Impact Effect Settings")]
    [SerializeField] private float impactLifetime = 0.2f;
    [SerializeField] private float impactSpeed = 2f;
    [SerializeField] private float impactSize = 0.2f;
    [SerializeField] private Color impactColor = new Color(1f, 1f, 0f, 0.9f);
    [SerializeField] private float impactEmissionRate = 100f;
    [SerializeField] private float impactRadius = 0.15f;

    [Header("Effect Sorting")]
    [SerializeField] private int effectSortingOrder = 6;

    //Privates Current values in run time
    int m_currentDamage;
    float m_currentRange;
    float m_currentReloadTime;
    float m_currentDuration;
    float m_currentTickRate;
    int m_currentNbTarget;

    //laser timers
    float timerDuration;
    float timerReload;
    float timerTick;

    private List<LineRenderer> laserLines = new List<LineRenderer>();
    private List<ParticleSystem> impactEffects = new List<ParticleSystem>();
    private ParticleSystem muzzleEffect;

    public TowerType GetTowerType() => TowerType.Laser;
    public string GetTowerName() => "Laser Tower";

    void Start()
    {
        m_currentDamage = m_baseDamage;
        m_currentRange = m_baseRange;
        m_currentReloadTime = m_baseReloadTime;
        m_currentDuration = m_baseDuration;
        m_currentTickRate = m_baseTickRate;
        m_currentNbTarget = m_baseNbTarget;

        // Créer l'effet à la base (muzzle)
        CreateMuzzleEffect();

        // Créer les LineRenderer ET les effets d'impact pour chaque cible
        for (int i = 0; i < m_currentNbTarget; i++)
        {
            CreateLaserLine();
            CreateImpactEffect();
        }

        timerReload = 0; // Commence en mode attaque
        timerDuration = m_currentDuration;
    }

    private void CreateLaserLine()
    {
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();

        // Configuration du LineRenderer
        lr.positionCount = curveResolution;
        lr.enabled = false;
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth; // CORRECTION: Même largeur partout

        // Utiliser le material assigné ou créer un par défaut
        if (laserMaterial != null)
        {
            lr.material = laserMaterial;
        }
        else
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
        }

        lr.startColor = laserStartColor;
        lr.endColor = laserEndColor;
        lr.sortingOrder = laserSortingOrder;

        laserLines.Add(lr);
    }

    private void CreateMuzzleEffect()
    {
        GameObject muzzleObj = new GameObject("MuzzleEffect");
        muzzleObj.transform.SetParent(transform);
        muzzleObj.transform.localPosition = Vector3.zero;

        muzzleEffect = muzzleObj.AddComponent<ParticleSystem>();
        var main = muzzleEffect.main;
        main.startLifetime = muzzleLifetime;
        main.startSpeed = muzzleSpeed;
        main.startSize = muzzleSize;
        main.startColor = muzzleColor;
        main.maxParticles = 30;
        main.loop = true;
        main.playOnAwake = false;

        var emission = muzzleEffect.emission;
        emission.rateOverTime = muzzleEmissionRate;

        var shape = muzzleEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = muzzleRadius;

        var colorOverLifetime = muzzleEffect.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = grad;

        var renderer = muzzleEffect.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = effectSortingOrder;
    }

    private void CreateImpactEffect()
    {
        GameObject impactObj = new GameObject($"ImpactEffect_{impactEffects.Count}");
        impactObj.transform.SetParent(transform);

        ParticleSystem impact = impactObj.AddComponent<ParticleSystem>();
        var main = impact.main;
        main.startLifetime = impactLifetime;
        main.startSpeed = impactSpeed;
        main.startSize = impactSize;
        main.startColor = impactColor;
        main.maxParticles = 20;
        main.loop = true;
        main.playOnAwake = false;

        var emission = impact.emission;
        emission.rateOverTime = impactEmissionRate;

        var shape = impact.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = impactRadius;

        var colorOverLifetime = impact.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = grad;

        var renderer = impact.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = effectSortingOrder;

        impactEffects.Add(impact);
    }

    void Update()
    {
        if (timerDuration <= 0) //reload
        {
            DisableAllLasers();
            DisableAllEffects(); // CORRECTION: Arrêter les effets pendant le reload
            timerReload -= Time.deltaTime;
            if (timerReload <= 0)
            {
                timerDuration = m_currentDuration;
                timerTick = 0; // Reset du tick
            }
        }
        else //attack
        {
            timerDuration -= Time.deltaTime;
            Attack();

            if (timerDuration <= 0)
            {
                timerReload = m_currentReloadTime;
                DisableAllLasers();
                DisableAllEffects(); // CORRECTION: Arrêter les effets à la fin de l'attaque
            }
        }
    }

    private void Attack()
    {
        if (GameManager.Instance.Enemies.Count == 0)
        {
            DisableAllLasers();
            DisableAllEffects();
            return;
        }

        List<Enemy> targets = GetNearestEnemies(transform, m_currentNbTarget);

        // Activer/désactiver l'effet muzzle
        if (targets.Count > 0 && !muzzleEffect.isPlaying)
        {
            muzzleEffect.Play();
        }
        else if (targets.Count == 0 && muzzleEffect.isPlaying)
        {
            muzzleEffect.Stop();
        }

        // Appliquer les dégâts selon le tick rate
        timerTick -= Time.deltaTime;
        if (timerTick <= 0)
        {
            timerTick = m_currentTickRate;
            foreach (Enemy target in targets)
            {
                if (target != null)
                {
                    target.TakeDamage(m_currentDamage);
                }
            }
        }

        // Dessiner les lasers et positionner les effets d'impact
        // CORRECTION IMPORTANTE: Utiliser le minimum entre les deux listes pour éviter les erreurs
        int maxIndex = Mathf.Min(laserLines.Count, impactEffects.Count);

        for (int i = 0; i < maxIndex; i++)
        {
            if (i < targets.Count && targets[i] != null)
            {
                laserLines[i].enabled = true;
                DrawCurvedLaser(laserLines[i], transform.position, targets[i].transform.position);

                impactEffects[i].transform.position = targets[i].transform.position;
                if (!impactEffects[i].isPlaying)
                {
                    impactEffects[i].Play();
                }
            }
            else
            {
                laserLines[i].enabled = false;
                if (impactEffects[i].isPlaying)
                {
                    impactEffects[i].Stop();
                }
            }
        }
    }

    private void DrawCurvedLaser(LineRenderer lr, Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // Calculer le point de contrôle pour la courbe de Bézier
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0).normalized;
        Vector3 controlPoint = (start + end) / 2f + perpendicular * curveHeight * curvature * distance;

        // Générer les points de la courbe de Bézier quadratique
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            Vector3 point = CalculateQuadraticBezierPoint(t, start, controlPoint, end);
            lr.SetPosition(i, point);
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0;
        point += 2 * u * t * p1;
        point += tt * p2;

        return point;
    }

    private void DisableAllLasers()
    {
        foreach (LineRenderer lr in laserLines)
        {
            if (lr != null)
                lr.enabled = false;
        }
    }

    private void DisableAllEffects()
    {
        if (muzzleEffect != null && muzzleEffect.isPlaying)
        {
            muzzleEffect.Stop();
        }

        foreach (ParticleSystem ps in impactEffects)
        {
            if (ps != null && ps.isPlaying)
            {
                ps.Stop();
            }
        }
    }

    public List<Enemy> GetNearestEnemies(Transform _transform, int count)
    {
        List<Enemy> nearestEnemies = new List<Enemy>();

        if (GameManager.Instance.Enemies.Count == 0)
            return nearestEnemies;

        // Créer une liste de paires (Enemy, Distance)
        List<System.Tuple<Enemy, float>> enemyDistances = new List<System.Tuple<Enemy, float>>();

        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            if (enemy == null) continue;

            float dist = Vector2.Distance(_transform.position, enemy.transform.position);

            // Ne considérer que les ennemis dans la portée
            if (dist <= m_currentRange)
            {
                enemyDistances.Add(new System.Tuple<Enemy, float>(enemy, dist));
            }
        }

        // Trier par distance (seulement ceux dans la portée)
        enemyDistances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        // Prendre les N premiers
        int targetCount = Mathf.Min(count, enemyDistances.Count);
        for (int i = 0; i < targetCount; i++)
        {
            nearestEnemies.Add(enemyDistances[i].Item1);
        }

        return nearestEnemies;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float range = Application.isPlaying ? m_currentRange : m_baseRange;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void ApplyUpgrade(string _upgradeName)
    {
        switch (_upgradeName)
        {
            case "IncreaseDamage":
                IncreaseDamage();
                break;

            case "IncreaseRange":
                IncreaseRange();
                break;

            case "IncreaseTargets":
                IncreaseTargets();
                break;
        }
    }

    public List<string> GetAvailableUpgrades()
    {
        var upgrades = new List<string>();

        upgrades.Add("IncreaseDamage");
        upgrades.Add("IncreaseRange");
        upgrades.Add("IncreaseTargets");

        return upgrades;
    }

    private void IncreaseDamage()
    {
        m_currentDamage = (int)(m_currentDamage * m_damageMultiplier);
    }

    private void IncreaseRange()
    {
        m_currentRange += m_rangeIncrease;
    }

    private void IncreaseTargets()
    {
        m_currentNbTarget += m_nbTargetIncrease;

        // CORRECTION: Ajouter des LineRenderer ET des ParticleSystem d'impact
        while (laserLines.Count < m_currentNbTarget)
        {
            CreateLaserLine();
            CreateImpactEffect(); // IMPORTANT: Créer l'effet d'impact correspondant
        }
    }
}