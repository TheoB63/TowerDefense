using System.Collections.Generic;
using UnityEngine;

public class LaserBeamTower : Tower
{
    [Header("Laser Base Stats")]
    [SerializeField] int m_baseNbTarget;

    [Header("Laser Visual Settings")]
    [SerializeField] float m_laserWidth;
    [SerializeField] Color m_laserStartColor;
    [SerializeField] Color m_laserEndColor;
    [SerializeField] Material m_laserMaterial;
    [SerializeField] int m_laserSortingOrder;

    [Header("Curve Settings")]
    [SerializeField] int m_curveResolutionPerSegment = 40;
    [SerializeField] float m_waveFrequency = 2f; // Nombre d'ondulations sinusoïdales
    [SerializeField] float m_waveAmplitude = 0.3f; // Amplitude des ondulations
    [SerializeField] float m_splitPointDistance = 0.5f;
    [SerializeField] float m_animationSpeed = 1f; // Vitesse de l'animation subtile (0 = pas d'animation)

    //Runtime values
    int m_nbTarget;

    //timers
    float m_timerDuration;
    float m_timerReload;
    float m_timerTick;

    //Random system
    private System.Random m_organicRandom;

    //Lines renderer
    private LineRenderer m_mainBranch;
    private List<LineRenderer> m_branches = new List<LineRenderer>();

    // Stockage des courbes organiques
    private class LaserCurveData
    {
        public List<Enemy> targets; // Garder référence aux targets
        public float phaseOffset; // Offset de phase pour variation
        public int seed;
    }

    private LaserCurveData m_mainCurveData;
    private List<LaserCurveData> m_branchCurveData = new List<LaserCurveData>();
    private List<Enemy> m_lastTargets = new List<Enemy>();
    private float m_animationTime;

    void Start()
    {
        InitializeTower();
    }

    protected override void InitializeTower()
    {
        base.InitializeTower();
        m_nbTarget = m_baseNbTarget;

        m_organicRandom = new System.Random();

        //Create Main branch
        m_mainBranch = CreateLineRenderer("MainLaser");

        m_timerReload = 0;
        m_timerDuration = m_duration;
        m_animationTime = 0f;
    }

    private LineRenderer CreateLineRenderer(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.enabled = false;
        lr.startWidth = m_laserWidth;
        lr.endWidth = m_laserWidth;

        if (m_laserMaterial != null)
        {
            lr.material = m_laserMaterial;
        }
        else
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
        }

        lr.startColor = m_laserStartColor;
        if (name != "MainLaser")
        {
            lr.startColor = m_mainBranch.endColor;
        }

        lr.endColor = m_laserEndColor;
        lr.sortingOrder = m_laserSortingOrder;

        lr.numCornerVertices = 5;
        lr.numCapVertices = 5;

