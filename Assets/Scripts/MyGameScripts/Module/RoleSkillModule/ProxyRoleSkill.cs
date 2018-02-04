using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.MyGameScripts.Module.SkillModule
{
    class ProxyRoleSkill
    {
        public static void OpenPanel(RoleSkillTab tab = RoleSkillTab.Potential)
        {
            RoleSkillDataMgr.RoleSkillViewLogic.Open(tab);
        }
    }
}
