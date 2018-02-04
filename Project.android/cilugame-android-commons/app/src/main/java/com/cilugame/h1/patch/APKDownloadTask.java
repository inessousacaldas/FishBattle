package com.cilugame.h1.patch;

import android.content.Context;
import android.os.AsyncTask;
import android.os.PowerManager;
import android.util.Log;

import com.cilugame.android.commons.HttpUtils;
import com.cilugame.android.commons.IOUtils;
import com.cilugame.h1.activity.CLGamePacketFragment;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.concurrent.TimeUnit;

/**
 * APK下载任务
 * <p/>
 * Created by Tony on 15/10/13.
 */
public class APKDownloadTask extends AsyncTask<String, Long, String> {

    private static final double NANOS_PER_SECOND = Double.valueOf(TimeUnit.SECONDS.toNanos(1));

    private final Context context;
    private CLGamePacketFragment progressDialog;
    private PowerManager.WakeLock wakeLock;
    private final String fileURL;
    private final File outputFile;

    private long downloaded = 0;
    private long fileLength = 0;

    public APKDownloadTask(Context context, CLGamePacketFragment progressDialog, String fileURL, File outputFile, long origFileLength) {
        this.context = context;
        this.progressDialog = progressDialog;
        this.fileURL = fileURL;
        this.outputFile = outputFile;
        if (outputFile.exists()) {
            downloaded = outputFile.length();
        }
        this.fileLength = origFileLength;
    }

    public void setProgressDialog(CLGamePacketFragment newDialog) {
        progressDialog = newDialog;
    }

    @Override
    protected String doInBackground(String... sUrl) {
        String result = downloadFile();
        return result;
    }


    private String downloadFile() {
        InputStream input = null;
        OutputStream output = null;
        HttpURLConnection connection = null;
        try {
            URL url = new URL(fileURL);
            connection = (HttpURLConnection) url.openConnection();
            if (outputFile.exists()) {
                downloaded = (int) outputFile.length();
                connection.setRequestProperty("Range", "bytes=" + (outputFile.length()) + "-");
            } else {
                connection.setRequestProperty("Range", "bytes=" + downloaded + "-");
            }
            connection.connect();

            // expect HTTP 200 OK, so we don't mistakenly save error report
            // instead of the file
            int responseCode = connection.getResponseCode();
            if (responseCode < HttpURLConnection.HTTP_OK ||
                    responseCode > 299) {
                String msg = "Server returned HTTP " + connection.getResponseCode()
                        + " " + connection.getResponseMessage();
                Log.e("APKDownloadTask", "download url:" + fileURL + "; " + msg);
                return msg;
            }

            HttpUtils.printHeaders(connection);
            // this will be useful to display download percentage
            // might be -1: server did not report the length
            // fileLength = connection.getContentLength();
            publishProgress(downloaded, fileLength, 0L, 0L);
            if (downloaded < fileLength) {
                // download the file
                input = connection.getInputStream();
                output = downloaded == 0 ? new FileOutputStream(outputFile) : new FileOutputStream(outputFile, true);

                byte data[] = new byte[4096];
                long count = 0;
                double totalRead = 0;
                long startTime = System.nanoTime();
                while ((count = input.read(data)) != -1) {
                    totalRead += count;
                    if (isCancelled()) {
                        input.close();
                        return null;
                    }
                    downloaded += count;
                    output.write(data, 0, (int) count);
                    long downloadTime = System.nanoTime() - startTime + 1;
                    long speedInByte = downloadSpeedInByte(totalRead, downloadTime);
                    if (count > 0) {
                        publishProgress(downloaded, fileLength, speedInByte);
                    }
                }
            }
        } catch (Exception e) {
            Log.d("APKDownloadTask", "download err:", e);
            publishProgress(downloaded, fileLength, 0L, 2L);
            return e.toString();
        } finally {
            try {
                if (output != null)
                    output.close();
                if (input != null)
                    input.close();
            } catch (IOException ignored) {
            }

            if (connection != null)
                connection.disconnect();
        }
        return null;
    }

    private long downloadSpeedInByte(double totalRead, long downloadTime) {
        double seconds = downloadTime / NANOS_PER_SECOND;
        double speedPerSecond = totalRead / seconds;
        return (long) speedPerSecond;
        //final double BYTES_PER_MIB = 1024 * 1024;
        //double speed = NANOS_PER_SECOND / BYTES_PER_MIB * totalRead / downloadTime;
        // System.out.println(IOUtils.byteCountToDisplaySize(speedPerSecond) + "/s");
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
    }

    @Override
    protected void onProgressUpdate(Long... progress) {
        super.onProgressUpdate(progress);
        boolean error = false;
        if (progress.length > 3 && progress[3] == 2L) {
            error = true;
        }
        progressDialog.progressUpdate(progress[0], progress[1], (error ? 2L : 1L), progress[2]);
        // System.out.println("progress : " + Arrays.toString(progress));
    }

    @Override
    protected void onPostExecute(String result) {
        wakeLock.release();
    }

    public void cancel() {
        try {
            if (wakeLock.isHeld())
                wakeLock.release();
        } catch (Exception e) {

        }
        if (!isCancelled())
            this.cancel(true);
    }
}
