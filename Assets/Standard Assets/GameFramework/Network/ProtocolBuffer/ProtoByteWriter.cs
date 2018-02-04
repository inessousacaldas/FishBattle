using System;
using System.IO;
using System.Text;
using UnityEngine;

public class ProtoByteWriter : BinaryWriter
{
    /// <summary>
    ///     Initializes a new instance of the AMFReader class based on the supplied stream and using UTF8Encoding.
    /// </summary>
    /// <param name="stream"></param>
    public ProtoByteWriter(Stream stream) : base(stream)
    {
        Reset();
    }

    public void Reset()
    {
    }

    /// <summary>
    ///     Writes a byte to the current position in the AMF stream.
    /// </summary>
    /// <param name="value">A byte to write to the stream.</param>
    public void WriteByte(byte value)
    {
        BaseStream.WriteByte(value);
    }

    /// <summary>
    ///     Writes a stream of bytes to the current position in the AMF stream.
    /// </summary>
    /// <param name="buffer">The memory buffer containing the bytes to write to the AMF stream</param>
    public void WriteBytes(byte[] buffer)
    {
        for (int i = 0; buffer != null && i < buffer.Length; i++)
            BaseStream.WriteByte(buffer[i]);
    }

    /// <summary>
    ///     Writes a 16-bit unsigned integer to the current position in the AMF stream.
    /// </summary>
    /// <param name="value">A 16-bit unsigned integer.</param>
    public void WriteShort(int value)
    {
        byte[] bytes = BitConverter.GetBytes((ushort) value);
        WriteBigEndian(bytes);
    }

    /// <summary>
    ///     Writes a UTF-8 string to the current position in the AMF stream.
    ///     The length of the UTF-8 string in bytes is written first, as a 16-bit integer, followed by the bytes representing
    ///     the Charactors of the string.
    /// </summary>
    /// <param name="value">The UTF-8 string.</param>
    /// <remarks>Standard or long string header is not written.</remarks>
    public void WriteUTF(string value)
    {
        //null string is not accepted
        //in case of custom serialization leads to TypeError: Error #2007: Parameter value must be non-null.  at flash.utils::ObjectOutput/writeUTF()

        //Length - max 65536.
        UTF8Encoding utf8Encoding = new UTF8Encoding();
        int byteCount = utf8Encoding.GetByteCount(value);
        byte[] buffer = utf8Encoding.GetBytes(value);
        WriteShort(byteCount);
        if (buffer.Length > 0)
            Write(buffer);
    }

    /// <summary>
    ///     Writes a UTF-8 string to the current position in the AMF stream.
    ///     Similar to WriteUTF, but does not prefix the string with a 16-bit length word.
    /// </summary>
    /// <param name="value">The UTF-8 string.</param>
    /// <remarks>Standard or long string header is not written.</remarks>
    public void WriteUTFBytes(string value)
    {
        //Length - max 65536.
        UTF8Encoding utf8Encoding = new UTF8Encoding();
        byte[] buffer = utf8Encoding.GetBytes(value);
        if (buffer.Length > 0)
            Write(buffer);
    }
    private void WriteLongUTF(string value)
    {
        UTF8Encoding utf8Encoding = new UTF8Encoding(true, true);
        uint byteCount = (uint) utf8Encoding.GetByteCount(value);
        byte[] buffer = new byte[byteCount + 4];
        //unsigned long (always 32 bit, big endian byte order)
        buffer[0] = (byte) ((byteCount >> 0x18) & 0xff);
        buffer[1] = (byte) ((byteCount >> 0x10) & 0xff);
        buffer[2] = (byte) ((byteCount >> 8) & 0xff);
        buffer[3] = (byte) (byteCount & 0xff);
#pragma warning disable 0219
        int bytesEncodedCount = utf8Encoding.GetBytes(value, 0, value.Length, buffer, 4);
#pragma warning restore
        if (buffer.Length > 0)
            BaseStream.Write(buffer, 0, buffer.Length);
    }

    public void WriteByteData(byte[] byteData)
    {
        try
        {
            Write(byteData);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    ///     Writes a null type marker to the current position in the AMF stream.
    /// </summary>
    public void WriteNull()
    {
        WriteByte(1);
    }

    /// <summary>
    ///     Writes a double-precision floating point number to the current position in the AMF stream.
    /// </summary>
    /// <param name="value">A double-precision floating point number.</param>
    /// <remarks>No type marker is written in the AMF stream.</remarks>
    public void WriteDouble(double value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        WriteBigEndian(bytes);
    }

    /// <summary>
    ///     Writes a single-precision floating point number to the current position in the AMF stream.
    /// </summary>
    /// <param name="value">A double-precision floating point number.</param>
    /// <remarks>No type marker is written in the AMF stream.</remarks>
    public void WriteFloat(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        WriteBigEndian(bytes);
    }

    /// <summary>
    ///     Writes a 32-bit signed integer to the current position in the AMF stream.
    /// </summary>
    /// <param name="value">A 32-bit signed integer.</param>
    /// <remarks>No type marker is written in the AMF stream.</remarks>
    public void WriteInt32(int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        WriteBigEndian(bytes);
    }

    /// <summary>
    ///     Writes a 32-bit signed integer to the current position in the AMF stream using variable length unsigned 29-bit
    ///     integer encoding.
    /// </summary>
    /// <param name="value">A 32-bit signed integer.</param>
    /// <remarks>No type marker is written in the AMF stream.</remarks>
    public void WriteUInt24(int value)
    {
        byte[] bytes = new byte[3];
        bytes[0] = (byte) (0xFF & (value >> 16));
        bytes[1] = (byte) (0xFF & (value >> 8));
        bytes[2] = (byte) (0xFF & (value >> 0));
        BaseStream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    ///     Writes a Boolean value to the current position in the AMF stream.
    /// </summary>
    /// <param name="value">A Boolean value.</param>
    /// <remarks>No type marker is written in the AMF stream.</remarks>
    public void WriteBoolean(bool value)
    {
        BaseStream.WriteByte(value ? (byte) 1 : (byte) 0);
    }

    /// <summary>
    ///     Writes a 64-bit signed integer to the current position in the AMF stream.
    /// </summary>
    /// <param name="value">A 64-bit signed integer.</param>
    /// <remarks>No type marker is written in the AMF stream.</remarks>
    public void WriteLong(long value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        WriteBigEndian(bytes);
    }

    private void WriteBigEndian(byte[] bytes)
    {
        if (bytes == null)
            return;
        for (int i = bytes.Length - 1; i >= 0; i--)
        {
            BaseStream.WriteByte(bytes[i]);
        }
    }
}
