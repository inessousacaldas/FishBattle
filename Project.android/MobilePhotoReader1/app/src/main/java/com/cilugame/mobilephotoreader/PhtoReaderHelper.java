package com.cilugame.mobilephotoreader;

import java.io.BufferedInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.MalformedURLException;
import java.net.URL;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.util.Log;

public class PhtoReaderHelper {
	
	public static Bitmap loadImageFromUrl(InputStream inputStream, int reqWidth, int reqHeight) 
	{
		PhotoReaderLogUtil.Log("loadImageFromUrl " + (inputStream != null));
		
        BufferedInputStream bis = null;
        ByteArrayOutputStream out = null;
        byte isBuffer[] = new byte[1024];

        try {

            bis = new BufferedInputStream(inputStream, 1024 * 4);
            out = new ByteArrayOutputStream();
            int len = 0;
            while ((len = bis.read(isBuffer)) != -1) {
                out.write(isBuffer, 0, len);
            }
            out.close();
            bis.close();
        } catch (MalformedURLException e1) {
        	PhotoReaderLogUtil.Log("MalformedURLException", e1);
            e1.printStackTrace();
            return null;
        } catch (IOException e) {
        	PhotoReaderLogUtil.Log("IOException", e);
            e.printStackTrace();
        }
        if (out == null){
        	PhotoReaderLogUtil.Log("out is NULL");
        	return null;
        }
        
        byte[] data = out.toByteArray();
        BitmapFactory.Options options = new BitmapFactory.Options();
        options.inJustDecodeBounds = true;
        BitmapFactory.decodeByteArray(data, 0, data.length, options);
        options.inJustDecodeBounds = false;

        options.inSampleSize = calculateInSampleSize(options.outWidth, options.outHeight, reqWidth,  reqHeight);
        Bitmap bmp = null;
        try {
            bmp = BitmapFactory.decodeByteArray(data, 0, data.length, options); // 返回缩略图
        } catch (OutOfMemoryError e) {
        	PhotoReaderLogUtil.Log("OutOfMemoryError");
            // TODO: handle exception
            System.gc();
            bmp = null;
        }
        return bmp;
    }
	
	private static int calculateInSampleSize(int picWidth, int picHeight, int reqWidth, int reqHeight) 
	{
	    int inSampleSize = 1;

	    if (picWidth > reqWidth || picHeight > reqHeight)
	    {
             int heightRatio = Math.round((float) picHeight / (float) reqHeight);
             int widthRatio = Math.round((float) picWidth / (float) reqWidth);
             inSampleSize = heightRatio < widthRatio ? widthRatio : heightRatio;
	    }
	    PhotoReaderLogUtil.Log("calculateInSampleSize = " +inSampleSize );
		return inSampleSize;
	}	
}
