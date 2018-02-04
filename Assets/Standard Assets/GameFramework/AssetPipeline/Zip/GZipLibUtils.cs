using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression;

/// <summary>
/// GZip仅用于压缩单个文件数据
/// </summary>
public class GZipLibUtils
{
    public static byte[] Compress(byte[] inpuBytes, int level = Deflater.BEST_COMPRESSION)
    {
        if (inpuBytes == null || inpuBytes.Length == 0)
        {
            Debug.LogError("Compress error inputBytes Len = 0");
            return null;
        }

        MemoryStream inputStream = new MemoryStream(inpuBytes);
        MemoryStream outputStream = new MemoryStream();
        GZipOutputStream gzipStream = new GZipOutputStream(outputStream);
        gzipStream.SetLevel(level);
        StreamUtils.Copy(inputStream, gzipStream, new byte[4096]);
        gzipStream.Finish();

        var result = outputStream.ToArray();
        gzipStream.Close();
        inputStream.Close();
        return result;
    }

    public static byte[] CompressBytes(byte[] inputBytes, int level = Deflater.BEST_COMPRESSION)
    {
        return Compress(inputBytes, level);
    }

    public static byte[] CompressFile(string filePath, int level = Deflater.BEST_COMPRESSION)
    {
        return Compress(File.ReadAllBytes(filePath), level);
    }

    public static byte[] CompressText(string rawStr, int level = Deflater.BEST_COMPRESSION)
    {
        return Compress(Encoding.UTF8.GetBytes(rawStr), level);
    }

    public static byte[] Decompress(byte[] inpuBytes)
    {
        if (inpuBytes == null || inpuBytes.Length == 0)
        {
            Debug.LogError("Decompress error inputBytes Len = 0");
            return null;
        }

        MemoryStream inputStream = new MemoryStream(inpuBytes);
        GZipInputStream gzipStream = new GZipInputStream(inputStream);
        MemoryStream outputStream = new MemoryStream();
        StreamUtils.Copy(gzipStream, outputStream, new byte[4096]);
        gzipStream.Close();

        var result = outputStream.ToArray();
        outputStream.Close();
        return result;
    }

    public static byte[] DecompressBytes(byte[] inputBytes)
    {
        return Decompress(inputBytes);
    }

    public static byte[] DecompressFile(string filePath)
    {
        return Decompress(File.ReadAllBytes(filePath));
    }

    public static byte[] DecompressBase64Str(string zippedStr)
    {
        return Decompress(Convert.FromBase64String(zippedStr));
    }
}
