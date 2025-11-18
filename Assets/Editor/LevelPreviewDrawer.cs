using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class LevelPreviewDrawer
{
    static LevelPreviewDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeObject is LevelConfig level)
        {
            DrawLevelPreview(level);
        }
    }

    static void DrawLevelPreview(LevelConfig level)
    {
        if (level.enemies.Count == 0)
        {
            return;
        }

        EnemyManager manager = GameObject.FindFirstObjectByType<EnemyManager>();

        Transform spawnPoint = manager != null ? manager.spawnPoint : null;

        if (spawnPoint == null)
        {
            DrawWarning("No EnemyManager with SpawnPoint found in scene!");
            return;
        }

        Vector3 spawnPos = spawnPoint.position;

        float maxTime = level.GetLevelDuration();
        if (maxTime <= 0) maxTime = 10f;

        DrawTitle(level);

        DrawTimeline(spawnPos, maxTime);

        var enemyGroups = level.enemies
            .Where(e => e.GetPrefab(level) != null)
            .GroupBy(e => Mathf.Round(e.spawnTime * 10f) / 10f) // Grouper par 0.1s
            .ToList();

        foreach (var group in enemyGroups)
        {
            float groupTime = group.Key;
            var enemiesAtTime = group.ToList();

            Vector3 basePos = spawnPos;
            basePos.y = spawnPos.y + groupTime;

            float totalWidth = (enemiesAtTime.Count - 1) * 0.6f;
            float startX = basePos.x - totalWidth / 2f;

            for (int i = 0; i < enemiesAtTime.Count; i++)
            {
                Vector3 markerPos = basePos;
                markerPos.x = startX + (i * 0.6f);

                DrawEnemyMarker(markerPos, enemiesAtTime[i], level);
            }
        }
    }

    static void DrawEnemyMarker(Vector3 position, EnemySpawnData enemyData, LevelConfig level)
    {
        Sprite sprite = enemyData.GetSprite(level);
        Color color = enemyData.GetPreviewColor(level);
        string name = enemyData.GetEnemyName(level);

        float size = 0.4f;

        if (enemyData.IsBoss())
        {
            size = 0.6f;
        }

        if (sprite != null)
        {
            DrawSpriteIcon(position, sprite, size, color, name);
        }
        else
        {
            Handles.color = color;
            Handles.DrawSolidDisc(position, Vector3.forward, size);
            Handles.color = Color.black;
            Handles.DrawWireDisc(position, Vector3.forward, size);
        }

        DrawLabel(position, enemyData, level);
    }

    static void DrawSpriteIcon(Vector3 position, Sprite sprite, float size, Color tint, string name)
    {
        Texture2D preview = AssetPreview.GetAssetPreview(sprite);

        if (preview != null)
        {
            Handles.BeginGUI();

            Vector3 screenCenter = HandleUtility.WorldToGUIPoint(position);
            float screenSize = 40f / HandleUtility.GetHandleSize(position);

            Rect iconRect = new Rect(
                screenCenter.x - screenSize / 2,
                screenCenter.y - screenSize / 2,
                screenSize,
                screenSize
            );

            GUI.color = tint;
            GUI.DrawTexture(iconRect, preview, ScaleMode.ScaleToFit);
            GUI.color = Color.white;

            Handles.EndGUI();
        }
        else
        {
            Handles.color = tint;
            Handles.DrawSolidDisc(position, Vector3.forward, size);
            Handles.color = Color.black;
            Handles.DrawWireDisc(position, Vector3.forward, size + 0.02f);

            Handles.BeginGUI();
            Vector3 screenPos = HandleUtility.WorldToGUIPoint(position);

            GUIStyle iconStyle = new GUIStyle(GUI.skin.label);
            iconStyle.fontSize = 16;
            iconStyle.fontStyle = FontStyle.Bold;
            iconStyle.normal.textColor = Color.white;
            iconStyle.alignment = TextAnchor.MiddleCenter;

            string slotName = name;
            string initial = slotName.Substring(slotName.Length - 1);
            GUI.Label(new Rect(screenPos.x - 10, screenPos.y - 10, 20, 20), initial, iconStyle);

            Handles.EndGUI();
        }
    }

    static void DrawLabel(Vector3 position, EnemySpawnData enemyData, LevelConfig level)
    {
        Handles.BeginGUI();

        Vector3 screenPos = HandleUtility.WorldToGUIPoint(position);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.box);
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 10;
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.padding = new RectOffset(5, 5, 2, 2);

        string bossTag = enemyData.IsBoss() ? " 💀" : "";
        string slotName = enemyData.GetSlotName(level);
        string label = $"{enemyData.spawnTime:F1}s | {slotName} ({enemyData.GetEnemyName(level)}) | {enemyData.GetXpValue(level)} XP{bossTag}";
        Vector2 labelSize = labelStyle.CalcSize(new GUIContent(label));

        GUI.backgroundColor = new Color(0, 0, 0, 0.8f);
        GUI.Box(new Rect(screenPos.x + 30, screenPos.y - 12, labelSize.x + 10, 24), label, labelStyle);

        Handles.EndGUI();
    }

    static void DrawTimeline(Vector3 spawnPos, float maxTime)
    {
        // 1s = 1u
        Vector3 timelineStart = spawnPos + new Vector3(-1.5f, 0, 0);
        Vector3 timelineEnd = timelineStart + new Vector3(0, maxTime, 0);

        Handles.color = new Color(1, 1, 1, 0.6f);
        Handles.DrawLine(timelineStart, timelineEnd);

        int numTicks = Mathf.CeilToInt(maxTime) + 1;

        for (int i = 0; i < numTicks; i++)
        {
            float time = i;
            if (time > maxTime) break;

            Vector3 tickPos = timelineStart + new Vector3(0, time, 0);

            Handles.color = new Color(1, 1, 1, 0.6f);
            Handles.DrawLine(tickPos, tickPos + Vector3.left * 0.2f);

            Handles.BeginGUI();
            Vector3 screenPos = HandleUtility.WorldToGUIPoint(tickPos);

            GUIStyle timeStyle = new GUIStyle(GUI.skin.label);
            timeStyle.normal.textColor = Color.white;
            timeStyle.fontSize = 10;
            timeStyle.alignment = TextAnchor.MiddleRight;
            timeStyle.fontStyle = FontStyle.Bold;

            string timeLabel = $"{time:F0}s";

            Vector2 labelSize = timeStyle.CalcSize(new GUIContent(timeLabel));
            Rect bgRect = new Rect(screenPos.x - labelSize.x - 10, screenPos.y - 10, labelSize.x + 5, 20);
            GUI.backgroundColor = new Color(0, 0, 0, 0.7f);
            GUI.Box(bgRect, "");

            // Label
            GUI.Label(new Rect(screenPos.x - labelSize.x - 8, screenPos.y - 8, labelSize.x, 16), timeLabel, timeStyle);

            Handles.EndGUI();
        }

        Handles.color = Color.yellow;
        Vector3 arrowBase = timelineEnd;
        Vector3 arrowTip = arrowBase + Vector3.up * 0.3f;
        Handles.DrawLine(arrowBase, arrowTip);
        Handles.DrawLine(arrowTip, arrowTip + new Vector3(-0.15f, -0.15f, 0));
        Handles.DrawLine(arrowTip, arrowTip + new Vector3(0.15f, -0.15f, 0));
    }

    static void DrawTitle(LevelConfig level)
    {
        Handles.BeginGUI();

        GUIStyle titleStyle = new GUIStyle(GUI.skin.box);
        titleStyle.fontSize = 14;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.cyan;
        titleStyle.padding = new RectOffset(10, 10, 5, 5);

        string title = $"📊 Preview: {level.levelName}";
        GUI.backgroundColor = new Color(0, 0, 0, 0.8f);
        GUI.Box(new Rect(10, 10, 350, 30), title, titleStyle);

        GUIStyle statsStyle = new GUIStyle(GUI.skin.box);
        statsStyle.fontSize = 11;
        statsStyle.normal.textColor = Color.white;
        statsStyle.alignment = TextAnchor.MiddleLeft;
        statsStyle.padding = new RectOffset(10, 10, 5, 5);

        string stats = $"Enemies: {level.GetTotalEnemies()} ({level.GetMobCount()} mobs, {level.GetBossCount()} bosses) | XP: {level.GetTotalXP()} | Duration: {level.GetLevelDuration():F1}s | Level {level.GetStartLevel()} → {level.GetEndLevel()}";
        GUI.Box(new Rect(10, 45, 700, 25), stats, statsStyle);

        Handles.EndGUI();
    }

    static void DrawWarning(string message)
    {
        Handles.BeginGUI();

        GUIStyle warningStyle = new GUIStyle(GUI.skin.box);
        warningStyle.fontSize = 12;
        warningStyle.normal.textColor = Color.yellow;
        warningStyle.padding = new RectOffset(10, 10, 5, 5);

        GUI.backgroundColor = new Color(1, 0.5f, 0, 0.8f);
        GUI.Box(new Rect(10, 10, 400, 30), $"⚠️ {message}", warningStyle);

        Handles.EndGUI();
    }
}