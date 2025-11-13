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
    [SerializeField] private float laserWidth;
    [SerializeField] private Color laserStartColor;
    [SerializeField] private Color laserEndColor;
    [SerializeField] private Material laserMaterial;
    [SerializeField] private int laserSortingOrder = 5;

    [Header("Curve Settings")]
    [SerializeField] private int curveResolutionPerSegment;
    [SerializeField] private float curvature;
    [SerializeField] private float curveHeight;
    [SerializeField] private float splitPointDistance;
    [SerializeField] private float organicVariation;

    // Runtime values
    int m_currentDamage;
    float m_currentRange;
    float m_currentReloadTime;
    float m_currentDuration;
    float m_currentTickRate;
    int m_currentNbTarget;

    float timerDuration;
    float timerReload;
    float timerTick;

    private System.Random organicRandom;

    private LineRenderer mainBranch;
    private List<LineRenderer> branches = new List<LineRenderer>();

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

        // Branchement principal
        mainBranch = CreateLineRenderer("MainLaserBranch");

        timerReload = 0;
        timerDuration = m_currentDuration;
    }

    private LineRenderer CreateLineRenderer(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = this.transform;

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.enabled = false;
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;

        if (laserMaterial != null)
            lr.material = laserMaterial;
        else
            lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.startColor = laserStartColor;
        lr.endColor = laserEndColor;
        lr.sortingOrder = laserSortingOrder;

        lr.numCornerVertices = 5;
        lr.numCapVertices = 5;

        return lr;
    }

    void Update()
    {
        if (timerDuration <= 0)
        {
            DisableAllLasers();
            timerReload -= Time.deltaTime;

            if (timerReload <= 0)
            {
                timerDuration = m_currentDuration;
                timerTick = 0;
            }
        }
        else
        {
            timerDuration -= Time.deltaTime;
            Attack();

            if (timerDuration <= 0)
            {
                DisableAllLasers();
                timerReload = m_currentReloadTime;
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

        // Damage tick
        timerTick -= Time.deltaTime;
        if (timerTick <= 0)
        {
            timerTick = m_currentTickRate;
            foreach (Enemy target in targets)
                target?.TakeDamage(m_currentDamage);
        }

        // Visual
        DrawLaserNetwork(targets);
    }

    private void DrawLaserNetwork(List<Enemy> targets)
    {
        if (targets.Count == 0 || Time.deltaTime == 0) //if pause
        {
            DisableAllLasers();
            return;
        }

        if (targets.Count == 1)
        {
            DrawCurve(mainBranch, transform.position, targets[0].transform.position);
            mainBranch.enabled = true;
        }
        else
        {
            // --- Calcul du point de split ---
            Vector3 center = Vector3.zero;
            foreach (var t in targets)
                center += t.transform.position;

            center /= targets.Count;

            Vector3 dir = (center - transform.position).normalized;
            Vector3 splitPoint = transform.position + dir * splitPointDistance;

            // --- Branche principale ---
            DrawCurve(mainBranch, transform.position, splitPoint);
            mainBranch.enabled = true;

            // --- Préparer les branches secondaires ---
            while (branches.Count < targets.Count)
                branches.Add(CreateLineRenderer("LaserBranch"));

            for (int i = 0; i < branches.Count; i++)
                branches[i].enabled = i < targets.Count;

            // --- Branches → targets ---
            for (int i = 0; i < targets.Count; i++)
                DrawCurve(branches[i], splitPoint, targets[i].transform.position);
        }
    }

    private void DrawCurve(LineRenderer lr, Vector3 start, Vector3 end)
    {
        start.z = 0;
        end.z = 0;

        int res = curveResolutionPerSegment;
        lr.positionCount = res;

        Vector3 dir = end - start;
        float dist = dir.magnitude;

        // Perpendiculaire organique
        Vector3 perp = GetOrganicPerpendicular(dir);
        Vector3 control = (start + end) * 0.5f + perp * curveHeight * (curvature * -dir.x) * dist;

        for (int i = 0; i < res; i++)
        {
            float t = i / (float)(res - 1);

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

        float random = ((float)organicRandom.NextDouble() - 0.5f) * organicVariation;

        return basePerp * (1f + random);
    }

    // -------------------------------------------------------------
    private void DisableAllLasers()
    {
        if (mainBranch != null)
            mainBranch.enabled = false;

        foreach (var b in branches)
            if (b != null) b.enabled = false;
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
    }
}
