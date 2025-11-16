using Codice.CM.Client.Differences.Graphic;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ⚠️ CE SCRIPT DOIT ÊTRE DANS UN DOSSIER NOMMÉ "Editor" À LA RACINE DE TON PROJET
// Il permet de visualiser les spawns directement dans la SceneView

[InitializeOnLoad]
public class LevelPreviewDrawer
{
    static LevelPreviewDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        // Vérifier si un LevelConfig est sélectionné dans le Project
        if (Selection.activeObject is LevelConfig level)
        {
            DrawLevelPreview(level);
        }

        // Vérifier si un EnemyManager est sélectionné dans la scène
        if (Selection.activeGameObject != null)
        {
            EnemyManager manager = Selection.activeGameObject.GetComponent<EnemyManager>();
            if (manager != null && manager.allLevels.Count > 0)
            {
                // Afficher le premier level par défaut
                LevelConfig levelToShow = manager.allLevels[0];
                DrawLevelPreview(levelToShow);
            }
        }
    }

    static void DrawLevelPreview(LevelConfig level)
    {
        if (level.enemies.Count == 0) return;

        // Trouver le spawn point dans la scène
        EnemyManager manager = GameObject.FindFirstObjectByType<EnemyManager>();
        Transform spawnPoint = manager != null ? manager.spawnPoint : null;

        if (spawnPoint == null)
        {
            DrawWarning("No EnemyManager with SpawnPoint found in scene!");
            return;
        }

        Vector3 spawnPos = spawnPoint.position;

        // Calculer la durée max
        float maxTime = level.GetLevelDuration();
        if (maxTime <= 0) maxTime = 10f;

        // Dessiner le titre en overlay
        DrawTitle(level);

        // Dessiner la timeline (1 seconde = 1 unité Unity)
        DrawTimeline(spawnPos, maxTime);

        // Grouper les ennemis par timing pour gérer les collisions
        var enemyGroups = level.enemies
            .Where(e => e.enemyPrefab != null)
            .GroupBy(e => Mathf.Round(e.spawnTime * 10f) / 10f) // Grouper par 0.1s
            .ToList();

        // Dessiner chaque groupe d'ennemis
        foreach (var group in enemyGroups)
        {
            float groupTime = group.Key;
            var enemiesAtTime = group.ToList();

            // Position Y = spawnPos.y + temps (1 seconde = 1 unité vers le HAUT)
            Vector3 basePos = spawnPos;
            basePos.y = spawnPos.y + groupTime; // VERS LE HAUT

            // Si plusieurs ennemis au même moment, les espacer horizontalement
            float totalWidth = (enemiesAtTime.Count - 1) * 0.6f;
            float startX = basePos.x - totalWidth / 2f;

            for (int i = 0; i < enemiesAtTime.Count; i++)
            {
                Vector3 markerPos = basePos;
                markerPos.x = startX + (i * 0.6f);

                DrawEnemyMarker(markerPos, enemiesAtTime[i]);
            }
        }
    }

    static void DrawEnemyMarker(Vector3 position, EnemySpawnData enemyData)
    {
        Sprite sprite = enemyData.GetSprite();
        Color color = enemyData.GetPreviewColor();

        // Taille du marker
        float size = 0.4f;

        // Dessiner avec le sprite si disponible, sinon cercle coloré
        if (sprite != null)
        {
            DrawSpriteIcon(position, size, enemyData);
        }
        else
        {
            // Fallback : cercle coloré
            Handles.color = color;
            Handles.DrawSolidDisc(position, Vector3.forward, size);
            Handles.color = Color.black;
            Handles.DrawWireDisc(position, Vector3.forward, size);
        }

        // Label avec les infos
        DrawLabel(position, enemyData);
    }

    static void DrawSpriteIcon(Vector3 position, float size, EnemySpawnData enemyData)
    {
        // Méthode 1 : Essayer avec AssetPreview
        Texture2D preview = AssetPreview.GetAssetPreview(enemyData.GetSprite());

        if (preview != null)
        {
            // Afficher la preview avec teinte
            Handles.BeginGUI();

            Vector3 screenCenter = HandleUtility.WorldToGUIPoint(position);
            float screenSize = HandleUtility.GetHandleSize(position) * 40f;

            Rect iconRect = new Rect(
                screenCenter.x - screenSize / 2,
                screenCenter.y - screenSize / 2,
                screenSize,
                screenSize
            );

            // Dessiner avec la couleur de teinte
            GUI.color = enemyData.GetPreviewColor();
            GUI.DrawTexture(iconRect, preview, ScaleMode.ScaleToFit);
            GUI.color = Color.white;

            Handles.EndGUI();
        }
        else
        {
            // Méthode 2 : Dessiner un carré coloré avec le nom
            Handles.color = enemyData.GetPreviewColor();
            Handles.DrawSolidDisc(position, Vector3.forward, size);
            Handles.color = Color.black;
            Handles.DrawWireDisc(position, Vector3.forward, size + 0.02f);

            // Afficher l'initiale du nom au centre
            Handles.BeginGUI();
            Vector3 screenPos = HandleUtility.WorldToGUIPoint(position);

            GUIStyle iconStyle = new GUIStyle(GUI.skin.label);
            iconStyle.fontSize = 16;
            iconStyle.fontStyle = FontStyle.Bold;
            iconStyle.normal.textColor = Color.white;
            iconStyle.alignment = TextAnchor.MiddleCenter;

            string initial = enemyData.GetEnemyName().Substring(0, 1).ToUpper();
            GUI.Label(new Rect(screenPos.x - 10, screenPos.y - 10, 20, 20), initial, iconStyle);

            Handles.EndGUI();
        }
    }

    static void DrawLabel(Vector3 position, EnemySpawnData enemyData)
    {
        Handles.BeginGUI();

        Vector3 screenPos = HandleUtility.WorldToGUIPoint(position);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.box);
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 10;
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.padding = new RectOffset(5, 5, 2, 2);

        string label = $"{enemyData.spawnTime:F1}s | {enemyData.GetEnemyName()} | {enemyData.GetXPValue()} XP";
        Vector2 labelSize = labelStyle.CalcSize(new GUIContent(label));

        GUI.backgroundColor = new Color(0, 0, 0, 0.8f);
        GUI.Box(new Rect(screenPos.x + 30, screenPos.y - 12, labelSize.x + 10, 24), label, labelStyle);

        Handles.EndGUI();
    }

    static void DrawTimeline(Vector3 spawnPos, float maxTime)
    {
        // Timeline : 1 seconde = 1 unité Unity vers le HAUT
        Vector3 timelineStart = spawnPos + new Vector3(-1.5f, 0, 0);
        Vector3 timelineEnd = timelineStart + new Vector3(0, maxTime, 0); // VERS LE HAUT

        // Ligne principale
        Handles.color = new Color(1, 1, 1, 0.6f);
        Handles.DrawLine(timelineStart, timelineEnd);

        // Graduations tous les 1 secondes
        int numTicks = Mathf.CeilToInt(maxTime) + 1;

        for (int i = 0; i < numTicks; i++)
        {
            float time = i;
            if (time > maxTime) break;

            // Position : spawn + time (1 unité par seconde)
            Vector3 tickPos = timelineStart + new Vector3(0, time, 0);

            // Tick mark
            Handles.color = new Color(1, 1, 1, 0.6f);
            Handles.DrawLine(tickPos, tickPos + Vector3.left * 0.2f);

            // Label du temps
            Handles.BeginGUI();
            Vector3 screenPos = HandleUtility.WorldToGUIPoint(tickPos);

            GUIStyle timeStyle = new GUIStyle(GUI.skin.label);
            timeStyle.normal.textColor = Color.white;
            timeStyle.fontSize = 10;
            timeStyle.alignment = TextAnchor.MiddleRight;
            timeStyle.fontStyle = FontStyle.Bold;

            string timeLabel = $"{time:F0}s";

            // Background pour le label
            Vector2 labelSize = timeStyle.CalcSize(new GUIContent(timeLabel));
            Rect bgRect = new Rect(screenPos.x - labelSize.x - 10, screenPos.y - 10, labelSize.x + 5, 20);
            GUI.backgroundColor = new Color(0, 0, 0, 0.7f);
            GUI.Box(bgRect, "");

            // Label
            GUI.Label(new Rect(screenPos.x - labelSize.x - 8, screenPos.y - 8, labelSize.x, 16), timeLabel, timeStyle);

            Handles.EndGUI();
        }

        // Flèche en haut pour indiquer le sens du temps
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

        // Stats du level
        GUIStyle statsStyle = new GUIStyle(GUI.skin.box);
        statsStyle.fontSize = 11;
        statsStyle.normal.textColor = Color.white;
        statsStyle.alignment = TextAnchor.MiddleLeft;
        statsStyle.padding = new RectOffset(10, 10, 5, 5);

        string stats = $"Enemies: {level.GetTotalEnemies()} | XP: {level.GetTotalXP()} | Duration: {level.GetLevelDuration():F1}s | Level {level.GetStartLevel()} → {level.GetEndLevel()}";
        GUI.Box(new Rect(10, 45, 600, 25), stats, statsStyle);

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