using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SyntaxTree.VisualStudio.Unity.Bridge;

/* IDK
 * 
 * Editor placement code article
 * https://80.lv/articles/object-placement-tool-for-unity/
 * 
*/

[InitializeOnLoad, ExecuteInEditMode]
public class ReadSheetDataTool : EditorWindow
{
    // Variable
    private string CSVFileName;

    private string ScriptPath;

    //private string[,] LinesAndRows;

    [MenuItem("ReadSheetDataTool/Tool")]
    public static void GetReadSheetDataToolWindow()
    {
	    GetWindow<ReadSheetDataTool>("ReadSheetDataTool");
    }

    private void OnGUI()
    {
        GUILayout.Space(6);

        CSVFileName = EditorGUILayout.TextField(new GUIContent("CSV File name", ""), CSVFileName);

        GUILayout.Space(6);

        if (GUILayout.Button("Reading Data From Data Sheet"))
        {
            string[] linesArray = System.IO.File.ReadAllText(Application.dataPath.Replace("Assets", CSVFileName)).Split('\n');

            List<string> EmptyLineFilterList = new List<string>();

            for (int i = 0; i < linesArray.Length; i++)
            {
                if(!string.IsNullOrEmpty(linesArray[i]) && !string.IsNullOrWhiteSpace(linesArray[i]))
                {
                    EmptyLineFilterList.Add(linesArray[i]);
                }
            }

            linesArray = null;
            linesArray = EmptyLineFilterList.ToArray();

            string[,]  LinesAndRows = new string[linesArray[0].Split(',').Length, linesArray.Length];

            Debug.LogFormat("{0} | {1}", linesArray[0].Split(',').Length, linesArray.Length);

            for (int x = 0; x < LinesAndRows.GetLength(0); x++)
            {
                string[] Y = linesArray[x].Split(',');

                for (int y = 0; y < LinesAndRows.GetLength(1); y++)
                {
                    //Debug.Log()
                    try
                    {
                        LinesAndRows[x, y] = Y[y];
                    }
                    catch
                    {
                        //LinesAndRows[x, y] = "";
                        Debug.LogErrorFormat("Error with adding the Y[{1}] to LinesAndRows[{0}, {1}]\n Set {0}, {1} to whitetext! check the Sheet file for empty Cells", x, y);

                        // Reset values to prevent parts of the array to be useable
                        x = LinesAndRows.GetLength(0);
                        y = LinesAndRows.GetLength(1);
                        LinesAndRows = null;
                    }

                    //Debug.Log(LinesAndRows[x, y]);
                }
            }

            ReadData();
        }
    }

    private void SaveFileName()
    {
        ScriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));


    }

    private void ReadData()
    {
        //Debug.Log(LinesAndRows[0, 0]);
        //Debug.Log(LinesAndRows[1, 0]);
        //Debug.Log(LinesAndRows[2, 0]);
        //Debug.Log(LinesAndRows[2, 1]);
    }
}