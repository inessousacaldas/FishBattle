using System;
using UnityEngine;

namespace SkillEditor
{
    /// <summary>
    /// 与具体游戏的技能表现业务逻辑相关，原则上换游戏只需要修改本处即可。
    /// @MarsZ 2017年05月02日15:58:20
    /// </summary>
    public class SBBattleSkillEditorController : BattleSkillEditorController
    {
        #region 业务相关逻辑

        protected override BaseEffectInfo GetAddEffectInfo()
        {
            BaseEffectInfo info = null;
            switch (_effectType)
            {
                case EffectType.Normal:
                    info = new NormalEffectInfo();
                    info.type = NormalEffectInfo.TYPE;
                    break;
                case EffectType.ShowInjure:
                    info = new ShowInjureEffectInfo();
                    info.type = ShowInjureEffectInfo.TYPE;
                    break;
//                case EffectType.ShowVirtualInjure:
//                    info = new ShowVirtualInjureEffectInfo();
//                    info.type = ShowVirtualInjureEffectInfo.TYPE;
//                    break;
                case EffectType.TakeDamage:
                    info = new TakeDamageEffectInfo();
                    info.type = TakeDamageEffectInfo.TYPE;
                    break;
                case EffectType.Sound:
                    info = new SoundEffectInfo();
                    info.type = SoundEffectInfo.TYPE;
                    break;
                case EffectType.Hide:
                    info = new HideEffectInfo();
                    info.type = HideEffectInfo.TYPE;
                    break;
//                case EffectType.Float:
//                    info = new FloatEffectInfo();
//                    info.type = FloatEffectInfo.TYPE;
//                    break;
                case EffectType.Shake:
                    info = new ShakeEffectInfo();
                    info.type = ShakeEffectInfo.TYPE;
                    break;
//                case EffectType.Area:
//                    info = new AreaEffectInfo();
//                    info.type = AreaEffectInfo.TYPE;
//                    break;
                case EffectType.BGIMG:
                    info = new ShowBGTextureEffectInfo();
                    info.type = ShowBGTextureEffectInfo.TYPE;
                    break;
            }

            return info;
        }

        protected override void DrawEffectInfoView(BaseActionInfo actionInfo, BaseEffectInfo info)
        {
            if (info == null)
            {
                return;
            }

            if (info.GetType() == typeof(NormalEffectInfo))
            {
                DrawNormalEffectInfoView(actionInfo, info as NormalEffectInfo);
            }
            else if (info.GetType() == typeof(TakeDamageEffectInfo))
            {
                DrawTakeDamageEffectInfoView(actionInfo, info as TakeDamageEffectInfo);
            }
            else if (info.GetType() == typeof(ShowInjureEffectInfo))
            {
                DrawShowInjureEffectInfoView(actionInfo, info as ShowInjureEffectInfo);
            }
//            else if (info.GetType() == typeof(ShowVirtualInjureEffectInfo))
//            {
//                DrawShowVirtualInjureEffectInfoView(actionInfo, info as ShowVirtualInjureEffectInfo);
//            }
            else if (info.GetType() == typeof(SoundEffectInfo))
            {
                DrawSoundEffectInfoView(actionInfo, info as SoundEffectInfo);
            }
            else if (info.GetType() == typeof(HideEffectInfo))
            {
                DrawHideEffectInfoView(actionInfo, info as HideEffectInfo);
            }
//            else if (info.GetType() == typeof(FloatEffectInfo))
//            {
//                DrawFloatEffectInfoView(actionInfo, info as FloatEffectInfo);
//            }
            else if (info.GetType() == typeof(ShakeEffectInfo))
            {
                DrawShakeEffectInfoView(actionInfo, info as ShakeEffectInfo);
            }
//            else if (info.GetType() == typeof(AreaEffectInfo))
//            {
//                DrawAreaEffectInfoView(actionInfo, info as AreaEffectInfo);
//            }
            else
            {
                Debug.LogError("DrawEffectInfoView type Error");
            }
        }

        #endregion

        #region 业务特色方法

        private void DrawNormalEffectInfoView(BaseActionInfo actionInfo, NormalEffectInfo info)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Space(50f);

            DrawBaseEffectInifoView(actionInfo, info);

            GUI.changed = false;

            SkillEffectType effectType = DrawSkillEffectType(info.GetHashCode().ToString(), "特效类型", info.name);
            string effectTypeStr = GUIHelper.DrawTextField("特效名字", info.name, 120, false, 0);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            float delayTime = GUIHelper.DrawFloatField("生命时间", info.delayTime, 30, false, 50);

            bool hitEff = GUIHelper.DrawToggle("是否受击", info.hitEff, 30, false);

            var target = DrawActTargetType(info.GetHashCode().ToString(), "作用目标", info.target);
            MountType mountType = DrawMountType(info.GetHashCode().ToString(), "作用锚点", info.mount);

