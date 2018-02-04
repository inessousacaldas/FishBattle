using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

/// <summary>
///     Provides a type converter to convert ProtoByteArray objects to and from various other representations.
/// </summary>
public class ProtoByteArrayConverter : TypeConverter
{
    /// <summary>
    ///     Overloaded. Returns whether this converter can convert the object to the specified type.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="destinationType">A Type that represents the type you want to convert to.</param>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof (byte[]))
            return true;
        return base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    ///     This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="culture">
    ///     A CultureInfo object. If a null reference (Nothing in Visual Basic) is passed, the current
    ///     culture is assumed.
    /// </param>
    /// <param name="value">The Object to convert.</param>
    /// <param name="destinationType">The Type to convert the value parameter to.</param>
    /// <returns>An Object that represents the converted value.</returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
        Type destinationType)
    {
        if (destinationType == typeof (byte[]))
        {
            return (value as ProtoByteArray).MemoryStream.ToArray();
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}

/// <summary>
///     Flex ProtoByteArray. The ProtoByteArray class provides methods and properties to optimize reading, writing, and
///     working with binary data.
/// </summary>
[TypeConverter(typeof (ProtoByteArrayConverter))]
public class ProtoByteArray // : IDataInput, IProtoDataOutput
{
    private MemoryStream _memoryStream;
    private ProtoByteReader _protoReader;
    //private ProtoDataOutput _dataOutput;
    //private ProtoDataInput _dataInput;
    private ProtoByteWriter _protoWriter;

    /// <summary>
    ///     Initializes a new instance of the ProtoByteArray class.
    /// </summary>
    public ProtoByteArray()
    {
        _memoryStream = new MemoryStream();
        _protoReader = new ProtoByteReader(_memoryStream);
        _protoWriter = new ProtoByteWriter(_memoryStream);
        //_dataOutput = new ProtoDataOutput(amfWriter);
        //_dataInput = new ProtoDataInput(amfReader);
    }

    /// <summary>
    ///     Initializes a new instance of the ProtoByteArray class.
    /// </summary>
    /// <param name="ms">The MemoryStream from which to create the current ProtoByteArray.</param>
    public ProtoByteArray(MemoryStream ms)
    {
        _memoryStream = ms;
        _protoReader = new ProtoByteReader(_memoryStream);
        _protoWriter = new ProtoByteWriter(_memoryStream);
        //_dataOutput = new ProtoDataOutput(amfWriter);
        //_dataInput = new ProtoDataInput(amfReader);
    }

    /// <summary>
    ///     Initializes a new instance of the ProtoByteArray class.
    /// </summary>
    /// <param name="buffer">The array of unsigned bytes from which to create the current ProtoByteArray.</param>
    public ProtoByteArray(byte[] buffer)
    {
        _memoryStream = new MemoryStream();
        _memoryStream.Write(buffer, 0, buffer.Length);
        _memoryStream.Position = 0;
        _protoReader = new ProtoByteReader(_memoryStream);
        _protoWriter = new ProtoByteWriter(_memoryStream);
        //_dataOutput = new ProtoDataOutput(amfWriter);
        //_dataInput = new ProtoDataInput(amfReader);
    }

    public ProtoByteArray(ByteArray byteArray)
    {
        byte[] buffer = byteArray.bytes;
        _memoryStream = new MemoryStream();
        _memoryStream.Write(buffer, 0, buffer.Length);
        _memoryStream.Position = 0;
        _protoReader = new ProtoByteReader(_memoryStream);
        _protoWriter = new ProtoByteWriter(_memoryStream);
        //_dataOutput = new ProtoDataOutput(amfWriter);
        //_dataInput = new ProtoDataInput(amfReader);
    }

    /// <summary>
    ///     Gets the length of the ProtoByteArray object, in bytes.
    /// </summary>
    public uint Length
    {
        get { return (uint) _memoryStream.Length; }
    }

    /// <summary>
    ///     Gets or sets the current position, in bytes, of the file pointer into the ProtoByteArray object.
    /// </summary>
    public int Position
    {
        get { return (int) _memoryStream.Position; }
        set { _memoryStream.Position = value; }
    }

    /// <summary>
    ///     Gets the MemoryStream from which this ProtoByteArray was created.
    /// </summary>
    public MemoryStream MemoryStream
    {
        get { return _memoryStream; }
    }

    public long bytesAvailable
    {
        get { return Length - Position; }
    }

    public int capacity
    {
        get { return _memoryStream.Capacity; }
        set { _memoryStream.Capacity = value; }
    }

    /// <summary>
    ///     Returns the array of unsigned bytes from which this ProtoByteArray was created.
    /// </summary>
    /// <returns>
    ///     The byte array from which this ProtoByteArray was created, or the underlying array if a byte array was not
    ///     provided to the ProtoByteArray constructor during construction of the current instance.
    /// </returns>
    public byte[] GetBuffer()
    {
        return _memoryStream.GetBuffer();
    }

    public byte[] ToArray()
    {
        return _memoryStream.ToArray();
    }

    /// <summary>
    ///     Reads a Boolean from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public bool ReadBoolean()
    {
        return _protoReader.ReadBoolean();
    }

    /// <summary>
    ///     Reads a signed byte from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public byte ReadByte()
    {
        return _protoReader.ReadByte();
    }

    /// <summary>
    ///     Reads length bytes of data from the byte stream or byte array.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    public void ReadBytes(byte[] bytes, uint offset, uint length)
    {
        byte[] tmp = _protoReader.ReadBytes((int) length);
        for (int i = 0; i < tmp.Length; i++)
            bytes[i + offset] = tmp[i];
    }

    /// <summary>
    ///     Reads an IEEE 754 double-precision floating point number from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public double ReadDouble()
    {
        return _protoReader.ReadDouble();
    }

    /// <summary>
    ///     Reads an IEEE 754 single-precision floating point number from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public float ReadFloat()
    {
        return _protoReader.ReadFloat();
    }

    /// <summary>
    ///     Reads a signed 32-bit integer from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public int ReadInt()
    {
        return _protoReader.ReadInt32();
    }

    ///// <summary>
    ///// Reads an object from the byte stream or byte array, encoded in AMF serialized format. 
    ///// </summary>
    ///// <returns></returns>
    //public object ReadObject()
    //{
    //       return _protoReader.ReadData(_memoryStream.ToArray());
    //}

    public string ReadBase64ByteStr()
    {
        byte[] bytes = _memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    ///     Reads a signed 16-bit integer from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public short ReadShort()
    {
        return _protoReader.ReadInt16();
    }

    /// <summary>
    ///     Reads an unsigned byte from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public byte ReadUnsignedByte()
    {
        return _protoReader.ReadByte();
    }

    /// <summary>
    ///     Reads an unsigned 32-bit integer from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public uint ReadUnsignedInt()
    {
        return (uint) _protoReader.ReadInt32();
    }

    /// <summary>
    ///     Reads an unsigned 16-bit integer from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public ushort ReadUnsignedShort()
    {
        return _protoReader.ReadUInt16();
    }

    /// <summary>
    ///     Reads a UTF-8 string from the byte stream or byte array.
    /// </summary>
    /// <returns></returns>
    public string ReadUTF()
    {
        return _protoReader.ReadString();
    }

    /// <summary>
    ///     Reads a sequence of length UTF-8 bytes from the byte stream or byte array, and returns a string.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public string ReadUTFBytes(uint length)
    {
        return _protoReader.ReadUTF((int) length);
    }

    /// <summary>
    ///     Writes a Boolean value.
    /// </summary>
    /// <param name="value"></param>
    public void WriteBoolean(bool value)
    {
        _memoryStream.WriteByte(value ? (byte) 1 : (byte) 0);

        _protoWriter.WriteBoolean(value);
    }

    /// <summary>
    ///     Writes a byte.
    /// </summary>
    /// <param name="value"></param>
    public void WriteByte(byte value)
    {
        _protoWriter.WriteByte(value);
    }

    /// <summary>
    ///     Writes a sequence of length bytes from the specified byte array, bytes, starting offset(zero-based index) bytes
    ///     into the byte stream.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    public void WriteBytes(byte[] bytes, int offset, int length)
    {
        for (int i = offset; i < offset + length; i++)
            _protoWriter.WriteByte(bytes[i]);
    }

    /// <summary>
    ///     Writes an IEEE 754 double-precision (64-bit) floating point number.
    /// </summary>
    /// <param name="value"></param>
    public void WriteDouble(double value)
    {
        _protoWriter.WriteDouble(value);
    }

    /// <summary>
    ///     Writes an IEEE 754 single-precision (32-bit) floating point number.
    /// </summary>
    /// <param name="value"></param>
    public void WriteFloat(float value)
    {
        _protoWriter.WriteFloat(value);
    }

    /// <summary>
    ///     Writes a 32-bit signed integer.
    /// </summary>
    /// <param name="value"></param>
    public void WriteInt(int value)
    {
        _protoWriter.WriteInt32(value);
    }

    public void WriteByteData(byte[] bytes)
    {
        _protoWriter.WriteByteData(bytes);
    }

    public void WriteByteData(string byteStr)
    {
        byte[] bytes = Convert.FromBase64String(byteStr);
        _protoWriter.WriteByteData(bytes);
    }

    /// <summary>
    ///     Writes a 16-bit integer.
    /// </summary>
    /// <param name="value"></param>
    public void WriteShort(short value)
    {
        _protoWriter.WriteShort(value);
    }

    /// <summary>
    ///     Writes a 32-bit unsigned integer.
    /// </summary>
    /// <param name="value"></param>
    public void WriteUnsignedInt(uint value)
    {
        _protoWriter.WriteInt32((int) value);
    }

    /// <summary>
    ///     Writes a UTF-8 string to the byte stream.
    ///     The length of the UTF-8 string in bytes is written first, as a 16-bit integer, followed by
    ///     the bytes representing the Charactors of the string.
    /// </summary>
    /// <param name="value"></param>
    public void WriteUTF(string value)
    {
        _protoWriter.WriteUTF(value);
    }

    /// <summary>
    ///     Writes a UTF-8 string. Similar to writeUTF, but does not prefix the string with a 16-bit length word.
    /// </summary>
    /// <param name="value"></param>
    public void WriteUTFBytes(string value)
    {
        _protoWriter.WriteUTFBytes(value);
    }

    public void compress()
    {
        byte[] data = ZipLibUtils.Compress(_memoryStream.ToArray());

        _memoryStream.SetLength(0);
        Position = 0;

        WriteBytes(data, 0, data.Length);
    }

    public void uncompress()
    {
        byte[] data = ZipLibUtils.Uncompress(_memoryStream.ToArray());

        _memoryStream.SetLength(0);
        Position = 0;

        WriteBytes(data, 0, data.Length);
    }
}