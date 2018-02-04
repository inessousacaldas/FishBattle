// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  FloatingJoystick.cs
// Author   : willson
// Created  : 2014/12/3 
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using AssetPipeline;
using AppDto;
using UnityEngine;

public sealed partial class JoystickModule
{
    public class JoystickController : MonoBehaviour
    {
        private BoxCollider _bgCollider;
        private NGUIJoystick _joystick;

        private UIEventListener _listener;

        private void Awake()
        {
            var trigger = transform.Find("Trigger");
            _bgCollider = trigger.GetComponent<BoxCollider>();
            _listener = trigger.GetComponent<UIEventListener>();
            _listener.onClick = OnClickBg;        //解决点击一下屏幕出现两个特效的问题 todo xush
            //_listener.onPress = OnPress;

            _joystick = trigger.GetComponent<NGUIJoystick>();
            //_joystick.OnMoveStart = OnStartDrag;
            //_joystick.OnMoveEnd = OnEndDrag;
            //_joystick.OnHolding = OnDragJoystick;
        }


        public void SetCollider(bool enable)
        {
            if (_bgCollider != null)
            {
                _bgCollider.enabled = enable;
            }
        }

        #if UNITY_EDITOR

        #region 绘制玩家点击射线

        private Ray mRay;
        private RaycastHit[] mRayHits;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(mRay.origin, mRay.direction * 1000f);

            if (mRayHits != null)
            {
                foreach (var hit in mRayHits)
                {
                    Gizmos.DrawSphere(hit.point, 0.5f);
                }
            }
        }

        #endregion

        #endif

        #region 摇杆操作

        public bool IsDragging { get; private set; }
        private void OnStartDrag()
        {
            if (JoystickModule.DisableMove) return;
            if (LayerManager.Instance.CurUIMode != UIMode.GAME) return;
            if (GamePlayer.CameraManager.Instance.SceneCamera.IsChangeCameraScaler()) return;

            if (!IsDragging)
            {
                IsDragging = true;
                var heroView = WorldManager.Instance.GetHeroView();
                if (heroView != null)
                {
                    heroView.DragBeginToWalk();
                }
            }
        }

        private void OnEndDrag()
        {
            if (JoystickModule.DisableMove) return;
            if (LayerManager.Instance.CurUIMode != UIMode.GAME) return;
            if (GamePlayer.CameraManager.Instance.SceneCamera.IsChangeCameraScaler()) return;

            if (IsDragging)
            {
                IsDragging = false;
                var heroView = WorldManager.Instance.GetHeroView();
                if (heroView != null)
                {
                    heroView.StopAndIdle();
                }
            }
        }

        void OnDragJoystick()
        {
            if (JoystickModule.DisableMove) return;
            if (LayerManager.Instance.CurUIMode != UIMode.GAME) return;
            if (GamePlayer.CameraManager.Instance.SceneCamera.IsChangeCameraScaler()) return;

            IsDragging = true;
            var heroView = WorldManager.Instance.GetHeroView();
            if (heroView != null)
            {
                heroView.WalkWithJoystick(_joystick.Forward);
            }
        }

        #endregion

        #region 点击操作

