using System;
using UnityEngine;

public class BattleLaunchTimer : MonoBehaviour
{
    public static int MAX_INSTRUCTION_TIME = 30000;
    public static int AUTO_TIME = 27;

    private bool _waitingAutoTime = true;

    public UILabel autoRoundTimeLabel;

    private bool isEnable;

    private float timeCounter;

    public UILabel timeLabel;

    public event Action OnFinishedDelegate;

    public event Action OnAutoTimeFinish;

    //Test
    //[System.NonSerializedAttribute]
    //public BattleController bc;
    //private float syncTimerCounter = 0;
    //-----------------

    private void Awake()
    {
        timeLabel = GetComponentInChildren<UILabel>();

        ResetTimer();
    }

    public void LaunchTimer(int time, int cancelAutoSec, bool autoMode)
    {
        gameObject.SetActive(true);
        autoRoundTimeLabel.gameObject.SetActive(autoMode);
        GameDebuger.TODO(@"AUTO_TIME = MAX_INSTRUCTION_TIME - cancelAutoSec; // - 1;");
        AUTO_TIME = 3;
        _waitingAutoTime = true;
        GameLog.Log_Battle("LaunchTimer  resetTimer ------ " + time);
        ResetTimer(time);
        EnableTimer(true);

        ////--TestCode--
        //if ( bc != null )
        //{
        //	int num = 0;//bc.GetPlayerUnit( true );
        //	int enemyNum = bc.GetMonsterList( BattlePosition.MonsterSide.Enemy ).Count;
        //	int playerNum = bc.GetMonsterList( BattlePosition.MonsterSide.Player ).Count;
        //	num = enemyNum + playerNum;
        //	syncTimerCounter = num * 3 + MAX_INSTRUCTION_TIME;
        //}
        //      //--TestCode--
    }

    public void EnableTimer(bool enable)
    {
        GameLog.Log_Battle("EnableTimer ----"  + enable.ToString());
        isEnable = enable;
    }

    public void StopTimer()
    {
        EnableTimer(false);
        timeCounter = 0;
//		gameObject.SetActive( false );
    }

    private void ResetTimer(int time = 0)
    {
        if (time == 0)
        {
            timeCounter = MAX_INSTRUCTION_TIME;
        }
        else
        {
            timeCounter = time;
        }
        GameLog.Log_Battle("resetTimer ------" + timeCounter);
    }

    public int GetSeconds()
    {
        return (int)Math.Ceiling(timeCounter); //Mathf.FloorToInt( timeCounter );
    }

    private void Update()
    {
        int currentSecond = 0;
        if (isEnable)
        {

            timeCounter -= Time.deltaTime;
            if (timeCounter <= AUTO_TIME && _waitingAutoTime)
            {
                _waitingAutoTime = false;
                if (OnAutoTimeFinish != null)
                    OnAutoTimeFinish();
            }

            if (timeCounter <= 0)
            {
                GameLog.Log_Battle("Update--------"+timeCounter);
                timeCounter = 0;

                if (OnFinishedDelegate != null)
                    OnFinishedDelegate();

                EnableTimer(false);
            }

            currentSecond = GetSeconds();

            timeLabel.text = currentSecond > 0 ? currentSecond.ToString() : string.Empty;// + "/" + Mathf.FloorToInt( syncTimerCounter ).ToString();
        }
        else
            currentSecond = GetSeconds();

        int autoSecond = currentSecond - AUTO_TIME;
        if (autoSecond > 0)
        {
            autoRoundTimeLabel.text = autoSecond.ToString();
        }
        else
        {
            autoRoundTimeLabel.text = string.Empty;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        autoRoundTimeLabel.gameObject.SetActive(false);
    }

    public void HideAutoRoundTimeLabel()
    {
        autoRoundTimeLabel.gameObject.SetActive(false);
    }

    public void DestroyIt()
    {
        OnAutoTimeFinish = null;
        OnFinishedDelegate = null;
    }
}