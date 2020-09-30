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

    // 
    //private Dictionary<string, HoldVegetationObjectData> vegetationDictionairy = new Dictionary<string, HoldVegetationObjectData>();
    private List<HoldVegetationObjectData> vegetationList = new List<HoldVegetationObjectData>();

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

        if (GUILayout.Button("Read Data From The Data Sheet"))
        {
            ReadSheetData();

            ReadVertexColourData();
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

    private void ReadSheetData()
    {
        // Read all the text in the File and split it by rows into an array.
        string[] Rows = System.IO.File.ReadAllText(string.Format("{0}/{1}.csv", Application.dataPath, csvFile.name)).Split('\n');

        List<string> EmptyLineFilterList = new List<string>();

        for (int i = 0; i < Rows.Length; i++)
        {
            if (!string.IsNullOrEmpty(Rows[i]) && !string.IsNullOrWhiteSpace(Rows[i]))
            {
                if (Rows[i].Contains("Name,Colour") == false)
                {
                    //EmptyLineFilterList.Add(Rows[i]);

                    // Local Variables
                    string[] split = Rows[i].Split(',');

                    Color currentColour = Color.black; ColorUtility.TryParseHtmlString(split[1], out currentColour);

                    // Create Data
                    HoldVegetationObjectData data = new HoldVegetationObjectData() { 
                        BiomesColour = currentColour, 
                        Thickness = int.Parse(split[2]),
                        AssetPath = split[3],
                    };

                    //Debug.LogFormat("{0} | {1} | {2}", data.BiomesColour, data.Thickness, data.AssetPath);

                    //vegetationDictionairy.Add(split[0], data);
                    vegetationList.Add(data);

                    // Cleanup local variables
                    data = null;
                    split = null;
                }
            }
        }

        Rows = null;
        /*Rows = EmptyLineFilterList.ToArray();
         * 
        LinesAndRows = new string[Rows.Length, Rows[0].Split(',').Length];

        for (int x = 0; x < Rows.Length; x++)
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
        }*/
    }

    private void ReadVertexColourData()
    {
        MeshFilter mf = worldTerainObject.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;

        Color32[] Colours = mesh.colors32;

        Matrix4x4 localToWorld = worldTerainObject.transform.localToWorldMatrix;

        for (int i = 0; i < mesh.vertices.Length; ++i)
        {
            Vector3 world_v = localToWorld.MultiplyPoint3x4(mesh.vertices[i]);
        }
    }
}

class HoldVegetationObjectData
{
    // Variables
    public string Name = "";
    public string AssetPath = "";

    public Color BiomesColour = Color.black;
    public int Thickness = 0;
}