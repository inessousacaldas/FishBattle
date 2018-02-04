package com.demiframe.game.api.common;

public class LHResult
{
    private int code;
    private String data;
    private Object extra;

    public int getCode()
    {
        return this.code;
    }

    public String getData()
    {
        return this.data;
    }

    public Object getExtra()
    {
        return this.extra;
    }

    public void setCode(int code)
    {
        this.code = code;
    }

    public void setData(String data)
    {
        this.data = data;
    }

    public void setExtra(Object obj)
    {
        this.extra = obj;
    }
}
