package com.cilugame.mobilephotoreader;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

public class PhotoReaderActivity extends Activity 
{
	public static final int DEFAULT_MAX_WIDTH = 2048;
	public static final int DEFAULT_MAX_HEIGHT = 2048;
	public static final int DEFAULT_MAX_BYTE = 4194304;
	public static final int DEFAULT_CROP_WIDTH = 230;
	public static final int DEFAULT_CROP_HEIGHT = 230;
	public static final int DEFAULT_COMPRESS_WIDTH = 1024;
	public static final int DEFAULT_COMPRESS_HEIGHT = 1024;
	
	private static final String IMAGE_UNSPECIFIED = "image/*";  
	
	private PhotoReaderConsts.OP_TYPE _opType = null;
	private String _unityObjectName = null;
	private String _unityFunctionName = null;
	private String _unityFilePath = null;
	private int _maxWidth = 0;
	private int _maxHeight = 0;
	private int _maxByte = 0;
	private int _cropWidth = 0;
	private int _cropHeight = 0;
	private int _compressWidth = 0;
	private int _compressHeight = 0;
	
	private Bitmap _sourcePic = null;

	@Override
	protected void onCreate(Bundle savedInstanceState)
	{		
		super.onCreate(savedInstanceState);
		InitExtraParam();
		
		PhotoReaderLogUtil.Log("savedInstanceState="+savedInstanceState);
		
		boolean IsCreate = getIntent().getBooleanExtra("IsCreate", false);
		if (IsCreate)
		{
			getIntent().putExtra("IsCreate", false);
			PhotoReaderLogUtil.Log("onCreate");
			
			Intent intent = new Intent(Intent.ACTION_PICK, null);
			intent.setDataAndType(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, IMAGE_UNSPECIFIED);
			this.startActivityForResult(intent, PhotoReaderConsts.OP_RESULT.OPEN_PHOTO.ordinal());
		}
	}
	
	
	private void InitExtraParam()
	{
		String strOpType = getIntent().getStringExtra(PhotoReaderConsts.NAME_OP_TYPE);
		_opType = PhotoReaderConsts.OP_TYPE.valueOf(PhotoReaderConsts.OP_TYPE.class, strOpType);
		
		_unityObjectName = getIntent().getStringExtra(PhotoReaderConsts.EXTRA_PARAM.ObjectName.toString());
		_unityFunctionName = getIntent().getStringExtra(PhotoReaderConsts.EXTRA_PARAM.FunctionName.toString());
		_unityFilePath = getIntent().getStringExtra(PhotoReaderConsts.EXTRA_PARAM.FilePath.toString());
		_maxWidth = getIntent().getIntExtra(PhotoReaderConsts.EXTRA_PARAM.MaxWidth.toString(), DEFAULT_MAX_WIDTH);
		_maxHeight = getIntent().getIntExtra(PhotoReaderConsts.EXTRA_PARAM.MaxHeight.toString(), DEFAULT_MAX_HEIGHT);
		_maxByte = getIntent().getIntExtra(PhotoReaderConsts.EXTRA_PARAM.MaxByte.toString(), DEFAULT_MAX_BYTE);
		
		if (_opType == PhotoReaderConsts.OP_TYPE.ReadAndCrop)
		{
			_cropWidth = getIntent().getIntExtra(PhotoReaderConsts.EXTRA_PARAM.CropWidth.toString(), DEFAULT_CROP_WIDTH);
			_cropHeight = getIntent().getIntExtra(PhotoReaderConsts.EXTRA_PARAM.CropHeight.toString(), DEFAULT_CROP_HEIGHT);
			_cropWidth = _cropWidth > DEFAULT_CROP_WIDTH ? DEFAULT_CROP_WIDTH : _cropWidth;
			_cropHeight = _cropHeight > DEFAULT_CROP_HEIGHT ? DEFAULT_CROP_HEIGHT : _cropHeight;
		}
		else if (_opType == PhotoReaderConsts.OP_TYPE.ReadAndCompress)
		{
			_compressWidth = getIntent().getIntExtra(PhotoReaderConsts.EXTRA_PARAM.CompressWidth.toString(), DEFAULT_COMPRESS_WIDTH);
			_compressHeight = getIntent().getIntExtra(PhotoReaderConsts.EXTRA_PARAM.CompressHeight.toString(), DEFAULT_COMPRESS_HEIGHT);
		}
	}
	
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) 
	{
		super.onActivityResult(requestCode, resultCode, data);
		
		PhotoReaderLogUtil.Log("onActivityResult requestCode="+requestCode+" resultCode="+resultCode + " data=" + (data != null) + " _opType=" + _opType);
		
		if (requestCode == PhotoReaderConsts.OP_RESULT.OPEN_PHOTO.ordinal())
        {  
			if(null == data)
        	{
				PhotoReaderLogUtil.Log("onActivityResult null == data ");
        		finish();
        		return;
        	}
			Uri uri = data.getData();
			
			/*
			if (!IsPhotoLegal (uri))
			{
				PhotoReaderLogUtil.Log("onActivityResult IsPhotoLegal");
				SendMsgToUnity(PhotoReaderConsts.OP_RESULT.ILLEGAL);
				finish();
				return;
			}
			*/
			
			if (_opType == PhotoReaderConsts.OP_TYPE.ReadAndCrop)
			{
				PhotoReaderLogUtil.Log("_opType ReadAndCrop");
				PhotoCropHandle(uri);
			}
			else if (_opType == PhotoReaderConsts.OP_TYPE.ReadAndCompress)
			{
				PhotoReaderLogUtil.Log("_opType ReadAndCompress");
				PhotoCompressHandle(uri);
				SendMsgToUnity(PhotoReaderConsts.OP_RESULT.COMPRESS_SUCC);
				finish();
				return;
			}
			else
			{
				PhotoReaderLogUtil.Log("_opType else");
				finish();
				return;
			}
				
        }
		else if (requestCode == PhotoReaderConsts.OP_RESULT.CROP_SUCC.ordinal())
		{
			if(null == data)
        	{	
        		finish();
        		return;
        	}
			Bundle extras = data.getExtras();
            if (null != extras) 
            {
                Bitmap photo = extras.getParcelable("data");
            	SavePicture(photo);
            	SendMsgToUnity(PhotoReaderConsts.OP_RESULT.CROP_SUCC);
            	finish();
            	return;
            } 
		}
	}
	
	
	@SuppressLint("NewApi")
	private boolean IsPhotoLegal(Uri uri)
	{
		BitmapFactory.Options bitmapOptions = new BitmapFactory.Options();
		try 
		{
			_sourcePic = BitmapFactory.decodeStream(this.getContentResolver().openInputStream(uri), null , bitmapOptions);
			
			if (_sourcePic == null)
				return false;
			//判断是否超出合法处理的范围
			if (_sourcePic.getWidth() > _maxWidth || _sourcePic.getHeight() > _maxHeight || _sourcePic.getByteCount() > _maxByte)
			{
				_sourcePic = null;
				return false;
			}
			
			return true;
		} 
		catch (Exception e)
		{
			PhotoReaderLogUtil.Log("Exception", e);
			e.printStackTrace();
			return false;
		}
	}
	
	
	private void PhotoCropHandle(Uri uri)
	{
		Intent intent = new Intent("com.android.camera.action.CROP");  
        intent.setDataAndType(uri, IMAGE_UNSPECIFIED);  
        intent.putExtra("crop", "true");
        
        // aspectX aspectY 是宽高的比例
        intent.putExtra("aspectX", _cropWidth);
        intent.putExtra("aspectY", _cropHeight);
        intent.putExtra("outputX", _cropWidth);
        intent.putExtra("outputY", _cropHeight);
        
        intent.putExtra("return-data", true);
        startActivityForResult(intent, PhotoReaderConsts.OP_RESULT.CROP_SUCC.ordinal());
	}
	
	
	private void PhotoCompressHandle(Uri uri)
	{
		try 
		{
			InputStream inputStream = this.getContentResolver().openInputStream(uri);
			Bitmap outPic = PhtoReaderHelper.loadImageFromUrl(inputStream, _compressWidth, _compressHeight);
			SavePicture(outPic);
		} 
		catch (Exception e)
		{
			PhotoReaderLogUtil.Log("Exception", e);
			e.printStackTrace();
		}
	}
	
	private int calculateInSampleSize(int picWidth, int picHeight, int reqWidth, int reqHeight) 
	{
	    int inSampleSize = 1;

	    if (picWidth > reqWidth || picHeight > reqHeight)
	    {
             int heightRatio = Math.round((float) picHeight / (float) reqHeight);
             int widthRatio = Math.round((float) picWidth / (float) reqWidth);
             inSampleSize = heightRatio < widthRatio ? widthRatio : heightRatio;
	    }
		
		return inSampleSize;
	}
	
	@SuppressLint("SdCardPath") 
	private void SavePicture(Bitmap photo)
	{
		PhotoReaderLogUtil.Log("SavePicture");
		PhotoReaderLogUtil.Log("photogetByteCount w=" + photo.getWidth() + " h="+photo.getHeight());
		
		FileOutputStream fOut = null;
		try
		{
			//String strPackgeName = getApplicationInfo().packageName;
			//String savePath = "/mnt/sdcard/Android/data/" + strPackgeName + "/files/" + _unityFilePath;
			String savePath = _unityFilePath;
			PhotoReaderLogUtil.Log("savePath= " +  savePath);
			savePath = savePath.replaceAll("file:///", "/");
			PhotoReaderLogUtil.Log("savePath2= " +  savePath);
			//file:///storage/emulated/0/Android/data/com.cilugame.h1.android.nucleus.localdev/files/selfZone/101277/head/figureImage.jpg
			
			PhotoReaderLogUtil.Log("savePath= " +  savePath);
			
			String dirpath = savePath.substring(0, savePath.lastIndexOf('/'));
			File destDir = new File(dirpath);
			if (!destDir.exists())
			{
				destDir.mkdirs();
			}
			
			fOut = new FileOutputStream(savePath);
		} 
		catch (FileNotFoundException e)
		{
			PhotoReaderLogUtil.Log("Exception", e);
			e.printStackTrace();
		}
		
		//将Bitmap对象写入本地路径中，Unity在去相同的路径来读取这个文件
		photo.compress(Bitmap.CompressFormat.JPEG, 80, fOut);
		
		try 
		{
			fOut.flush();
		} 
		catch (IOException e)
		{
			PhotoReaderLogUtil.Log("Exception", e);
			e.printStackTrace();
		}
		
		try 
		{
			fOut.close();
		} 
		catch (IOException e) 
		{
			PhotoReaderLogUtil.Log("Exception", e);
			e.printStackTrace();
		}
	}
	
	private void SendMsgToUnity(PhotoReaderConsts.OP_RESULT msg)
	{
		 PhotoReaderLogUtil.Log("SendMsgToUnity " +  "msg=" + msg.toString());
		UnityPlayer.UnitySendMessage(_unityObjectName, _unityFunctionName, msg.toString());
	}
	
}
