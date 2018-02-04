package com.cilugame.mobilephotoreader;


public class PhotoReaderConsts 
{
	public enum EXTRA_PARAM
	{
		ObjectName,
		FunctionName,
		FilePath,
		MaxWidth,
		MaxHeight,
		MaxByte,
		CropWidth,
		CropHeight,
		CompressWidth,
		CompressHeight
	}
	
	public enum OP_RESULT
	{
		OPEN_PHOTO,
		CROP_SUCC,
		COMPRESS_SUCC,
		CANCEL,
		ILLEGAL
	}
	
	public static final String NAME_OP_TYPE = "OP_TYPE";
	public enum OP_TYPE
	{
		ReadAndCrop,
		ReadAndCompress
	}
	
}
