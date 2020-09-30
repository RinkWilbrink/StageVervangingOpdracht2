using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SyntaxTree.VisualStudio.Unity.Bridge;
using System.Security;
using UnityEditorInternal;

/* 
 * Editor placement code article
 * https://80.lv/articles/object-placement-tool-for-unity/
 * 
 * Poly Brush
 * https://docs.unity3d.com/Packages/com.unity.polybrush@1.0/manual/index.html
 * 
 * Getting Vertex Colour data
 * https://answers.unity.com/questions/1304791/getting-the-world-position-of-mesh-vertices.html
 * https://answers.unity.com/questions/1429986/mesh-class-how-do-get-the-color-of-each-vertex.html
 * 
 * Find Assets in Editor
 * https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
 * 
 * 
 * 
 * Enable/DisableGroups
 * https://docs.unity3d.com/ScriptReference/EditorGUI.BeginDisabledGroup.html
 * 
 * Changing GUI colours
 * https://www.reddit.com/r/Unity3D/comments/88zt5f/how_do_i_change_the_color_of_a_guilayout_button/
 * 
*/

[InitializeOnLoad, ExecuteInEditMode]
public class ReadSheetDataTool : EditorWindow
{
    // Variable
    private string[,] LinesAndRows;

    private TextAsset csvFile;
    private GameObject worldTerainObject;

    //File Paths
    private string ScriptPath;

    [MenuItem("ReadSheetDataTool/Tool")]
    public static void GetReadSheetDataToolWindow()
    {
	    GetWindow<ReadSheetDataTool>("ReadSheetDataTool");
    }

    private void OnGUI()
    {
        GUILayout.Space(6);

        GUILayout.Label("CSV File / Excel Sheet that contains all the data for where and when trees and vegetation will be placed.");

        csvFile = (TextAsset)EditorGUILayout.ObjectField(csvFile, typeof(TextAsset), true);

        GUILayout.Space(6);

        GUILayout.Label("Input the Terain GameObject / Floor of what you want the biomes to become!");

        worldTerainObject = (GameObject)EditorGUILayout.ObjectField(worldTerainObject, typeof(GameObject), true);

        GUILayout.Space(6);

        if (GUILayout.Button("Reading Data From Data Sheet"))
        {
            // Read all the text in the File and split it by rows into an array.
            string[] Rows = System.IO.File.ReadAllText(string.Format("{0}/{1}.csv", Application.dataPath, csvFile.name)).Split('\n');

            List<string> EmptyLineFilterList = new List<string>();

            for (int i = 0; i < Rows.Length; i++)
            {
                if(!string.IsNullOrEmpty(Rows[i]) && !string.IsNullOrWhiteSpace(Rows[i]))
                {
                    EmptyLineFilterList.Add(Rows[i]);
                }
            }

            Rows = null;
            Rows = EmptyLineFilterList.ToArray();

            LinesAndRows = new string[Rows[0].Split(',').Length, Rows.Length];

            //Debug.LogFormat("{0} | {1}", Rows[0].Split(',').Length, Rows.Length);

            for (int x = 0; x < LinesAndRows.GetLength(0); x++)
            {
                string[] Y = Rows[x].Split(',');

                for (int y = 0; y < LinesAndRows.GetLength(1); y++)
                {
                    try
                    {
                        LinesAndRows[x, y] = Y[y];
                    }
                    catch
                    {
                        Debug.LogErrorFormat("Error with adding the Y[{1}] to LinesAndRows[{0}, {1}]\n Set {0}, {1} to whitetext! check the Sheet file for empty Cells", x, y);

                        // Reset values to prevent parts of the array to be useable
                        x = LinesAndRows.GetLength(0);
                        y = LinesAndRows.GetLength(1);
                        LinesAndRows = null;
                    }
                }
            }

            ReadColourData();
        }

        GUILayout.Space(6);

        EditorGUI.BeginDisabledGroup(LinesAndRows == null);

        if(LinesAndRows == null)
        {
            if(GUILayout.Button("LinesAndRowes is NULL, Button has been disabled!")) { }
        }
        else
        {
            if (GUILayout.Button("Start Vegetation Placement"))
            {
                Debug.Log("Cool!");
            }
        }

        EditorGUI.EndDisabledGroup();
    }

    private void ReadColourData()
    {
        MeshFilter mf = worldTerainObject.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;

        Color32[] Colours = mesh.colors32;

        Matrix4x4 localToWorld = worldTerainObject.transform.localToWorldMatrix;

        for (int i = 0; i < mesh.vertices.Length; ++i)
        {
            Vector3 world_v = localToWorld.MultiplyPoint3x4(mesh.vertices[i]);

            //Debug.LogFormat("i={0}, r={1}, g={2}, b={3}, a={4}", i, Colours[i].r, Colours[i].g, Colours[i].b, Colours[i].a);
        }
    }
}

class HoldVegetationObjectData
{
    // Variables
    public string VegetationName = "";
    public string AssetPath = "";

    public Color BiomesColour = Color.black;
    public int Thickness = 0;
}