
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UnityEngine;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorMainViewController : SkillEditorBaseController<SkillEditorMainView>
    {
        private enum ReturnInfoTypeMode
        {
            Insert,
            Modify,
        }
        private ReturnInfoTypeMode _returnMode;

        private SkillConfigInfo _curSkillInfo;
        private TabContainerController<SkillEditorNodeItemController> _nodeContainerController = new TabContainerController<SkillEditorNodeItemController>();
        private List<SkillEditorInfoNode> _infoNodeList = new List<SkillEditorInfoNode>();

        protected override void AfterInitView()
        {
            base.AfterInitView();

            UpdateBattle_view();
            UpdateSkillInfo();
            UpdateCharcterName(SkillEditorController.Instance.CurChar);
            UpdateTeamNumber();
            UpdateSkillConfig();
            RandomBattleTarget();

            UICamera.onScreenResize += UpdateBattle_view;
            UIFollowTarget.GameCameraRectFix += GameCameraRectFix;

            _nodeContainerController.SetData(CreateNodeItem, null, NodeItemOnClick, true);

            EventDelegate.Set(_view.DelBtn_UIButton.onClick, DeleteCurSkillInfo);
            EventDelegate.Set(_view.SaveAsBtn_UIButton.onClick, SaveAsBtnOnClick);
            EventDelegate.Set(_view.SelectBtn_UIButton.onClick, SelectBtnOnClick);
            EventDelegate.Set(_view.SaveBtn_UIButton.onClick, SaveSkillInfo);
            EventDelegate.Set(_view.CharBtn_UIButton.onClick, CharBtnOnClick);
            EventDelegate.Set(_view.PlayBtn_UIButton.onClick, PlayBtnOnClick);
            EventDelegate.Set(_view.RandomBtn_UIButton.onClick, RandomBattleTarget);
            EventDelegate.Set(_view.InsertInfoBtn_UIButton.onClick, InertInfoBtnOnClick);
            EventDelegate.Set(_view.DelInfoBtn_UIButton.onClick, DelInfoBtnOnClick);
            EventDelegate.Set(_view.UpInfoBtn_UIButton.onClick, UpBtnOnClick);
            EventDelegate.Set(_view.DownInfoBtn_UIButton.onClick, DownBtnOnClick);
            EventDelegate.Set(_view.LineBtn_UIButton.onClick, LineBtnOnClick);
            EventDelegate.Set(_view.ExitBtn_UIButton.onClick, ExitBtnOnClick);
        }

        private void ExitBtnOnClick()
        {
            ProxySkillEditorModule.CloseMainView();
            GamePlayer.CameraManager.Instance.BattleCameraController.ResetCamera();
            SkillEditor.CameraManager.Instance.ShowGameUI ();
        }

        protected override void OnDispose()
        {
            UICamera.onScreenResize -= UpdateBattle_view;
            UIFollowTarget.GameCameraRectFix -= GameCameraRectFix;

            _nodeContainerController.Dispose();
            _infoNodeList.Clear();

            base.OnDispose();
        }

        #region Play相关
        private void UpdateBattle(CharactorAgent charac)
        {
            var teamANum = int.Parse(_view.TeamAInput_UIInput.value);
            var teamBNum = int.Parse(_view.TeamBInput_UIInput.value);
            UpdateCharcterName(charac);

            SkillEditorController.Instance.UpdateBattle(teamANum, teamBNum, charac);
        }

        private void PlayBtnOnClick()
        {
            if (_curSkillInfo == null)
            {
                return;
            }

            var teamANum = int.Parse(_view.TeamAInput_UIInput.value);
            var teamBNum = int.Parse(_view.TeamBInput_UIInput.value);
            var attackUid = long.Parse(_view.AttackInput_UIInput.value);
            var defendUid = long.Parse(_view.DefendInput_UIInput.value);
            var targetNum = int.Parse(_view.TargetNumInput_UIInput.value);
            var atOnce = _view.AtOnceToggle_UIToggle.value;
            var multiPart = int.Parse(_view.MultiPartInput_UIInput.value);

            SkillEditorController.Instance.PlayBattle(teamANum, teamBNum, _curSkillInfo, attackUid, defendUid, targetNum, atOnce, multiPart);
        }
        #endregion


        #region 窗口调整
        private void UpdateBattle_view()
        {
            var root = _view.LeftBack_UISprite.root;
            var width = _view.LeftBack_UISprite.width / root.pixelSizeAdjustment;
            var height = _view.TopBack_UISprite.height / root.pixelSizeAdjustment;
            SkillEditorController.Instance.UpdateBattleCameraView(1f * width / Screen.width, 0, 1, 1 - 1f * height / Screen.height);
        }

        private Vector3 GameCameraRectFix(Camera gameCamera, Vector3 pos)
        {
            return new Vector3(pos.x * (1 - gameCamera.rect.x) + gameCamera.rect.x, pos.y * gameCamera.rect.height, pos.z);
        }
        #endregion


        #region 技能读写表
        private void UpdateSkillInfo()
        {
            UpdateCurSkillInfoName();
            UpdateInfoNodeList();
        }

        private void UpdateCurSkillInfo(SkillConfigInfo info)
        {
            if (info == null)
            {
                return;
            }

            _curSkillInfo = SkillEditorInfoCollection.DeepCopySkillInfo(info);
            UpdateSkillInfo();
            UpdateSkillConfig();
        }

        private void UpdateCurSkillInfoName()
        {
            if (_curSkillInfo != null)
            {
                _view.SkillName_UILabel.text = SkillEditorInfoCollection.GetSkillInfoName(_curSkillInfo);
            }
            else
            {
                _view.SkillName_UILabel.text = "请选择技能!";
            }
        }

        private void DeleteCurSkillInfo()
        {
            if (_curSkillInfo != null)
            {
                SkillEditorController.Instance.DeleteSkillInfo(_curSkillInfo.id);
            }
        }

        private void SaveAsBtnOnClick()
        {
            ProxySkillEditorModule.OpenSaveAsView(SaveAsSkillInfo);
        }

        private void SaveAsSkillInfo(int id, string name)
        {
            if (_curSkillInfo == null)
            {
                _curSkillInfo = new SkillConfigInfo();
                _curSkillInfo.attackerActions = new List<BaseActionInfo>();
                _curSkillInfo.injurerActions = new List<BaseActionInfo>();
            }
            _curSkillInfo.id = id;
            _curSkillInfo.name = name;

            UpdateSkillInfo();
            SaveSkillInfo();
        }

        private void SelectBtnOnClick()
        {
            ProxySkillEditorModule.OpenSelectView(SkillEditorController.Instance.SkillInfoList, UpdateCurSkillInfo, SkillEditorInfoCollection.GetSkillInfoName, _curSkillInfo != null ?SkillEditorController.Instance.SkillInfoList.Find(info => info.id == _curSkillInfo.id) : null);
        }

        private void SaveSkillInfo()
        {
            if (_curSkillInfo != null)
            {
                SkillEditorController.Instance.AddOrUpdateSkillInfo(_curSkillInfo);
            }
        }
        #endregion

        #region 场景配置
        private void UpdateCharcterName(CharactorAgent charac)
        {
            _view.CharName_UILabel.text = SkillEditorInfoCollection.GetCharcterName(charac);
        }

        private void UpdateTeamNumber()
        {
            _view.TeamAInput_UIInput.value =
                SkillEditorController.Instance.GetMonsterListNum(BattlePositionAgent.BattleSide.TeamA).ToString();
            _view.TeamBInput_UIInput.value =
                SkillEditorController.Instance.GetMonsterListNum(BattlePositionAgent.BattleSide.TeamB).ToString();
        }

        private void CharBtnOnClick()
        {
            ProxySkillEditorModule.OpenSelectView(SkillEditorInfoCollection.GetCharacterList(), CharChangeAction, SkillEditorInfoCollection.GetCharcterName);
        }

        private void CharChangeAction(CharactorAgent charac)
        {
            UpdateBattle(charac);
        }
        #endregion

        #region 战斗配置
        private void SetDefaultSkillConfig()
        {
            _view.TargetNumInput_UIInput.value = SkillEditorConst.DefaultTargetNum;
            _view.AtOnceToggle_UIToggle.value = SkillEditorConst.DefaultAtOnce;
            _view.MultiPartInput_UIInput.value = SkillEditorConst.DefaultMultiPart;
        }


        private void UpdateSkillConfig()
        {
            SetDefaultSkillConfig();

            if (_curSkillInfo != null)
            {
                var skill = DataCache.getDtoByCls<Skill>(_curSkillInfo.id);
                if (skill != null)
                {
                    _view.TargetNumInput_UIInput.value = skill.targetNum.ToString();
                    _view.AtOnceToggle_UIToggle.value = skill.atOnce;
                }
            }
        }
        #endregion

        #region 攻击目标配置
        private void RandomBattleTarget()
        {
            _view.AttackInput_UIInput.value =
                SkillEditorController.Instance.GetMonsterList(BattlePositionAgent.BattleSide.TeamA)
                    .Random()
                    .GetId()
                    .ToString();
            _view.DefendInput_UIInput.value =
                SkillEditorController.Instance.GetMonsterList(BattlePositionAgent.BattleSide.TeamB)
                    .Random()
                    .GetId()
                    .ToString();
        }
        #endregion

        #region 技能节点
        private void LineBtnOnClick()
        {
            ProxySkillEditorModule.OpenSkillEditorLineView(_infoNodeList);
        }

        private SkillEditorNodeItemController CreateNodeItem()
        {
            return SkillEditorHelper.CreateAndSetItemFast<SkillEditorNodeItemController>(SkillEditorNodeItem.NAME,
                _view.InfoGrid_UIGrid.gameObject);
        }

        private void NodeItemOnClick(int index, int lastIndex)
        {
            // 选中之后再次点击
            if (index == lastIndex && index >= 0)
            {
                var infoNode = _infoNodeList[index];
                OpenSkillEditorInfoTypeView(infoNode);
            }
        }

        private void UpdateInfoNodeList(bool updatePost = true, int selectedIndex = TabConst.NoneSelected)
        {
            _infoNodeList.Clear();
            if (_curSkillInfo != null)
            {
                SortInfoNodeList();
            }

            var mainIndex = 0;
            var subIndex = 0;
            _nodeContainerController.UpdateItemList(_infoNodeList.Count, (i, controller) =>
            {
                var node = _infoNodeList[i];
                if (node.IsActionInfo())
                {
                    mainIndex++;
                    subIndex = 0;
                    controller.SetData(node, mainIndex, subIndex);
                }
                else
                {
                    subIndex++;
                    controller.SetData(node, mainIndex, subIndex);
                }
            });
            _nodeContainerController.SetSelected(selectedIndex);

            _view.InfoGrid_UIGrid.Reposition();
            if (updatePost)
            {
                _view.InfoScrollView_UIScrollView.ResetPosition();
            }
        }


        private void SortInfoNodeList()
        {
            _infoNodeList.Clear();
            if (_curSkillInfo != null)
            {
                var hasCountInjuire = false;

                foreach (var attackerAction in _curSkillInfo.attackerActions)
                {
                    if (InsertNode(_infoNodeList, attackerAction, true, hasCountInjuire))
                    {
                        hasCountInjuire = true;
                        InsertInjurerActionList(_infoNodeList, _curSkillInfo.injurerActions);
                    }
                }
                if (!hasCountInjuire)
                {
                    InsertInjurerActionList(_infoNodeList, _curSkillInfo.injurerActions);
                }
            }
        }

        private void InsertInjurerActionList(List<SkillEditorInfoNode> nodeList, List<BaseActionInfo> injurerList)
        {
            foreach (var injurerAction in injurerList)
            {
                InsertNode(nodeList, injurerAction, false, true);
            }
        }

        private bool InsertNode(List<SkillEditorInfoNode> nodeList, BaseActionInfo info, bool isAttack, bool hasCountInjuire)
        {
            var isDamageNode = false;

            nodeList.Add(new SkillEditorInfoNode(info, null, isAttack));
            foreach (var effectInfo in info.effects)
            {
                nodeList.Add(new SkillEditorInfoNode(info, effectInfo, isAttack));
                if (hasCountInjuire) continue;
                if (effectInfo is TakeDamageEffectInfo)
                {
                    isDamageNode = true;
                }
            }

            return isDamageNode;
        }

        private void InertInfoBtnOnClick()
        {
            OpenSkillEditorInfoTypeView();
        }

        private void DelInfoBtnOnClick()
        {
            var item = _nodeContainerController.CurrentSelected;
            if (item == null)
            {
                return;
            }

            var node = _infoNodeList[_nodeContainerController.CurrentSelectedIndex];
            if (node.IsActionInfo())
            {
                var actionList = GetCurSkillInfoActionList(node.IsAttack);
                actionList.Remove(node.ActionInfo);
            }
            else
            {
                node.ActionInfo.effects.Remove(node.EffectInfo);
            }

            UpdateInfoNodeList(false);
        }

        private void UpBtnOnClick()
        {
            AdjustNodeInfo(true);
        }

        private void DownBtnOnClick()
        {
            AdjustNodeInfo(false);
        }

        private void AdjustNodeInfo(bool isUp)
        {
            var item = _nodeContainerController.CurrentSelected;
            if (item == null)
            {
                return;
            }
            var node = _infoNodeList[_nodeContainerController.CurrentSelectedIndex];
            if (!node.IsActionInfo())
            {
                return;
            }

            var actionList = GetCurSkillInfoActionList(node.IsAttack);
            var actionIndex = actionList.FindIndex(info => info == node.ActionInfo);
            var newIndex = isUp ? (actionIndex - 1) : (actionIndex + 1);
            if (newIndex >= 0 && newIndex < actionList.Count)
            {
                actionList.RemoveAt(actionIndex);
                actionList.Insert(newIndex, node.ActionInfo);
                UpdateInfoNodeList(false);
            }
        }

        private void OpenSkillEditorInfoTypeView(SkillEditorInfoNode infoNode = null)
        {
            _returnMode = infoNode == null ? ReturnInfoTypeMode.Insert : ReturnInfoTypeMode.Modify;

            ProxySkillEditorModule.OpenSkillEditorInfoTypeView(infoNode, OnInfoNodeReturn);
        }

        private void OnInfoNodeReturn(SkillEditorInfoNode infoNode)
        {
            if (_curSkillInfo == null)
            {
                return;
            }

            SkillEditorInfoCollection.FixInfoNode(infoNode);

            var item = _nodeContainerController.CurrentSelected;
            var curInfoNode = item != null ? _infoNodeList[_nodeContainerController.CurrentSelectedIndex] : null;
            var needUpdate = false;
            if (_returnMode == ReturnInfoTypeMode.Modify)
            {
                // 修改的话当然嘚是选中一个的状况下了
                if (curInfoNode != null)
                {
                    needUpdate = true;

                    if (curInfoNode.IsActionInfo())
                    {
                        // 修改 Action
                        infoNode.ActionInfo.effects = curInfoNode.ActionInfo.effects;
                        var actionList = GetCurSkillInfoActionList(curInfoNode.IsAttack);
                        actionList[actionList.IndexOf(curInfoNode.ActionInfo)] = infoNode.ActionInfo;
                    }
                    else
                    {
                        // 修改 Effect
                        var effects = curInfoNode.ActionInfo.effects;
                        effects[effects.IndexOf(curInfoNode.EffectInfo)] = infoNode.EffectInfo;
                        SortEffectInfoList(curInfoNode.ActionInfo.effects);
                    }
                }
            }
            else
            {
                if (curInfoNode != null)
                {
                    if (curInfoNode.IsActionInfo())
                    {
                        needUpdate = true;
                        if (infoNode.IsActionInfo())
                        {
                            // 插入节点，往后插
                            var actionList = GetCurSkillInfoActionList(infoNode.IsAttack);
                            var index = 0;
                            if (curInfoNode.IsAttack == infoNode.IsAttack)
                            {
                                index = actionList.FindIndex(info => info == curInfoNode.ActionInfo) + 1;
                            }
                            actionList.Insert(index, infoNode.ActionInfo);
                        }
                        else
                        {
                            // 插入 Effect，根据时间重排
                            infoNode.IsAttack = curInfoNode.IsAttack;
                            var effects = curInfoNode.ActionInfo.effects;
                            effects.Insert(effects.Count, infoNode.EffectInfo);
                            SortEffectInfoList(effects);
                        }
                    }
                }
                else
                {
                    // 插入一个节点，则自动往列表最前插入
                    if (infoNode.IsActionInfo())
                    {
                        needUpdate = true;

                        var actionList = GetCurSkillInfoActionList(infoNode.IsAttack);
                        actionList.Insert(0, infoNode.ActionInfo);
                    }
                }
            }

            if (needUpdate)
            {
                UpdateInfoNodeList(false);
            }
        }
        #endregion


        #region 辅助

        private List<BaseActionInfo> GetCurSkillInfoActionList(bool isAttack)
        {
            return isAttack ? _curSkillInfo.attackerActions : _curSkillInfo.injurerActions;
        }

        private void SortEffectInfoList(List<BaseEffectInfo> effectInfoList)
        {
            effectInfoList.Sort((info, effectInfo) => info.playTime.CompareTo(effectInfo.playTime));
        }
        #endregion
    }
}

#endif