        private Coroutine coroutine;
        private void OnPress(GameObject go, bool state)
        {
            GameDebuger.Log(string.Format("Joystick OnPress {0} ,{1}",go.name,state));
            if (state)
            {
                if(coroutine == null)
                    coroutine = StartCoroutine(OnPressIE());
            }
            else
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
            }
        }

        IEnumerator OnPressIE()
        {
            while (true)
            {
                OnClickBg(null);
                yield return new WaitForSeconds(0.5f);
            } 
        }

        private void OnClickBg(GameObject go)
        {
            if (LayerManager.Instance.CurUIMode != UIMode.GAME) return;
            if (GamePlayer.CameraManager.Instance.SceneCamera.IsChangeCameraScaler()) return;

            Ray ray = LayerManager.Root.SceneCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 500f);

            bool hitNpc = false;
            bool hitPlayer = false;
            Transform hitTrans = null;
            Vector3 hitPos = Vector3.zero;

            #if UNITY_EDITOR
            mRay = ray;
            mRayHits = hits;
            #endif

            for (int i = 0, len = hits.Length; i < len; i++)
            {
                var hit = hits[i];
                var col = hit.collider;
                //NPC//
                if (IsNpc(col))
                {
                    hitNpc = true;
                    hitTrans = hit.transform;
                    break;
                }

                if (IsPlayer(col))
                {
                    hitPlayer = true;
                    hitTrans = hit.transform;
                    break;
                }

                if (IsTerrain(col))
                {
                    hitPos = hit.point;
                    JoystickModule.Instance.CanControlByPlayer(true);
                }
               
            }
            if (hitNpc)
            {
                HandleNpcClick(hitTrans.gameObject);
            }
            else if (hitPlayer)
            {
                HandlePlayerClick(hitTrans.gameObject);
            }
            else
            {
                HandleTerrainClick(hitPos, true);
                Instance.UpdateSelectPlayerInfo(null);
                GameDebuger.TODO(@"ModelManager.MissionData.HeroCharacterControllerEnable(true, 0);");
            }
        }

        private bool IsNpc(Collider c)
        {
            if (c == null) return false;
            return c.CompareTag(GameTag.Tag_Npc);
        }

        private bool IsPlayer(Collider c)
        {
            if (c == null) return false;
            return c.CompareTag(GameTag.Tag_Player);
        }

        private bool IsTerrain(Collider c)
        {
            if (c == null) return false;
            return c.CompareTag(GameTag.Tag_Terrain);
        }

        private void HandleNpcClick(GameObject npcGo)
        {
            if (JoystickModule.DisableMove) return;

            GameDebuger.TODO(@"if (ModelManager.BridalSedan.IsMe())
        {
            TipManager.AddTip(BridalSedanModel.CanNotMove);
            return;
        }");

            BaseNpcUnit npcUnit = WorldManager.Instance.GetNpcViewManager().GetNpcUnit(npcGo);
            if (npcUnit == null) return;

            Npc npc = npcUnit.GetNpc();
            if (npc == null) return;
            if (npc is NpcDoubleTeleport) return;

            GameDebuger.TODO(@"ModelManager.MissionData.HeroCharacterControllerEnable(true, 0);");
            JoystickModule.Instance.CanControlByPlayer(true);
            npcUnit.Trigger();
        }

        private void HandlePlayerClick(GameObject playerGo)
        {
            if (playerGo != null)
            {
                GameDebuger.TODO(@"if (ModelManager.BridalSedan.IsMe())
            {
                TipManager.AddTip(BridalSedanModel.CanNotMove);
                return;
            }");

                PlayerView playerView = playerGo.GetComponent<PlayerView>();
                if (playerView == null)
                    return;

                if (playerView.GetPlayerDto().id != ModelManager.Player.GetPlayerId())
                {
                    Instance.UpdateSelectPlayerInfo(playerView.GetPlayerDto());
                    var effpath = PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_CharactorClick);
                    OneShotSceneEffect.BeginFollowEffect(effpath, playerView.transform, 2f, 1f);
                }
                //HandleTerrainClick(playerView.cachedTransform.position, false);
            }
        }

        private void HandleTerrainClick(Vector3 hitPos, bool showEff)
        {
            if (JoystickModule.DisableMove) return;
            GameDebuger.TODO(@"if (ModelManager.BridalSedan.IsMe())
        {
            TipManager.AddTip(BridalSedanModel.CanNotMove);
            return;
        }");
            HeroView heroView = WorldManager.Instance.GetHeroView();

            if (heroView)
            {
                heroView.WalkToPoint(hitPos);
                GameDebuger.TODO(@"ModelManager.MissionData.HeroCharacterControllerEnable(true, 0);");
                JoystickModule.Instance.CanControlByPlayer(true);
                ModelManager.Player.StopAutoNav();
            }

            if (showEff)
            {
                OneShotSceneEffect.Begin(PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_TerrainClick), hitPos, 2f,
                    1, null);
            }
        }
        #endregion
    }
}

