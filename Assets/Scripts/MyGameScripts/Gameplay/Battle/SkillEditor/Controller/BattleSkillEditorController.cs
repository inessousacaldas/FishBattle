using System;
using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;
using AnimType = ModelHelper.AnimType;

namespace SkillEditor
{
    /// <summary>
    /// 战斗技能编辑器编辑窗
    /// @MarsZ 2017年04月25日20:39:05
    /// </summary>
    public class BattleSkillEditorController : MonoBehaviour
    {
        #region interface

        public void Show()
        {
            //默认加载好数据
            StartCoroutine(AutoLoadConfigDelay());
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            DestroySkillEditorInfo();
        }

        #endregion

        #region

        private System.Collections.IEnumerator AutoLoadConfigDelay()
        {
            UICamera.ignoreAllEvents = true;
            yield return new WaitForSeconds(0.1f);
            LoadFontSizeConfig();
            LoadConfig();
        }

        #endregion

        private const string BattleConfig_Path = "Assets/GameResources/ConfigFiles/BattleConfig/BattleConfig.bytes";

        private List<SkillConfigInfo> _configInfoDic = new List<SkillConfigInfo>();

        private SkillConfigInfo _skillConfigInfo = null;
        private int mEnemyCount = 0;
        private int mFormation = 0;
        //是否因为人数改变了而需要重新打开预览界面
        private bool mIsEnemyInfoChanged = false;
        private readonly string tTimerName = "DrawReviewToolView";

        private bool mHideAllUIForSkillPreview = false;
        private Vector3 mScreenGUIArea;
        private int mFontSize = GUIHelper.DEFAULT_FONT_SIZE;
        //所有特效的最大播放时间（播放时间点）
        private const float MAX_PLAY_TIME = 10f;

        protected ActionType _actionType = ActionType.Normal;

        protected EffectType _effectType = EffectType.Normal;

        #region

        private void OnGUI()
        {
            mScreenGUIArea = GUILayout.BeginScrollView(mScreenGUIArea, "Window", GUILayout.Width(Screen.width), GUILayout.ExpandHeight(true));

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (!mHideAllUIForSkillPreview)
            {
                UpdateFontSize(Math.Max(GUIHelper.DrawIntField("字体大小", mFontSize, 100, true, 90), 1));
                DrawSkillSelectionListView();
            }
            DrawReviewToolView();
            GUILayout.EndVertical();

            if (!mHideAllUIForSkillPreview)
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                DrawEditorInfoView();
                GUILayout.Space(5f);
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        private void UpdateFontSize(int pTempSize)
        {
            if (GUI.changed && mFontSize != pTempSize)
            {
                mFontSize = pTempSize;
                GUI.skin.toggle.fontSize = GUI.skin.textField.fontSize = GUI.skin.textArea.fontSize = GUI.skin.scrollView.fontSize = 
                                GUI.skin.box.fontSize = GUI.skin.label.fontSize = GUI.skin.button.fontSize = mFontSize;
                SaveFontSizeConfig();
            }
        }

        private int _newSkillId = 0;
        private string _newSkillName = "";

        private void DrawConfigToolView()
        {
            if (GUIHelper.DrawButton("加载配置", GUILayout.Width(200f), GUILayout.Height(GUIHelper.DEFAULT_FUNCTION_BTN_HEIGHT)))
            {
                LoadConfig();
            }

            GUILayout.BeginHorizontal();
            _newSkillId = GUIHelper.DrawIntField("ID", _newSkillId, 50, false, 20);
            _newSkillName = GUIHelper.DrawTextField("名字", _newSkillName, 80, false, 40);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUIHelper.DrawButton("添加技能", GUILayout.Width(100f), GUILayout.Height(GUIHelper.DEFAULT_FUNCTION_BTN_HEIGHT)))
            {
                if (!ExistsSkill())
                {
                    SkillConfigInfo info = new SkillConfigInfo();
                    info.attackerActions = new System.Collections.Generic.List<BaseActionInfo>();
                    info.injurerActions = new System.Collections.Generic.List<BaseActionInfo>();
                    info.id = _newSkillId;
                    info.name = _newSkillName;
                    _skillConfigInfo = info;
                    _configInfoDic.Add(info);
                }
            }

            if (GUIHelper.DrawButton("查找技能", GUILayout.Width(100f), GUILayout.Height(GUIHelper.DEFAULT_FUNCTION_BTN_HEIGHT)))
            {
                if (!ExistsSkill())
                    TipManager.AddTip(string.Format("技能不存在！_newSkillId：{0}，_newSkillName：{1}", _newSkillId, _newSkillName));
                else
                {
                    GetSkillIndexPercent();
                }
            }
            GUILayout.EndHorizontal();

            if (GUIHelper.DrawButton("保存配置", GUILayout.Width(200f), GUILayout.Height(GUIHelper.DEFAULT_FUNCTION_BTN_HEIGHT)))
            {
                if (SaveConfig())
                    TipManager.AddTip("保存成功！");
                else
                    TipManager.AddTip("保存失败！");

                //          if (_battleConfigInfo != null)
                //          {
                //              _battleConfigInfo.list.Clear();
                //          }
                //          _battleConfigInfo = null;
                //          _skillConfigInfo = null;
            }
        }

