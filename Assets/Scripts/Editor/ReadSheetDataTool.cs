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

        if (GUILayout.Button("Banaan"))
        {
            string[] lines = System.IO.File.ReadAllText(Application.dataPath.Replace("Assets", CSVFileName)).Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {

            }

            string[,]  LinesAndRows = new string[lines[0].Split(',').Length, lines.Length];

            Debug.LogFormat("{0} | {1}", lines[0].Split(',').Length, lines.Length);

            for (int x = 0; x < LinesAndRows.GetLength(0); x++)
            {
                string[] Y = lines[x].Split(',');

                for (int y = 0; y < LinesAndRows.GetLength(1); y++)
                {
                    //Debug.Log()
                    LinesAndRows[x, y] = Y[y];

                    //Debug.Log(LinesAndRows[x, y]);
                }
            }

            ReadData();
        }

        if (GUILayout.Button("adaw"))
        {
            string[] SPLIT = ",\n".Split(',');

            Debug.Log(SPLIT.Length);

            for (int i = 0; i < SPLIT.Length; i++)
            {
                Debug.Log(SPLIT[i]);
            }
        }
    }

    private void ReadData()
    {
        //Debug.Log(LinesAndRows[0, 0]);
        //Debug.Log(LinesAndRows[1, 0]);
        //Debug.Log(LinesAndRows[2, 0]);
        //Debug.Log(LinesAndRows[2, 1]);
    }
}