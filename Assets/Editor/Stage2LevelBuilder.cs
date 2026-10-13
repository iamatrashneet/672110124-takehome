using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;
using TMPro;

namespace Laliphat
{
    [InitializeOnLoad]
    public static class Stage2LevelBuilder
    {
        private const string BUILT_KEY = "Laliphat_Stage2_Built_v1";

        static Stage2LevelBuilder()
        {
            EditorApplication.delayCall += CheckAndBuild;
        }

        private static void CheckAndBuild()
        {
            if (!SessionState.GetBool(BUILT_KEY, false))
            {
                SessionState.SetBool(BUILT_KEY, true);
                BuildLevel();
            }
        }

        [MenuItem("Laliphat/Build Stage 2 Level")]
        public static void BuildLevel()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Stage 2.unity");

            // Clean previous LevelRoot and Player if any
            var existingLevelRoot = GameObject.Find("LevelRoot");
            if (existingLevelRoot != null) Object.DestroyImmediate(existingLevelRoot);

            var existingPlayer = GameObject.FindGameObjectWithTag("Player");
            if (existingPlayer != null) Object.DestroyImmediate(existingPlayer);

            var existingGameplay = GameObject.Find("GameplayManager");
            if (existingGameplay != null) Object.DestroyImmediate(existingGameplay);

            // Clean old temporary test buttons (WinButton, LoseButton)
            var winBtn = GameObject.Find("WinButton");
            if (winBtn != null) Object.DestroyImmediate(winBtn);
            var loseBtn = GameObject.Find("LoseButton");
            if (loseBtn != null) Object.DestroyImmediate(loseBtn);

            GameObject levelRoot = new GameObject("LevelRoot");

            // Materials
            Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Starter Assets/Sample/Environment/Art/Materials/GridBlue_01_Mat.mat");
            Material accentMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Starter Assets/Sample/Environment/Art/Materials/GridOrange_01_Mat.mat");

            // 1. ProBuilder Platforms (> 50 sqm):
            // Main Plaza (14m x 1m x 14m = 196 sq. meters)
            CreatePlatform(levelRoot.transform, "MainPlaza", new Vector3(0, -0.5f, 0), new Vector3(14, 1, 14), floorMat);

            // Upper Deck (8m x 1m x 8m = 64 sq. meters) elevated at Y = 1.5
            CreatePlatform(levelRoot.transform, "UpperDeck", new Vector3(0, 1f, 16), new Vector3(8, 1, 8), accentMat);

            // Connecting Ramp / Bridge to Upper Deck (3m x 1m x 10m)
            CreatePlatform(levelRoot.transform, "Ramp_To_UpperDeck", new Vector3(0, 0.25f, 9), new Vector3(3, 1, 6), floorMat);

            // East Wing Platform (7m x 1m x 7m = 49 sq. meters)
            CreatePlatform(levelRoot.transform, "EastWing", new Vector3(15, -0.5f, 0), new Vector3(7, 1, 7), accentMat);
            CreatePlatform(levelRoot.transform, "EastBridge", new Vector3(9, -0.5f, 0), new Vector3(5, 1, 2.5f), floorMat);

            // West Wing Platform (7m x 1m x 7m = 49 sq. meters)
            CreatePlatform(levelRoot.transform, "WestWing", new Vector3(-15, -0.5f, 0), new Vector3(7, 1, 7), accentMat);
            CreatePlatform(levelRoot.transform, "WestBridge", new Vector3(-9, -0.5f, 0), new Vector3(5, 1, 2.5f), floorMat);

