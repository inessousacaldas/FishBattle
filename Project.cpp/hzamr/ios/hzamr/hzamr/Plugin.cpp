
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
	return "HelloHzamr";
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