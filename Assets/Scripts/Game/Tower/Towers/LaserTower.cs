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
    [SerializeField] private int curveResolutionPerSegment = 15; // Points par segment de courbe
    [SerializeField] private float curvature = 0.5f;
    [SerializeField] private float curveHeight = 1f;
    [SerializeField] private float splitPointDistance = 2f; // Distance du point de split depuis la tour
    [SerializeField] private float organicVariation = 0.2f; // Variation aléatoire

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

    [Header("Split Effect Settings")]
    [SerializeField] private float splitEffectSize = 0.4f;
    [SerializeField] private Color splitEffectColor = new Color(1f, 0f, 1f, 0.9f);

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

    // UN SEUL LineRenderer pour tout le réseau
    private LineRenderer laserNetwork;
    private List<ParticleSystem> impactEffects = new List<ParticleSystem>();
    private ParticleSystem muzzleEffect;
    private ParticleSystem splitEffect;

    // Pour la variation organique
    private System.Random organicRandom;

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

        organicRandom = new System.Random();

        // Créer LE LineRenderer unique
        CreateLaserNetwork();

        // Créer l'effet à la base (muzzle)
        CreateMuzzleEffect();

        // Créer l'effet au point de split
        CreateSplitEffect();

        // Créer les effets d'impact pour chaque cible
        for (int i = 0; i < m_currentNbTarget; i++)
        {
            CreateImpactEffect();
        }

        timerReload = 0;
        timerDuration = m_currentDuration;
    }

    private void CreateLaserNetwork()
    {
        laserNetwork = gameObject.AddComponent<LineRenderer>();
        laserNetwork.enabled = false;
        laserNetwork.startWidth = laserWidth;
        laserNetwork.endWidth = laserWidth;

        if (laserMaterial != null)
        {
            laserNetwork.material = laserMaterial;
        }
        else
        {
            laserNetwork.material = new Material(Shader.Find("Sprites/Default"));
        }

        laserNetwork.startColor = laserStartColor;
        laserNetwork.endColor = laserEndColor;
        laserNetwork.sortingOrder = laserSortingOrder;

        // Pas de coins durs, courbes lisses
        laserNetwork.numCornerVertices = 5;
        laserNetwork.numCapVertices = 5;
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

    private void CreateSplitEffect()
    {
        GameObject splitObj = new GameObject("SplitEffect");
        splitObj.transform.SetParent(transform);

        splitEffect = splitObj.AddComponent<ParticleSystem>();
        var main = splitEffect.main;
        main.startLifetime = 0.3f;
        main.startSpeed = 3f;
        main.startSize = splitEffectSize;
        main.startColor = splitEffectColor;
        main.maxParticles = 50;
        main.loop = true;
        main.playOnAwake = false;

        var emission = splitEffect.emission;
        emission.rateOverTime = 80;

        var shape = splitEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        var colorOverLifetime = splitEffect.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.magenta, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = grad;

        var renderer = splitEffect.GetComponent<ParticleSystemRenderer>();
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
            DisableAllEffects();
            timerReload -= Time.deltaTime;
            if (timerReload <= 0)
            {
                timerDuration = m_currentDuration;
                timerTick = 0;
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
                DisableAllEffects();
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

        if (targets.Count == 0)
        {
            DisableAllLasers();
            DisableAllEffects();
            return;
        }

        // Activer les effets
        if (!muzzleEffect.isPlaying)
            muzzleEffect.Play();

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

        // Dessiner le réseau de lasers
        DrawLaserNetwork(targets);

        // Positionner les effets d'impact
        for (int i = 0; i < targets.Count && i < impactEffects.Count; i++)
        {
            if (targets[i] != null)
            {
                impactEffects[i].transform.position = targets[i].transform.position;
                if (!impactEffects[i].isPlaying)
                    impactEffects[i].Play();
            }
        }

        // Arrêter les effets d'impact inutilisés
        for (int i = targets.Count; i < impactEffects.Count; i++)
        {
            if (impactEffects[i].isPlaying)
                impactEffects[i].Stop();
        }
    }

    private void DrawLaserNetwork(List<Enemy> targets)
    {
        if (targets.Count == 0)
        {
            laserNetwork.enabled = false;
            return;
        }

        laserNetwork.enabled = true;

        // Si un seul target, juste une courbe simple
        if (targets.Count == 1)
        {
            laserNetwork.positionCount = curveResolutionPerSegment;
            DrawCurve(laserNetwork, 0, transform.position, targets[0].transform.position);

            // Pas d'effet de split
            if (splitEffect.isPlaying)
                splitEffect.Stop();

            return;
        }

        // Calculer le point de split
        Vector3 targetsCenter = Vector3.zero;
        foreach (Enemy target in targets)
        {
            if (target != null)
                targetsCenter += target.transform.position;
        }
        targetsCenter /= targets.Count;

        Vector3 directionToTargets = (targetsCenter - transform.position).normalized;
        Vector3 splitPoint = transform.position + directionToTargets * splitPointDistance;

        // Activer et positionner l'effet de split
        splitEffect.transform.position = splitPoint;
        if (!splitEffect.isPlaying)
            splitEffect.Play();

        // Calculer le nombre total de points nécessaires
        // 1 segment de la tour au split + N segments du split aux targets
        int totalPoints = curveResolutionPerSegment + (targets.Count * curveResolutionPerSegment);
        laserNetwork.positionCount = totalPoints;

        // Dessiner la courbe principale (tour -> split)
        DrawCurve(laserNetwork, 0, transform.position, splitPoint);

        // Dessiner les courbes vers chaque target
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                int startIndex = curveResolutionPerSegment + (i * curveResolutionPerSegment);
                DrawCurve(laserNetwork, startIndex, splitPoint, targets[i].transform.position);
            }
        }
    }

    private void DrawCurve(LineRenderer lr, int startIndex, Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // Calculer la perpendiculaire avec variation organique
        Vector3 perpendicular = GetOrganicPerpendicular(direction);
        Vector3 controlPoint = (start + end) / 2f + perpendicular * curveHeight * curvature * distance;

        // Générer les points de la courbe de Bézier quadratique
        for (int i = 0; i < curveResolutionPerSegment; i++)
        {
            float t = i / (float)(curveResolutionPerSegment - 1);
            Vector3 point = CalculateQuadraticBezierPoint(t, start, controlPoint, end);
            lr.SetPosition(startIndex + i, point);
        }
    }

    private Vector3 GetOrganicPerpendicular(Vector3 direction)
    {
        // Calculer la perpendiculaire de base
        Vector3 basePerpendicular = new Vector3(-direction.y, direction.x, 0).normalized;

        // Déterminer de quel côté courber pour prendre le chemin le plus court
        float directionBias = 1f;

        // Si la direction pointe vers la droite, courber à gauche
        if (direction.x > 0)
            directionBias = -1f;

        // Si c'est plutôt vertical, utiliser y
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y > 0)
                directionBias = 1f;
            else
                directionBias = -1f;
        }

        // Ajouter une légère variation aléatoire
        float randomVariation = ((float)organicRandom.NextDouble() - 0.5f) * organicVariation;

        return basePerpendicular * directionBias * (1f + randomVariation);
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
        if (laserNetwork != null)
            laserNetwork.enabled = false;
    }

    private void DisableAllEffects()
    {
        if (muzzleEffect != null && muzzleEffect.isPlaying)
            muzzleEffect.Stop();

        if (splitEffect != null && splitEffect.isPlaying)
            splitEffect.Stop();

        foreach (ParticleSystem ps in impactEffects)
        {
            if (ps != null && ps.isPlaying)
                ps.Stop();
        }
    }

    public List<Enemy> GetNearestEnemies(Transform _transform, int count)
    {
        List<Enemy> nearestEnemies = new List<Enemy>();

        if (GameManager.Instance.Enemies.Count == 0)
            return nearestEnemies;

        List<System.Tuple<Enemy, float>> enemyDistances = new List<System.Tuple<Enemy, float>>();

        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            if (enemy == null) continue;

            float dist = Vector2.Distance(_transform.position, enemy.transform.position);

            if (dist <= m_currentRange)
            {
                enemyDistances.Add(new System.Tuple<Enemy, float>(enemy, dist));
            }
        }

        enemyDistances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

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

        // Créer de nouveaux effets d'impact si nécessaire
        while (impactEffects.Count < m_currentNbTarget)
        {
            CreateImpactEffect();
        }
    }
}