// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleFormationInfoViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using AppDto;
using UnityEngine;

public partial class BattleFormationInfoViewController
{
    private List<FormationGrade> _formationGradeList = new List<FormationGrade>();
    private readonly string[] _rankStr = { "前排", "中排", "后排" };
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _formationGradeList = DataCache.getArrayByCls<FormationGrade>();
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

    public void SetSelfFormation(FormationInfoDto formation)
    {
        var f = DataCache.getDtoByCls<Formation>(formation.formationId);
        if (f == null)
        {
            GameDebuger.LogError(string.Format("formation表找不到{0},请检查", formation.formationId));
            return;
        }
        if (formation.formationId == (int)Formation.FormationType.Regular)
        {
            _view.SelfName_UILabel.text = "无队形";
            _view.SelfDesc_UILabel.text = "";
            _view.SelfFormation_UISprite.spriteName = "";
            return;
        }

        _view.SelfFormation_UISprite.spriteName = string.Format("formationicon_{0}", f.id);
        _view.SelfName_UILabel.text = string.Format("{0}  等级: {1}", f.name, formation.level.WrapColor(ColorConstantV3.Color_Green_Str));
        SetFormationDesc(_view.SelfDesc_UILabel, formation.level, f.descMaxNum, f.description);
    }

    public void SetEnemyFormation(FormationInfoDto formation)
    {
        var f = DataCache.getDtoByCls<Formation>(formation.formationId);
        if (f == null)
        {
            GameDebuger.LogError(string.Format("formation表找不到{0},请检查", formation.formationId));
            return;
        }
        if (formation.formationId == (int) Formation.FormationType.Regular)
        {
            _view.EnemyName_UILabel.text = "无队形";
            _view.EnemyDesc_UILabel.text = "";
            _view.EnemyFormation_UISprite.spriteName = "";
            return;
        }

        _view.EnemyFormation_UISprite.spriteName = string.Format("formationicon_{0}", f.id);
        _view.EnemyName_UILabel.text = string.Format("{0} 等级: {1}", f.name, formation.level.WrapColor(ColorConstantV3.Color_Green_Str));
        SetFormationDesc(_view.EnemyDesc_UILabel, formation.level, f.descMaxNum, f.description);
    }

    public void UpdateBGHeight()
    {
        _view.Enemy_UIWidget.transform.localPosition = new Vector3(0, 93 - (_view.SelfDesc_UILabel.height - 22), 0);
        _view.BackGround_UISprite.height = (_view.SelfDesc_UILabel.height - 22) + (_view.EnemyDesc_UILabel.height - 22) + 240;
    }

    private void SetFormationDesc(UILabel label, int lv, string descMaxNum, string desc)
    {
        StringBuilder sb = new StringBuilder();
        var info = desc.Split(';');
        var max = descMaxNum.Split(';');
        info.ForEachI((f, i) =>
        {
            string str = "";
            var txt = f.Split(',');
            txt.ForEachI((s, idx) =>
            {
                if(idx < txt.Length - 1)
                    str += string.Format(s+",", GetFormationDescValue(lv, float.Parse(max[i].Split(',')[idx])));
                else
                    str += string.Format(s, GetFormationDescValue(lv, float.Parse(max[i].Split(',')[idx])));
            });
            if(i == info.Length - 1)
                sb.Append(_rankStr[i] + ":" + str);
            else
                sb.AppendLine(_rankStr[i] + ":" + str);
        });
        label.text = sb.ToString();
    }

    private string GetFormationDescValue(int lv, float maxValue)
    {
        var formationGrade = _formationGradeList[lv - 1];

        if (formationGrade == null)
        {
            GameDebuger.LogError(string.Format("FormationGrade表找不到{0}级相关的数据", lv));
            return "";
        }
        return string.Format("{0}%", maxValue * formationGrade.effect).WrapColor(ColorConstantV3.Color_Green_Str);
    }

}
