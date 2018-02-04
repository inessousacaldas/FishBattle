using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

public class HzamrPlugin
{
    private static byte[] AMR_HEAD = Encoding.UTF8.GetBytes("#!AMR\n");
    public const int Channels = 1;
    public const int Frequency = 8000;

#if UNITY_EDITOR
    const string HZAMRDLL = "hzamr";
#elif UNITY_IPHONE 
		const string HZAMRDLL = "__Internal"; 
#else 
		const string HZAMRDLL = "hzamr"; 
#endif

    private enum Mode
    {
        MR475 = 0,
        MR515,
        MR59,
        MR67,
        MR74,
        MR795,
        MR102,
        MR122,
        MRDTX,
    }

    [DllImport(HZAMRDLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr Decoder_Interface_init();

    [DllImport(HZAMRDLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Decoder_Interface_exit(IntPtr decoder);

    [DllImport(HZAMRDLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Decoder_Interface_Decode(IntPtr decoder, byte[] inBuf, Int16[] outBuf, int index);

    [DllImport(HZAMRDLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr Encoder_Interface_init(int num);

    [DllImport(HZAMRDLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Encoder_Interface_exit(IntPtr encoder);

    [DllImport(HZAMRDLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Encoder_Interface_Encode(IntPtr encoder, Mode mode, Int16[] inBuf, byte[] outBuf, int forceSpeech);


    private static short[] encInBuffer = new short[160];
    private static byte[] encOutBuffer = new byte[256];

    private static byte[] buffer = new byte[512];
    private static short[] decOutBuffer = new short[160];
    private static List<float> decOutSamples = new List<float>();
    private static int[] sizes = new int[] { 12, 13, 15, 17, 19, 20, 26, 31, 5, 0, 0, 0, 0, 0, 0, 0 };

    public static byte[] Encode(float[] data, int length, bool header = true)
    {
        if (data == null || length == 0)
        {
            return null;
        }
        MemoryStream stream = new MemoryStream();
        if (header)
        {
            stream.Write(AMR_HEAD, 0, AMR_HEAD.Length);
        }
        IntPtr encoder = Encoder_Interface_init(0);
        int index = 0;
        while (index < length)
        {
            if (length - index >= 160)
            {
                for (int i = 0; i < 160; ++i)
                {
                    encInBuffer[i] = (short)(data[index + i] * 32767.0f);
                }
                int num = Encoder_Interface_Encode(encoder, Mode.MR122, encInBuffer, encOutBuffer, 0);
                stream.Write(encOutBuffer, 0, num);
            }
            index += 160;
        }
        Encoder_Interface_exit(encoder);
        stream.Close();
        return stream.ToArray();
    }

    public static float[] Decode(byte[] data, int index, bool header = true)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }
        if (header)
        {
            index = index + AMR_HEAD.Length;
        }
        decOutSamples.Clear();
        IntPtr decoder = Decoder_Interface_init();
        //int index = 0;
        while (index < data.Length)
        {
            Buffer.BlockCopy(data, index, buffer, 0, 1);
            int size = sizes[(buffer[0] >> 3) & 0x0f];
            if (data.Length - index < size)
            {
                break;
            }
            Buffer.BlockCopy(data, index + 1, buffer, 1, size);
            Decoder_Interface_Decode(decoder, buffer, decOutBuffer, 0);
            index += size + 1;
            for (int i = 0; i < decOutBuffer.Length; ++i)
            {
                decOutSamples.Add(decOutBuffer[i] / 32767.0f);
            }
        }
        Decoder_Interface_exit(decoder);
        return decOutSamples.ToArray();
    }

}