            bool fixRotation = GUIHelper.DrawToggle("固定旋转", info.fixRotation, 30, false);
            bool faceToPrevious = GUIHelper.DrawToggle("指向", info.faceToPrevious, 30, false);
            
            bool faceToTarget = GUIHelper.DrawToggle("朝向目标", info.faceToTarget, 30, false);

            bool IsEffectHasCamera = GUIHelper.DrawToggle("特效带镜头", info.IsEffectHasCamera, 30, false);
            
            bool fly = GUIHelper.DrawToggle("是否飞行", info.fly, 30, false);
            var flyTarget = GUIHelper.DrawIntField("飞向目标", (int)info.flyTarget, 30, false, 60);
            float flyTime = GUIHelper.DrawFloatField("飞行停顿时间", info.flyTime, 30, false);
            bool follow = GUIHelper.DrawToggle("是否跟随", info.follow, 30, false);
            float offX = GUIHelper.DrawFloatField("起点偏移X", info.offX, 30, false);
            float offY = GUIHelper.DrawFloatField("起点偏移Y", info.offY, 30, false);
            float offZ = GUIHelper.DrawFloatField("起点偏移Z", info.offZ, 30, false);
            if (GUI.changed)
            {
                if (effectTypeStr != null && (effectTypeStr.Contains("skill_") || effectTypeStr.Contains("game_")))
                {
                    info.name = effectTypeStr;
                }
                else
                {
                    info.name = effectType.ToString();
                }
                info.delayTime = delayTime;
                info.target = (EffectTargetType)target;
                info.mount = mountType.ToString();
                info.fixRotation = fixRotation;
                info.faceToPrevious = faceToPrevious;
                info.faceToTarget = faceToTarget;
                info.hitEff = hitEff;

                info.IsEffectHasCamera = IsEffectHasCamera;
                
                info.flyTarget = (EffectTargetType)flyTarget;
                if (info.fly != fly)
                {
                    info.fly = fly;

                    info.follow = false;
                }
                else if (info.follow != follow)
                {
                    info.fly = false;
                    info.flyTarget = 0;

                    info.follow = follow;
                }
                
                info.offX = offX;
                info.offY = offY;
                info.offZ = offZ;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(50f);
            DragEffectTimeLine(info);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private int DrawActTargetType(string pGUIDPrefix, string title, EffectTargetType target)
        {
            GUIHelper.DrawBox(title);
            return GUIHelper.EnumPopup(pGUIDPrefix, "", target, GUILayout.Width(50));
        }

        private void DrawShakeEffectInfoView(BaseActionInfo actionInfo, ShakeEffectInfo info)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(50f);

            DrawBaseEffectInifoView(actionInfo, info);

            GUI.changed = false;
            float delayTime = GUIHelper.DrawFloatField("持续时间", info.delayTime, 30, false, 0);
            float intensity_x = GUIHelper.DrawFloatField("强度 (0.3) x", info.intensity.x, 30, false, 0);
            float intensity_y = GUIHelper.DrawFloatField("强度 (0.3) y", info.intensity.y, 30, false, 0);

            float intensity_z = GUIHelper.DrawFloatField("强度 (0.3) z", info.intensity.z, 30, false, 0);

            bool isHit = GUIHelper.DrawToggle("是否击中", info.isHit, 30, false);
            int PlayIndex = GUIHelper.DrawIntField("攻击或受击索引(-1)", info.PlayIndex, 30, false);

            if (GUI.changed)
            {
                info.delayTime = delayTime;
                info.intensity = new Vector3(intensity_x, intensity_y, intensity_z);
                info.isHit = isHit;
                info.PlayIndex = PlayIndex;
            }

            GUILayout.EndHorizontal();
        }
        
        private void DrawTakeDamageEffectInfoView(BaseActionInfo actionInfo, TakeDamageEffectInfo info)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Space(50f);

            DrawBaseEffectInifoView(actionInfo, info);

            float randomTime = GUIHelper.DrawFloatField("随机", info.randomTime, 30, false);
            if (GUI.changed)
            {
                info.randomTime = randomTime;
            }

            GUI.changed = false;

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(50f);
            DragEffectTimeLine(info);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawShowInjureEffectInfoView(BaseActionInfo actionInfo, ShowInjureEffectInfo info)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Space(50f);

            DrawBaseEffectInifoView(actionInfo, info);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(50f);
            DragEffectTimeLine(info);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawSoundEffectInfoView(BaseActionInfo actionInfo, SoundEffectInfo info)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Space(50f);

            DrawBaseEffectInifoView(actionInfo, info);

