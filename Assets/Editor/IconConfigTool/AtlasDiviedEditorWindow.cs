using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BaseClassNS;
using UnityEditor;

public class AtlasDiviedEditorWindow: BaseEditorWindow
{
    public const string ImagePath = "/imgs";
    public const string MoveSame = "Trying to move asset to location it came from";
    private int _divideNum = 2;

    [MenuItem("Custom/AtlasDivied/Window")]
    private static void Open()
    {
        Open<AtlasDiviedEditorWindow>();
    }


    protected override void CustomOnGUI()
    {
        Space();
        TitleField("划分路径：", IconConfigTool.IconRootPath);
        TitleField("路径中必须包含：", ImagePath);

        Space();
        _divideNum = IntSlider("划分数量", _divideNum, 1, 4);

        Space();
        Button("拆分选中文件夹的图片", DivideSelected);
    }


    private void DivideSelected()
    {
        var folderPath = new List<string>();
        foreach (var obj in Selection.objects)
        {
            folderPath.Add(AssetDatabase.GetAssetPath(obj));
        }
        DivideAtlas(folderPath, _divideNum);
    }


    public static void DivideAtlas(List<string> folderPathList, int divideNum)
    {
        if (folderPathList == null || folderPathList.Count == 0 || divideNum <= 0)
        {
            return;
        }

        foreach (var folder in folderPathList)
        {
            if (!folder.Contains(ImagePath) || !Directory.Exists(folder))
            {
                return;
            }
        }

        var fileList = new List<string>();
        foreach (var folder in folderPathList)
        {
            fileList.AddRange(Directory.GetFiles(folder, "*.png"));
        }
        fileList.Sort((s, s1) => Path.GetFileNameWithoutExtension(s).CompareTo(Path.GetFileNameWithoutExtension(s1)));

        int sortNum = fileList.Count/divideNum;
        if (sortNum <= 0)
        {
            return;
        }

        var rootPath = Path.GetDirectoryName(folderPathList[0]);
        var startIndex = 0;
        for (int i = 0; i < divideNum; i++)
        {
            var imgPath = rootPath + "/imgs_" + (i + 1);
            if (!Directory.Exists(imgPath))
            {
                Directory.CreateDirectory(imgPath);
                AssetDatabase.Refresh();
            }
            //补全剩余的那个
            if (i == divideNum - 1)
            {
                sortNum = fileList.Count - i*sortNum;
            }
            for (int j = 0; j < sortNum; j++)
            {
                var num = j + sortNum*i;
                if (num >= fileList.Count)
                {
                    break;
                }
//                Debug.Log(fileList[num] + "\n" +  imgPath + "/" + Path.GetFileName(fileList[num]));
                
                var error = AssetDatabase.MoveAsset(fileList[num], imgPath + "/" + Path.GetFileName(fileList[num]));
                if (!string.IsNullOrEmpty(error) && !error.Contains(MoveSame))
                {
                    Debug.LogError(error);
                    return;
                }
            }
        }
    }
}

