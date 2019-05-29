/*
 * @Author: fasthro
 * @Date: 2019-05-17 14:28:56
 * @Description: 关卡创建窗口
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace LevelEditor
{
    public class CreateWindow : EditorWindow
    {
        static CreateWindow window;

        // 关卡场景名称
        private string levelSceneName;

        // 关卡规模
        private Vector2Int dimension = new Vector2Int(20, 20);

        [MenuItem("LevelEditor/Create Level Window")]
        public static void Initialize()
        {
            window = GetWindow<CreateWindow>();
            window.titleContent.text = "Create Level";
        }

        void OnGUI()
        {
            if (Application.isPlaying)
                return;

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("LevelSceneName");
            levelSceneName = EditorGUILayout.TextField(levelSceneName);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            dimension = EditorGUILayout.Vector2IntField("Grid Dimensions", dimension);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Create Level", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(levelSceneName))
                {
                    ShowNotification(new GUIContent("please input level scene name."));
                    return;
                }

                // 首字母转成大写
                levelSceneName = Utils.ToUpperFirstChar(levelSceneName);

                // 判断场景是否已经存在
                if (Directory.Exists(Utils.GetLevelSceneDirectory(levelSceneName)) || Directory.Exists(Utils.GetLevelScenePath(levelSceneName)))
                {
                    if (EditorUtility.DisplayDialog("Create Level", "The level scene already exists to create a new one?", "Create New", "Cancel"))
                    {
                        Directory.Delete(Utils.GetLevelSceneDirectory(Utils.GetLevelSceneDirectory(levelSceneName), true), true);

                        CreateLevel();
                    }
                }
                else
                {
                    CreateLevel();
                }
            }
        }

        /// <summary>
        /// 创建关卡
        /// </summary>
        private void CreateLevel()
        {
            if (!Directory.Exists(Utils.GetLevelSceneDirectory(levelSceneName)))
                Directory.CreateDirectory(Utils.GetLevelSceneDirectory(levelSceneName));

            // 创建空场景
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            // 创建网格
            GameObject templateGo = GameObject.Find(typeof(TemplateGrid).Name);
            if (templateGo != null)
            {
                DestroyImmediate(templateGo);
            }
            templateGo = new GameObject();
            templateGo.layer = LayerMask.NameToLayer(typeof(TemplateGrid).Name);
            templateGo.name = typeof(TemplateGrid).Name;
            TemplateGrid templateGrid = templateGo.AddComponent<TemplateGrid>();
            templateGrid.width = dimension.x;
            templateGrid.lenght = dimension.y;
            templateGrid.height = 0;

            // level
            GameObject levelGo = GameObject.Find(typeof(Environment).Name);
            if (levelGo != null)
            {
                DestroyImmediate(levelGo);
            }
            levelGo = new GameObject();
            levelGo.name = typeof(Environment).Name;
            var leLevel = levelGo.AddComponent<Environment>();
            leLevel.levelName = Utils.ToUpperFirstChar(levelSceneName);

            // navmesh
            GameObject navmeshGo = new GameObject();
            navmeshGo.transform.position = Vector3.zero;
            navmeshGo.name = "Navmesh";

            var navmesh = navmeshGo.AddComponent<NavMeshSurface>();
            navmesh.collectObjects = CollectObjects.All;
            //navmesh.layerMask = 1 << LayerMask.NameToLayer(LevelFunctionType.Ground.ToString());

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), Utils.GetLevelScenePath(levelSceneName));

            Close();

            LevelEditorWindow.OpenLevelEditorWindow();
        }
    }
}