            string soundName = GUIHelper.DrawTextField("音效", info.name, 120, false, 40);
            if (GUI.changed)
            {
                info.name = soundName;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(50f);
            DragEffectTimeLine(info);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawHideEffectInfoView(BaseActionInfo actionInfo, HideEffectInfo info)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Space(50f);

            DrawBaseEffectInifoView(actionInfo, info);

            GUI.changed = false;
            float delayTime = GUIHelper.DrawFloatField("生命时间", info.delayTime, 30, false, 50);

            if (GUI.changed)
            {
                info.delayTime = delayTime;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(50f);
            DragEffectTimeLine(info);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        protected override void DrawNormalActionInfoView(NormalActionInfo info, bool pAttackerAction = false)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(20f);

            DrawBaseActionInifoView(info);

            GUI.changed = false;

            float startTime = GUIHelper.DrawFloatField("延时", info.startTime, 30, false);
            float delayTime = GUIHelper.DrawFloatField("生命时间", info.delayTime, 30, false);

            if (GUI.changed)
            {
                info.startTime = startTime;
                info.delayTime = delayTime;
            }

            GUILayout.EndHorizontal();
        }

//        private void DrawShowVirtualInjureEffectInfoView(BaseActionInfo actionInfo, ShowVirtualInjureEffectInfo info)
//        {
//            GUILayout.BeginHorizontal();
//
//            GUILayout.Space(50f);
//
//            DrawBaseEffectInifoView(actionInfo, info);
//            float percent = GUIHelper.DrawFloatField("虚拟伤害占比", info.percent, 120,false, 0);
//            if (info.injureShowTime < 0.05f)
//                info.injureShowTime = 0.3f;
//            float injureShowTime = GUIHelper.DrawFloatField("伤害飘字持续时间", info.injureShowTime, 120, false, 0);
//            if (GUI.changed)
//            {
//                info.percent = percent;
//                info.injureShowTime = injureShowTime;
//            }
//            GUILayout.EndHorizontal();
//        }


//        private void DrawFloatEffectInfoView(BaseActionInfo actionInfo, FloatEffectInfo info)
//        {
//            GUILayout.BeginHorizontal();
//
//            GUILayout.Space(50f);
//
//            DrawBaseEffectInifoView(actionInfo, info);
//
//            GUI.changed = false;
//            float delayTime = GUIHelper.DrawFloatField("持续时间", info.delayTime, 30, false, 0);
//            float floatHeight = GUIHelper.DrawFloatField("浮空高度", info.floatHeight, 30, false, 0);
//            float floatingTime = GUIHelper.DrawFloatField("上升与下落时间", info.floatingTime, 30, false, 0);
//
//            if (GUI.changed)
//            {
//                info.delayTime = delayTime;
//                info.floatHeight = floatHeight;
//                info.floatingTime = floatingTime;
//            }
//
//            GUILayout.EndHorizontal();
//        }

//        private void DrawShakeEffectInfoView(BaseActionInfo actionInfo, ShakeEffectInfo info)
//        {
//            GUILayout.BeginHorizontal();
//
//            GUILayout.Space(50f);
//
//            DrawBaseEffectInifoView(actionInfo, info);
//
//            GUI.changed = false;
//            float delayTime = GUIHelper.DrawFloatField("持续时间", info.delayTime, 30, false, 0);
//            float intensity = GUIHelper.DrawFloatField("强度 (0.3)", info.intensity, 30, false, 0);
//            bool isHit = GUIHelper.DrawToggle("是否击中", info.isHit, 30, false);
//
//            if (GUI.changed)
//            {
//                info.delayTime = delayTime;
//                info.intensity = intensity;
//                info.isHit = isHit;
//            }
//
//            GUILayout.EndHorizontal();
//        }

//        private void DrawAreaEffectInfoView(BaseActionInfo actionInfo, AreaEffectInfo info)
//        {
//            GUILayout.BeginHorizontal();
//
//            GUILayout.Space(50f);
//
//            DrawBaseEffectInifoView(actionInfo, info);
//
//            GUI.changed = false;
//            float delayTime = GUIHelper.DrawFloatField("持续时间", info.delayTime, 30, false, 0);
//            string effectTypeStr = GUIHelper.DrawTextField("资源名字", info.name, 120, false, 0);
//
//            bool isCrit = GUIHelper.DrawToggle("是否暴击", info.isCrit, 30, false);
//            bool isSeal = GUIHelper.DrawToggle("是否封印", info.isSeal, 30, false);
//            bool isDeBuff = GUIHelper.DrawToggle("是否减益", info.isDeBuff, 30, false);
//
//            if (GUI.changed)
//            {
//                info.delayTime = delayTime;
//                info.name = effectTypeStr;
//
//                info.isCrit = isCrit;
//                info.isSeal = isSeal;
//                info.isDeBuff = isDeBuff;
//            }
//
//            GUILayout.EndHorizontal();
//        }

        #endregion
    }

    #region 枚举

    public enum ActionType
    {
        Normal,
        Move,
        MoveBack,
    }

    public enum MountType
    {
        Mount_Hit,
        Mount_HUD,
        Mount_Shadow,
    }

    public enum SkillEffectType
    {
        hit,
        hit1,
        hit2,
        att,
        fly,
        full,
        follow,
    }

    #endregion
}