        private void DrawReviewToolView()
        {
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (!mHideAllUIForSkillPreview)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(200));
                int tEnemyCount = GUIHelper.DrawIntField("敌人数目", EnemyCount);
                if (GUI.changed && EnemyCount != tEnemyCount)
                    EnemyCount = tEnemyCount;
                int tEnemyFormation = GUIHelper.DrawIntField("敌人阵型", EnemyFormation);
                if (GUI.changed && EnemyFormation != tEnemyFormation)
                    EnemyFormation = tEnemyFormation;
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUIHelper.DrawButton("播放", GUILayout.Width(100f), GUILayout.Height(GUIHelper.DEFAULT_FUNCTION_BTN_HEIGHT)))
            {
                if (null == _skillConfigInfo || _skillConfigInfo.id <= 0)
                    return;
                ProxyBattleSkillEditor.UpdateSkillConfigInfo(_skillConfigInfo);
                mHideAllUIForSkillPreview = true;

                PlaySkillPreview();
            }
            if (mHideAllUIForSkillPreview)
            {
                if (GUIHelper.DrawButton("编辑", GUILayout.Width(100f), GUILayout.Height(GUIHelper.DEFAULT_FUNCTION_BTN_HEIGHT)))
                {
                    mHideAllUIForSkillPreview = false;
                }
            }
            if (GUIHelper.DrawButton("退出", GUILayout.Width(100f), GUILayout.Height(GUIHelper.DEFAULT_FUNCTION_BTN_HEIGHT)))
            {
                if (GUIHelper.DisplayDialog("温馨提示", "是否已保存修改？", "继续退出", "取消退出"))
                    ProxyBattleSkillEditor.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void PlaySkillPreview()
        {
            if (mIsEnemyInfoChanged)
            {
                if (BattleSkillEditorPreviewManager.Instance.IsInBattle)
                    ProxyBattleSkillEditorPreview.ClosePreview(false);
                JSTimer.Instance.SetupCoolDown(tTimerName, 0.1f, null, () =>
                    {
                        ProxyBattleSkillEditor.OpenPreview(EnemyCount,EnemyFormation);
                        mIsEnemyInfoChanged = false;
                        PlaySkillPreview();
                    });
            }
            else
            {
                if (!BattleSkillEditorPreviewManager.Instance.IsInBattle)
                    ProxyBattleSkillEditor.OpenPreview(EnemyCount,EnemyFormation);
                JSTimer.Instance.SetupCoolDown(tTimerName, 0.1f, null, () =>
                    {
                        ProxyBattleSkillEditor.ReplayPreview(_skillConfigInfo.id);
                    });
            }
        }

        private void GetSkillIndexPercent()
        {
            if (!IsSkillListInited)
                return;
            bool tResult = false;
            SkillConfigInfo tSkillConfigInfo = null;
            int tIndex = _configInfoDic.FindIndex((info) =>
                {
                    tResult = info.id > 0 && info.id == _newSkillId;
                    tResult = tResult || (!string.IsNullOrEmpty(info.name) && info.name == _newSkillName);
                    if (tResult)
                        tSkillConfigInfo = info;
                    return tResult;
                });
            if (tResult)
            {
                _skillConfigInfo = tSkillConfigInfo;
                skillConfigListViewPos = new Vector2(0, tIndex * 23f);
            }
            return;
        }

        private bool ExistsSkill()
        {
            return ExistsSkill(_newSkillId, _newSkillName);
        }

        private bool ExistsSkill(int pSkillId, string pSkillName = null)
        {
            return _configInfoDic.Exists(
                (info) =>
                {
                    return info.id == _newSkillId || info.name == pSkillName;
                });
        }


        private Vector2 mSkillSelectionListViewPos;

        private void DrawSkillSelectionListView()
        {
            GUILayout.BeginVertical(GUILayout.Width(200));
            mSkillSelectionListViewPos = GUILayout.BeginScrollView(mSkillSelectionListViewPos, "TextField");
            DrawSkillConfigListView();
            GUILayout.Space(5);
            DrawConfigToolView();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void SortSkillConfigInfoByTime(SkillConfigInfo pSkillConfigInfo)
        {
            UnityEngine.Assertions.Assert.IsNotNull(pSkillConfigInfo);
            SortBaseActionInfoListByEffectTime(pSkillConfigInfo.attackerActions);
            SortBaseActionInfoListByEffectTime(pSkillConfigInfo.injurerActions);
        }

        private void SortBaseActionInfoListByEffectTime(List<BaseActionInfo> pAttackerActions)
        {
            if (null == pAttackerActions || pAttackerActions.Count <= 0)
                return;
            BaseActionInfo tBaseActionInfo = null;
            for (int tCounter = 0; tCounter < pAttackerActions.Count; tCounter++)
            {
                tBaseActionInfo = pAttackerActions[tCounter];
                UnityEngine.Assertions.Assert.IsNotNull(tBaseActionInfo);
                SortEffectsByTime(tBaseActionInfo.effects);
            }
        }

        private void SortBaseActionInfoByEffectTime()
        {
            SortSkillConfigInfoByTime(_skillConfigInfo);
        }

        private void SortEffectsByTime(List<BaseEffectInfo> pBaseEffectInfoList)
        {
            if (null == pBaseEffectInfoList || pBaseEffectInfoList.Count <= 1)
                return;
            pBaseEffectInfoList.Sort((pBaseEffectInfoA, pBaseEffectInfoB) =>
                {
                    return pBaseEffectInfoA.playTime.CompareTo(pBaseEffectInfoB.playTime);
                });
        }

        #region SelectionComponentView

        private Vector2 skillConfigListViewPos;

        #endregion

        private int _removeId = -1;

        private void DrawSkillConfigListView()
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(200f));

            if (GUIHelper.DrawHeader("技能列表"))
            {
                if (IsSkillListInited)
                {
                    skillConfigListViewPos = GUILayout.BeginScrollView(skillConfigListViewPos, "TextField");

                    foreach (SkillConfigInfo info in _configInfoDic)
                    {
                        DrawSkillCellView(info);
                    }
                    GUILayout.EndScrollView();
                }
            }
            GUILayout.EndVertical();

            if (_removeId >= 0)
            {
                _configInfoDic.Remove((info) =>
                    {
                        return info.id == _removeId;
                    });
                _removeId = -1;
            }
        }

