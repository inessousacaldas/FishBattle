// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MoneyInfoViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IMoneyInfoViewController : ICloseView
{

}

public partial class MoneyInfoViewController:FRPBaseController<
    MoneyInfoViewController
    , MoneyInfoView
    , IMoneyInfoViewController
    , IPlayerModel>
    , IMoneyInfoViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {

        }
        
        protected override void RemoveEvent()
        {

        }
    }
