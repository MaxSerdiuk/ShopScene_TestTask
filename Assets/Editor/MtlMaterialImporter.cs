using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MtlMaterialImporter : EditorWindow
{
    private string mtlPath = "";
    private string materialsFolder = "";

    [MenuItem("Tools/MTL Material Importer")]
    public static void ShowWindow()
    {
        GetWindow<MtlMaterialImporter>("MTL Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("MTL файл:", EditorStyles.boldLabel);
        mtlPath = GUILayout.TextField(mtlPath);
        if (GUILayout.Button("Обрати MTL файл"))
        {
            mtlPath = EditorUtility.OpenFilePanel("Обери MTL файл", "Assets", "mtl");
        }

        GUILayout.Label("Папка з матеріалами Unity:", EditorStyles.boldLabel);
        materialsFolder = GUILayout.TextField(materialsFolder);
        if (GUILayout.Button("Обрати папку матеріалів"))
        {
            materialsFolder = EditorUtility.OpenFolderPanel("Обери папку", "Assets", "");
        }

        if (GUILayout.Button("Призначити кольори"))
        {
            AssignColors();
        }
    }

    void AssignColors()
    {
        // Читаємо MTL файл
        Dictionary<string, Color> colors = new Dictionary<string, Color>();
        string currentMat = "";

        foreach (string line in File.ReadAllLines(mtlPath))
        {
            if (line.StartsWith("newmtl "))
                currentMat = line.Substring(7).Trim();
            else if (line.Trim().StartsWith("Kd ") && currentMat != "")
            {
                string[] parts = line.Trim().Substring(3).Split(' ');
                float r = float.Parse(parts[0]);
                float g = float.Parse(parts[1]);
                float b = float.Parse(parts[2]);
                colors[currentMat] = new Color(r, g, b);
            }
        }

        // Читаємо OBJ файл щоб знайти mesh→mat відповідності
        string objPath = mtlPath.Replace(".mtl", ".obj");
        Dictionary<string, string> meshToMat = new Dictionary<string, string>();
        string currentMesh = "";
        string firstMat = "";

        foreach (string line in File.ReadAllLines(objPath))
        {
            if (line.StartsWith("g "))
                currentMesh = line.Substring(2).Trim();
            else if (line.StartsWith("usemtl ") && currentMesh != "" && !meshToMat.ContainsKey(currentMesh))
            {
                firstMat = line.Substring(7).Trim();
                meshToMat[currentMesh] = firstMat;
            }
        }

        // Призначаємо кольори матеріалам Unity
        string relativePath = "Assets" + materialsFolder.Substring(Application.dataPath.Length);
        string[] matFiles = Directory.GetFiles(materialsFolder, "*.mat");

        int count = 0;
        foreach (string matFile in matFiles)
        {
            string matName = Path.GetFileNameWithoutExtension(matFile);
            string assetPath = relativePath + "/" + Path.GetFileName(matFile);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            if (mat == null) continue;

            if (meshToMat.ContainsKey(matName) && colors.ContainsKey(meshToMat[matName]))
            {
                mat.SetColor("_BaseColor", colors[meshToMat[matName]]);
                EditorUtility.SetDirty(mat);
                count++;
                Debug.Log($"✅ {matName} → {meshToMat[matName]} → {colors[meshToMat[matName]]}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Готово! Призначено кольорів: {count}");
    }
}