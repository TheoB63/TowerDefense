using UnityEngine;
using UnityEditor;
using System.Linq;

// CE SCRIPT VA DANS UN DOSSIER "Editor" à la racine de ton projet
// Il customise l'affichage du LevelConfig dans l'Inspector

[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : Editor
{
    private bool showTimeline = true;

    public override void OnInspectorGUI()
    {
        LevelConfig level = (LevelConfig)target;

        // ========== BOX AVEC LES STATS DU LEVEL ==========
        EditorGUILayout.BeginVertical("box");
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;
        titleStyle.normal.textColor = Color.cyan;

        EditorGUILayout.LabelField("📊 LEVEL STATISTICS", titleStyle);
        EditorGUILayout.Space(5);

        // Stats principales
        EditorGUILayout.LabelField($"Total Enemies: {level.GetTotalEnemies()} ({level.GetMobCount()} mobs, {level.GetBossCount()} bosses)", EditorStyles.label);
        EditorGUILayout.LabelField($"Total XP Reward: {level.GetTotalXP()} XP", EditorStyles.label);
        EditorGUILayout.LabelField($"Level Duration: ~{level.GetLevelDuration():F1} seconds", EditorStyles.label);

        EditorGUILayout.Space(10);

        // Calcul de progression de level
        int startLevel = level.GetStartLevel();
        int endLevel = level.GetEndLevel();

        GUIStyle levelStyle = new GUIStyle(EditorStyles.boldLabel);
        levelStyle.normal.textColor = Color.yellow;

        EditorGUILayout.LabelField($"🎯 Player Level Progression:", levelStyle);
        EditorGUILayout.LabelField($"   Start: Level {startLevel} ({level.baseXP} XP)");
        EditorGUILayout.LabelField($"   End:   Level {endLevel} ({level.baseXP + level.GetTotalXP()} XP)");

        if (startLevel == endLevel)
        {
            EditorGUILayout.HelpBox($"⚠️ Player stays at Level {startLevel} during this level", MessageType.Warning);
        }
        else if (endLevel - startLevel > 2)
        {
            EditorGUILayout.HelpBox($"✨ Player gains {endLevel - startLevel} levels! Might be too much.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox($"✅ Balanced progression ({endLevel - startLevel} level gain)", MessageType.Info);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // ========== TIMELINE DES ENNEMIS ==========
        showTimeline = EditorGUILayout.Foldout(showTimeline, "⏱️ ENEMIES TIMELINE", true);

        if (showTimeline && level.enemies.Count > 0)
        {
            EditorGUILayout.BeginVertical("box");

            // Trier par temps de spawn pour l'affichage
            var sortedEnemies = level.enemies.OrderBy(e => e.spawnTime).ToList();

            foreach (var enemy in sortedEnemies)
            {
                GameObject prefab = enemy.GetPrefab(level);
                string slotName = enemy.GetSlotName(level);
                string enemyName = prefab ? prefab.name : "⚠️ MISSING";
                string timeStr = FormatTime(enemy.spawnTime);
                float xp = enemy.GetXPValue(level);
                Color previewColor = enemy.GetPreviewColor(level);
                bool isBoss = enemy.IsBoss();

                EditorGUILayout.BeginHorizontal("helpbox");

                // Petit carré de couleur
                Rect colorRect = EditorGUILayout.GetControlRect(GUILayout.Width(20), GUILayout.Height(20));
                EditorGUI.DrawRect(colorRect, previewColor);

                EditorGUILayout.LabelField($"{timeStr}", GUILayout.Width(60));
                EditorGUILayout.LabelField($"→ {slotName}", GUILayout.Width(70));
                EditorGUILayout.LabelField($"({enemyName})", GUILayout.Width(150));
                EditorGUILayout.LabelField($"{xp} XP", GUILayout.Width(60));

                if (isBoss)
                {
                    GUIStyle bossStyle = new GUIStyle(EditorStyles.miniLabel);
                    bossStyle.normal.textColor = Color.red;
                    bossStyle.fontStyle = FontStyle.Bold;
                    EditorGUILayout.LabelField("💀 BOSS", bossStyle, GUILayout.Width(60));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        else if (level.enemies.Count == 0)
        {
            EditorGUILayout.HelpBox("No enemies configured yet. Add enemies below!", MessageType.Warning);
        }

        EditorGUILayout.Space(10);

        // ========== BOUTONS UTILES ==========
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Sort by Spawn Time"))
        {
            Undo.RecordObject(level, "Sort Enemies");
            level.enemies = level.enemies.OrderBy(e => e.spawnTime).ToList();
            EditorUtility.SetDirty(level);
        }

        if (GUILayout.Button("Show Preview in Scene"))
        {
            // Sélectionner l'objet pour que SceneView soit actif
            Selection.activeObject = level;
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Clear All Enemies"))
        {
            if (EditorUtility.DisplayDialog("Clear Enemies",
                "Are you sure you want to remove all enemies?", "Yes", "Cancel"))
            {
                Undo.RecordObject(level, "Clear Enemies");
                level.enemies.Clear();
                EditorUtility.SetDirty(level);
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Inspector par défaut pour modifier les valeurs
        DrawDefaultInspector();
    }

    string FormatTime(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);

        if (mins > 0)
            return $"{mins}:{secs:D2}";
        else
            return $"{secs}s";
    }
}