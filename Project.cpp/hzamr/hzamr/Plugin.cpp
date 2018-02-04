
#include "Pluging.h"
#include "interf_enc.h"

#include "wavwriter.h"
#include "interf_dec.h"

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>
#include <errno.h>

const EXPORT_API char*  PrintHello(){
	return "HelloHzamr ~~~~~~~~~~";
}

int wav_read_buf_data(char* wavBuf,int* wavBufIndex,int* wavBufSize, unsigned char* data, unsigned int length) 
{
	if (length > *wavBufSize)
		length = *wavBufSize;
	//n = fread(data, 1, length, wr->wav);
	memcpy(data,&wavBuf[*wavBufIndex],length);
	*wavBufIndex += length;
	*wavBufSize -= length;
	return (int) length;
}

unsigned int EXPORT_API wav_to_amr(char wavBuf[],int wavBufSize,char amrBuf[])
{
	int wavBufIndex = 44;
	enum Mode mode = MR122;
	int channels = 1;
	int sampleRate = 8000;

	int inputSize = channels*2*160;
	uint8_t* inputBuf = (uint8_t*) malloc(inputSize);

	int dtx = 0;
	void *amr = Encoder_Interface_init(dtx);

	unsigned int amrBufIndex = 0;
	char* armHead = "#!AMR\n";
	memcpy(&amrBuf[amrBufIndex],armHead,6);
	amrBufIndex += 6;

	wavBufSize -= wavBufIndex;
	while (1) {
		short buf[160];
		uint8_t outbuf[500];
		int read, i, n;
		read = wav_read_buf_data(wavBuf, &wavBufIndex, &wavBufSize, inputBuf, inputSize);
		read /= channels;
		read /= 2;
		if (read < 160)
			break;
		for (i = 0; i < 160; i++) {
			const uint8_t* in = &inputBuf[2*channels*i];
			buf[i] = in[0] | (in[1] << 8);
		}
		n = Encoder_Interface_Encode(amr, mode, buf, outbuf, 0);
		//fwrite(outbuf, 1, n, out);
		memcpy(&amrBuf[amrBufIndex],outbuf,n);
		amrBufIndex += n;
	}

	free(inputBuf);
	Encoder_Interface_exit(amr);

	return amrBufIndex;
}

#define AMR_MAGIC_NUMBER "#!AMR\n"

#define PCM_FRAME_SIZE 160 // 8khz 8000*0.02=160
#define MAX_AMR_FRAME_SIZE 32
#define AMR_FRAME_COUNT_PER_SECOND 50


// 从WAVE文件读一个完整的PCM音频帧
// 返回值: 0-错误 >0: 完整帧大小
int ReadPCMFrame(short speech[], FILE* fpwave, int nChannels, int nBitsPerSample)
{
	int nRead = 0;
	int x = 0, y = 0;
	unsigned short ush1 = 0, ush2 = 0, ush = 0;

	// 原始PCM音频帧数据
	unsigned char pcmFrame_8b1[PCM_FRAME_SIZE];
	unsigned char pcmFrame_8b2[PCM_FRAME_SIZE << 1];
	unsigned short pcmFrame_16b1[PCM_FRAME_SIZE];
	unsigned short pcmFrame_16b2[PCM_FRAME_SIZE << 1];

	if (nBitsPerSample == 8 && nChannels == 1)
	{
		nRead = fread(pcmFrame_8b1, (nBitsPerSample / 8), PCM_FRAME_SIZE*nChannels, fpwave);
		for (x = 0; x < PCM_FRAME_SIZE; x++)
		{
			speech[x] = (short)((short)pcmFrame_8b1[x] << 7);
		}
	}
	else
		if (nBitsPerSample == 8 && nChannels == 2)
		{
			nRead = fread(pcmFrame_8b2, (nBitsPerSample / 8), PCM_FRAME_SIZE*nChannels, fpwave);
			for (x = 0, y = 0; y < PCM_FRAME_SIZE; y++, x += 2)
			{
				// 1 - 取两个声道之左声道
				speech[y] = (short)((short)pcmFrame_8b2[x + 0] << 7);
				// 2 - 取两个声道之右声道
				//speech[y] =(short)((short)pcmFrame_8b2[x+1] << 7);
				// 3 - 取两个声道的平均值
				//ush1 = (short)pcmFrame_8b2[x+0];
				//ush2 = (short)pcmFrame_8b2[x+1];
				//ush = (ush1 + ush2) >> 1;
				//speech[y] = (short)((short)ush << 7);
			}
		}
		else
			if (nBitsPerSample == 16 && nChannels == 1)
			{
				nRead = fread(pcmFrame_16b1, (nBitsPerSample / 8), PCM_FRAME_SIZE*nChannels, fpwave);
				for (x = 0; x < PCM_FRAME_SIZE; x++)
				{
					speech[x] = (short)pcmFrame_16b1[x + 0];
				}
			}
			else
				if (nBitsPerSample == 16 && nChannels == 2)
				{
					nRead = fread(pcmFrame_16b2, (nBitsPerSample / 8), PCM_FRAME_SIZE*nChannels, fpwave);
					for (x = 0, y = 0; y < PCM_FRAME_SIZE; y++, x += 2)
					{
						//speech[y] = (short)pcmFrame_16b2[x+0];
						speech[y] = (short)((int)((int)pcmFrame_16b2[x + 0] + (int)pcmFrame_16b2[x + 1])) >> 1;
					}
				}

	// 如果读到的数据不是一个完整的PCM帧, 就返回0
	if (nRead < PCM_FRAME_SIZE*nChannels) return 0;

	return nRead;
}

