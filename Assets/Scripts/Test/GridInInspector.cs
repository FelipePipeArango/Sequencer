//using System;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UIElements;


//public enum Elements { Water, Fire, Earth }

//[CreateAssetMenu(fileName = "New Test Grid", menuName = "Test/Grid")]
//public class GridHolder : ScriptableObject
//{
//    public Wrapper<Elements>[] grid;

//    public const int size = 4;

//    private void Awake()
//    {
//        if (grid == null)
//            ResetGrid();
//    }

//    public void ResetGrid()
//    {
//        grid = new Wrapper<Elements>[size];
//        for (int i = 0; i < size; i++)
//        {
//            grid[i] = new Wrapper<Elements>();
//            grid[i].values = new Elements[size];
//        }
//    }
//}

//#if UNITY_EDITOR
//[CustomEditor(typeof(GridHolder))]
//public class GridHolderEditor : Editor
//{
//    SerializedProperty grid;
//    SerializedProperty array;
    
//    int length;

//    private void OnEnable()
//    {
//        grid = serializedObject.FindProperty("grid");
//        length = Enum.GetValues(typeof(Elements)).Length;
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        GridHolder script = (GridHolder)target;

//        DrawGrid();

//        if (GUILayout.Button("Reset"))
//            script.ResetGrid();

//        serializedObject.ApplyModifiedProperties();
//    }

//    private void DrawGrid()
//    {
//        try
//        {
//            GUILayout.BeginVertical();
//            for (int i = 0; i < GridHolder.size; i++)
//            {
//                if (grid.arraySize <= i)
//                {
//                    Debug.LogWarning($"Grid array size mismatch. Index {i} is out of bounds.");
//                    return;
//                }
//                GUILayout.BeginHorizontal();
//                array = grid.GetArrayElementAtIndex(i).FindPropertyRelative("values");
//                for (int j = 0; j < GridHolder.size; j++)
//                {
//                    if (array.arraySize <= j)
//                    {
//                        Debug.LogWarning($"Grid row array size mismatch. Index {j} is out of bounds.");
//                        return;
//                    }
//                    var value = array.GetArrayElementAtIndex(j);
//                    Elements element = (Elements)value.intValue;
//                    if (GUILayout.Button(element.ToString(), GUILayout.MaxWidth(50)))
//                    {
//                        value.intValue = NextIndex(value.intValue);
//                        GameObject obj = new GameObject();
//                        ;
//                        Debug.Log("click");
//                    }
//                }
//                GUILayout.EndHorizontal();
//            }
//            GUILayout.EndVertical();

//        }
//        catch (System.Exception e)
//        {
//            Debug.LogWarning(e);
//        }
//    }

//    private int NextIndex(int index)
//    {
//        int result = ++index % length;
//        return result;
//    }
//}
//#endif

//[System.Serializable]
//public class Wrapper<T>
//{
//    public T[] values;
//}