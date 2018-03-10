//// **********************************************************************
//// Copyright (c)  Baoyugame. All rights reserved.
//// File     :  FormationConstrainViewController.cs
//// Author   : 
//// Created  : $timeDecls$
//// Porpuse  : 
//// **********************************************************************
//
//using System;
//using System.Collections.Generic;
//using AppDto;
//using UniRx;
//using UnityEngine;
//
//public sealed partial class FormationDataMgr
//{
//    public partial class FormationConstrainViewController    {
//
//        public static void Open()
//        {
//            UIModuleManager.Instance.OpenFunModule<FormationConstrainViewController>(
//                FormationConstrainView.NAME
//                , UILayerType.SubModule
//                , true
//            );
//        }
//
//        private List<Formation> _formations;
//        private List<FormationDebuffItemController> positiveItemList = new List<FormationDebuffItemController>();
//        private List<FormationDebuffItemController> negativeItemList = new List<FormationDebuffItemController>();
//        private int curPage;
//
//	    // 界面初始化完成之后的一些后续初始化工作
//        protected override void AfterInitView ()
//        {
//            _formations = Instance._data.allFormationList;
//
//            var pageTurn = AddController<PageTurnViewController, PageTurnView>(View.PageTurnViewGO);
//            var max = _formations.Count;
//            GameLog.Log_Formation(" max  "+  max);
//            curPage = _formations.FindIndex(s => s.id == Instance._data._curFormation.id);
//            pageTurn.InitData(
//                curPage
//                , max
//                , false
//                , ShowType.blank
//                , actType:BtnActType.None);
//            _disposable = _disposable.CombineRelease(pageTurn.Stream.Subscribe(page =>
//            {
//                curPage = page;
//                UpdateView();
//            }));
//            UpdateView();
//        }
//
//	    // 客户端自定义代码
//	    protected override void RegistCustomEvent ()
//        {
//        
//        }
//
//        protected override void OnDispose()
//        {
//            if (_disposable != null)
//                _disposable.Dispose();
//            _disposable = null;
//        }
//
//	    //在打开界面之前，初始化数据
//	    protected override void InitData()
//    	{
//
//    	}
//
//        private void CloseBtn_UIButtonClickHandler()
//        {
//            UIModuleManager.Instance.CloseModule(FormationConstrainView.NAME);
//        }
//
//        private void UpdateView()
//        {
//            var formation = Instance._data.GetFormationByIdx(curPage);
//            if (formation == null)
//                return;
//            var formationId = formation.id;
//            var effectDic = FormationData._positiveEffectDict[formationId];
//            int index = 0;
//            foreach (var pair in effectDic)
//            {
//                FormationDebuffItemController com = null;
//                int percent = pair.Key;
//                pair.Value.ForEachI(delegate(int i, int idx)
//                {
//                    positiveItemList.TryGetValue(index, out com);
//                    if (com == null)
//                    {
//                        com = CreateDebuffItem(View.PositiveGrid.gameObject, View.PositiveScrollView);
//                        positiveItemList.Add(com);
//                    }
//
//                    var _formation = DataCache.getDtoByCls<Formation>(pair.Value[i]);
//                    string sPercent = percent + "%";
//                    sPercent = sPercent.WrapColor(ColorConstantV3.Color_Green_Strong_Str);
//                    com.SetName(string.Format("#f{0} {1}", _formation.id, _formation.name));
//                    com.SetEffect(sPercent);
//                    com.gameObject.SetActive(true);
//                    ++index;
//                });
//
//            }
//
//            for(int i= index; i< positiveItemList.Count; ++i)
//            {
//                positiveItemList[i].gameObject.SetActive(false);
//            }
//
//            View.PositiveGrid.Reposition();
//            View.PositiveScrollView.ResetPosition();
//        }
//
//        private FormationDebuffItemController CreateDebuffItem(GameObject parent, UIScrollView scroll)
//        {
//            var com = AddChild<FormationDebuffItemController, FormationDebuffItem>(parent, FormationDebuffItem.NAME);
//            com.SetScroll(scroll);
//            return com;
//        }
//
//        private void UpdateNegativeItem(int formationId)
//        {
//            var effectDic = FormationData._negativeEffectDict[formationId];
//
//            int index = 0;
//            foreach (var pair in effectDic)
//            {
//                float percent = pair.Key * 100f;
//                for (int i = 0, imax = pair.Value.Count; i < imax; ++i)
//                {
//                    FormationDebuffItemController com = null;
//                    negativeItemList.TryGetValue(index, out com);
//                    if (com == null)
//                    {
//                        com = CreateDebuffItem(View.NegativeGrid.gameObject, View.NegativeScrollView);
//                        negativeItemList.Add(com);
//                    }
//
//                    Formation formation = DataCache.getDtoByCls<Formation>(pair.Value[i]);
//                    string sPercent = percent + "%";
//                    sPercent = sPercent.WrapColor("c30000");
//                    negativeItemList[index].SetName(string.Format("#f{0} {1}", formation.id, formation.name));
//                    negativeItemList[index].SetEffect(sPercent);
//                    negativeItemList[index].gameObject.SetActive(true);
//                    ++index;
//                }
//            }
//
//            for (int i = index; i < negativeItemList.Count; ++i)
//            {
//                negativeItemList[i].gameObject.SetActive(false);
//            }
//            View.NegativeGrid.Reposition();
//            View.NegativeScrollView.ResetPosition();
//        }
//    }
//}
