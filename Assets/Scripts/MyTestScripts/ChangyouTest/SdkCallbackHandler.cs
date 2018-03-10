using UnityEngine;
using System.Collections;

using CySdk;
using LITJson;

public class SdkCallbackHandler : MonoBehaviour {

	public static SdkCallbackHandler Instance;

	void Awake ()
	{
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
	}

	public void onResult(string jsonParam)
	{
		if (GameHandler.Instance != null)
			GameHandler.Instance.writeLog (jsonParam);
		
		JsonData jsonData = JsonMapper.ToObject (jsonParam);
		int state_code = (int)jsonData ["state_code"];
		string message = (string)jsonData ["message"];
		switch (state_code) {
			case ResultCode.INIT_SUCCESS:
				PlayerPrefs.SetString ("initresult", jsonParam);
				UnityEngine.SceneManagement.SceneManager.LoadScene("sdk");
				break;
			case ResultCode.LOGIN_SUCCESS:
            case ResultCode.LOGIN_CANCEL:
			case ResultCode.LOGIN_FAILED:
			case ResultCode.SWITCH_USER_SUCCESS:
                GameHandler.Instance.showLoginView();
                GameHandler.Instance.verifyToken (jsonParam);
				break;
            case ResultCode.SWITCH_USER_FAILED:
                SPSDK.showToast("切换账号失败");
                Debug.Log(jsonParam);
                break;
            case ResultCode.LOGOUT:
				GameHandler.Instance.doLogout (jsonParam);
				break;
			case ResultCode.HOST_SUCCESS:
			case ResultCode.HOST_FAILED:
				GameHandler.Instance.doGotHost (jsonParam);
				break;
			case ResultCode.GOODS_SUCCESS:
			case ResultCode.GOODS_FAILED:
				GameHandler.Instance.showGoods (jsonParam);
				break;
			case ResultCode.PAY_SUCCESS:
			case ResultCode.PAY_FAILED:
			case ResultCode.PAY_CANCEL:
				GameHandler.Instance.doPayment (jsonParam);
				break;
			case ResultCode.EXIT_GAME:
			case ResultCode.EXIT_GAME_DIALOG:
				GameHandler.Instance.doExit (jsonParam);
				break;
			case ResultCode.AUTH_SUCCESS:
				GameHandler.Instance.doAuth (jsonParam);
				break;
            case ResultCode.PLUGIN_RESULT:
                GameHandler.Instance.doPlugin(jsonParam);
                break;
            default:
				Debug.LogError (message);
				break;
		}
	}
}
