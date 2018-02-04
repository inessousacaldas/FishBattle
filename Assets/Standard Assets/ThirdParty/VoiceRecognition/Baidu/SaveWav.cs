//	Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveWav
{
    private const int HEADER_SIZE = 44;

    public static byte[] ToWav(float[] samplesBuf, int hz, int channels, int samples)
    {
        //float[] samplesBuf = new float[clip.samples];
        //clip.GetData(samplesBuf, 0);

        byte[] wavData = new byte[HEADER_SIZE + samplesBuf.Length*2];
        using (MemoryStream memStream = new MemoryStream(wavData))
        {
            // empty header
            byte[] emptyByte = new byte[HEADER_SIZE];
            memStream.BeginWrite(emptyByte, 0, emptyByte.Length, delegate(IAsyncResult ar) { memStream.EndWrite(ar); },
                null);

            // sound data
            short[] intData = new short[samplesBuf.Length];
            //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

            byte[] bytesData = new byte[samplesBuf.Length*2];
            //bytesData array is twice the size of
            //dataSource array because a float converted in Int16 is 2 bytes.

            int rescaleFactor = 32767; //to convert float to Int16

            for (int i = 0; i < samplesBuf.Length; i++)
            {
                intData[i] = (short) (samplesBuf[i]*rescaleFactor);
                byte[] byteArr = new byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i*2);
            }

            memStream.BeginWrite(bytesData, 0, bytesData.Length, delegate(IAsyncResult ar) { memStream.EndWrite(ar); },
                null);

            // header
            //int hz = clip.frequency;
            //int channels = clip.channels;
            //int samples = clip.samples;

            memStream.Seek(0, SeekOrigin.Begin);

            byte[] riff = Encoding.UTF8.GetBytes("RIFF");
            memStream.BeginWrite(riff, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            byte[] chunkSize = BitConverter.GetBytes(memStream.Length - 8);
            memStream.BeginWrite(chunkSize, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            //Byte[ ] tempByteArray = MergerArray( riff , chunkSize );

            byte[] wave = Encoding.UTF8.GetBytes("WAVE");
            memStream.BeginWrite(wave, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            //tempByteArray = MergerArray( tempByteArray , wave );

            byte[] fmt = Encoding.UTF8.GetBytes("fmt ");
            memStream.BeginWrite(fmt, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            //tempByteArray = MergerArray( tempByteArray , fmt );

            byte[] subChunk1 = BitConverter.GetBytes(16);
            //tempByteArray = MergerArray( tempByteArray , subChunk1 );
            memStream.BeginWrite(subChunk1, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            ushort two = 2;
            ushort one = 1;

            byte[] audioFormat = BitConverter.GetBytes(one);
            //tempByteArray = MergerArray( tempByteArray , audioFormat );
            memStream.BeginWrite(audioFormat, 0, 2, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            byte[] numChannels = BitConverter.GetBytes(channels);
            //tempByteArray = MergerArray( tempByteArray , numChannels );
            memStream.BeginWrite(numChannels, 0, 2, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            byte[] sampleRate = BitConverter.GetBytes(hz);
            //tempByteArray = MergerArray( tempByteArray , sampleRate );
            memStream.BeginWrite(sampleRate, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            byte[] byteRate = BitConverter.GetBytes(hz*channels*2);
                // sampleRate * bytesPerSample*number of channels, here 44100*2*2
            //tempByteArray = MergerArray( tempByteArray , byteRate );
            memStream.BeginWrite(byteRate, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            ushort blockAlign = (ushort) (channels*2);
            //tempByteArray = MergerArray( tempByteArray , BitConverter.GetBytes(blockAlign) );
            memStream.BeginWrite(BitConverter.GetBytes(blockAlign), 0, 2,
                delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            ushort bps = 16;
            byte[] bitsPerSample = BitConverter.GetBytes(bps);
            //tempByteArray = MergerArray( tempByteArray , bitsPerSample );
            memStream.BeginWrite(bitsPerSample, 0, 2, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            byte[] datastring = Encoding.UTF8.GetBytes("data");
            //tempByteArray = MergerArray( tempByteArray , datastring );
            memStream.BeginWrite(datastring, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

            byte[] subChunk2 = BitConverter.GetBytes(samples*channels*2);
            //tempByteArray = MergerArray( tempByteArray , subChunk2 );
            memStream.BeginWrite(subChunk2, 0, 4, delegate(IAsyncResult ar) { memStream.EndWrite(ar); }, null);

//			memStream.BeginWrite(tempByteArray, 0, tempByteArray.Length , delegate(IAsyncResult ar) {
//				memStream.EndWrite( ar );
//			} ,null );
        }
        return wavData;
    }

    public static byte[] MergerArray(byte[] ary_1, byte[] ary_2)
    {
        byte[] newByte = new byte[ary_1.Length + ary_2.Length];
        ary_1.CopyTo(newByte, 0);
        ary_2.CopyTo(newByte, ary_1.Length);
        return newByte;
    }


    public static AudioClip TrimSilence(AudioClip clip, float min)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
    }

    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
    {
        return TrimSilence(samples, min, channels, hz, false, false);
    }

    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
    {
        int i;

        for (i = 0; i < samples.Count; i++)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(0, i);

        for (i = samples.Count - 1; i > 0; i--)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(i, samples.Count - i);

        var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);

        clip.SetData(samples.ToArray(), 0);

        return clip;
    }

    private static FileStream CreateEmpty(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }
        iOSUtility.ExcludeFromBackupUrl(filepath);
        return fileStream;
    }

    private static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        byte[] bytesData = new byte[samples.Length*2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short) (samples[i]*rescaleFactor);
            byte[] byteArr = new byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i*2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    public static byte[] GetByteUntoWav(AudioClip clip)
    {
        float[] samplesBuf = new float[clip.samples];
        clip.GetData(samplesBuf, 0);

        byte[] wavData = new byte[samplesBuf.Length];
        using (MemoryStream memStream = new MemoryStream(wavData))
        {
            byte[] tempByte = new byte[samplesBuf.Length];
            for (int index = 0; index < samplesBuf.Length; index ++)
            {
                byte[] indexByte = new byte[1];
                indexByte.CopyTo(tempByte, index);
            }

            memStream.BeginWrite(tempByte, 0, tempByte.Length, delegate(IAsyncResult ar) { memStream.EndWrite(ar); },
                null);
        }

        return wavData;
    }

    /*
	static void WriteHeader(FileStream fileStream, AudioClip clip) {

		var hz = clip.frequency;
		var channels = clip.channels;
		var samples = clip.samples;

		fileStream.Seek(0, SeekOrigin.Begin);

		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		fileStream.Write(riff, 0, 4);

		Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
		fileStream.Write(chunkSize, 0, 4);

		Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
		fileStream.Write(wave, 0, 4);

		Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
		fileStream.Write(fmt, 0, 4);

		Byte[] subChunk1 = BitConverter.GetBytes(16);
		fileStream.Write(subChunk1, 0, 4);

		UInt16 two = 2;
		UInt16 one = 1;

		Byte[] audioFormat = BitConverter.GetBytes(one);
		fileStream.Write(audioFormat, 0, 2);

		Byte[] numChannels = BitConverter.GetBytes(channels);
		fileStream.Write(numChannels, 0, 2);

		Byte[] sampleRate = BitConverter.GetBytes(hz);
		fileStream.Write(sampleRate, 0, 4);

		Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
		fileStream.Write(byteRate, 0, 4);

		UInt16 blockAlign = (ushort) (channels * 2);
		fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		UInt16 bps = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(bps);
		fileStream.Write(bitsPerSample, 0, 2);

		Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
		fileStream.Write(datastring, 0, 4);

		Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
		fileStream.Write(subChunk2, 0, 4);

//		fileStream.Close();
	}
	*/
}