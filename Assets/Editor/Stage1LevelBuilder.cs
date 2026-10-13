using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

namespace Laliphat
{
    [InitializeOnLoad]
    public static class Stage1LevelBuilder
    {
        private const string BUILT_KEY = "Laliphat_Stage1_Built_v1";

        static Stage1LevelBuilder()
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

        [MenuItem("Laliphat/Build Stage 1 Level")]
        public static void BuildLevel()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Stage 1.unity");

            // Remove previous LevelRoot if any
            var existingRoot = GameObject.Find("LevelRoot");
            if (existingRoot != null)
            {
                Object.DestroyImmediate(existingRoot);
            }

            // Remove old Ground plane if any
            var oldGround = GameObject.Find("Ground");
            if (oldGround != null)
            {
                Object.DestroyImmediate(oldGround);
            }

            GameObject levelRoot = new GameObject("LevelRoot");

            // Materials
            Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Starter Assets/Sample/Environment/Art/Materials/GridOrange_01_Mat.mat");
            if (floorMat == null) floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Starter Assets/Sample/Environment/Art/Materials/GridBlue_01_Mat.mat");

            Material pathMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Starter Assets/Sample/Environment/Art/Materials/GridBlue_01_Mat.mat");

            // 1. Central Start Platform (10 x 1 x 10)
            CreatePlatform(levelRoot.transform, "StartPlatform", new Vector3(0, -0.5f, 0), new Vector3(10, 1, 10), floorMat);

            // 2. Narrow Bridge North (2.2 x 1 x 14)
            CreatePlatform(levelRoot.transform, "NarrowPath_North", new Vector3(0, -0.5f, 12), new Vector3(2.2f, 1, 14), pathMat);

            // 3. North Platform (8 x 1 x 8)
            CreatePlatform(levelRoot.transform, "NorthPlatform", new Vector3(0, -0.5f, 23), new Vector3(8, 1, 8), floorMat);

            // 4. Narrow East Path (10 x 1 x 2)
            CreatePlatform(levelRoot.transform, "NarrowPath_East", new Vector3(10, -0.5f, 0), new Vector3(10, 1, 2), pathMat);

            // 5. East Platform (8 x 1 x 8)
            CreatePlatform(levelRoot.transform, "EastPlatform", new Vector3(19, -0.5f, 0), new Vector3(8, 1, 8), floorMat);

            // 6. Narrow West Path (10 x 1 x 2)
            CreatePlatform(levelRoot.transform, "NarrowPath_West", new Vector3(-10, -0.5f, 0), new Vector3(10, 1, 2), pathMat);

            // 7. West Platform (8 x 1 x 8)
            CreatePlatform(levelRoot.transform, "WestPlatform", new Vector3(-19, -0.5f, 0), new Vector3(8, 1, 8), floorMat);

            // 8. Diagonal Floating Islands North-East
            CreatePlatform(levelRoot.transform, "Island_NE_1", new Vector3(9, -0.5f, 9), new Vector3(3, 1, 3), pathMat);
            CreatePlatform(levelRoot.transform, "Island_NE_2", new Vector3(14, -0.5f, 14), new Vector3(4, 1, 4), floorMat);

            // 9. Diagonal Floating Islands North-West
            CreatePlatform(levelRoot.transform, "Island_NW_1", new Vector3(-9, -0.5f, 9), new Vector3(3, 1, 3), pathMat);
            CreatePlatform(levelRoot.transform, "Island_NW_2", new Vector3(-14, -0.5f, 14), new Vector3(4, 1, 4), floorMat);

            // Item Prefabs
            GameObject scoreBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_ScoreBonus.prefab");
            GameObject scorePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_ScorePenalty.prefab");
            GameObject timeBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_TimeBonus.prefab");
            GameObject timePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Item_TimePenalty.prefab");

            GameObject itemsRoot = new GameObject("Items_And_Obstacles");
            itemsRoot.transform.SetParent(levelRoot.transform);

            // Start Platform items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-3, 0.5f, -3));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(3, 0.5f, -3));
            SpawnItem(timeBonusPrefab, itemsRoot.transform, new Vector3(0, 0.5f, -3));

            // Narrow North Bridge items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(0, 0.5f, 7));
            SpawnItem(scorePenaltyPrefab, itemsRoot.transform, new Vector3(0, 0.5f, 11)); // Obstacle in narrow path!
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(0, 0.5f, 15));

            // North Platform items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-2, 0.5f, 21));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(2, 0.5f, 21));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-2, 0.5f, 25));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(2, 0.5f, 25));
            SpawnItem(timeBonusPrefab, itemsRoot.transform, new Vector3(0, 0.5f, 23));
            SpawnItem(timePenaltyPrefab, itemsRoot.transform, new Vector3(0, 0.5f, 26));

            // Narrow East Bridge items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(7, 0.5f, 0));
            SpawnItem(scorePenaltyPrefab, itemsRoot.transform, new Vector3(10, 0.5f, 0)); // Obstacle in narrow path!
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(13, 0.5f, 0));

            // East Platform items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(17, 0.5f, -2));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(21, 0.5f, -2));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(17, 0.5f, 2));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(21, 0.5f, 2));
            SpawnItem(timeBonusPrefab, itemsRoot.transform, new Vector3(19, 0.5f, 0));
            SpawnItem(timePenaltyPrefab, itemsRoot.transform, new Vector3(19, 0.5f, 3));

            // Narrow West Bridge items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-7, 0.5f, 0));
            SpawnItem(scorePenaltyPrefab, itemsRoot.transform, new Vector3(-10, 0.5f, 0)); // Obstacle in narrow path!
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-13, 0.5f, 0));

            // West Platform items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-17, 0.5f, -2));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-21, 0.5f, -2));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-17, 0.5f, 2));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-21, 0.5f, 2));
            SpawnItem(timeBonusPrefab, itemsRoot.transform, new Vector3(-19, 0.5f, 0));
            SpawnItem(scorePenaltyPrefab, itemsRoot.transform, new Vector3(-19, 0.5f, 3));

            // Islands items:
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(9, 0.5f, 9));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(14, 0.5f, 14));
            SpawnItem(timeBonusPrefab, itemsRoot.transform, new Vector3(13, 0.5f, 15));

            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-9, 0.5f, 9));
            SpawnItem(scoreBonusPrefab, itemsRoot.transform, new Vector3(-14, 0.5f, 14));
            SpawnItem(timeBonusPrefab, itemsRoot.transform, new Vector3(-13, 0.5f, 15));

            // Ensure CapsulePlayer is at (0, 1.1, 0)
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(0, 1.1f, 0);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Laliphat] Stage 1 Level with ProBuilder platforms and Prefabs built and saved!");
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

        private static void SpawnItem(GameObject prefab, Transform parent, Vector3 pos)
        {
            if (prefab == null) return;
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.transform.position = pos;
        }
    }
}
