package com.cilugame.h1.patch;

import android.content.Context;
import android.os.AsyncTask;
import android.os.Handler;
import android.os.Message;
import android.os.PowerManager;
import android.util.Log;
import android.view.View;
import android.widget.ProgressBar;

import com.cilugame.android.commons.IOUtils;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.security.MessageDigest;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.zip.Inflater;

/**
 * ZIP DLL解压任务
 *
 * Created by Tony on 22/11/16.
 */

public class DLLUnZipTask extends AsyncTask<String, Integer, Integer> {

    public static final byte[] CILU_DLL_MAGIC_TAG = "CLDLL".getBytes();
    public static final byte[] CILU_ZIP_DLL_MAGIC_TAG = "CLZDLL".getBytes();
    public static final int BUFF_SZIE = 4096;
    public static final int MAX_PROGRESS = 100;

    private final Context context;
    private final ProgressBar pbLoader;
    private PowerManager.WakeLock wakeLock;
    private final List<FileInfo> fileURLs;
    private final Handler handler;
    private final File outputDir;
    private final DLLDownloadTask parentTask;

    public DLLUnZipTask(DLLDownloadTask parent, Context context, Handler handler, ProgressBar pbLoader, List<FileInfo> diffFileURLs, File outputDir) {
        this.parentTask = parent;
        this.context = context;
        this.handler = handler;
        this.pbLoader = pbLoader;
        this.fileURLs = diffFileURLs;
        this.outputDir = outputDir;
    }

    // xor the key bytes
    public byte[] dk(byte[] bytes, byte k) {
        // xor
        byte[] newBytes = new byte[bytes.length];
        for(int i=0; i<newBytes.length; i++) {
            newBytes[i] = (byte) (bytes[i] ^ k);
        }
        return newBytes;
    }

    @Override
    protected Integer doInBackground(String... params) {
        int step = MAX_PROGRESS / fileURLs.size();
        int rest = MAX_PROGRESS % fileURLs.size();
        // step: 1. 解密；2. 解压；3. 加密；4. MD5； 5. 保存文件
        for (int i = 0; i < fileURLs.size(); i++) {
            int subStep = step / 5;
            FileInfo fileInfo = fileURLs.get(i);
            unzipTip(i, fileInfo.fileURL);
            byte[] orig = fileInfo.stream.toByteArray();
            byte[] dezipBytes = dz(orig, subStep);
            try {
                MessageDigest digest = MessageDigest.getInstance("MD5");
                digest.update(orig);
                fileInfo.version = IOUtils.bytesHex(digest.digest());
                // 4.
                publishProgress(subStep);
                IOUtils.write(dezipBytes, new FileOutputStream(fileInfo.file));
                // 5.
                publishProgress(step-(subStep*5));
            } catch (Exception e) {
                //写入出错，直接版本错误
                fileInfo.version = "";
                Log.e("DLLUnZipTask", "writeFile: " + fileInfo.file, e);
            }
            if (!fileInfo.isValid()) {
                fileInfo.clearState();
                parentTask.addError(fileInfo);
            }
        }
        publishProgress(rest);
        return 0;
    }

    // deZipDll
    public byte[] dz(byte[] orig, int step) {
        int tagLength = CILU_DLL_MAGIC_TAG.length;
        byte[] magicTag = Arrays.copyOf(orig, tagLength);
        Log.d("e", "check unzip");
        if (Arrays.equals(magicTag, CILU_DLL_MAGIC_TAG)) {
            Log.d("e", "start unzip");
            byte[] k = "QHE8BxTiPWzMr8Je".getBytes();
            //byte[] zipKey = Arrays.copyOfRange(orig, orig.length-keyLength-1, orig.length-1);
            byte[] zipDllBytes = new byte[orig.length - tagLength];
            System.arraycopy(orig, tagLength, zipDllBytes, 0, zipDllBytes.length);
            zipDllBytes = e.xh(zipDllBytes, k);
            // 1.
            publishProgress(step);
            byte[] unzipBytes = eb(zipDllBytes);
            // 2.
            publishProgress(step);
            byte[] encryptBytes = e.eh(unzipBytes, k);
            ByteBuffer buf = ByteBuffer.allocate(encryptBytes.length + CILU_DLL_MAGIC_TAG.length);
            buf.put(CILU_DLL_MAGIC_TAG);
            buf.put(encryptBytes);
            // 3.
            publishProgress(step);
            return buf.array();
        }

        //非项目文件，不解密，解压处理
        return eb(orig);
    }

    // public byte[] unzlib(byte input[]) {
    public byte[] eb(byte input[]) {
        if (input == null || input.length <= 0)
            return input;
        Inflater decompresser = new Inflater();
        byte output[] = new byte[0];
        decompresser.reset();
        decompresser.setInput(input);
        ByteArrayOutputStream o = new ByteArrayOutputStream(input.length);
        try {
            byte[] buf = new byte[BUFF_SZIE];
            int got;
            while (!decompresser.finished()) {
                got = decompresser.inflate(buf);
                o.write(buf, 0, got);
            }
            output = o.toByteArray();
        } catch (Exception e) {
            Log.e("DLLUnZipTask", "eb", e);
        } finally {
            try {
                o.close();
            } catch (IOException e) {
                Log.e("DLLUnZipTask", "eb_io", e);
            }
            decompresser.end();
        }
        return output;
    }

    private void unzipTip(int curIndex, String curFileURL) {
        final Message msg = new Message();
        msg.what = DLLDownloadTip.MSG_DLL_DOWNLOAD_FILE;
        msg.obj = "正在解压资源，不消耗流量: " + (curIndex + 1) + "/" + fileURLs.size();
        Log.d("DLLUnZip", "正在解压资源: " + DLLDownloadTask.getFileName(curFileURL) + ", 总数: " + (curIndex + 1) + "/" + fileURLs.size());
        handler.sendMessage(msg);
    }

    @Override
    protected void onPreExecute() {
        super.onPreExecute();
        // take CPU lock to prevent CPU from going off if the user
        // presses the power button during download
        PowerManager pm = (PowerManager) context.getSystemService(Context.POWER_SERVICE);
        wakeLock = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK,
                getClass().getName());
        wakeLock.acquire();
        pbLoader.setProgress(0);
        pbLoader.setMax(MAX_PROGRESS);
        pbLoader.setVisibility(View.VISIBLE);
    }

    @Override
    protected void onProgressUpdate(Integer... progress) {
        super.onProgressUpdate(progress);
        // if we get here, length is known, now set indeterminate to false
        pbLoader.setIndeterminate(false);
        pbLoader.setProgress(pbLoader.getProgress() + progress[0]);
        System.out.println(pbLoader.getProgress() + "/" + pbLoader.getMax());
    }

    @Override
    protected void onPostExecute(Integer result) {
        wakeLock.release();
        if(parentTask.hasErrorURL()) {
            sendMessage(DLLDownloadTip.ERR_DLL_DOWNLOAD_FAIL, null);
        } else {
            sendMessage(DLLDownloadTip.MSG_DLL_DOWNLOAD_FINISH, null);
        }
    }

    protected void sendMessage(int what, String message) {
        try {
            if (wakeLock.isHeld()) {
                wakeLock.release();
            }
        } catch (Exception e) {
            Log.e("DLLUnZipTask", "sendMessage:" + what + ", msg:" + message, e);
        }
        final Message msg = new Message();
        msg.what = what;
        msg.obj = message;
        handler.sendMessage(msg);
    }
}
