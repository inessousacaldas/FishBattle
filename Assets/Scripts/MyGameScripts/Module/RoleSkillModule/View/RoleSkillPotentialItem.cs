using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using Assets.Scripts.MyGameScripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.MyGameScripts.Module.RoleSkillModule.View
{
    public class RoleSkillPotentialItem : BaseView
    {
        public const string NAME = "RoleSkillPotentialItem";
        public UITexture spPotentialIcon;
        public UISprite spSelected;
        public UILabel lblName;
        public Transform locked;
        public UISprite lblbg;
        public UILabel lblGrade;
        public UILabel lblLimit;

        public RoleSkillPotentialVO vo;
        public bool isCanSelect;
        private int curIndex;

        protected override void InitElementBinding()
        {
            InitAllChildComponents();
        }
        public void SetData(int index,IRoleSkillPotentialData iData)
        {
            curIndex = index;
            vo = iData.GetVOByID(iData.GetIDByIndex(curIndex));
            UIHelper.SetUITexture(spPotentialIcon, vo.cfgVO.icon, false);
            if (vo != null)
            {
                var roleLV = ModelManager.Player.GetPlayerLevel();
                var limitLV = iData.GetLimitByID(vo.id);
                var potentialLv = vo.infoDto != null ? vo.infoDto.grade : 0;
                isCanSelect = roleLV >= limitLV;
                spPotentialIcon.isGrey = !isCanSelect;
                locked.gameObject.SetActive(!isCanSelect);
                lblLimit.text = isCanSelect ? "" : string.Format("{0}级\n解锁", limitLV);
                lblGrade.text = isCanSelect ? potentialLv.ToString() : "";
                lblName.text = vo.cfgVO.name;
            }
            lblbg.width = lblName.width + 12;
        }

        public void SetSelected(bool sel)
        {
            spSelected.gameObject.SetActive(sel);
        }
        
        public void DisposeClient()
        {
            UIHelper.DisposeUITexture(spPotentialIcon);
        }
    }
}