unsigned int EXPORT_API wav_to_amr_file(char* pchWAVEFilename, char* pchAMRFileName)
{
	int nChannels = 1;
	int nBitsPerSample = 8;

	FILE* fpwave;
	FILE* fpamr;

	/* input speech vector */
	short speech[160];

	/* counters */
	int byte_counter, frames = 0, bytes = 0;

	/* pointer to encoder state structure */
	void *enstate;

	/* requested mode */
	enum Mode req_mode = MR122;
	int dtx = 0;

	/* bitstream filetype */
	unsigned char amrFrame[MAX_AMR_FRAME_SIZE];

	fpwave = fopen(pchWAVEFilename, "rb");
	if (fpwave == NULL)
	{
		return 0;
	}

	// 创建并初始化amr文件
	fpamr = fopen(pchAMRFileName, "wb");
	if (fpamr == NULL)
	{
		fclose(fpwave);
		return 0;
	}
	/* write magic number to indicate single channel AMR file storage format */
	bytes = fwrite(AMR_MAGIC_NUMBER, sizeof(char), strlen(AMR_MAGIC_NUMBER), fpamr);

	/* skip to pcm audio data*/
	//SkipToPCMAudioData(fpwave);

	enstate = Encoder_Interface_init(dtx);

	while (1)
	{
		// read one pcm frame
		if (!ReadPCMFrame(speech, fpwave, nChannels, nBitsPerSample)) break;

		frames++;

		/* call encoder */
		byte_counter = Encoder_Interface_Encode(enstate, req_mode, speech, amrFrame, 0);

		bytes += byte_counter;
		fwrite(amrFrame, sizeof(unsigned char), byte_counter, fpamr);
	}

	Encoder_Interface_exit(enstate);

	fclose(fpamr);
	fclose(fpwave);

	return frames;
}

int EXPORT_API amr_to_wav_file(char* fileName,char amrBuf[],long amrBufSize)
{
	const int sizes[] = { 12, 13, 15, 17, 19, 20, 26, 31, 5, 6, 5, 5, 0, 0, 0, 0 };

	int fileSeek;
	void *wav, *amr;

	fileSeek = 6;

	wav = wav_write_open(fileName, 8000, 16, 1);
	if (!wav) {
		//fprintf(stderr, "Unable to open %s\n", fileName);
		return errno;
	}

	amr = Decoder_Interface_init();
	while (1) {
		uint8_t buffer[500], littleendian[320], *ptr;
		int size, i;
		int16_t outbuffer[160];
		memcpy(buffer,&amrBuf[fileSeek],1);
		fileSeek += 1;
		if(fileSeek > amrBufSize)
			break;

		// Find the packet size
		size = sizes[(buffer[0] >> 3) & 0x0f];
		memcpy(&buffer[1],&amrBuf[fileSeek],size);
		fileSeek += size;
		if(fileSeek > amrBufSize)
			break;

		// Decode the packet
		Decoder_Interface_Decode(amr, buffer, outbuffer, 0);

		// Convert to little endian and write to wav
		ptr = littleendian;
		for (i = 0; i < 160; i++) {
			*ptr++ = (outbuffer[i] >> 0) & 0xff;
			*ptr++ = (outbuffer[i] >> 8) & 0xff;
		}
		wav_write_data(wav, littleendian, 320);
	}
	Decoder_Interface_exit(amr);
	wav_write_close(wav);
	return 0;
}