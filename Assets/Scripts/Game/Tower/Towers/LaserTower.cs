using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
    [Header("Laser Base Stats")]
    [SerializeField] float m_baseReloadTime;
    [SerializeField] float m_baseDuration;
    [SerializeField] float m_baseTickRate;
    [SerializeField] int m_baseNbTarget;

    [Header("Upgrades")]
    [SerializeField] float m_reloadTimeDecrease;
    [SerializeField] float m_durationIncrease;
    [SerializeField] float m_tickRateIncrease;
    [SerializeField] int m_nbTargetIncrease;

    [Header("Laser Visual Settings")]
    [SerializeField] float m_laserWidth;
    [SerializeField] Color m_laserStartColor;
    [SerializeField] Color m_laserEndColor;
    [SerializeField] Material m_laserMaterial;
    [SerializeField] int m_laserSortingOrder;

    [Header("Curve Settings")]
    [SerializeField] int m_curveResolutionPerSegment;
    [SerializeField] float m_curvature;
    [SerializeField] float m_curveHeight;
    [SerializeField] float m_splitPointDistance;
    [SerializeField] float m_organicVariation;

    //Runtime values
    float m_currentReloadTime;
    float m_currentDuration;
    float m_currentTickRate;
    int m_currentNbTarget;

    //timers
    float m_timerDuration;
    float m_timerReload;
    float m_timerTick;

    //Random system
    private System.Random m_organicRandom;

    //Lines renderer
    private LineRenderer m_mainBranch;
    private List<LineRenderer> m_branches = new List<LineRenderer>();

    void Start()
    {
        InitializeTower();
    }

    protected override void InitializeTower()
    {
        base.InitializeTower();
        m_currentReloadTime = m_baseReloadTime;
        m_currentDuration = m_baseDuration;
        m_currentTickRate = m_baseTickRate;
        m_currentNbTarget = m_baseNbTarget;

        m_organicRandom = new System.Random();

        //Create Main branch
        m_mainBranch = CreateLineRenderer("MainLaser");

        m_timerReload = 0;
        m_timerDuration = m_currentDuration;
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
                    m_timerDuration = m_currentDuration;
                    m_timerTick = 0;
                }
            }
            else
            {
                m_timerDuration -= Time.deltaTime;
                Attack();

                if (m_timerDuration <= 0)
                {
                    DisableAllLasers();
                    m_timerReload = m_currentReloadTime;
                }
            }
        }
    }

    private void Attack()
    {
        if (GameManager.Instance.Enemies.Count == 0)
        {
            DisableAllLasers();
            return;
        }

        List<Enemy> targets = GetNearestEnemies(transform, m_currentNbTarget);

        if (targets.Count == 0)
        {
            DisableAllLasers();
            return;
        }

        while (targets.Count < m_currentNbTarget)
        {
            List<Enemy> addTargets = GetNearestEnemies(transform, m_currentNbTarget - targets.Count);
            foreach (Enemy enemy in addTargets)
            {
                targets.Add(enemy);
            }
        }

        //TickDamage to all targets (xAttack if same enemy)
        m_timerTick -= Time.deltaTime;
        if (m_timerTick <= 0)
        {
            m_timerTick = m_currentTickRate;
            foreach (Enemy target in targets)
            {
                target?.TakeDamage(m_currentDamage);
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

        //Visual
        SetupLaserNetwork(targets);
    }

    private void SetupLaserNetwork(List<Enemy> targets)
    {
        if (targets.Count == 1)
        {
            DrawCurve(m_mainBranch, m_barrel.position, targets[0].transform.position);
            m_mainBranch.enabled = true;
        }
        else
        {
            //Split Point
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
            DrawCurve(m_mainBranch, m_barrel.position, splitPoint);
            m_mainBranch.enabled = true;

            //Other Branch
            for (int i = 0; i < targets.Count; i++)
            {
                m_branches.Add(CreateLineRenderer("LaserBranch"));
            }

            for (int i = 0; i < m_branches.Count; i++)
            {
                m_branches[i].enabled = i < targets.Count;
            }

            //Link other branch to targets
            for (int i = 0; i < targets.Count; i++)
            {
                DrawCurve(m_branches[i], splitPoint, targets[i].transform.position);
            }
        }
    }

    private void DrawCurve(LineRenderer lr, Vector3 start, Vector3 end)
    {
        //reset z
        start.z = 0;
        end.z = 0;

        int resolution = m_curveResolutionPerSegment;
        lr.positionCount = resolution;

        Vector3 dir = end - start;
        float dist = dir.magnitude;

        // Perpendiculaire organique
        Vector3 perp = GetOrganicPerpendicular(dir);
        Vector3 control = (start + end) * 0.5f + perp * m_curveHeight * (m_curvature * -dir.x) * dist;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);

            Vector3 p =
                (1 - t) * (1 - t) * start +
                2 * (1 - t) * t * control +
                t * t * end;

            lr.SetPosition(i, p);
        }
    }

    private Vector3 GetOrganicPerpendicular(Vector3 dir)
    {
        Vector3 basePerp = new Vector3(-dir.y, dir.x, 0).normalized;

        float random = ((float)m_organicRandom.NextDouble() - 0.5f) * m_organicVariation;

        return basePerp * (1f + random);
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
    }

    public List<Enemy> GetNearestEnemies(Transform _transform, int count)
    {
        List<Enemy> nearestEnemies = new List<Enemy>();

        if (GameManager.Instance.Enemies.Count == 0)
        {
            return nearestEnemies;
        }

        List<System.Tuple<Enemy, float>> enemyDistances = new List<System.Tuple<Enemy, float>>();

        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            if (enemy == null) continue;

            float dist = Vector2.Distance(_transform.position, enemy.transform.position);

            if (dist <= m_currentRange)
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
        float range = Application.isPlaying ? m_currentRange : m_baseRange;
        Gizmos.DrawWireSphere(transform.position, range);
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

    public override List<UpgradeBase> GetAvailableUpgrades()
    {
        List<UpgradeBase> available = new List<UpgradeBase>();

        foreach (TowerUpgrade upgrade in m_towerUpgrades)
        {
            TowerUpgrade copy = Instantiate(upgrade);
            available.Add(copy);
        }
        return available;
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
    }
}
