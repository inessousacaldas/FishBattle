package com.demiframe.game.api.common;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;

public class LHUser
{
    private boolean isGuest;
    private String loginMsg;
    private String nickName;

    private String uid;

    private String channelUid;

    private String sid;

    public String getLoginMsg()
    {
        return this.loginMsg;
    }

    public String getNickName()
    {
        return this.nickName;
    }

    public String getUid()
    {
        return this.uid;
    }

    public boolean isGuest()
    {
        return this.isGuest;
    }

    public String isLoginMsg()
    {
        return this.loginMsg;
    }

    public void setGuest(boolean bGuest)
    {
        this.isGuest = bGuest;
    }

    public void setLoginMsg(String loginMsg)
    {
        this.loginMsg = loginMsg;
    }

    public void setNickName(String nickName)
    {
        this.nickName = nickName;
    }

    public void setUid(String uid)
    {
        this.uid = uid;
    }

    public void setChannelUid(String channelUid){
        this.channelUid = channelUid;
    }

    public String getChannelUid(){
        return this.channelUid;
    }

    public void setSid(String sid){
        try
        {
            this.sid = sid;
        }
        catch(Exception e)
        {
            e.printStackTrace();
        }
    }

    public String getSid(){
        return this.sid;
    }

    public String encode(String value)
    {
        String encoded = null;
        try
        {
            encoded = URLEncoder.encode(value, "UTF-8");
        }
        catch(UnsupportedEncodingException unsupportedencodingexception) {

        }

        StringBuffer buf = new StringBuffer(encoded.length());
        for(int i = 0; i < encoded.length(); i++)
        {
            char focus = encoded.charAt(i);
            if(focus == '*')
                buf.append("%2A");
            else
            if(focus == '+')
                buf.append("%20");
            else
            if(focus == '%' && i + 1 < encoded.length() && encoded.charAt(i + 1) == '7' && encoded.charAt(i + 2) == 'E')
            {
                buf.append('~');
                i += 2;
            } else
            {
                buf.append(focus);
            }
        }

        return buf.toString();
    }

    private String payExt;
    //支付下订单时需要上传的字段
    public void setPayExt(String payExt){
        this.payExt = payExt;
    }

    public String getPayExt(){
        return this.payExt;
    }

}