// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : fmd
// Created  : 8/1/2017 2:44:15 PM
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.UI;
using Assets.Scripts.MyGameScripts.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public class GMDtoConnVO
{
    public object dto;
    public string time;
    public int size;
    public int count;
    public string stackTrace;

    public string Name
    {
        get {
            if(dto != null)
            {
                if(dto is GeneralRequest)
                {
                    return "Req_" + (dto as GeneralRequest).action;
                }
                else
                {
                    return dto.ToString().Split('.')[1];
                }
            }else
            {
                return "未知协议";
            }
        }
    }
}

public sealed partial class GMDataMgr
{
    public sealed partial class GMDtoConnData:IGMDtoConnData
    {
        public int maxCount = 50;
        public bool isShield = true;
        public GMDtoConnItemController curCtrl;
        private List<GMDtoConnVO> listDto = new List<GMDtoConnVO>();
        private static Queue<GMDtoConnVO> readlyDtoList = new Queue<GMDtoConnVO>(); //发送和接受的列表准备列表
        private static List<string> hideMessage = new List<string> { "m_move_pos_tos", "m_system_hb_toc", "m_system_hb_tos", "m_move_role_walk_tos", "m_move_pet_walk_tos", "m_move_start_tos", "m_move_stop_tos" };//, "m_move_path_tos"
        private static List<string> hideMessage2 = new List<string>(); //不屏蔽高频协议

        public GMDtoConnData()
        {

        }

        public void InitData()
        {
            JSTimer.Instance.SetupTimer("GMDtoConnData.InitData",Loop,0.03f);
        }

        public void Dispose()
        {
            JSTimer.Instance.RemoveTimerUpdateHandler("GMDtoConnData.InitData",Loop);
        }

