using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
                    // Local Variables
                    string[] split = Rows[i].Split(',');

                    Color currentColour = Color.black; ColorUtility.TryParseHtmlString(split[1], out currentColour);

                    // Create Data
                    HoldVegetationObjectData data = new HoldVegetationObjectData() {
                        Name = split[0],
                        BiomesColour = currentColour,
                        type = split[2],
                        AssetPath = split[3],
                    };
                    vegetationList.Add(data);

                    Debug.Log(data.AssetPath);

                    // THIS CODE SHOULD BE PLACED IN A FOR LOOP THAT GOES THROUGH ALL VERTECES OR POSITIONS WHERE SHIT SHOULD BE PLACED, A.K.A MOVE THIS FUNCTION!!!
                    PlaceObject((GameObject)AssetDatabase.LoadAssetAtPath(data.AssetPath.TrimEnd(), typeof(GameObject)), /*Placeholder for the position*/Vector3.zero, data);

                    // Cleanup local variables
                    data = null;
                    split = null;
                }
            }
        }

        Rows = null;

        Debug.Log("LOL!");
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

    private void PlaceObject(GameObject prefab, Vector3 position, HoldVegetationObjectData data)
    {
        GameObject gameObject = Instantiate(prefab, position, Quaternion.identity);
        gameObject.name = data.Name;
    }
}

class HoldVegetationObjectData
{
    // Variables
    public string Name = "";
    public string AssetPath = "";

    public Color BiomesColour = Color.black;
    public string type = "";
}