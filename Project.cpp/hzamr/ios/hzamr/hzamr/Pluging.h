
#ifndef _PLUGING_H_
#define _PLUGING_H_

#if _MSC_VER // this is defined when compiling with Visual Studio
#define EXPORT_API __declspec(dllexport) // Visual Studio needs annotating exported functions with this
#else
#define EXPORT_API // XCode does not need annotating exported functions, so define is empty
#endif

// ------------------------------------------------------------------------
// Plugin itself


// Link following functions C-style (required for plugins)
extern "C"
{
	// The functions we will call from Unity.
	//
	const EXPORT_API char*  PrintHello();

	unsigned int EXPORT_API wav_to_amr(char wavBuf[],int wavBufSize,char amrBuf[]);

	int EXPORT_API amr_to_wav_file(char* fileName,char amrBuf[],long amrBufSize);
} // end of export C block

#endif