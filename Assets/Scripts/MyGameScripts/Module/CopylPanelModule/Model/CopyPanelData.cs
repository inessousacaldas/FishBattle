// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/20/2018 11:55:37 AM
// **********************************************************************

using AppDto;

public interface ICopyPanelData
{
    int GetCurCopyID();
    int GetCopyType();

    int GetEnterCopyID();
}

public sealed partial class CopyPanelDataMgr
{
    public sealed partial class CopyPanelData:ICopyPanelData
    {
        //当前副本ID界面索引
        public int CopyID = -1;

        //当前副本类型 0 普通副本 1 精英副本
        public int CopyType = 0;

        //实际进入副本ID
        public int EnterCopy = 0;

        public int GetCurCopyID(){
            return CopyID;
        }

        public int GetCopyType()
        {
            return CopyType;
        }

        public int GetEnterCopyID() {
            return EnterCopy;
        }


        public void InitData()
        {

        }

        public void Dispose()
        {

        }

        public void OnClose() {
            CopyID = -1;
            CopyType = 0;
            EnterCopy = 0;
        }
    }
}
