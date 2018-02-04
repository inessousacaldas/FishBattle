package com.cilugame.mobilephotoreader;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

public class PhotoReaderMain 
{
	public static void ReadAndCropPhoto(String objectName, String funName, String filePath, Activity activity)
	{
		PhotoReaderMain.ReadAndCropPhoto(objectName, funName, filePath, activity, 
				PhotoReaderActivity.DEFAULT_MAX_WIDTH, PhotoReaderActivity.DEFAULT_MAX_HEIGHT, PhotoReaderActivity.DEFAULT_MAX_BYTE,
				PhotoReaderActivity.DEFAULT_CROP_WIDTH, PhotoReaderActivity.DEFAULT_CROP_HEIGHT);
	}
	
	public static void ReadAndCropPhoto(String objectName, String funName, String filePath, Activity activity,
					int maxWidth, int maxHeight, int maxByte, int cropWidth, int cropHeight)
	{
		PhotoReaderLogUtil.Log("ReadAndCropPhoto");
		
		Intent intent = new Intent(activity, PhotoReaderActivity.class);
		intent.putExtra(PhotoReaderConsts.NAME_OP_TYPE, PhotoReaderConsts.OP_TYPE.ReadAndCrop.toString());
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.ObjectName.toString(), objectName);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.FunctionName.toString(), funName);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.FilePath.toString(), filePath);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.MaxWidth.toString(), maxWidth);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.MaxHeight.toString(), maxHeight);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.MaxByte.toString(), maxByte);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.CropWidth.toString(), cropWidth);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.CropHeight.toString(), cropHeight);
		intent.putExtra("IsCreate",  true);
		activity.startActivity(intent);
	}
	
	public static void ReadAndCompressPhoto(String objectName, String funName, String filePath, Activity activity)
	{
		PhotoReaderMain.ReadAndCompressPhoto(objectName, funName, filePath, activity,
				PhotoReaderActivity.DEFAULT_MAX_WIDTH, PhotoReaderActivity.DEFAULT_MAX_HEIGHT, PhotoReaderActivity.DEFAULT_MAX_BYTE,
				PhotoReaderActivity.DEFAULT_COMPRESS_WIDTH, PhotoReaderActivity.DEFAULT_COMPRESS_HEIGHT);
	}
	
	public static void ReadAndCompressPhoto(String objectName, String funName, String filePath, Activity activity,
			int maxWidth, int maxHeight, int maxByte, int compressWidth, int compressHeight)
	{
		PhotoReaderLogUtil.Log("ReadAndCompressPhoto");
		
		Intent intent = new Intent(activity, PhotoReaderActivity.class);
		intent.putExtra(PhotoReaderConsts.NAME_OP_TYPE, PhotoReaderConsts.OP_TYPE.ReadAndCompress.toString());
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.ObjectName.toString(), objectName);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.FunctionName.toString(), funName);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.FilePath.toString(), filePath);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.MaxWidth.toString(), maxWidth);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.MaxHeight.toString(), maxHeight);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.MaxByte.toString(), maxByte);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.CompressWidth.toString(), compressWidth);
		intent.putExtra(PhotoReaderConsts.EXTRA_PARAM.CompressHeight.toString(), compressHeight);
		intent.putExtra("IsCreate",  true);
		activity.startActivity(intent);
	}
}
