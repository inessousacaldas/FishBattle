 using System;
#if ENABLE_SKILLEDITOR
namespace SkillEditor
 {
     public class TabContainerController<C>: ContainerController<C> where C : IViewController, ITabItemController
    {
         private TabController<C> _tabController = new TabController<C>();

         public int CurrentSelectedIndex
         {
             get { return _tabController.CurrentSelectedIndex; }
         }

         public C CurrentSelected
         {
             get { return _tabController.CurrentSelected; }
         }

         public virtual void SetData(Func<C> createFunc, Action<C> disposeAction = null, Action<int, int> selcted = null, bool canTriggerSame = false, bool canBeNone = false,
             Func<int, bool> check = null)
         {
             base.SetData(createFunc, disposeAction);

             _tabController.SetData(selcted, canTriggerSame, canBeNone, check);
         }

         protected override void OnDispose()
         {
             _tabController.Dispose();

             base.OnDispose();
         }

         public void UpdateItemList(int num, Action<int, C> setDataAction)
         {
             base.UpdateItemList(num, setDataAction);

             _tabController.UpdateTabList(_itemList, num);
         }

         public void SetSelected(int index)
         {
             _tabController.SetSelected(index);
         }
     }
 }
#endif