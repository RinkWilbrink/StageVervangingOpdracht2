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
    //private string[,] LinesAndRows;

    private TextAsset csvFile;
    private GameObject worldTerainObject;

    // 
    private Dictionary<string, List<VegetationData>> vegetationDictionairy = new Dictionary<string, List<VegetationData>>();

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

        if(GUILayout.Button("Read Data From The Data Sheet"))
        {
            ClearVariableData();

            ReadSheetData();

            //ReadVertexColourData();
        }

        GUILayout.Space(6);

        //EditorGUI.BeginDisabledGroup(LinesAndRows == null);
        //
        //if(LinesAndRows == null)
        //{
        //    if(GUILayout.Button("LinesAndRowes is NULL, Button has been disabled!")) { }
        //}
        //else
        //{
        //    if(GUILayout.Button("Start Vegetation Placement"))
        //    {
        //        Debug.Log("Cool!");
        //    }
        //}
        //EditorGUI.EndDisabledGroup();
    }

    private void ReadSheetData()
    {
        // Read all the text in the File and split it by rows into an array.
        string[] Rows = System.IO.File.ReadAllText(string.Format("{0}/{1}.csv", Application.dataPath, csvFile.name)).Split('\n');

        List<string> EmptyLineFilterList = new List<string>();

        for(int i = 0; i < Rows.Length; i++)
        {
            if(!string.IsNullOrEmpty(Rows[i]) && !string.IsNullOrWhiteSpace(Rows[i]))
            {
                if(Rows[i].Contains("Name,Colour") == false)
                {
                    // Local Variables
                    string[] split = Rows[i].Split(',');

                    Color currentColour = Color.black;
                    ColorUtility.TryParseHtmlString(split[1], out currentColour);

                    // Create Data
                    VegetationData data = new VegetationData()
                    {
                        Name = split[0],
                        BiomesColour = currentColour,
                        type = split[2],
                        AssetPath = split[3],
                        VegetationThickness = int.Parse(split[4])
                    };

                    // If the current Type doesnt exist yet, create a new TKey in de Dictionairy, if it does exist add the new DATA variable into the list.
                    if(vegetationDictionairy.ContainsKey(split[2]))
                    {
                        List<VegetationData> OldList = new List<VegetationData>();
                        vegetationDictionairy.TryGetValue(split[2], out OldList);

                        OldList.Add(data);

                        vegetationDictionairy[split[2]] = OldList;

                        for(int x = 0; x < OldList.Count; x++)
                        {
                            Debug.LogFormat("x={0} | OL={1}", x, OldList[x].Name);
                        }
                    }
                    else
                    {
                        List<VegetationData> vegData = new List<VegetationData>();
                        vegData.Add(data);
                        vegetationDictionairy.Add(split[2], vegData);

                        Debug.LogFormat("New TKey! {0}", split[2]);
                    }

                    // Cleanup local variables
                    data = null;
                    split = null;
                }
            }
        }
        Rows = null;
    }

    private void ClearVariableData()
    {
        // Clear up / empty variables and Lists/Dictionairies
        vegetationDictionairy = null;

        // Initialize Variables that need to be to be used again.
        vegetationDictionairy = new Dictionary<string, List<VegetationData>>();
    }

    private void ReadVertexColourData()
    {
        MeshFilter mf = worldTerainObject.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;

        Color[] Colours = mesh.colors;

        Matrix4x4 localToWorld = worldTerainObject.transform.localToWorldMatrix;

        GameObject Parent = new GameObject();
        Parent.name = "Vegetation Parent";

        for(int i = 0; i < mesh.vertices.Length; ++i)
        {
            Vector3 world_v = localToWorld.MultiplyPoint3x4(mesh.vertices[i]);

            /* REWORK THIS PIECE OF CODE TO WORK WITH A DICTIONAIRY, SMALL BUT IMPORTANT CHANGES NEEDED FOR IT TO WORK THE SAME. 
            
            for(int x = 0; x < vegetationList.Count; x++)
            {
                if(vegetationList[x].BiomesColour == Colours[i])
                {
                    float cool = CalculateBiomeFloat(world_v.x, world_v.z, 11f, 11f, 1f, 12345f);
            
                    if(Random.Range(0, 10) <= (vegetationList[x].VegetationThickness))
                    {
                        PlaceObject(vegetationList[x], world_v, Parent);
                    }
                }
            }
            */
        }
    }

    private float CalculateBiomeFloat(float _x, float _y, float _width, float _height, float _scale, float _worldSeed)
    {
        float xCoord = (_x + _worldSeed) / _width * _scale;
        float yCoord = (_y + _worldSeed) / _height * _scale;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    private void PlaceObject(VegetationData data, Vector3 position, GameObject parent)
    {
        GameObject newVegetation = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(data.AssetPath.TrimEnd(), typeof(GameObject)),
            new Vector3(position.x + Random.Range(-1f, 1f), position.y, position.z + Random.Range(-1f, 1f)), Quaternion.identity);

        // Set new Vegetation Object attributes
        newVegetation.name = data.Name;
        newVegetation.transform.parent = parent.transform;
    }
}

class VegetationData
{
    // Variables
    public string Name = "";
    public string AssetPath = "";

    public Color BiomesColour = Color.black;
    public string type = "";

    public int VegetationThickness = 0;
}