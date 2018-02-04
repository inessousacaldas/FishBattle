// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BossBloodBarController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UnityEngine;

public partial class BossBloodBarController
{

    private int bloodWidth = 200;
    private BloodBar cfgBarVO;
    private string[] bloodResList;

    private int maxBloodNum;
    private int curBloodNum;
    private int singleBloodHP;
    private int lastBloodNum;
    private int reduce = 0;
    private bool isLooping = false;
    private int speed = 0;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        View.spStatus_UISprite.alpha = 0.5f;
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private int curHP;
    private int curMaxHp;
    public void Reset(VideoSoldier soldier)
    {
        CheckMutiBlood();
        UpdateBlood(soldier.hp,soldier.maxHp);
        //JSTimer.Instance.SetupTimer("BloodBarCheck",() =>
        //{
        //    UpdateBlood(curHP - new RandomHelper().GetRandomInt(1,10),curMaxHp);
        //},0.3f);
    }

    public void UpdateBlood(int hp,int maxHP)
    {
        curHP = hp;
        curMaxHp = maxHP;
        if(maxBloodNum > 1)
        {
            UpdateMituBlood(hp,maxHP);
        }
        else
        {
            UpdateSingle(hp,maxHP);
        }
    }

    private void CheckMutiBlood()
    {
        maxBloodNum = 1;
        cfgBarVO = DataCache.getDtoByCls<BloodBar>(maxBloodNum);
        bloodResList = cfgBarVO.colorRes.Split('|');
        View.lblNum_UILabel.gameObject.SetActive(maxBloodNum > 1);
    }

    private void UpdateSingle(int hp,int maxHP)
    {
        View.spNext_UISprite.gameObject.SetActive(false);
        float per = Math.Max((float)hp / maxHP,0);
        //避免血量过少，血条出现0%的情况
        ResetStatusBar(per);

        if(per > 0.66f)
        {
            View.spCur_UISprite.spriteName = "blood_2";
        }
        else if(per > 0.33f && per <= 0.66f)
        {
            View.spCur_UISprite.spriteName = "blood_8";
        }
        else
        {
            View.spCur_UISprite.spriteName = "blood_10";
            if(per == 0)
            {
                View.spCur_UISprite.enabled = false;
            }
        }
        View.spCur_UISprite.width = (int)(bloodWidth * per);
        View.spStatus_UISprite.transform.localPosition = new Vector3(View.spCur_UISprite.width,0);
        View.spStatus_UISprite.width = reduce;
        StartLoop();
    }

    public void UpdateMituBlood(int hp,int maxHP)
    {
        View.spNext_UISprite.gameObject.SetActive(true);
        singleBloodHP = Mathf.FloorToInt(maxHP / maxBloodNum);
        float per;
        if(hp == maxHP)
        {
            curBloodNum = maxBloodNum;
            per = 1;
        }
        else
        {
            curBloodNum = Mathf.FloorToInt(hp / singleBloodHP) + 1;
            per = (float)(hp % singleBloodHP) / singleBloodHP;
        }
        lastBloodNum = curBloodNum;
        View.lblNum_UILabel.text = "×" + curBloodNum;
        //当前血条
        if(curBloodNum > 0)
        {
            View.spCur_UISprite.spriteName = bloodResList[(curBloodNum - 1) % 10];
        }
        ResetStatusBar(per);
        View.spCur_UISprite.width = (int)(bloodWidth * per);
        View.spStatus_UISprite.transform.localPosition = new Vector3(View.spCur_UISprite.width,0);
        View.spStatus_UISprite.width = reduce;
        StartLoop();

        //下条血
        if(curBloodNum > 1)
        {
            View.spNext_UISprite.spriteName = bloodResList[(curBloodNum - 2) % 10];
        }
        else
        {
            View.spNext_UISprite.gameObject.SetActive(false);
        }
    }

    private void ResetStatusBar(float per)
    {
        //重设
        View.spStatus_UISprite.width = 0;
        reduce = View.spCur_UISprite.width - (int)(bloodWidth * per);
        View.spStatus_UISprite.width = reduce;
    }

    private void OnLoop()
    {
        if(speed % 10 == 0)
        {
            if(reduce > 0)
            {
                reduce = View.spStatus_UISprite.width < 1 ? 0 : reduce / 2;
                View.spStatus_UISprite.width = reduce;
            }
            else
            {
                StopLoop();
            }
        }
        speed++;
    }

    private void StartLoop()
    {
        if(!isLooping)
        {
            View.spStatus_UISprite.spriteName = View.spCur_UISprite.spriteName;
            View.spStatus_UISprite.gameObject.SetActive((View.spCur_UISprite.width)< (bloodWidth - 14) && reduce > 13);
            if(View.spStatus_UISprite.gameObject.activeSelf)
            {
                isLooping = true;
                JSTimer.Instance.SetupTimer("BossBloodBarController.StartLoop",OnLoop,0.03f);
            }
        }
    }

    private void StopLoop()
    {
        if(isLooping)
        {
            isLooping = false;
            View.spStatus_UISprite.gameObject.SetActive(false);
            JSTimer.Instance.RemoveTimerUpdateHandler("BossBloodBarController.StartLoop",OnLoop);
        }
    }
}
