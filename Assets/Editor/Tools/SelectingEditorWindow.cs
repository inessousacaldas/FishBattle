using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BaseClassNS;
using UnityEditor;

public class SelectingEditorWindow: BaseEditorWindow
{
    private List<GameObject> _goList;
    private string _msg;

    private const string ListScroll = "ListScroll";

    public static void Show(List<GameObject> goList, string msg = null)
    {
        if (goList != null && goList.Count > 0)
        {
            Open(goList, msg);
        }
    }

    private static SelectingEditorWindow Open(List<GameObject> goList, string msg)
    {
        var ew = Open<SelectingEditorWindow>();
        ew.Setup(goList, msg);

        return ew;
    }

    private void Setup(List<GameObject> goList, string msg)
    {
        _goList = goList;
        _msg = msg;
    }

    protected override void CustomOnGUI()
    {
        if (_msg != null)
        {
            TitleField(_msg);
        }

        Space();
        BeginTempScrollView(ListScroll);
        {
            var count = _goList.Count;
            for (int i = 0; i < count; i++)
            {
                var go = _goList[i];
                DrawLine();
                BeginHorizontal();
                {
                    ObjectField(string.Empty, go, null, true);
                    Button("选中", () => EditorHelper.SelectObject(go));
                    var index = i;
                    Button("移除", () => RemoveGo(index));
                }
                EndHorizontal();
            }
        }
        EndTempScrollView();
    }

    private void RemoveGo(int index)
    {
        EditorApplication.delayCall += () => _goList.RemoveAt(index);
    }
}
