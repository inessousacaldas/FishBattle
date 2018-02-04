package com.demiframe.game.api.util;

/**
 * Created by xianjian on 2017/3/28.
 * 此加密模块打包工具有用到，如果修改，请更新打包工具目录下的 platforms/demiframe.jar
 */

import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

public class EncryptUtil
{
    private static byte[] password = {0x0a, 0x0d, 0x0c, 0x01, 0x06,
            0x00, 0x0b, 0x0e, 0x0f, 0x09, 0x04, 0x03, 0x08, 0x03, 0x01, 0x09};

    public EncryptUtil()
    {
    }

    public static String encrypt(String cleartext)
            throws Exception
    {
        IvParameterSpec zeroIv = new IvParameterSpec(VIPARA.getBytes());
        SecretKeySpec key = new SecretKeySpec(password, "AES");
        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        cipher.init(Cipher.ENCRYPT_MODE, key, zeroIv);
        byte encryptedData[] = cipher.doFinal(cleartext.getBytes("GBK"));
        return Base64.encode(encryptedData);
    }

    public static String decrypt(String encrypted)
            throws Exception
    {
        byte byteMi[] = Base64.decode(encrypted);
        IvParameterSpec zeroIv = new IvParameterSpec(VIPARA.getBytes());
        SecretKeySpec key = new SecretKeySpec(password, "AES");
        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        cipher.init(Cipher.DECRYPT_MODE, key, zeroIv);
        byte decryptedData[] = cipher.doFinal(byteMi);
        return new String(decryptedData, "GBK");
    }

    public static String VIPARA = "aldrich.demi1234";
    public static final String bm = "GBK";

}