        private void Loop()
        {
            lock(readlyDtoList)
            {
                var count = readlyDtoList.Count;
                for(var i = 0;i < count;i++)
                {
                    try
                    {
                        if(listDto.Count > maxCount)
                        {
                            listDto.RemoveAt(0);
                        }
                        listDto.Add(readlyDtoList.Dequeue());
                    }
                    catch(Exception e)
                    {
                        GameLog.ShowError(e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 添加一个协议信息到列表
        /// </summary>
        /// <param name="value"></param>
        public void AddSendDto(GMDtoConnVO value)
        {
            if(GMDataMgr.isOpenDtoConn == false) return;
            if(isShield)
            {
                if(hideMessage.Contains(value.Name))
                {
                    return;
                }
            }
            else
            {
                if(hideMessage2.Contains(value.Name))
                {
                    return;
                }
            }

            var now = DateTime.Now;
            value.time = now.Hour + "时" + now.Minute + "分" + now.Second + "秒" + now.Millisecond + "毫秒";
            readlyDtoList.Enqueue(value);
        }

        public List<GMDtoConnVO> ListDto
        {
            get { return listDto; }
        }

        public void Clear()
        {
            if(listDto.Count > 0)
            {
                listDto.Clear();
            }
            curCtrl = null;
        }

        private string[] posList = new string[] { "pos", "point" };
        public StringBuilder stringBuilder = new StringBuilder();
        private int LayerLength = 1;//递归的层级
        public StringBuilder PrintProto(object proto)
        {
            // string classpackage = "Proto" + "." + proto.GetClassName();
             if(proto == null)
            {
                return stringBuilder;
            } 
            Type atype = proto.GetType();
            var FieldsList = atype.GetFields();
            foreach(var item in FieldsList)
            {
                stringBuilder.Append(PrintEmptyStringLayer() + GetFileInfo(item));
                //如果是数组类型的
                if(item.FieldType.Name.ToLower().Contains("list"))
                {
                    string FileName = item.FieldType.Name;
                    string FileTypeName = item.FieldType.GetGenericArguments()[0].Name;
                    var array = ObjectListToArray(GetValue(proto,item.Name));
                    if(array == null)
                    {
                        stringBuilder.Append(" = null\n");
                        continue;
                    }
                    stringBuilder.Append(" " + array.Length + "个元素[" + GameColor.GREEN + "]【[-]");
                    //是基本类型，直接打印
                    if(CheckIsBaseType(FileTypeName.ToLower()))
                    {
                        for(int i = 0;i < array.Length;i++)
                        {
                            LayerLength++;
                            stringBuilder.Append("\n" + PrintEmptyStringLayer());
                            stringBuilder.Append("[" + GetColorByLayer() + "]" + (i + 1) + "、{\n[-]");
                            stringBuilder.Append(PrintEmptyStringLayer() + array.GetValue(i) + "\n");
                            stringBuilder.Append(PrintEmptyStringLayer() + "[" + GetColorByLayer() + "]}[-]");
                            LayerLength--;
                            //stringBuilder.Append(array.GetValue(i));
                            //if(i != array.Length - 1)
                            //{
                            //    stringBuilder.Append(",");
                            //}
                        }
                        stringBuilder.Append(PrintEmptyStringLayer() + "[" + GameColor.GREEN + "]" + "】\n[-]");
                    }
                    //是数组，但不是基本类型，递归
                    else
                    {
                        for(int i = 0;i < array.Length;i++)
                        {
                            LayerLength++;
                            stringBuilder.Append("\n" + PrintEmptyStringLayer());
                            stringBuilder.Append("[" + GetColorByLayer() + "]" + (i + 1) + "、{\n[-]");
                            PrintProto(array.GetValue(i));
                            stringBuilder.Append(PrintEmptyStringLayer() + "[" + GetColorByLayer() + "]}[-]");
                            LayerLength--;
                        }
                        stringBuilder.Append("\n" + PrintEmptyStringLayer() + "[" + GameColor.GREEN + "]" + "】\n[-]");
                    }
                }
                //如果是基本类型的
                else if(CheckIsBaseType(item.FieldType.Name.ToLower()))
                {
                    if(GetValue(proto,item.Name) != null)
                    {
                        stringBuilder.Append("= " + GetValue(proto,item.Name).ToString());
                        stringBuilder.Append("\n");
                    }
                    else
                    {
                        stringBuilder.Append("= nulll\n");
                    }
                }
                //如果是自定义类型，递归吧
                else
                {
                    if(GetValue(proto,item.Name) != null)
                    {
                        LayerLength++;
                        stringBuilder.Append("\n");
                        //stringBuilder.Append(PrintEmptyStringLayer());
                        PrintProto(GetValue(proto,item.Name) );
                        LayerLength--;
                    }
                    else
                    {
                        stringBuilder.Append("= nulll\n");
                    }
                }
            }
            return stringBuilder;
        }
        private string GetColorByLayer()
        {
            if(LayerLength == 2)
            {
                return GameColor.RED;
            }
            if(LayerLength == 3)
            {
                return GameColor.BLUE;
            }
            else
            {
                return GameColor.GOLD;
            }
        }
        private string PrintEmptyStringLayer()
        {
            string result = "";
            for(int i = 0;i < LayerLength;i++)
            {
                if(i != 0)
                {
                    result += "  ";
                    for(int j = 0;j < LayerLength;j++)
                    {
                        result += " ";
                    }
                }
            }
            return result;
        }
        private string GetFileInfo(FieldInfo info)
        {
            string result = "";
            result += "[" + GameColor.ATTR_LIGHT + "]" + info.FieldType.Name + "[-] ";
            result += info.Name + " ";
            return result;
        }
        private bool CheckIsBaseType(string t)
        {
            return t == "sbyte" || t == "byte" || t == "int16" || t == "uint16" || t == "int32" || t == "uint32" || t == "int64" || t == "uint64" || t == "string" || t == "boolean" || t == "bytes" || t == "single" || t == "double" || t == "object";
        }

        public object GetValue(object resp, string name)
        {
            return resp.GetType().GetField(name).GetValue(resp);
        }

        public void SetValue(object resp,string name,object value)
        {
            try
            {
                resp.GetType().GetField(name).SetValue(resp,value);
            }
            catch
            {
                GameLog.ShowError("协议" + this.GetType().ToString() + ":" + name + "类型错误");
            }
        }

        public Array ObjectListToArray(object resp)
        {
            if(resp != null)
            {
                var methods = resp.GetType().GetMethods();
                Array tempList = null;
                for(var i = 0;i < methods.Length;i++)
                {
                    if(methods[i].Name == "ToArray")
                    {
                        tempList = methods[i].Invoke(resp,null) as Array;
                    }
                }
                return tempList;
            }
            else
            {
                return null;
            }
        }
    }
}
