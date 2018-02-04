//author: Fish
//#define GenerateFRPView
//using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public static class FRPCodeGeneratorHelper
{
    private static readonly string TempPath = "Assets/Temp/";
    private static readonly string AutoGenViewTemplatePath = TempPath + "FRPTemplate/ViewAutoGenTemp.txt";
    private static readonly string CustomFRPControllerTemplatePath = TempPath + "FRPTemplate/FRPControllerTemp.txt";
    private static readonly string CustomControllerTemplatePath = TempPath + "FRPTemplate/MonoControllerTemp.txt";
    private static readonly string AutoGenFRPBaseControllerTemplatePath = TempPath + "FRPTemplate/FRPBaseControllerAutoGenTemp.txt";
    private static readonly string AutoGenMonoViewControllerTemplatePath = TempPath + "FRPTemplate/MonoControllerAutoGenTemp.txt";
    private static readonly string FRPLogicTemplatePath = TempPath + "FRPTemplate/LogicTemp.txt";
    private static readonly string LogicTemplatePath = TempPath + "FRPTemplate/MonoLogicTemp.txt";
    private static readonly string AutoGenMonolessControllerTemplatePath = TempPath + "FRPTemplate/MonolessControllerAutoGenTemp.txt";
    private static readonly string CustomMonolessControllerTemplatePath = TempPath + "FRPTemplate/MonolessControllerTemp.txt";
    private static readonly string CustomModelTemplatePath = TempPath + "FRPTemplate/ModelTemp.txt";
    private static readonly string AutoGenCustomModelTemplatePath = TempPath + "FRPTemplate/ModelAutoGenTemp.txt";
    private static readonly string CustomModelDataTemplatePath = TempPath + "FRPTemplate/ModeldDataTemp.txt";
    private static readonly string NetMsgTemplatePath = TempPath + "FRPTemplate/NetMsgTemp.txt";
    private static readonly string ProxyTemplatePath = TempPath + "FRPTemplate/ProxyTemp.txt";
    private static readonly string HelperTemplatePath = TempPath + "FRPTemplate/HelperTemp.txt";


    private static readonly string GameObjectStr="GameObject";
    private static readonly string ButtonObjectStr="UIButton";
    private static readonly string TransformStr="Transform";

    private static string RelativePath;
    private static string TemplateFileName;
    private static string ConfigFileName;
    private static string defaultoutputPath;
    private static char PathSep;

    static readonly char psep='/';
    static readonly string indent ="    ";
    static readonly string indent2 =indent+indent;
    static readonly string indent3 =indent2+indent;

    static readonly string getter="{get;}";

    public static void GenerateHelper(GenParams genParams)
    {
        var path =  GetModelPath(genParams.generatePath) + "/" + genParams.moduleName + "Helper.cs";
        var success = GenerateFile(path, false);
        if (!success) return;
        var replacements = BuildProxyReplacements(genParams);
        UpdateFileContent(path, HelperTemplatePath, replacements);
    }

    public static void GenerateProxy(GenParams genParams)
    {
        var path =  genParams.generatePath + "/Proxy" + genParams.moduleName + ".cs";
        var success = GenerateFile(path, false);
        if (!success) return;
        var replacements = BuildProxyReplacements(genParams);
        UpdateFileContent(path, ProxyTemplatePath, replacements);
    }

    public static void GenerateNetMsg(GenParams genParams){
        var path = GetModelPath(genParams.generatePath) + genParams.moduleName + "NetMsg.cs";
        var success = GenerateFile(path, false);
        if (!success) return;
        var replacements = BuildNetMsgReplacements(genParams);
        UpdateFileContent(path, NetMsgTemplatePath, replacements);
    }

    public static void GenerateModule(GenParams genParams)
    {
        GenerateCustomModule(genParams);
        GenerateAutoGenModule(genParams);
    }

    private static void GenerateAutoGenModule(GenParams genParams){
        var path = GetModelPath(genParams.generatePath) + genParams.dataClassName + "MgrAutoGen.cs";
        var success = GenerateFile(path, true);
        if (!success) return;
        var replacements = BuildModelReplacements(genParams);
        UpdateFileContent(path, AutoGenCustomModelTemplatePath, replacements);
    }

    private static void GenerateCustomModule(GenParams genParams){
        var path = GetModelPath(genParams.generatePath) + genParams.dataClassName + "Mgr.cs";
        var success = GenerateFile(path, false);
        if (!success) return;
        var replacements = BuildModelReplacements(genParams);
        UpdateFileContent(path, CustomModelTemplatePath, replacements);
    }

    public static void GenerateModuleData(GenParams genParams){
        var path = GetModelPath(genParams.generatePath) + genParams.dataClassName + ".cs";
        var success = GenerateFile(path, false);
        if (!success) return;
        var replacements = BuildModelReplacements(genParams);
        UpdateFileContent(path, CustomModelDataTemplatePath, replacements);
    }

    public static void GenerateMonolessControllerAutoGen(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        var path = GetControllerPath(genParams.generatePath) + genParams.uiViewName + "ControllerAutoGen.cs";    
        var success = GenerateFile(path, true);
        if (!success) return;
        var replacements = BuildMonolessControllerAutoGenReplacements(comSet, genParams);
        UpdateFileContent(path, AutoGenMonolessControllerTemplatePath, replacements);
    }
    
    public static void GenerateMonolessController(GenParams genParams){
        var path = GetControllerPath(genParams.generatePath) + genParams.uiViewName + "Controller.cs";;     
        var success = GenerateFile(path, false);
        if (!success) return;
        var replacements = BuildMonolessControllerReplacements(genParams);
        UpdateFileContent(path, CustomMonolessControllerTemplatePath, replacements);
    }
                
    public static void GenerateMonoViewController(List<UIComponentInfo> comSet, GenParams genParams)
    {
        GenerateViewController(comSet, genParams, CustomControllerTemplatePath);
    }
    
    public static void GenerateFRPController(IEnumerable<UIComponentInfo> comSet, GenParams genParams)
    {
        GenerateViewController(comSet, genParams, CustomFRPControllerTemplatePath);
    }
    
    private static void GenerateViewController(IEnumerable<UIComponentInfo> comSet, GenParams genParams, string tempPath)
    {
        var path = GetControllerPath(genParams.generatePath) + genParams.uiViewName + "Controller.cs";
        var success = GenerateFile(path, false);
        if (success)
        {
            var replacements = BuildControllerReplacements(comSet, genParams);
            UpdateFileContent(path, tempPath, replacements);    
        }
    }
    
    public static void GenerateLogic(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        GenerateCode(comSet, genParams, "Logic.cs", PathType.Controller, false, LogicTemplatePath);
    }
    public static void GenerateFRPLogic(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        GenerateCode(comSet, genParams, "Logic.cs", PathType.Controller, false, FRPLogicTemplatePath);
    }
    
    private static void GenerateCode(
        IEnumerable<UIComponentInfo> comSet
        , GenParams genParams
        , string name
        , PathType ty
        , bool isOverwrite
        , string tempPath)
    {
        var path = GetControllerPath(genParams.generatePath) + genParams.uiViewName + name;
        var success = GenerateFile(path, isOverwrite);
        if (!success) return;
        var replacements = BuildLogicReplacements(comSet, genParams);
        UpdateFileContent(path, tempPath, replacements);
    }

    private static void GenerateControllerAutoGen(IEnumerable<UIComponentInfo> comSet, GenParams genParams, string tempPath){
        var path = GetControllerPath(genParams.generatePath) + genParams.uiViewName + "ControllerAutoGen.cs";
        var success = GenerateFile(path, true);
        if (!success) return;
        var replacements = BuildControllerAutoGenReplacements(comSet, genParams);
        UpdateFileContent(path, tempPath, replacements);
    }
    
    public static void GenerateMonoViewControllerAutoGen(IEnumerable<UIComponentInfo> comSet, GenParams genParams)
    {
        GenerateControllerAutoGen(comSet, genParams, AutoGenMonoViewControllerTemplatePath);
    }
    
    public static void GenerateFRPControllerAutoGen(IEnumerable<UIComponentInfo> comSet, GenParams genParams)
    {
        GenerateControllerAutoGen(comSet, genParams, AutoGenFRPBaseControllerTemplatePath);
    }

    public static void GenerateUICodeAutoGen(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        var path = GetViewPath(genParams.generatePath) + genParams.uiViewName + "AutoGen.cs";
        var success = GenerateFile(path, true);
        if (success)
        {
            var replacements = BuildViewAutoGenReplacements(comSet, genParams);
            UpdateFileContent(path, AutoGenViewTemplatePath, replacements);
        }
    }

    private static string GetViewPath(string path)
    {
        return GetFilePath(path, PathType.View);
    }
    private static string GetControllerPath(string path){
        return GetFilePath(path, PathType.Controller);
    }

    private static string GetModelPath(string path){
        return GetFilePath(path, PathType.Model);
    }

    private enum PathType
    {
        View
        , Controller
        , Model
    }

    private static string GetFilePath(string path, PathType folder)
    {
        var str = string.Format("/{0}/",folder.ToString()); 
        return path.Contains(str) ? path : path  + str;
    }
    private static bool GenerateFile(string pCreatedFilePath, bool isOverwrite)
    {
        var isExit = File.Exists(pCreatedFilePath);
        if (!isExit)
        {
            Refactor.RefactorUtils.CreateFileDirectory(pCreatedFilePath, true);
            return true;
        }
        else
        {
            if (!isOverwrite) Debug.LogWarning("the file " + pCreatedFilePath + "is not allowed to overwrite");
            return isOverwrite;
        }
    }

    private static void UpdateFileContent(string pFilePath, Dictionary<string, string> replacements)
    {
        if (!string.IsNullOrEmpty(pFilePath) && File.Exists(pFilePath))
        {
            string template = File.ReadAllText(pFilePath);
            if (!string.IsNullOrEmpty(template))
            {
                //TODO support fold
                foreach(var replacePair in replacements){
                    var oldValue = replacePair.Key;
                    var newValue = replacePair.Value;
                    template = template.Replace(oldValue,newValue);
                }

                using (var cs = new StreamWriter(pFilePath,false,Encoding.UTF8)){
                    cs.WriteLine(template);
                    Debug.Log("Generate UIView success: "+pFilePath);
                }
            }else{
                Debug.LogError("Template FileName not exits: "+TemplateFileName);
            }

            //            TestReplaceMent();

        }
    }

    private static void UpdateFileContent(string pFilePath, string tempFilePath, Dictionary<string,string> replacements)
    {
        if (string.IsNullOrEmpty(pFilePath) || !File.Exists(pFilePath))
        {
            Debug.LogError("the file path " + pFilePath + "is wrong");
            return;
        }

        if (string.IsNullOrEmpty(tempFilePath) || !File.Exists(tempFilePath))
        {
            Debug.LogError("Template FileName not exits: "+tempFilePath);
        }

        var template = File.ReadAllText(tempFilePath);

        //TODO support fold
        foreach(var replacePair in replacements){
            var oldValue = replacePair.Key;
            var newValue = replacePair.Value;
            template = template.Replace(oldValue,newValue);
        }

        using (var cs = new StreamWriter(pFilePath,false,Encoding.UTF8)){
            cs.WriteLine(template);
            Debug.Log("Generate UIView success: "+pFilePath);
        }

//            TestReplaceMent();

    }

    private static Dictionary<string, string> BuildProxyReplacements(GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$moduleName$", genParams.moduleName);
        result.Add("$timeDecls$", System.DateTime.Now.ToString());
        return result;
    }

    private static Dictionary<string, string> BuildNetMsgReplacements(GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$moduleName$", genParams.moduleName);
        result.Add("$Author$", genParams.author);
        result.Add("$timeDecls$", System.DateTime.Now.ToString());
        return result; 
    }

    private static Dictionary<string,string> BuildModelReplacements(GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$moduleName$", genParams.moduleName);
        result.Add("$Author$", genParams.author);
        result.Add("$timeDecls$", System.DateTime.Now.ToString());
        return result;
    }

    private static Dictionary<string,string> BuildMonolessControllerReplacements(GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$uiViewName$", genParams.uiViewName);
        return result;
    }

    private static Dictionary<string,string> BuildMonolessControllerAutoGenReplacements(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$uiViewName$", genParams.uiViewName);
        var CreateEventStream = new StringBuilder();
        var CloseEventStream = new StringBuilder();
        var ExposeEventStream = new StringBuilder();
        var EventStreamDecl = new StringBuilder();
    
        comSet.Filter(item=>ButtonObjectStr == item.comType)
            .ForEach(item =>
            {
                EventStreamDecl.AppendFormat("{0} UniRx.IObservable<Unit> On{1}Click{2}\n",indent,item.memberName,getter);
                CreateEventStream.AppendFormat("{0}{1}Evt = View.{1}.AsObservable();\n",indent2,item.memberName);
                CloseEventStream.AppendFormat("{0}{1}Evt = {1}Evt.CloseOnceNull();\n",indent2,item.memberName);

                ExposeEventStream.AppendFormat("{0}private Subject<Unit> {1}Evt;\n",indent,item.memberName);
                ExposeEventStream.AppendFormat("{0}public UniRx.IObservable<Unit> On{1}Click{2}\n",indent,item.memberName,"{");
                ExposeEventStream.AppendFormat("{0}get {1} {2}Evt;{3}\n",indent2,"{return",item.memberName,"}");
                ExposeEventStream.AppendFormat("{0}{1}\n",indent,"}\n");
            });

        result.Add("$EventStreamDecl$",EventStreamDecl.ToString());
        result.Add("$CreateEventStream$", CreateEventStream.ToString());
        result.Add("$CloseEventStream$", CloseEventStream.ToString());
        result.Add("$ExposeEventStream$", ExposeEventStream.ToString());

        return result;
    }

    private static Dictionary<string,string> BuildControllerReplacements(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$uiViewName$",genParams.uiViewName);
        result.Add("$dataClassName$",genParams.dataClassName);
        result.Add("$Author$", genParams.author);
        result.Add("$timeDecls$", System.DateTime.Now.ToString());
        return result;
    }
    
    private static Dictionary<string,string> BuildLogicReplacements(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$uiViewName$",genParams.uiViewName);
        result.Add("$dataClassName$",genParams.dataClassName);
        result.Add("$Author$", genParams.author);
        result.Add("$timeDecls$", System.DateTime.Now.ToString());
        
        var subscribeEvts = new StringBuilder();
        var reactiveImplements = new StringBuilder();
        
        foreach (var item in comSet)
        {
            if (item.comType != ButtonObjectStr)
                continue;
            
            var funName = item.memberName + "Click";
            subscribeEvts.AppendFormat("{0}_disposable.Add(ctrl.On{1}.Subscribe(_=>{1}()));\n", indent3, funName);
            reactiveImplements.AppendFormat("{0}private static void {1}()\n{2}{3}",indent2,funName, indent2 + "{\n", indent2 + "}\n");
        }
        
        result.Add("$subscribeEvts$", subscribeEvts.ToString());
        result.Add("$reactiveImplements$", reactiveImplements.ToString());

        return result;
    }
    
    private static Dictionary<string,string> BuildControllerAutoGenReplacements(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        var result = new Dictionary<string, string>();
        result.Add("$uiViewName$",genParams.uiViewName);
        result.Add("$dataClassName$",genParams.dataClassName);
        
        var EventStreamDecl = new StringBuilder();
        var CloseEventStream = new StringBuilder();
        var CreateEventStream = new StringBuilder();
        var ExposeEventStream = new StringBuilder();
        
        foreach (var item in comSet)
        {
            if (item.comType != ButtonObjectStr)
                continue;
            EventStreamDecl.AppendFormat("{0} UniRx.IObservable<Unit> On{1}Click{2}\n",indent,item.memberName,getter);
            CreateEventStream.AppendFormat("{0}{1}Evt = View.{1}.AsObservable();\n",indent,item.memberName);
            CloseEventStream.AppendFormat("{0}{1}Evt = {1}Evt.CloseOnceNull();\n",indent2,item.memberName);
            
            ExposeEventStream.AppendFormat("{0}private Subject<Unit> {1}Evt;\n",indent,item.memberName);
            ExposeEventStream.AppendFormat("{0}public UniRx.IObservable<Unit> On{1}Click{2}\n",indent,item.memberName,"{");
            ExposeEventStream.AppendFormat("{0}get {1} {2}Evt;{3}\n",indent2,"{return",item.memberName,"}");
            ExposeEventStream.AppendFormat("{0}{1}\n",indent,"}\n");
        }
        
        result.Add("$EventStreamDecl$",EventStreamDecl.ToString());
        result.Add("$CreateEventStream$",CreateEventStream.ToString());
        result.Add("$CloseEventStream$",CloseEventStream.ToString());
        result.Add("$ExposeEventStream$", ExposeEventStream.ToString());
        return result;
    }

    private static Dictionary<string,string> BuildViewAutoGenReplacements(IEnumerable<UIComponentInfo> comSet, GenParams genParams){
        var result = new Dictionary<string, string>();

        result.Add("$uiViewName$",genParams.uiViewName);

        var BindElemets = new StringBuilder();

        var GameObjectBindingDecl = new StringBuilder();

        foreach(var item in comSet){
            if (GameObjectStr == item.comType)
            {
                GameObjectBindingDecl.AppendFormat("{0}public GameObject {1};\n", indent, item.memberName);
                BindElemets.AppendFormat("{0}{1} = root.FindGameObject(\"{2}\");\n", indent2, item.memberName, item.path);
            }
            else if (TransformStr == item.comType)
            {
                GameObjectBindingDecl.AppendFormat("{0}public {1} {2};\n", indent, item.comType, item.memberName);
                BindElemets.AppendFormat("{0}{1} = root.FindTrans(\"{2}\");\n", indent2, item.memberName, item.path);
            }
            else
            {
                GameObjectBindingDecl.AppendFormat("{0}public {1} {2};\n", indent, item.comType, item.memberName);
                BindElemets.AppendFormat("{0}{1} = root.FindScript<{2}>(\"{3}\");\n", indent2, item.memberName, item.comType, item.path);
            }

        }

        result.Add("$BindElemets$",BindElemets.ToString());
        result.Add("$GameObjectBindingDecl$",GameObjectBindingDecl.ToString());

        return result;
    }

//		private static void TestReplaceMent(){
			/*
			TemplateFileName = RelativePath + (genParams.isPartPrefab ? "UIPartTemplate.txt" : "UIViewTemplate.txt");

			var tmp = Application.dataPath+TemplateFileName;

			if (File.Exists(tmp))
			{
				using (var f = File.Open(tmp,FileMode.OpenOrCreate))
				{
					using (var t = new MemoryStream())
					{
						while (true)
						{
							f.ReadByte();
						}
					}
				}
			}else{

			}*/
//		}
    
}

