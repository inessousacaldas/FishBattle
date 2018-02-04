using AppDto;
using System.Collections.Generic;

namespace StaticDispose
{
	using System;

	public sealed class StaticDelegateRunner
	{
		public StaticDelegateRunner(Action act)
		{
			GameUtil.SafeRun(act);
		}
	}

	public partial class StaticDispose
	{

		public static void doOnce()
		{
			new StaticDispose();
		}
	}
}

namespace StaticInit
{
	public partial class StaticInit
	{
		public static void doOnce()
		{
			new StaticInit();
		}
	}
}

/// <summary>
/// 数据模型管理器
/// </summary>
public class ModelManager
{
	#region 各种Model

    #region PlayerModule

    private static PlayerModel mPlayer = null;

    public static IPlayerModel IPlayer {
        get { return Player; }
    }
    public static PlayerModel Player { 
        get { 
            if (null == mPlayer) {
                mPlayer = PlayerModel.Create ();
                AddToModelList (mPlayer);
            }
            return mPlayer; 
        } 
    }

	public static void DisposePlayerModel()
	{
		if (mPlayer == null) return;
		IModuleModelList.RemoveItem(mPlayer);
		mPlayer.Dispose();
		mPlayer = null;
	}

	#endregion

    #region BattleDemoConfig

    private static BattleDemoConfigModel mBattleDemoConfig = null;

    public static BattleDemoConfigModel BattleDemoConfig { 
        get { 
            if (null == mBattleDemoConfig) {
                mBattleDemoConfig = new BattleDemoConfigModel ();
                AddToModelList (mBattleDemoConfig);
            }
            return mBattleDemoConfig; 
        } 
    }

    #endregion

    #region BattleConfig

    private static BattleConfigModel mBattleConfig = null;

    public static BattleConfigModel BattleConfig { 
        get { 
            if (null == mBattleConfig) {
                mBattleConfig = new BattleConfigModel ();
                AddToModelList (mBattleConfig);
            }
            return mBattleConfig; 
        } 
    }

    #endregion

    #region
    private static GMModel mGM = null;

    public static GMModel GM
    {
        get
        {
            if (null == mGM)
            {
                mGM = new GMModel();
                AddToModelList(mGM);
            }
            return mGM;
        }
    }

    #endregion

    #endregion

    /// <summary>
    /// 需要获取登录数据的 Model 请在本处手动设置。
    /// </summary>
    /// <param name="pAfterLoginDto">P after login dto.</param>
    public static void Setup (AfterLoginDto pAfterLoginDto)//TODO@MarsZ RefactorLog AfterLoginDto 暂无临时屏蔽
	{
		
	}

	/// <summary>
	/// 必要时统一清理各种 Model ，比如切换角色和帐号等时。
	/// </summary>
	/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="ModelManager"/>. The <see cref="Dispose"/>
	/// method leaves the <see cref="ModelManager"/> in an unusable state. After calling <see cref="Dispose"/>, you must
	/// release all references to the <see cref="ModelManager"/> so the garbage collector can reclaim the memory that the
	/// <see cref="ModelManager"/> was occupying.</remarks>
	public static void Dispose ()
	{
		if (mIModuleModelList.IsNullOrEmpty()) return;
		for (int tCounter = 0; tCounter < mIModuleModelList.Count; tCounter++) {
			try {
				if (mIModuleModelList[tCounter] != null)
				{
					mIModuleModelList [tCounter].Dispose ();
					mIModuleModelList[tCounter] = null;
				}
			} catch (System.Exception ex) {
				GameDebuger.LogException (ex);
			}

		}
	}

	#region Model List

	private static List<IModuleModel> mIModuleModelList = null;

	private static List<IModuleModel> IModuleModelList {
		get {
			if (null == mIModuleModelList)
				mIModuleModelList = new List<IModuleModel> ();
			return mIModuleModelList;
		}
	}

	private static void AddToModelList (IModuleModel pIModuleModel)
	{
		if (null != pIModuleModel) {
			if (IModuleModelList.IndexOf (pIModuleModel) == -1)
				IModuleModelList.Add (pIModuleModel);
		}	
	}

	#endregion
}