        private void DrawSkillCellView(SkillConfigInfo info)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT));

            GUI.color = Color.white;
            GUI.skin.box.normal.textColor = new Color(145f / 255f, 228f / 255f, 1, 1);
            GUILayout.Box(info.id.ToString(), GUILayout.Width(50));
            //GUILayout.Box (info.name.ToString(), GUILayout.Width(50));

            GUI.color = Color.green;
            GUI.skin.button.normal.textColor = new Color(1, 1, 1, 1);
            if (GUIHelper.DrawButton(info.name.ToString(), GUILayout.Width(90)))
            {
                _skillConfigInfo = info;
            }
            GUI.color = Color.white;


            GUI.color = Color.red;
            if (GUIHelper.DrawButton("删", GUILayout.Width(30)))
            {
                if (_skillConfigInfo == info)
                {
                    _skillConfigInfo = null;
                }
                _removeId = info.id;
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }

        private void DrawTypeSelectGroup()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.Width(400));

            if (_skillConfigInfo != null)
            {
                GUILayout.Box("ID", GUILayout.Width(25));
                GUILayout.Box(_skillConfigInfo.id.ToString(), GUILayout.Width(50));
                string skillName = GUIHelper.DrawTextField("技能名字", _skillConfigInfo.name, 100, true, 100);
                if (GUI.changed)
                {
                    _skillConfigInfo.name = skillName;
                }
                GUILayout.Space(10f);
                if (GUIHelper.DrawButton("按时间重新排序", GUILayout.Width(100)))
                    SortBaseActionInfoByEffectTime();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUIHelper.DrawBox("行动类型", 100);
            _actionType = (ActionType)GUIHelper.EnumPopup("_actionType", string.Empty, _actionType, GUILayout.Width(100));
            GUILayout.Space(10f);
            GUIHelper.DrawBox("特效类型", 100);
            _effectType = (EffectType)GUIHelper.EnumPopup("_effectType", string.Empty, _effectType, GUILayout.Width(100f));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            //            GUILayout.Space(50f);
        }

        private void DrawSkillConfigView()
        {
            if (_skillConfigInfo != null)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.green;
                if (GUIHelper.DrawButton("加", GUILayout.Width(30), GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
                {
                    _skillConfigInfo.attackerActions.Insert(0, GetAddActionInfo());
                }
                GUI.color = Color.white;
                bool tShowDetail = GUIHelper.DrawHeader("攻击者信息");
                GUILayout.EndHorizontal();

                if (tShowDetail)
                {
                    for (int i = 0; i < _skillConfigInfo.attackerActions.Count; i++)
                    {
                        var actionInfo = _skillConfigInfo.attackerActions[i];
                        DrawActionInfoView(actionInfo,true);
                        GUILayout.Space(5f);
                    }

                }

                GUILayout.Space(10f);

                GUILayout.BeginHorizontal();
                GUI.color = Color.green;
                if (GUIHelper.DrawButton("加", GUILayout.Width(30), GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
                {
                    _skillConfigInfo.injurerActions.Insert(0, GetAddActionInfo());
                }
                GUI.color = Color.white;
                tShowDetail = GUIHelper.DrawHeader("受击者信息");
                GUILayout.EndHorizontal();

                if (tShowDetail)
                { 
                    for (int i = 0; i < _skillConfigInfo.injurerActions.Count; i++)
                    {
                        BaseActionInfo actionInfo = _skillConfigInfo.injurerActions[i] as BaseActionInfo;
                        DrawActionInfoView(actionInfo);
                        GUILayout.Space(5f);
                    }
                }
            }
        }

        private Vector2 mEditorInfoViewPos;

        private void DrawEditorInfoView()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            mEditorInfoViewPos = GUILayout.BeginScrollView(mEditorInfoViewPos, "Window", GUILayout.ExpandWidth(true));
            DrawTypeSelectGroup();
            GUILayout.Space(5f);
            DrawSkillConfigView();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private Vector2 mTimeLineViewPos;
        private float nTestSliderA;
        private float nTestSliderB;
        private float nTestSliderC;


        protected void DrawBaseEffectInifoView(BaseActionInfo actionInfo, BaseEffectInfo info)
        {
            GUI.color = Color.yellow;
            GUIHelper.DrawBox(info.type, 100);
            GUI.color = Color.white;

            GUI.color = Color.red;
            if (GUIHelper.DrawButton("删", GUILayout.Width(30), GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
            {
                actionInfo.effects.Remove(info);
                return;
            }
            GUI.color = Color.white;

            GUI.color = Color.green;
            if (GUIHelper.DrawButton("效", GUILayout.Width(30), GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
            {
                BaseEffectInfo effectInfo = GetAddEffectInfo();
                int index = actionInfo.effects.IndexOf(info);
                actionInfo.effects.Insert(index + 1, effectInfo);
                SortEffectsByTime(actionInfo.effects);
            }
            GUI.color = Color.white;

            GUI.changed = false;

            float playTime = GUIHelper.DrawFloatField("播放时间", info.playTime, 30, false, 0);

            if (GUI.changed)
            {
                info.playTime = playTime;
            }
        }

        protected void DragEffectTimeLine(BaseEffectInfo pBaseEffectInfo)
        {
            GUILayout.BeginHorizontal();
            GUIHelper.Space(20);
            GUIHelper.DrawBox("开始时间：", 100);
            float tPlayTime = pBaseEffectInfo.playTime;
            GUILayout.BeginVertical();
            GUIHelper.Space(10);
            tPlayTime = GUILayout.HorizontalSlider(tPlayTime, 0, MAX_PLAY_TIME, GUILayout.Width(400),GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT));
            if (GUI.changed)
            {
                if (pBaseEffectInfo.playTime != tPlayTime)
                {
                    pBaseEffectInfo.playTime = tPlayTime;
                    //                    SortBaseActionInfoByEffectTime();//legacy 编辑时不即时改变位置，因为会看起来怪异，2017年05月02日12:15:21
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void LoadConfig()
        {
            JsonBattleConfigInfo configInfo = FileHelper.ReadJsonFile<JsonBattleConfigInfo>(BattleConfig_Path, false);

            if (configInfo != null)
            {
                JsonSkillConfigInfo[] list = configInfo.list.ToArray();
                Array.Sort(list, delegate(JsonSkillConfigInfo x, JsonSkillConfigInfo y)
                    {
                        if (x.id >= y.id)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }); 
                _configInfoDic.Clear();
                SkillConfigInfo tSkillConfigInfo = null;
                foreach (JsonSkillConfigInfo info in list)
                {
                    tSkillConfigInfo = info.ToSkillConfigInfo();
                    SortSkillConfigInfoByTime(tSkillConfigInfo);
                    _configInfoDic.Add(tSkillConfigInfo);
                }
            }
        }

        private bool SaveConfig()
        {
            if (IsSkillListInited)
            {
                BattleConfigInfo configInfo = new BattleConfigInfo();
                configInfo.time = (DateTime.UtcNow.Ticks / 10000).ToString();
                configInfo.list = _configInfoDic.ToList();

                FileHelper.SaveJsonObj(configInfo, BattleConfig_Path, false);

                return true;
            }
            else
            {
                Debug.LogError("Battle Config is Empty!");
                return false;
            }
        }

        private void LoadFontSizeConfig()
        {
            mFontSize = PlayerPrefsExt.GetPlayerInt("BattleSkillEditorController_Font_Size", GUIHelper.DEFAULT_FONT_SIZE);
        }

        private void SaveFontSizeConfig()
        {
            PlayerPrefsExt.SetPlayerInt("BattleSkillEditorController_Font_Size", mFontSize);
        }

        private void DestroySkillEditorInfo()
        {
            UICamera.ignoreAllEvents = false;
            _skillConfigInfo = null;
        }

        private bool IsSkillListInited
        {
            get
            { 
                return null != _configInfoDic && _configInfoDic.Count > 0;
            }
        }

        private void DrawTimeLineView()
        {
            GUILayout.BeginVertical(/**GUILayout.Width(500)*/);
            mTimeLineViewPos = GUILayout.BeginScrollView(mTimeLineViewPos, "TextField");
            nTestSliderA = GUILayout.HorizontalSlider(nTestSliderA, 0, 500);
            nTestSliderB = GUILayout.HorizontalSlider(nTestSliderB, 0, 500);
            nTestSliderC = GUILayout.HorizontalSlider(nTestSliderC, 0, 500);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        protected void DrawBaseActionInifoView(BaseActionInfo info)
        {
            GUI.color = Color.yellow;
            GUIHelper.DrawBox(info.type, 100);
            GUI.color = Color.white;

            GUI.color = Color.red;
            if (GUIHelper.DrawButton("删", GUILayout.Width(30), GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
            {
                _skillConfigInfo.attackerActions.Remove(info);
                _skillConfigInfo.injurerActions.Remove(info);
                return;
            }
            GUI.color = Color.white;

            GUI.color = Color.green;
            if (GUIHelper.DrawButton("加", GUILayout.Width(30), GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
            {

                BaseActionInfo actionInfo = GetAddActionInfo();
                if (actionInfo != null)
                {
                    if (_skillConfigInfo.attackerActions.Contains(info))
                    {
                        int index = _skillConfigInfo.attackerActions.IndexOf(info);
                        _skillConfigInfo.attackerActions.Insert(index + 1, actionInfo);
                    }

                    if (_skillConfigInfo.injurerActions.Contains(info))
                    {
                        int index = _skillConfigInfo.injurerActions.IndexOf(info);
                        _skillConfigInfo.injurerActions.Insert(index + 1, actionInfo);
                    }
                }
            }

            if (GUIHelper.DrawButton("效", GUILayout.Width(30), GUILayout.Height(GUIHelper.DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
            {
                BaseEffectInfo effectInfo = GetAddEffectInfo();
                info.effects.Insert(0, effectInfo);
                SortEffectsByTime(info.effects);
            }
            GUI.color = Color.white;

            GUI.changed = false;

            AnimType animType = DrawAnimType(info.GetHashCode().ToString(), "动作", info.name);

            //      int rotateX = GUIExt.DrawIntField("rotateX", info.rotateX);
            //      int rotateY = GUIExt.DrawIntField("rotateY", info.rotateY);
            //      int rotateZ = GUIExt.DrawIntField("rotateZ", info.rotateZ);

            if (GUI.changed)
            {
                info.name = animType.ToString();
                //          info.rotateX = rotateX;
                //          info.rotateY = rotateY;
                //          info.rotateZ = rotateZ;
            }
        }

        protected SkillEffectType DrawSkillEffectType(string pGUIDPrefix, string title, string effect, int pWidth = 90)
        {
            GUIHelper.DrawBox(title);
            return (SkillEffectType)GUIHelper.EnumPopup(pGUIDPrefix, "", GetSkillEffectType(effect), GUILayout.Width(50));
        }

        private AnimType DrawAnimType(string pGUIDPrefix, string title, string anim, int pWidth = 30)
        {
            GUIHelper.DrawBox(title);
            return (AnimType)GUIHelper.EnumPopup(pGUIDPrefix, "", GetAnimType(anim), GUILayout.Width(70));
        }

        protected MountType DrawMountType(string pGUIDPrefix, string title, string mount, int pWidth = 90)
        {
            GUIHelper.DrawBox(title);
            return (MountType)GUIHelper.EnumPopup(pGUIDPrefix, "", EnumParserHelper.TryParse(mount, MountType.Mount_Hit), GUILayout.Width(90));
        }

        private int EnemyCount
        {
            get
            {
                if (mEnemyCount <= 0)
                    mEnemyCount = PlayerPrefsExt.GetPlayerInt("EnemyCountorSkillEditor", 1);
                return mEnemyCount;
            }
            set
            {
                if (mEnemyCount == value)
                    return;
                mEnemyCount = value;
                mIsEnemyInfoChanged = true;
                PlayerPrefsExt.SetPlayerInt("EnemyCountorSkillEditor", mEnemyCount);
            }
        }

        private int EnemyFormation
        {
            get
            {
                if (mFormation <= 0)
                    mFormation = PlayerPrefsExt.GetPlayerInt("EnemyFormationSkillEditor", 1);
                return mFormation;
            }
            set
            {
                if (mFormation == value)
                    return;
                mFormation = value;
                mIsEnemyInfoChanged = true;
                PlayerPrefsExt.SetPlayerInt("EnemyFormationSkillEditor", mFormation);
            }
        }
        #endregion

        #region 可选择性重载部分

        protected virtual AnimType GetAnimType(string name)
        {
            return EnumParserHelper.TryParse(name, AnimType.hit);
        }

        private SkillEffectType GetSkillEffectType(string name){
            return EnumParserHelper.TryParse(name, SkillEffectType.hit, false);    
        }

        private void DrawMoveActionInfoView(MoveActionInfo info)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(30f);

            DrawBaseActionInifoView(info);

            GUI.changed = false;

            var time = GUIHelper.DrawFloatField("时长", info.time, 30, false, 50);
            var distance = GUIHelper.DrawFloatField("距离", info.distance, 30, false, 50);
            var center = GUIHelper.DrawToggle("是否中心", info.center, 30, false);

            if (GUI.changed)
            {
                info.time = time;
                info.distance = distance;
                info.center = center;
            }

            GUILayout.EndHorizontal();
        }

        protected virtual void DrawActionInfoView(BaseActionInfo info, bool pAttackerAction = false)
        {
            if (info == null)
            {
                return;
            }

            if (info.GetType() == typeof(MoveActionInfo))
            {
                DrawMoveActionInfoView(info as MoveActionInfo);
            }
            else if (info.GetType() == typeof(NormalActionInfo))
            {
                DrawNormalActionInfoView(info as NormalActionInfo,pAttackerAction);
            }
            else if (info.GetType() == typeof(MoveBackActionInfo))
            {
                DrawMoveBackActionInfoView(info as MoveBackActionInfo);
            }
            else
            {
                Debug.LogError("DrawActionInfoView type Error");
            }

            for (int i = 0; i < info.effects.Count; i++)
            {
                BaseEffectInfo effectInfo = info.effects[i] as BaseEffectInfo;
                DrawEffectInfoView(info, effectInfo);
                GUILayout.Space(5f);
            }
        }

        protected virtual void DrawNormalActionInfoView(NormalActionInfo info, bool pAttackerAction = false)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(20f);

            DrawBaseActionInifoView(info);

            GUI.changed = false;

            float startTime = GUIHelper.DrawFloatField("延时", info.startTime, 30, false, 50);
            float delayTime = GUIHelper.DrawFloatField("生命时间", info.delayTime, 30, false, 50);

            if (GUI.changed)
            {
                info.startTime = startTime;
                info.delayTime = delayTime;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawMoveBackActionInfoView(MoveBackActionInfo info)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(20f);

            DrawBaseActionInifoView(info);

            GUI.changed = false;

            float time = GUIHelper.DrawFloatField("时长", info.time, 30, false, 50);

            if (GUI.changed)
            {
                info.time = time;
            }

            GUILayout.EndHorizontal();
        }

        protected virtual BaseActionInfo GetAddActionInfo()
        {
            BaseActionInfo info = null;
            switch (_actionType)
            {
                case ActionType.Normal:
                    info = new NormalActionInfo();
                    info.type = NormalActionInfo.TYPE;
                    break;
                case ActionType.Move:
                    info = new MoveActionInfo();
                    info.type = MoveActionInfo.TYPE;
                    break;
                case ActionType.MoveBack:
                    info = new MoveBackActionInfo();
                    info.type = MoveBackActionInfo.TYPE;
                    break;
            }

            if (info != null)
            {
                info.effects = new System.Collections.Generic.List<BaseEffectInfo>();
            }

            return info;
        }

        #endregion

        #region 必须重写的方法

        protected virtual BaseEffectInfo GetAddEffectInfo()
        {
            throw new NotImplementedException("必须重写 GetAddEffectInfo！");
        }

        protected virtual void DrawEffectInfoView(BaseActionInfo actionInfo, BaseEffectInfo info)
        {
            throw new NotImplementedException("必须重写 DrawEffectInfoView！");
        }

        #endregion

    }
}