        return lr;
    }

    void Update()
    {
        MoveLauncher();

        if (Time.timeScale != 0) // != pause need to rework later with level up menu
        {
            if (m_timerDuration <= 0)
            {
                DisableAllLasers();
                m_timerReload -= Time.deltaTime;

                if (m_timerReload <= 0)
                {
                    m_timerDuration = m_duration;
                    m_timerTick = 0;
                }
            }
            else
            {
                m_timerDuration -= Time.deltaTime;
                m_animationTime += Time.deltaTime * m_animationSpeed;
                Attack();

                if (m_timerDuration <= 0)
                {
                    DisableAllLasers();
                    m_timerReload = m_cooldown;
                    m_lastTargets.Clear();
                }
            }
        }
    }

    private void Attack()
    {
        if (EnemyManager.Instance.Enemies.Count == 0)
        {
            DisableAllLasers();
            m_lastTargets.Clear();
            return;
        }

        List<Enemy> targets = GetNearestEnemies(transform, m_nbTarget);

        if (targets.Count == 0)
        {
            DisableAllLasers();
            m_lastTargets.Clear();
            return;
        }

        while (targets.Count < m_nbTarget)
        {
            List<Enemy> addTargets = GetNearestEnemies(transform, m_nbTarget - targets.Count);
            foreach (Enemy enemy in addTargets)
            {
                targets.Add(enemy);
            }
        }

        //TickDamage to all targets (xAttack if same enemy)
        m_timerTick -= Time.deltaTime;
        if (m_timerTick <= 0)
        {
            m_timerTick = m_damageInterval;
            foreach (Enemy target in targets)
            {
                target?.TakeDamage(m_damage);
            }
        }

        //remove if same target
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = i + 1; j < targets.Count; j++)
            {
                if (targets[i] == targets[j])
                {
                    targets.RemoveAt(j);
                    j--;
                }
            }
        }

        // Vérifier si les cibles ont changé (mort ou nouvelle cible)
        bool targetsChanged = HasTargetsChanged(targets);

        //Visual
        SetupLaserNetwork(targets, targetsChanged);

        m_lastTargets = new List<Enemy>(targets);
    }

    private bool HasTargetsChanged(List<Enemy> currentTargets)
    {
        if (m_lastTargets.Count != currentTargets.Count)
            return true;

        for (int i = 0; i < currentTargets.Count; i++)
        {
            // Vérifier si l'ennemi est null (mort) ou différent
            if (currentTargets[i] == null || m_lastTargets[i] == null || currentTargets[i] != m_lastTargets[i])
                return true;
        }

        return false;
    }

    private void SetupLaserNetwork(List<Enemy> targets, bool regenerateCurves)
    {
        // Filtrer les cibles nulles (ennemis morts)
        targets.RemoveAll(enemy => enemy == null);

        if (targets.Count == 0)
        {
            DisableAllLasers();
            return;
        }

        if (targets.Count == 1)
        {
            if (regenerateCurves || m_mainCurveData == null)
            {
                m_mainCurveData = GenerateCurveData(targets);
            }
            else
            {
                m_mainCurveData.targets = targets; // Mettre à jour la référence
            }

            DrawOrganicCurve(m_mainBranch, m_mainCurveData, m_barrel.position, targets[0].transform.position);
            m_mainBranch.enabled = true;

            // Désactiver toutes les branches secondaires
            foreach (LineRenderer branch in m_branches)
            {
                if (branch != null) branch.enabled = false;
            }
        }
        else
        {
            //Split Point - Recalculé chaque frame car les ennemis bougent
            Vector3 center = Vector3.zero;
            foreach (Enemy target in targets)
            {
                center += target.transform.position;
            }

            center /= targets.Count;

            float centerDist = (center - m_barrel.position).magnitude;
            Vector3 dir = (center - m_barrel.position).normalized;
            Vector3 splitPoint = m_barrel.position + (dir * Mathf.Abs(centerDist) * m_splitPointDistance);

            //Main Branch
            if (regenerateCurves || m_mainCurveData == null)
            {
                m_mainCurveData = GenerateCurveData(null);
            }

            DrawOrganicCurve(m_mainBranch, m_mainCurveData, m_barrel.position, splitPoint);
            m_mainBranch.enabled = true;

            //Other Branch - S'assurer qu'on a assez de branches
            while (m_branches.Count < targets.Count)
            {
                m_branches.Add(CreateLineRenderer("LaserBranch"));
            }

            // S'assurer qu'on a assez de données de courbe
            while (m_branchCurveData.Count < targets.Count)
            {
                m_branchCurveData.Add(null);
            }

            // Activer/désactiver les branches selon le nombre de cibles
            for (int i = 0; i < m_branches.Count; i++)
            {
                if (i < targets.Count)
                {
                    m_branches[i].enabled = true;
                }
                else
                {
                    m_branches[i].enabled = false;
                }
            }

            //Link other branch to targets - Dessiner chaque branche
            for (int i = 0; i < targets.Count; i++)
            {
                // Vérifier que la cible existe toujours
                if (targets[i] == null)
                {
                    m_branches[i].enabled = false;
                    continue;
                }

                // Régénérer la courbe si nécessaire
                if (regenerateCurves || m_branchCurveData[i] == null)
                {
                    m_branchCurveData[i] = GenerateCurveData(new List<Enemy> { targets[i] });
                }
                else
                {
                    m_branchCurveData[i].targets = new List<Enemy> { targets[i] };
                }

                // Dessiner la courbe avec les positions actuelles
                DrawOrganicCurve(m_branches[i], m_branchCurveData[i], splitPoint, targets[i].transform.position);
            }
        }
    }

    private LaserCurveData GenerateCurveData(List<Enemy> targets)
    {
        LaserCurveData data = new LaserCurveData
        {
            targets = targets,
            seed = m_organicRandom.Next(),
            phaseOffset = (float)m_organicRandom.NextDouble() * Mathf.PI * 2f
        };

        return data;
    }

    private void DrawOrganicCurve(LineRenderer lr, LaserCurveData curveData, Vector3 start, Vector3 end)
    {
        if (curveData == null) return;

        start.z = 0;
        end.z = 0;

        int resolution = m_curveResolutionPerSegment;
        lr.positionCount = resolution;

        Vector3 dir = end - start;
        float dist = dir.magnitude;
        Vector3 perpendicular = new Vector3(-dir.y, dir.x, 0).normalized;

        // Générer une belle courbe sinusoïdale lisse avec seed unique par branche
        System.Random localRandom = new System.Random(curveData.seed);
        float randomDirection = ((float)localRandom.NextDouble() - 0.5f) * 2f; // -1 à 1

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);

            // Position de base (interpolation linéaire)
            Vector3 basePosition = Vector3.Lerp(start, end, t);

            // Onde sinusoïdale pure et lisse
            float sineWave = Mathf.Sin(t * Mathf.PI * m_waveFrequency + curveData.phaseOffset) * m_waveAmplitude * dist;

            // Atténuation aux extrémités pour que ça parte/arrive bien droit
            float edgeFade = Mathf.Sin(t * Mathf.PI); // 0 au début et à la fin, 1 au milieu
            sineWave *= edgeFade;

            // Animation subtile si activée
            if (m_animationSpeed > 0)
            {
                float animWave = Mathf.Sin(t * Mathf.PI * m_waveFrequency + m_animationTime + curveData.phaseOffset) * 0.03f * dist;
                sineWave += animWave * edgeFade;
            }

            // Appliquer l'ondulation perpendiculairement avec direction unique
            Vector3 finalPosition = basePosition + perpendicular * sineWave * randomDirection;

            lr.SetPosition(i, finalPosition);
        }
    }

    private void DisableAllLasers()
    {
        if (m_mainBranch != null)
        {
            m_mainBranch.enabled = false;
        }

        foreach (LineRenderer branch in m_branches)
        {
            if (branch != null) branch.enabled = false;
        }

        // Réinitialiser les données de courbe
        m_mainCurveData = null;
        m_branchCurveData.Clear();
    }

    public List<Enemy> GetNearestEnemies(Transform _transform, int count)
    {
        List<Enemy> nearestEnemies = new List<Enemy>();

        if (EnemyManager.Instance.Enemies.Count == 0)
        {
            return nearestEnemies;
        }

        List<System.Tuple<Enemy, float>> enemyDistances = new List<System.Tuple<Enemy, float>>();

        foreach (Enemy enemy in EnemyManager.Instance.Enemies)
        {
            if (enemy == null) continue;

            float dist = Vector2.Distance(_transform.position, enemy.transform.position);

            if (dist <= m_range)
                enemyDistances.Add(new System.Tuple<Enemy, float>(enemy, dist));
        }

        enemyDistances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        int targetCount = Mathf.Min(count, enemyDistances.Count);
        for (int i = 0; i < targetCount; i++)
            nearestEnemies.Add(enemyDistances[i].Item1);

        return nearestEnemies;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_range);
    }

    public override void ApplyUpgrade(string _upgradeName)
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

    private void IncreaseDamage()
    {
        m_damage += m_stats.damage * 0.5f;
    }

    private void IncreaseRange()
    {
        m_range += 1;
    }

    private void IncreaseTargets()
    {
        m_nbTarget += 2;
    }
}