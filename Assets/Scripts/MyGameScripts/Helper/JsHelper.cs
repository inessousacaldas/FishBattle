using System;
using SharpKit.JavaScript;
using System.Collections.Generic;
using LITJson;
using System.Linq;
using System.Reflection;
using UnityEngine;
using AppProtobuf;

[assembly: JsMethod(TargetType = typeof(string), TargetMethod = ".ctor", OmitNewOperator = true)]
[assembly: JsProperty(TargetType = typeof(Time), Global = true, NativeField = true,
    TargetProperty = "deltaTime", Name = "_jsComManager.dT")]
[assembly: JsProperty(TargetType = typeof(Time), Global = true, NativeField = true,
    TargetProperty = "fixedDeltaTime", Name = "_jsComManager.fDT")]
[assembly: JsProperty(TargetType = typeof(Time), Global = true, NativeField = true,
    TargetProperty = "unscaledDeltaTime", Name = "_jsComManager.uDT")]

[JsType(JsMode.Prototype)]
public static class JsHelper
{
    [JsMethod(Code = @"return Number.isInteger(obj);")]
    public static bool IsInt(this object obj)
    {
        return obj is int;
    }

    [JsMethod(Code = @"return typeof obj === 'number';")]
    public static bool IsFloat(this object obj)
    {
        return obj is float;
    }

    [JsMethod(Code = @"return String.fromCharCode(c);")]
    public static string FromCharCode(this char c)
    {
        return c.ToString();
    }

    #region Collection

    [JsMethod(Code = @"var count = list.get_Count();
    if (count == 0)
        return null;
    var randomIndex = Math.floor(count*Math.random());
    return list.get_Item$$Int32(randomIndex);")]
    public static T Random<T>(this IList<T> list)
    {
        var count = list.Count;

        if (count == 0)
            return default(T);

        return list.ElementAt(UnityEngine.Random.Range(0, count));
    }

    #endregion

    #region Json

    [JsMethod(Code = @"return JsonUtils.stringify(obj, tFlag);")]
    public static string ToJson(object obj, bool tFlag = false)
    {
        if (tFlag)
        {
            JsonWriter writer = new JsonWriter();
            writer.TypeWriter = ProtobufMap.getType;
            writer.TypeHinting = true;
            JsonMapper.ToJson(obj, writer);
            return writer.ToString();
        }
        else
        {
            return JsonMapper.ToJson(obj);
        }
    }

    [JsMethod(Code = @"return JsonUtils.parse(json, T);", IgnoreGenericArguments = false)]
    public static T ToObject<T>(string json)
    {
        try
        {
            return JsonMapper.ToObject<T>(json);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return default(T);
        }
    }

    [JsMethod(Code = @"return JsonUtils.parse(json, T, TChild);", IgnoreGenericArguments = false)]
    public static T ToCollection<T, TChild>(string json)
    {
        return JsonMapper.ToObject<T>(json);
    }

    #endregion

    #region Protocol

    [JsMethod(Code = @"return obj.id != null ? obj.id : 0;")]
    public static int GetDataObjectKey(object obj)
    {
        FieldInfo field = obj.GetType().GetField("id", BindingFlags.Public | BindingFlags.Instance);

        if (field == null)
        {
            return 0;
        }

        int objId = (int)field.GetValue(obj);
        return objId;
    }

    public static object ParseProtoObj(ByteArray ba, bool needUnCompress)
    {
        if (needUnCompress)
        {
            ba.UnCompress();
        }

#if ENABLE_JSB
        string byteStr = ba.ToBase64String();

        object decodeObj = Decode(byteStr);
        return decodeObj;
#else
        return ProtobufUtilsNet.parseTypedMessageFrom(ba.bytes);
#endif
    }

    [JsMethod(IgnoreGenericArguments = false)]
    public static T ParseJsz<T>(ByteArray ba, bool needUnCompress)
    {
        if (needUnCompress)
        {
            ba.UnCompress();
        }

#if ENABLE_JSB
        try
        {
            return ToObject<T>(ba.ToUTF8String());
        }
        catch (JsError e)
        {
            //兼容处理,PBZ->JSZ的过渡,因为以前包内资源为PBZ格式
            Debug.LogWarning("ParseJsz Error:" + e.message + "\n" + e.StackTrace);
            return (T)ParseProtoObj(ba, needUnCompress);
        }
#else
        try
        {
            JsonReader reader = new JsonReader(ba.ToUTF8String());
            reader.TypeHinting = true;
            reader.TypeReader = typeParam =>
            {
                int typeId = Convert.ToInt32(typeParam);
                return ProtobufMap.getClass(typeId);
            };
            return JsonMapper.ToObject<T>(reader);
        }
        catch (Exception e)
        {
            //兼容处理,PBZ->JSZ的过渡,因为以前包内资源为PBZ格式
            Debug.LogWarning("ParseJsz Error:" + e);
            return (T)ParseProtoObj(ba, needUnCompress);
        }
#endif
    }

    public static ByteArray EncodeObjToJsz(object obj)
    {
        string json = ToJson(obj, true);
        ByteArray ba = ByteArray.CreateFromUtf8(json);
        ba.Compress();
        return ba;
    }

    public static ProtoByteArray EncodeProtoObj(ProtoByteArray protoByteArray, object obj)
    {
#if ENABLE_JSB
        string str = Encode(obj);
        protoByteArray.WriteByteData(str);
#else
        byte[] byteData = ProtobufUtilsNet.packIntoData(obj);
        protoByteArray.WriteByteData(byteData);
#endif
        return protoByteArray;
    }

    public static ByteArray EncodeObjToPbz(object obj)
    {
        ByteArray byteArray = null;
#if ENABLE_JSB
        string str = Encode(obj);
        byteArray = ByteArray.CreateFromBase64(str);
#else
        byte[] byteData = ProtobufUtilsNet.packIntoData(obj);
        byteArray = new ByteArray(byteData);
#endif
        byteArray.Compress();
        return byteArray;
    }

    [JsMethod(Code = @"JsonUtils.TypeGetter = AppProtobuf.ProtobufMap.getClass;
    JsonUtils.TypeWritter = AppProtobuf.ProtobufMap.getType;
    ProtoJsUtils.loadProto();")]
    public static void SetupJsProto()
    {
    }

    [JsMethod(Code = @"return ProtoJsUtils.encodeToBase64(dto);")]
    public static string Encode(object dto)
    {
        return null;
    }

    [JsMethod(Code = @"return ProtoJsUtils.parseBase64(b64Str);")]
    public static JsObject Decode(string b64Str)
    {
        return null;
    }

    #endregion
}