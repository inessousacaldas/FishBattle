using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Checksums;
using UnityEngine;

public class CRC32Hashing
{
    private static Crc32 _crc;

    private static Crc32 Crc
    {
        get
        {
            if (_crc == null)
                _crc = new Crc32();
            return _crc;
        }
    }

    /// <summary>
    ///     使用utf8编码将字符串散列
    /// </summary>
    /// <param name="sourceString">要散列的字符串</param>
    /// <returns>散列后的字符串</returns>
    public static uint HashString(string sourceString)
    {
        return HashString(Encoding.UTF8, sourceString);
    }

    /// <summary>
    ///     使用指定的编码将字符串散列
    /// </summary>
    /// <param name="encode">编码</param>
    /// <param name="sourceString">要散列的字符串</param>
    /// <returns>散列后的字符串</returns>
    public static uint HashString(Encoding encode, string sourceString)
    {
        Crc.Reset();
        Crc.Update(encode.GetBytes(sourceString));
        return (uint) Crc.Value;
    }

    public static uint HashFile(string path)
    {
        try
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            return HashBytes(fileBytes);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return 0U;
        }
    }

    public static uint HashBytes(byte[] bytes)
    {
        Crc.Reset();
        Crc.Update(bytes);
        return (uint) Crc.Value;
    }

    public static uint HashBytes(ByteArray byteArray)
    {
        Crc.Reset();
        Crc.Update(byteArray.bytes);
        return (uint) Crc.Value;
    }
}