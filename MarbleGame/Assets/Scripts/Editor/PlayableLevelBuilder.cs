#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MarbleGame.Editor
{
    public static class PlayableLevelBuilder
    {
        private const string ScenePath = "Assets/Scenes/GlassMachine_Prototype.unity";
        private const string MaterialRoot = "Assets/Generated/Materials";
        private static Material metal;
        private static Material darkMetal;
        private static Material track;
        private static Material accent;
        private static Material marble;
        private static Material glass;
        private static Material warning;
        private static Material store;
        private static Material shelf;
        private static PhysicsMaterial trackPhysics;
        private static PhysicsMaterial marblePhysics;

        [MenuItem("MarbleGame/Build First Playable Level")]
        public static void BuildPlayableLevel()
        {
            EnsureFolders();
            LoadMaterials();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ConfigureLighting();
            BuildRetailEnvironment();
            BuildGlassMachine();
            BuildCourseAndPuzzles();
            BuildPlayerAndCamera();
            BuildInterface();
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("MarbleGame: rebuilt the Glass Run first playable around gravity-driven marble physics.");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Generated")) AssetDatabase.CreateFolder("Assets", "Generated");
            if (!AssetDatabase.IsValidFolder(MaterialRoot)) AssetDatabase.CreateFolder("Assets/Generated", "Materials");
        }

        private static void LoadMaterials()
        {
            metal = MakeMaterial("Machine_Metal", new Color(0.24f, 0.29f, 0.34f), 0.84f, 0.72f);
            darkMetal = MakeMaterial("Machine_DarkMetal", new Color(0.018f, 0.027f, 0.04f), 0.92f, 0.48f);
            track = MakeMaterial("Track_Ceramic", new Color(0.10f, 0.42f, 0.54f), 0.34f, 0.64f);
            accent = MakeMaterial("Machine_Amber", new Color(0.95f, 0.28f, 0.045f), 0.28f, 0.5f);
            marble = MakeMaterial("Player_Marble", new Color(0.035f, 0.42f, 0.9f), 0.18f, 0.88f);
            warning = MakeMaterial("Warning_Orange", new Color(1f, 0.09f, 0.015f), 0.12f, 0.56f);
            store = MakeMaterial("Store_Atmosphere", new Color(0.07f, 0.085f, 0.10f), 0.2f, 0.3f);
            shelf = MakeMaterial("Store_Shelf", new Color(0.18f, 0.21f, 0.23f), 0.62f, 0.5f);
            trackPhysics = MakePhysicsMaterial("Track_Slope", 0f, 0f, 0f, PhysicsMaterialCombine.Minimum);
            marblePhysics = MakePhysicsMaterial("Marble_Rolling", 0.16f, 0.2f, 0.12f, PhysicsMaterialCombine.Multiply);
            glass = MakeMaterial("Machine_Glass", new Color(0.34f, 0.78f, 0.96f, 0.13f), 0.05f, 0.92f);
            glass.SetFloat("_Surface", 1f);
            glass.SetFloat("_Blend", 0f);
            glass.SetFloat("_AlphaClip", 0f);
            glass.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            glass.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            glass.SetInt("_ZWrite", 0);
            glass.renderQueue = 3000;
        }

        private static Material MakeMaterial(string name, Color color, float metallic, float smoothness)
        {
            string path = MaterialRoot + "/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color;
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Smoothness", smoothness);
            return material;
        }

        private static PhysicsMaterial MakePhysicsMaterial(string name, float dynamicFriction, float staticFriction,
            float bounciness, PhysicsMaterialCombine frictionCombine)
        {
            string path = MaterialRoot + "/" + name + ".physicMaterial";
            PhysicsMaterial material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
            if (material == null)
            {
                material = new PhysicsMaterial(name);
                AssetDatabase.CreateAsset(material, path);
            }
            material.dynamicFriction = dynamicFriction;
            material.staticFriction = staticFriction;
            material.bounciness = bounciness;
            material.frictionCombine = frictionCombine;
            material.bounceCombine = PhysicsMaterialCombine.Minimum;
            return material;
        }

        private static GameObject Empty(string name, Transform parent = null)
        {
            GameObject go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent);
            return go;
        }

        private static GameObject Cube(string name, Vector3 position, Vector3 scale, Material material, Transform parent = null, Quaternion rotation = default)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            if (parent != null) go.transform.SetParent(parent);
            go.transform.SetPositionAndRotation(position, rotation == default ? Quaternion.identity : rotation);
            go.transform.localScale = scale;
            if (material != null) go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static GameObject Cylinder(string name, Vector3 position, Vector3 scale, Material material, Transform parent = null, Quaternion rotation = default)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            if (parent != null) go.transform.SetParent(parent);
            go.transform.SetPositionAndRotation(position, rotation == default ? Quaternion.identity : rotation);
            go.transform.localScale = scale;
            if (material != null) go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static void ConfigureLighting()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.09f, 0.12f, 0.16f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.07f, 0.085f, 0.10f);
            RenderSettings.fogDensity = 0.0045f;

            GameObject key = new GameObject("Store_Softbox_Key");
            Light directional = key.AddComponent<Light>();
            directional.type = LightType.Directional;
            directional.intensity = 1.4f;
            directional.color = new Color(0.82f, 0.9f, 1f);
            directional.shadows = LightShadows.Soft;
            key.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private static void BuildRetailEnvironment()
        {
            Transform root = Empty("Giant_Generic_Retail_Environment").transform;
            Cube("Store_Floor", new Vector3(0f, -42f, 25f), new Vector3(180f, 1f, 220f), store, root);
            Cube("Store_Ceiling", new Vector3(0f, 53f, 25f), new Vector3(180f, 1f, 220f), darkMetal, root);

            for (int side = -1; side <= 1; side += 2)
            {
                for (int i = 0; i < 8; i++)
                {
                    float z = -35f + i * 17f;
                    Vector3 shelfPosition = new Vector3(side * 63f, -12f, z);
                    Cube("Distant_Shelf_Block", shelfPosition, new Vector3(16f, 28f, 5f), store, root);
                    for (int level = 0; level < 5; level++)
                        Cube("Shelf_Front_Beam", new Vector3(side * 54f, -34f + level * 6.5f, z), new Vector3(1.1f, 0.22f, 7.5f), shelf, root);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                float z = -45f + i * 16f;
                Cylinder("Retail_Structural_Column", new Vector3(-49f, 5f, z), new Vector3(2f, 48f, 2f), shelf, root);
                Cylinder("Retail_Structural_Column", new Vector3(49f, 5f, z), new Vector3(2f, 48f, 2f), shelf, root);
                Cube("Overhead_Light_Bank", new Vector3(-29f, 45f, z), new Vector3(10f, 0.25f, 1.4f), accent, root);
                Cube("Overhead_Light_Bank", new Vector3(29f, 45f, z), new Vector3(10f, 0.25f, 1.4f), accent, root);
                GameObject practical = new GameObject("Store_Overhead_Practical");
                practical.transform.SetParent(root);
                practical.transform.position = new Vector3(0f, 43f, z);
                Light point = practical.AddComponent<Light>();
                point.type = LightType.Point;
                point.color = new Color(0.62f, 0.82f, 1f);
                point.intensity = 2.4f;
                point.range = 32f;
                point.shadows = LightShadows.None;
            }

            for (int i = 0; i < 6; i++)
            {
                GameObject cart = Empty("Distant_Shopping_Cart_Silhouette", root);
                cart.transform.position = new Vector3(-34f + i * 13f, -38f, 78f - i * 9f);
                Cube("Cart_Basket", cart.transform.position + Vector3.up * 2f, new Vector3(4f, 2f, 2f), shelf, cart.transform);
                Cylinder("Cart_Handle", cart.transform.position + new Vector3(0f, 4f, -1.2f), new Vector3(0.12f, 1.6f, 0.12f), metal, cart.transform, Quaternion.Euler(90f, 0f, 0f));
            }
        }

        private static void BuildGlassMachine()
        {
            Transform root = Empty("Glass_Marble_Machine").transform;
            Vector3 center = new Vector3(0f, 5f, 25f);
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Thick_Fogged_Glass_Sphere";
            sphere.transform.SetParent(root);
            sphere.transform.position = center;
            sphere.transform.localScale = Vector3.one * 120f;
            sphere.GetComponent<Renderer>().sharedMaterial = glass;
            Object.DestroyImmediate(sphere.GetComponent<Collider>());

            Cylinder("Gumball_Neck", new Vector3(0f, -42f, 25f), new Vector3(15f, 4f, 15f), darkMetal, root);
            Cylinder("Gumball_Base", new Vector3(0f, -47f, 25f), new Vector3(24f, 1.2f, 24f), darkMetal, root);
            Cylinder("Gumball_Base_Rim", new Vector3(0f, -44.7f, 25f), new Vector3(21f, 0.7f, 21f), accent, root);

            for (int i = 0; i < 16; i++)
            {
                float a0 = i / 16f * Mathf.PI * 2f;
                float a1 = (i + 1) / 16f * Mathf.PI * 2f;
                Vector3 a = center + new Vector3(Mathf.Cos(a0) * 60.8f, 0f, Mathf.Sin(a0) * 60.8f);
                Vector3 b = center + new Vector3(Mathf.Cos(a1) * 60.8f, 0f, Mathf.Sin(a1) * 60.8f);
                Beam("Sphere_Equator_Frame", a, b, 0.04f, metal, root);
            }
            // Keep the sphere silhouette readable from the gameplay camera. The earlier crown ribs
            // intersected the intake camera frustum because the marble starts close to the shell.
        }

        private static void BuildCourseAndPuzzles()
        {
            Transform root = Empty("Engineered_Marble_Course_And_Puzzles").transform;

            List<Vector3> opening = new List<Vector3>
            {
                new Vector3(0f, 28f, -14f), new Vector3(0f, 26f, -7f), new Vector3(0f, 24f, 1f),
                new Vector3(0f, 22f, 9f), new Vector3(0f, 20f, 17f)
            };
            BuildTrackPath("Intake_Gravity_Run", opening, 8f, root);
            AddMarker("01  /  INTAKE", new Vector3(0f, 23f, 2f), root);

            List<Vector3> falseCenter = new List<Vector3>
            {
                new Vector3(0f, 20f, 17f), new Vector3(0f, 18f, 25f), new Vector3(0f, 16.5f, 33f)
            };
            BuildTrackPath("Obvious_Center_Dead_End", falseCenter, 5.2f, root);
            AddFailure("Center_Route_Trap", new Vector3(0f, 17.1f, 31.2f), new Vector3(4.8f, 3f, 2.8f), "THE CENTER LINE WAS THE LESSON.", root);

            List<Vector3> correctBranch = new List<Vector3>
            {
                new Vector3(0f, 20f, 17f), new Vector3(-6.4f, 18.3f, 24f), new Vector3(-7.8f, 16.4f, 31f),
                new Vector3(-5.5f, 15f, 36f), new Vector3(0f, 14f, 40f)
            };
            BuildTrackPath("Learned_Left_Branch", correctBranch, 7.4f, root);
            AddMarker("02  /  THE SPLIT", new Vector3(-5.8f, 17f, 28f), root);

            List<Vector3> middle = new List<Vector3>
            {
                new Vector3(0f, 14f, 40f), new Vector3(6.5f, 13f, 46f), new Vector3(7.8f, 12f, 51.5f),
                new Vector3(6f, 11f, 56f), new Vector3(0f, 10f, 59f)
            };
            BuildTrackPath("Banked_Momentum_S", middle, 7.2f, root);
            AddMarker("03  /  MOMENTUM TAX", new Vector3(6f, 12.5f, 45f), root);

            GameObject sweeper = Cube("Timed_Sweeper", new Vector3(7f, 14f, 47f), new Vector3(0.75f, 3f, 2.6f), warning, root);
            OscillatingGate sweeperMotion = sweeper.AddComponent<OscillatingGate>();
            sweeperMotion.Configure(new Vector3(-4.5f, 0f, 0f), 1.05f, 0.7f);

            GameObject plate = Cube("Pressure_Plate", new Vector3(6f, 11.75f, 51.4f), new Vector3(5.5f, 0.18f, 2.8f), accent, root);
            BoxCollider plateTrigger = plate.AddComponent<BoxCollider>();
            plateTrigger.isTrigger = true;
            plateTrigger.size = new Vector3(1f, 6f, 1f);
            GameObject pressureGateObject = Cube("Pressure_Riser", new Vector3(2.4f, 13f, 54.2f), new Vector3(5f, 1.2f, 0.6f), warning, root);
            PressureGate pressureGate = plate.AddComponent<PressureGate>();
            pressureGate.Configure(pressureGateObject.transform);
            AddMarker("04  /  WEIGHT OF EVIDENCE", new Vector3(5.6f, 12f, 52f), root);

            List<Vector3> finalRun = new List<Vector3>
            {
                new Vector3(0f, 10f, 59f), new Vector3(-5f, 8.5f, 63f), new Vector3(-7.5f, 7.2f, 67f),
                new Vector3(-6f, 5.3f, 72f), new Vector3(0f, 4.2f, 76f), new Vector3(0f, 5f, 82f)
            };
            BuildTrackPath("Controlled_Release", finalRun, 7.4f, root);
            AddMarker("05  /  CONTROLLED DROP", new Vector3(-6f, 6.8f, 66f), root);

            List<Vector3> falseExit = new List<Vector3>
            {
                new Vector3(0f, 10f, 59f), new Vector3(7f, 9f, 63f), new Vector3(8f, 7.5f, 68f)
            };
            BuildTrackPath("False_Exit_Branch", falseExit, 4.7f, root);
            AddFailure("False_Exit_Trap", new Vector3(8f, 7.6f, 67.5f), new Vector3(4f, 3f, 2.5f), "THE EXIT WAS A RESET.", root);

            GameObject finalGate = Cube("Final_Release_Gate", new Vector3(0f, 7.1f, 77f), new Vector3(5.6f, 2.6f, 0.65f), accent, root);
            OscillatingGate finalMotion = finalGate.AddComponent<OscillatingGate>();
            finalMotion.Configure(new Vector3(0f, 2.8f, 0f), 0.8f, 1.3f);
            GameObject finish = Empty("Machine_Exit", root);
            finish.transform.position = new Vector3(0f, 5f, 82f);
            BoxCollider finishCollider = finish.AddComponent<BoxCollider>();
            finishCollider.isTrigger = true;
            finishCollider.size = new Vector3(7f, 5f, 3f);
            finish.AddComponent<FinishZone>();
            Beam("Exit_Ring_Top", new Vector3(-4f, 6f, 82f), new Vector3(4f, 6f, 82f), 0.28f, accent, root);
            Object.DestroyImmediate(GameObject.Find("Exit_Ring_Top").GetComponent<Collider>());
            AddMarker("06  /  THE RELEASE", new Vector3(0f, 5f, 78f), root);

            GameObject ambience = Empty("Calm_Machine_Ambience", root);
            ambience.AddComponent<MachineAmbience>();
            GameObject bounds = Empty("Machine_Safety_Reset", root);
            bounds.AddComponent<OutOfBoundsReset>();
        }

        private static void BuildTrackPath(string name, List<Vector3> points, float width, Transform parent)
        {
            Transform root = Empty(name, parent).transform;
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 a = points[i];
                Vector3 b = points[i + 1];
                Vector3 delta = b - a;
                Quaternion rotation = Quaternion.LookRotation(delta.normalized, Vector3.up);
                Vector3 midpoint = (a + b) * 0.5f;
                float length = delta.magnitude + 0.7f;
                GameObject deck = Cube("Track_Deck", midpoint, new Vector3(width, 0.5f, length), track, root, rotation);
                deck.GetComponent<BoxCollider>().material = trackPhysics;
                Vector3 right = rotation * Vector3.right;
                Vector3 up = rotation * Vector3.up;
                bool openFork = name == "Learned_Left_Branch" && i == 0;
                bool centerFork = name == "Obvious_Center_Dead_End" && i == 0;
                if (!openFork)
                    Cube("Track_Rail", midpoint + right * (width * 0.5f - 0.12f) + up * 0.9f, new Vector3(0.3f, 1.6f, length), metal, root, rotation);
                if (!openFork && !centerFork)
                    Cube("Track_Rail", midpoint - right * (width * 0.5f - 0.12f) + up * 0.9f, new Vector3(0.3f, 1.6f, length), metal, root, rotation);
                if (i % 2 == 0) AddSupport(midpoint, up, root);
            }
            for (int i = 1; i < points.Count - 1; i++)
                Cube("Track_Junction", points[i], new Vector3(width * 0.9f, 0.45f, 1.8f), track, root,
                    Quaternion.LookRotation((points[i + 1] - points[i - 1]).normalized, Vector3.up));
        }

        private static void AddSupport(Vector3 point, Vector3 trackUp, Transform parent)
        {
            Vector3 bottom = new Vector3(point.x, -36f, point.z);
            Vector3 delta = point - bottom;
            Cylinder("Machine_Support", (point + bottom) * 0.5f, new Vector3(0.22f, delta.magnitude * 0.5f, 0.22f), darkMetal, parent,
                Quaternion.FromToRotation(Vector3.up, delta.normalized));
            Cylinder("Support_Foot", bottom, new Vector3(0.8f, 0.25f, 0.8f), metal, parent);
        }

        private static void AddFailure(string name, Vector3 position, Vector3 size, string message, Transform parent)
        {
            GameObject go = Cube(name, position, size, warning, parent);
            BoxCollider collider = go.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            PuzzleTrigger trigger = go.AddComponent<PuzzleTrigger>();
            trigger.Configure(message);
        }

        private static void AddMarker(string label, Vector3 position, Transform parent)
        {
            GameObject go = Empty("Section_Marker_" + label.Replace("/", "_"), parent);
            go.transform.position = position;
            BoxCollider collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(16f, 12f, 2.5f);
            go.AddComponent<SectionMarker>().Configure(label);
        }

        private static void BuildPlayerAndCamera()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            player.name = "Marble_Player";
            player.transform.position = new Vector3(0f, 30.2f, -12.5f);
            player.transform.localScale = Vector3.one * 1.7f;
            player.GetComponent<Renderer>().sharedMaterial = marble;
            SphereCollider sphereCollider = player.GetComponent<SphereCollider>();
            sphereCollider.material = marblePhysics;
            Rigidbody body = player.AddComponent<Rigidbody>();
            body.mass = 1.1f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            MarblePlayer marblePlayer = player.AddComponent<MarblePlayer>();
            player.AddComponent<MarbleAudio>();

            GameObject spawn = Empty("Marble_Spawn");
            spawn.transform.SetPositionAndRotation(player.transform.position, Quaternion.LookRotation(Vector3.forward));
            marblePlayer.SetSpawn(spawn.transform);

            GameObject camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            Camera cameraComponent = camera.AddComponent<Camera>();
            cameraComponent.fieldOfView = 64f;
            cameraComponent.nearClipPlane = 0.05f;
            camera.AddComponent<AudioListener>();
            camera.AddComponent<ThirdPersonCamera>();

            GameObject director = new GameObject("Game_Director");
            director.AddComponent<GameDirector>();
        }

        private static void BuildInterface()
        {
            GameObject canvasObject = new GameObject("Game_UI");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440f, 900f);
            canvasObject.AddComponent<GraphicRaycaster>();

            MakePanel("HUD_TopBacking", canvasObject.transform, new Vector2(0f, 0.86f), new Vector2(1f, 1f), new Color(0.005f, 0.012f, 0.02f, 0.28f));
            MakePanel("HUD_BottomBacking", canvasObject.transform, new Vector2(0f, 0f), new Vector2(0.8f, 0.12f), new Color(0.005f, 0.012f, 0.02f, 0.36f));

            TMPro.TMP_Text section = MakeText("HUD_Section", canvasObject.transform, new Vector2(0.035f, 0.93f), new Vector2(0.7f, 0.98f), 22f, TMPro.TextAlignmentOptions.Left, Color.white);
            section.fontStyle = TMPro.FontStyles.Bold;
            TMPro.TMP_Text title = MakeText("HUD_Title", canvasObject.transform, new Vector2(0.035f, 0.875f), new Vector2(0.7f, 0.92f), 11f, TMPro.TextAlignmentOptions.Left, new Color(0.44f, 0.82f, 0.95f));
            title.text = "MARBLE MACHINE  //  THE GLASS RUN";
            TMPro.TMP_Text telemetry = MakeText("HUD_Telemetry", canvasObject.transform, new Vector2(0.82f, 0.9f), new Vector2(0.965f, 0.98f), 13f, TMPro.TextAlignmentOptions.Right, new Color(0.85f, 0.92f, 0.95f));
            TMPro.TMP_Text hint = MakeText("HUD_Hint", canvasObject.transform, new Vector2(0.035f, 0.035f), new Vector2(0.75f, 0.09f), 14f, TMPro.TextAlignmentOptions.Left, Color.white);
            hint.text = "←  →   STEER  •  PHYSICS OWNS TRAVEL       R   FAST RESET";

            GameObject overlay = new GameObject("Result_Overlay");
            overlay.transform.SetParent(canvasObject.transform, false);
            RectTransform overlayRect = overlay.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = overlayRect.offsetMax = Vector2.zero;
            Image image = overlay.AddComponent<Image>();
            image.color = new Color(0.008f, 0.018f, 0.03f, 0.88f);
            CanvasGroup group = overlay.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            TMPro.TMP_Text result = MakeText("Result_Text", overlay.transform, new Vector2(0.16f, 0.34f), new Vector2(0.84f, 0.66f), 26f, TMPro.TextAlignmentOptions.Center, Color.white);
            result.fontStyle = TMPro.FontStyles.Bold;

            GameDirector director = GameObject.Find("Game_Director").GetComponent<GameDirector>();
            SetPrivate(director, "resultOverlay", group);
            SetPrivate(director, "resultText", result);
            SetPrivate(director, "sectionText", section);
            SetPrivate(director, "telemetryText", telemetry);
        }

        private static TMPro.TMP_Text MakeText(string name, Transform parent, Vector2 min, Vector2 max, float size, TMPro.TextAlignmentOptions alignment, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            TMPro.TextMeshProUGUI text = go.AddComponent<TMPro.TextMeshProUGUI>();
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.outlineWidth = 0.16f;
            text.outlineColor = new Color(0f, 0f, 0f, 0.85f);
            return text;
        }

        private static Image MakePanel(string name, Transform parent, Vector2 min, Vector2 max, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            Image image = go.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static void SetPrivate(Object target, string field, Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Beam(string name, Vector3 a, Vector3 b, float radius, Material material, Transform parent)
        {
            Vector3 delta = b - a;
            Cylinder(name, (a + b) * 0.5f, new Vector3(radius, delta.magnitude * 0.5f, radius), material, parent,
                Quaternion.FromToRotation(Vector3.up, delta.normalized));
        }
    }
}
#endif
