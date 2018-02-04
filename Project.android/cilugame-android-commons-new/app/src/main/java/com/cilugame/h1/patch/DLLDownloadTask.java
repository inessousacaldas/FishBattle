package com.cilugame.h1.patch;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.security.MessageDigest;
import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.os.AsyncTask;
import android.os.Handler;
import android.os.Message;
import android.os.PowerManager;
import android.util.Log;
import android.view.View;
import android.widget.ProgressBar;

import com.cilugame.android.commons.IOUtils;

/**
 * DLL下载
 * <p/>
 * Created by Tony on 15/10/13.
 */
public class DLLDownloadTask extends AsyncTask<String, Integer, Integer> {

    private final Context context;
    private final ProgressBar pbLoader;
    private PowerManager.WakeLock wakeLock;
    private final List<FileInfo> fileURLs;
    private final Handler handler;
    private final File outputDir;
    private final List<FileInfo> errURLs = new ArrayList<FileInfo>(5);
    public final boolean zip;

    public DLLDownloadTask(Context context, Handler handler, ProgressBar pbLoader, List<FileInfo> diffFileURLs, File outputDir, boolean zip) {
        this.context = context;
        this.handler = handler;
        this.pbLoader = pbLoader;
        this.fileURLs = diffFileURLs;
        this.outputDir = outputDir;
        this.zip = zip;
    }

    public DLLDownloadTask clone() {
        DLLDownloadTask task = new DLLDownloadTask(context, handler, pbLoader, errURLs, outputDir, zip);
        return task;
    }

    @Override
    protected Integer doInBackground(String... sUrl) {
        for (int i = 0; i < fileURLs.size(); i++) {
            FileInfo fileInfo = fileURLs.get(i);
            downloadTip(i, fileInfo.fileURL);
            downloadFile(fileInfo);
            if (!zip && !fileInfo.isValid()) {
                fileInfo.clearState();
                errURLs.add(fileInfo);
            }
        }
        return 0;
    }

    private void downloadTip(int curIndex, String curFileURL) {
        final Message msg = new Message();
        msg.what = DLLDownloadTip.MSG_DLL_DOWNLOAD_FILE;
        msg.obj = "正在下载: " + (curIndex + 1) + "/" + fileURLs.size();
        Log.d("DLLDownload", "正在下载: " + getFileName(curFileURL) + ", 总数: " + (curIndex + 1) + "/" + fileURLs.size());
        handler.sendMessage(msg);
    }

    public static String getFileName(String strURL) {
        String fileName = strURL.substring(strURL.lastIndexOf('/') + 1);
        if (fileName.indexOf("?") >= 0) {
            fileName = fileName.substring(0, fileName.indexOf("?"));
        }
        return fileName;
    }

    private void downloadFile(FileInfo fileInfo) {
        InputStream input = null;
        HttpURLConnection connection = null;
        int errorCode = 0;
        String curFileURL = fileInfo.fileURL;
        try {
            URL url = new URL(curFileURL);
            connection = (HttpURLConnection) url.openConnection();
            connection.connect();

            // expect HTTP 200 OK, so we don't mistakenly save error report
            // instead of the file
            if (connection.getResponseCode() != HttpURLConnection.HTTP_OK) {
                String msg = "Server returned HTTP " + connection.getResponseCode()
                        + " " + connection.getResponseMessage();
                Log.e("DLLDownloadTask", "download url:" + curFileURL + "; " + msg);
                throw new RuntimeException(msg);
            }

            // this will be useful to display download percentage
            // might be -1: server did not report the length
            int fileLength = connection.getContentLength();
            MessageDigest digest = MessageDigest.getInstance("MD5");

            // download the file
            input = connection.getInputStream();

            byte data[] = new byte[4096];
            int count;
            while ((count = input.read(data)) != -1) {
                // allow canceling with back button
                if (isCancelled()) {
                    input.close();
                    errorCode = DLLDownloadTip.ERR_DLL_DOWNLOAD_CANCEL;
                    break;
                }
                // publishing the progress....
                if (fileLength > 0) // only if total length is known
                    publishProgress(count);
                fileInfo.stream.write(data, 0, count);
            }
            byte[] raw = fileInfo.stream.toByteArray();
            if(!zip) {
                digest.update(raw);
                fileInfo.version = IOUtils.bytesHex(digest.digest());
                IOUtils.write(raw, new FileOutputStream(fileInfo.file));
            }
        } catch (IOException e) {
            // 无法写文件
            errorCode = DLLDownloadTip.ERR_DLL_IO_EXCEPTION;
            Log.e("DLLDownloadTask", "download url:" + curFileURL + "; ", e);
        } catch (Exception e) {
            errorCode = DLLDownloadTip.ERR_DLL_DOWNLOAD_FAIL;
            Log.e("DLLDownloadTask", "download url:" + curFileURL + "; ", e);
        } finally {
            try {
                if (input != null)
                    input.close();
            } catch (IOException ignored) {
            }

            if (connection != null)
                connection.disconnect();
        }
        fileInfo.errorCode = errorCode;
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
        pbLoader.setVisibility(View.VISIBLE);
    }

    @Override
    protected void onProgressUpdate(Integer... progress) {
        super.onProgressUpdate(progress);
        // if we get here, length is known, now set indeterminate to false
        pbLoader.setIndeterminate(false);
        // pbLoader.setMax(100 * fileURLs.size());
        pbLoader.setProgress(pbLoader.getProgress() + progress[0]);
    }

    @Override
    protected void onPostExecute(Integer result) {
        wakeLock.release();
        if(errURLs.size()>0) {
            sendMessage(DLLDownloadTip.ERR_DLL_DOWNLOAD_FAIL, null);
        } else {
            if(zip && fileURLs.size()>0) {
                DLLUnZipTask unZipTask = new DLLUnZipTask(this, context, handler, pbLoader, fileURLs, outputDir);
                unZipTask.execute();
            } else {
                sendMessage(DLLDownloadTip.MSG_DLL_DOWNLOAD_FINISH, null);
            }
        }
    }

    protected void sendMessage(int what, String message) {
        try {
            if (wakeLock.isHeld()) {
                wakeLock.release();
            }
        } catch (Exception e) {
            Log.e("DLLDownloadTask", "sendMessage:" + what + ", msg:" + message, e);
        }
        final Message msg = new Message();
        msg.what = what;
        msg.obj = message;
        handler.sendMessage(msg);
    }

    public boolean hasErrorURL() {
        return errURLs.size()>0;
    }

    public void addError(FileInfo fileInfo) {
        errURLs.add(fileInfo);
    }
}
