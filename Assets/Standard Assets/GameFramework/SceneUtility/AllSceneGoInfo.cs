using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SceneUtility;

[System.Serializable]
public class BoundBox
{
    public Vector3[] vertexList;
    public BoundBox()
    {
        this.vertexList = new Vector3[8];
    }
}

[System.Serializable]
public class SceneGoInfo: ISceneQuadObject<SceneGoInfo>
{
    // 序列化用 ushot
    public int id;
    public string gameobjectName;
    public string prefabName;
    public Vector3 pos;
    public Quaternion rotation;
    public Vector3 scale;
    // 序列化用 byte
    public int treeLevel;
    public Bounds bounds;
    public List<LightMapDetail> lightMapDetailList = new List<LightMapDetail>();

    /// <summary>
    /// 业务层动态算，方便修改
    /// </summary>
    [System.NonSerialized]
    private SceneGoType _goType = SceneGoType.Null;

    Bounds ISceneQuadObject<SceneGoInfo>.Bounds
    {
        get { return bounds; }
    }

    public SceneGoType GoType
    {
        get { return _goType; }
        set { _goType = value; }
    }

    bool IEqualityComparer<SceneGoInfo>.Equals(SceneGoInfo x, SceneGoInfo y)
    {
        return x == y;
    }

    int IEqualityComparer<SceneGoInfo>.GetHashCode(SceneGoInfo obj)
    {
        return id.GetHashCode();
    }
}

[System.Serializable]
public class LightMapDetail
{
    // 序列化用 sbyte
    public int index;
    public Vector4 offset;
    public string gameobjectName;
}
	
public class AllSceneGoInfo : ScriptableObject
{
    [SerializeField]
    public SceneGoInfo[] sceneGoInfoArray;


    public byte[] Serialized()
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
        binaryWriter.Write("ver1");
        if (sceneGoInfoArray != null)
        {
            binaryWriter.Write(sceneGoInfoArray.Length);
            for (int i = 0; i < sceneGoInfoArray.Length; i++)
            {
                var item = sceneGoInfoArray[i];
                binaryWriter.Write((ushort)item.id);
                binaryWriter.Write(item.gameobjectName);
                binaryWriter.Write(item.prefabName);
                binaryWriter.Write(item.pos);
                binaryWriter.Write(item.rotation);
                binaryWriter.Write(item.scale);
                binaryWriter.Write((byte)item.treeLevel);
                binaryWriter.Write(item.bounds);

                if (item.lightMapDetailList != null)
                {
                    binaryWriter.Write(item.lightMapDetailList.Count);
                    foreach (LightMapDetail lightMapDetail in item.lightMapDetailList)
                    {
                        binaryWriter.Write((sbyte)lightMapDetail.index);
                        binaryWriter.Write(lightMapDetail.offset);
                        binaryWriter.Write(lightMapDetail.gameobjectName);
                    }
                }
                else
                {
                    binaryWriter.Write(0);
                }
            }
        }
        else
        {
            binaryWriter.Write(0);
        }

        return memoryStream.ToArray();
    }

    public static AllSceneGoInfo Deserialized(byte[] rawBytes)
    {
        MemoryStream memoryStream = new MemoryStream(rawBytes, false);
        BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8);
        string ver = binaryReader.ReadString();
        if (ver == "ver1")
            return DeserializedVer1(binaryReader);
        return null;
    }

    private static AllSceneGoInfo DeserializedVer1(BinaryReader binaryReader)
    {
        AllSceneGoInfo allSceneGoInfo = new AllSceneGoInfo();
        int sceneGoInfoArrayLenght = binaryReader.ReadInt32();
        allSceneGoInfo.sceneGoInfoArray = new SceneGoInfo[sceneGoInfoArrayLenght];
        for (int i = 0; i < sceneGoInfoArrayLenght; i++)
        {
            SceneGoInfo item = new SceneGoInfo();
            allSceneGoInfo.sceneGoInfoArray[i] = item;
            item.id = binaryReader.ReadUInt16();
            item.gameobjectName = binaryReader.ReadString();
            item.prefabName = binaryReader.ReadString();
            item.pos = binaryReader.ReadVector3();
            item.rotation = binaryReader.ReadQuaternion();
            item.scale = binaryReader.ReadVector3();
            item.treeLevel = binaryReader.ReadByte();
            item.bounds = binaryReader.ReadBounds();

            int lightMapDetailLenght = binaryReader.ReadInt32();
            item.lightMapDetailList = new List<LightMapDetail>(lightMapDetailLenght);
            for (int j = 0; j < lightMapDetailLenght; j++)
            {
                LightMapDetail lightMapDetail = new LightMapDetail();
                item.lightMapDetailList.Add(lightMapDetail);
                lightMapDetail.index = binaryReader.ReadSByte();
                lightMapDetail.offset = binaryReader.ReadVector4();
                lightMapDetail.gameobjectName = binaryReader.ReadString();
            }
        }
        return allSceneGoInfo;
    }
}

public static class BinaryHelper
{
    public static void Write(this BinaryWriter binaryWriter, Vector3 v3)
    {
        binaryWriter.Write(v3.x);
        binaryWriter.Write(v3.y);
        binaryWriter.Write(v3.z);
    }
    public static void Write(this BinaryWriter binaryWriter, Quaternion quaternion)
    {
        binaryWriter.Write(quaternion.x);
        binaryWriter.Write(quaternion.y);
        binaryWriter.Write(quaternion.z);
        binaryWriter.Write(quaternion.w);
    }
    public static void Write(this BinaryWriter binaryWriter, Vector4 v4)
    {
        binaryWriter.Write(v4.x);
        binaryWriter.Write(v4.y);
        binaryWriter.Write(v4.z);
        binaryWriter.Write(v4.w);
    }
    public static void Write(this BinaryWriter binaryWriter, Bounds bounds)
    {
        binaryWriter.Write(bounds.center);
        binaryWriter.Write(bounds.size);
    }

    public static Vector3 ReadVector3(this BinaryReader binaryReader)
    {
        return new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
    }

    public static Quaternion ReadQuaternion(this BinaryReader binaryReader)
    {
        return new Quaternion(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
    }

    public static Vector4 ReadVector4(this BinaryReader binaryReader)
    {
        return new Vector4(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
    }
    public static Bounds ReadBounds(this BinaryReader binaryReader)
    {
        return new Bounds(binaryReader.ReadVector3(), binaryReader.ReadVector3());
    }
}