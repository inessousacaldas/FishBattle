using System;
using System.Collections.Generic;

namespace SceneTrigger
{
    public class SceneTriggerMgr
    {
        private string sceneName;
        private List<TriggerItem> triggerItems = new List<TriggerItem>();
        public void Init(string triggerConfigJson, string sceneName)
        {
            this.sceneName = sceneName;
            if (string.IsNullOrEmpty(triggerConfigJson))
            {
                GameDebuger.LogError("SceneTriggerMgr.Init : triggerConfigJson is NullOrEmpty");
                return;
            }
            CreateTriggerItem(triggerConfigJson);
            InitTriggerItem();
            JSTimer.Instance.SetupTimer("SceneTriggerMgr:"+ sceneName, TriggerUpdate, 0.2f);
        }


        private void CreateTriggerItem(string triggerConfigJson)
        {

            List<TriggerConfigItem> config = JsHelper.ToCollection<List<TriggerConfigItem>, TriggerConfigItem>(triggerConfigJson);

            for (int i = 0; i < config.Count; i++)
            {
                TriggerConfigItem item = config[i];
                TriggerItem triggerItem = new TriggerItem();

                triggerItem.condition = SceneTriggerHelper.ConditionFactory(item.conditionType, item.conditionParamJson);
                triggerItem.trigger = SceneTriggerHelper.TriggerFactory(item.triggerType, item.triggerParamJson);

                triggerItems.Add(triggerItem);
            }
        }


        private void InitTriggerItem()
        {
            for (int i = 0; i < triggerItems.Count; i++)
            {
                try
                {
                    TriggerItem triggerItem = triggerItems[i];
                    triggerItem.condition.Init();
                    triggerItem.trigger.Init();
                }
                catch (Exception ex)
                {
                    GameDebuger.LogException(ex);
                }
            }
        }

        private void TriggerUpdate()
        {
            for (int i = 0; i < triggerItems.Count; i++)
            {
                try
                {
                    TriggerItem triggerItem = triggerItems[i];
                    TriggerBase trigger = triggerItem.trigger;
                    ConditionBase condition = triggerItem.condition;

                    if (!condition.initFinish || !trigger.initFinish || trigger.IsFinish())
                        continue;
                    if (condition.Update())
                    {
                        if (!trigger.active)
                        {
                            trigger.active = true;
                            trigger.OnActive();
                        }
                        trigger.Update();
                    }
                    else if(trigger.active)
                    {
                        trigger.active = false;
                        trigger.OnDeactive();
                    }
                }
                catch (Exception ex)
                {
                    GameDebuger.LogException(ex);
                }
            }
        }

        public void Dispose()
        {
            JSTimer.Instance.CancelTimer("SceneTriggerMgr:"+ sceneName);
            for (int i = 0; i < triggerItems.Count; i++)
            {
                TriggerItem triggerItem = triggerItems[i];

                try
                {
                    triggerItem.condition.Dispose();
                }
                catch (Exception ex)
                {
                    GameDebuger.LogException(ex);
                }
                try
                {
                    triggerItem.trigger.Dispose();
                }
                catch (Exception ex)
                {
                    GameDebuger.LogException(ex);
                }
            }
        }
        #region Helper


        private class TriggerItem
        {
            public ConditionBase condition;
            public TriggerBase trigger;
        }
        #endregion

    }
}