            // 2. Setup Humanoid Player:
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Starter Assets/Runtime/ThirdPersonController/Prefabs/PlayerArmature.prefab");
            GameObject playerInstance = null;
            if (playerPrefab != null)
            {
                playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                playerInstance.name = "HumanoidPlayer";
                playerInstance.transform.position = new Vector3(0, 0.05f, 0);
                playerInstance.tag = "Player";

                // Swap / assign Ch02_nonPBR Avatar
                var animator = playerInstance.GetComponent<Animator>();
                var ch02Model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PracticalExamResources/Character Models/Ch02_nonPBR.fbx");
                if (animator != null && ch02Model != null)
                {
                    // Find Avatar from Ch02_nonPBR
                    var avatar = AssetDatabase.LoadAssetAtPath<Avatar>("Assets/PracticalExamResources/Character Models/Ch02_nonPBR.fbx");
                    if (avatar != null)
                    {
                        animator.avatar = avatar;
                    }

                    // Instantiate Ch02 visual mesh under player if geometry is replaced
                    var existingGeom = playerInstance.transform.Find("Geometry");
                    if (existingGeom != null)
                    {
                        existingGeom.gameObject.SetActive(false);
                    }

                    var ch02Visual = (GameObject)PrefabUtility.InstantiatePrefab(ch02Model, playerInstance.transform);
                    ch02Visual.name = "Ch02_Visual";
                    ch02Visual.transform.localPosition = Vector3.zero;
                    ch02Visual.transform.localRotation = Quaternion.identity;
                }

                // Add PlayerInteraction component
                var interaction = playerInstance.GetComponent<PlayerInteraction>();
                if (interaction == null)
                {
                    interaction = playerInstance.AddComponent<PlayerInteraction>();
                }
            }

            // 3. Setup Main Camera Tracking
            var mainCam = Camera.main;
            if (mainCam != null && playerInstance != null)
            {
                var camFollow = mainCam.GetComponent<CameraFollow>();
                if (camFollow == null)
                {
                    camFollow = mainCam.gameObject.AddComponent<CameraFollow>();
                }
            }

            // 4. Setup UI on Canvas (HUD Overlay + Interaction Prompt)
            var canvas = Object.FindFirstObjectByType<Canvas>();
            TextMeshProUGUI hudTmp = null;
            TextMeshProUGUI promptTmp = null;

