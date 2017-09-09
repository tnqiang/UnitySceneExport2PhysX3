using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityPhysXExport
{
    public class ScenePhsyXExportWnd : EditorWindow 
    {
        [MenuItem("Window/PhysXExport/OpenWindow")]
        public static void OpenPhsyXExportWnd()
        {
            ScenePhsyXExportWnd creator = (ScenePhsyXExportWnd)EditorWindow.GetWindow(typeof(ScenePhsyXExportWnd));
            creator.titleContent = new GUIContent("PhysXExport");
            creator.Show();
        }

        private Vector2 vs;
        void OnGUI()
        {
            List<string> scenePaths = new List<string>();
            foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    string scenePath = scene.path;
                    scenePaths.Add(scenePath);
                }
            }

            vs = EditorGUILayout.BeginScrollView(vs);
            for (int i = 0; i < scenePaths.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (GUILayout.Button("Export"))
                {
                    string outputPath = string.Empty;
                    if (PlayerPrefs.HasKey("ScenePhysXExportPath") && !string.IsNullOrEmpty(PlayerPrefs.GetString("ScenePhysXExportPath")))
                    {
                        outputPath = PlayerPrefs.GetString ("ScenePhysXExportPath");
                    }
                    else
                    {
                        outputPath = EditorUtility.SaveFolderPanel("Select Folder To Save", outputPath, "");
                        PlayerPrefs.SetString("ScenePhysXExportPath", outputPath);
                    }
                    if (ExportScene.Export(scenePaths[i], outputPath))
                    {
                        EditorUtility.DisplayDialog("Success", string.Format("Export Scene {0} Success", scenePaths[i]), "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Failure", string.Format("Export Scene {0} Failure, for detail infomation in the console window", scenePaths[i]), "OK");
                    }
                }
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(60));
                EditorGUILayout.LabelField(scenePaths[i].Replace("Assets/", "").Replace(".unity", ""), GUILayout.Width(500));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.Space(20);
        }
    }
}