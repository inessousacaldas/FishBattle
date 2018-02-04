package com.cilugame.android.commons;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.util.Log;

import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.List;
import java.util.Map;

/**
 * http网络请求能用类
 * <p/>
 * Created by Tony on 15/10/13.
 */
public class HttpUtils {

    private static final String TAG = "HttpUtils";

    public static boolean isNetworkAvailable(final Context context) {
        ConnectivityManager connMgr = (ConnectivityManager)
                context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connMgr.getActiveNetworkInfo();
        if (networkInfo != null && networkInfo.isConnected()) {
            return true;
        } else {
            return false;
        }
    }

    public static void printHeaders(HttpURLConnection con) {
        Map<String, List<String>> headers = con.getHeaderFields();
        for (String headerName : headers.keySet()) {
            List<String> headerValues = headers.get(headerName);
            System.out.println(headerName + " : " + headerValues);
        }
    }

    public static String strURLWithoutParams(String strURL) {
        int indexOfParams = strURL.lastIndexOf("?");
        if(indexOfParams>=0) {
            return strURL.substring(0, indexOfParams);
        }
        return strURL;
    }

    public static Map<String, List<String>> head(String strURL) {
        HttpURLConnection con = null;
        Map<String, List<String>> headers = null;
        try {
            HttpURLConnection.setFollowRedirects(true);
            // note : you may also need
            //        HttpURLConnection.setInstanceFollowRedirects(false)
            con = (HttpURLConnection) new URL(strURL).openConnection();
            con.setConnectTimeout(20000);
            con.setReadTimeout(20000);
            con.setRequestMethod("HEAD");
            boolean success = (con.getResponseCode() == HttpURLConnection.HTTP_OK);
            if (success) {
                headers = con.getHeaderFields();
                for (String headerName : headers.keySet()) {
                    List<String> headerValues = headers.get(headerName);
                    System.out.println(headerName + " : " + headerValues);
                }
            }
        } catch (Exception e) {
            Log.d(TAG, "get header for: " + strURL, e);
        } finally {
            if (con != null) {
                con.disconnect();
            }
        }
        return headers;
    }

    public static String get(String strUrl, int timeoutMillis) {
        InputStream input = null;
        HttpURLConnection connection = null;
        try {
            Log.i(TAG, "start get: " + strUrl);
            URL url = new URL(strUrl);
            connection = (HttpURLConnection) url.openConnection();
            connection.setConnectTimeout(timeoutMillis);
            connection.setReadTimeout(timeoutMillis);
            connection.connect();

            // expect HTTP 200 OK, so we don't mistakenly save error report
            // instead of the file
            if (connection.getResponseCode() != HttpURLConnection.HTTP_OK) {
                Log.e(TAG, "get url error:" + connection.getResponseCode() + ", from: " + strUrl);
                return null;
            }

            // this will be useful to display download percentage
            // might be -1: server did not report the length
            int fileLength = connection.getContentLength();

            // download the file
            input = connection.getInputStream();
            return IOUtils.toString(input);
        } catch (Exception e) {
            Log.e(TAG, "get url error, from: " + strUrl, e);
        } finally {
            IOUtils.closeQuietly(input);
            if (connection != null)
                connection.disconnect();
        }
        return null;
    }
}