            if (canvas != null)
            {
                // HUD Overlay (Top-Left)
                var existingHud = canvas.transform.Find("HUD_Overlay");
                if (existingHud == null)
                {
                    var hudObj = new GameObject("HUD_Overlay");
                    hudObj.transform.SetParent(canvas.transform, false);
                    var rt = hudObj.AddComponent<RectTransform>();
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(0, 1);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = new Vector2(220, -80);
                    rt.sizeDelta = new Vector2(350, 120);

                    hudTmp = hudObj.AddComponent<TextMeshProUGUI>();
                    hudTmp.fontSize = 28;
                    hudTmp.color = Color.white;
                    hudTmp.text = "Time: 30s\nScore: 0";
                }
                else
                {
                    hudTmp = existingHud.GetComponent<TextMeshProUGUI>();
                }

                // Interaction Prompt (Center-Bottom)
                var existingPrompt = canvas.transform.Find("Interaction_Prompt");
                if (existingPrompt == null)
                {
                    var promptObj = new GameObject("Interaction_Prompt");
                    promptObj.transform.SetParent(canvas.transform, false);
                    var rt = promptObj.AddComponent<RectTransform>();
                    rt.anchorMin = new Vector2(0.5f, 0.2f);
                    rt.anchorMax = new Vector2(0.5f, 0.2f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = Vector2.zero;
                    rt.sizeDelta = new Vector2(400, 60);

                    promptTmp = promptObj.AddComponent<TextMeshProUGUI>();
                    promptTmp.fontSize = 26;
                    promptTmp.alignment = TextAlignmentOptions.Center;
                    promptTmp.color = new Color(1f, 0.9f, 0.2f, 1f); // Vibrant gold
                    promptTmp.text = "Press [E] to Interact";
                    promptObj.SetActive(false);
                }
                else
                {
                    promptTmp = existingPrompt.GetComponent<TextMeshProUGUI>();
                }
            }

            // 5. Setup GameplayManager in Stage 2
            GameObject gmObj = new GameObject("GameplayManager");
            var gm = gmObj.AddComponent<GameplayManager>();
            var sl = gmObj.AddComponent<SceneLoader>();

            // 6. Spawn Stage 2 Items (with requireInteraction = true for Stage 2!)
            GameObject scoreBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_ScoreBonus.prefab");
            GameObject scorePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_ScorePenalty.prefab");
            GameObject timeBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_TimeBonus.prefab");
            GameObject timePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_TimePenalty.prefab");

            GameObject itemsRoot = new GameObject("Items_And_Obstacles");
            itemsRoot.transform.SetParent(levelRoot.transform);

            // Spawn 24 score bonus items (= 12,000 pts > 10,000 pts win threshold)
            // Main Plaza items:
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-4, 0.5f, -4));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(4, 0.5f, -4));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-4, 0.5f, 4));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(4, 0.5f, 4));
            SpawnStage2Item(timeBonusPrefab, itemsRoot.transform, new Vector3(0, 0.5f, 3));
            SpawnStage2Item(scorePenaltyPrefab, itemsRoot.transform, new Vector3(0, 0.5f, -3));

            // Upper Deck items (elevated platform):
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-2, 2f, 14));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(2, 2f, 14));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-2, 2f, 18));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(2, 2f, 18));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(0, 2f, 16));
            SpawnStage2Item(timeBonusPrefab, itemsRoot.transform, new Vector3(0, 2f, 19));
            SpawnStage2Item(timePenaltyPrefab, itemsRoot.transform, new Vector3(3, 2f, 16));

            // East Wing items:
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(9, 0.5f, 0));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(13, 0.5f, -2));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(17, 0.5f, -2));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(13, 0.5f, 2));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(17, 0.5f, 2));
            SpawnStage2Item(timeBonusPrefab, itemsRoot.transform, new Vector3(15, 0.5f, 0));
            SpawnStage2Item(scorePenaltyPrefab, itemsRoot.transform, new Vector3(15, 0.5f, 3));

            // West Wing items:
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-9, 0.5f, 0));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-13, 0.5f, -2));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-17, 0.5f, -2));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-13, 0.5f, 2));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-17, 0.5f, 2));
            SpawnStage2Item(timeBonusPrefab, itemsRoot.transform, new Vector3(-15, 0.5f, 0));
            SpawnStage2Item(timePenaltyPrefab, itemsRoot.transform, new Vector3(-15, 0.5f, 3));

            // Extra Plaza items for complete score distribution:
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(-5, 0.5f, 0));
            SpawnStage2Item(scoreBonusPrefab, itemsRoot.transform, new Vector3(5, 0.5f, 0));
            SpawnStage2Item(timeBonusPrefab, itemsRoot.transform, new Vector3(-2, 0.5f, -5));
            SpawnStage2Item(timeBonusPrefab, itemsRoot.transform, new Vector3(2, 0.5f, -5));
            SpawnStage2Item(scorePenaltyPrefab, itemsRoot.transform, new Vector3(0, 0.5f, 7));

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Laliphat] Stage 2 Humanoid, ProBuilder Level (>50sqm), and Interaction items built successfully!");
        }

        private static GameObject CreatePlatform(Transform parent, string name, Vector3 pos, Vector3 size, Material mat)
        {
            ProBuilderMesh pbMesh = ShapeFactory.Instantiate(typeof(Cube));
            pbMesh.name = name;
            pbMesh.transform.SetParent(parent);
            pbMesh.transform.position = pos;
            pbMesh.transform.localScale = size;

            if (mat != null)
            {
                var renderer = pbMesh.GetComponent<MeshRenderer>();
                if (renderer != null) renderer.sharedMaterial = mat;
            }

            var collider = pbMesh.gameObject.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = pbMesh.gameObject.AddComponent<BoxCollider>();
            }

            return pbMesh.gameObject;
        }

        private static void SpawnStage2Item(GameObject prefab, Transform parent, Vector3 pos)
        {
            if (prefab == null) return;
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.transform.position = pos;

            var stageItem = instance.GetComponent<StageItem>();
            if (stageItem != null)
            {
                stageItem.RequireInteraction = true; // Set Stage 2 items to require interaction!
            }
        }
    }
}
