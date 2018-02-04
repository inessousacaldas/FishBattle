package com.cilugame.h1.patch;

import java.io.ByteArrayOutputStream;
import java.io.File;

/**
 * 文件信息
 * <p/>
 * Created by Tony on 2/2/16.
 */
public class FileInfo {

    public final String name;
    public final String fileURL;
    public final File file;
    public final String remoteVersion;
    public String version;
    public int errorCode;
    public ByteArrayOutputStream stream = new ByteArrayOutputStream(4096);

    public FileInfo(String name, String fileURL, File file, String remoteVersion) {
        this.name = name;
        this.fileURL = fileURL;
        this.file = file;
        this.remoteVersion = remoteVersion;
    }

    public boolean isValid() {
        return errorCode == 0 && (remoteVersion.equalsIgnoreCase(version));
    }

    public void clearState() {
        errorCode = 0;
        version = "";
        stream = new ByteArrayOutputStream(4096);
    }
}
