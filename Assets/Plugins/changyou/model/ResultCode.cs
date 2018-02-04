namespace CySdk{

	public class ResultCode{
		public const int PAY_SUCCESS = 100;//支付成功
		public const int PAY_FAILED = 101;//支付失败
		public const int PAY_CANCEL = 102;//支付取消
		public const int LOGOUT = 200;//注销
		public const int LOGIN_SUCCESS = 300;//登录成功
		public const int LOGIN_FAILED = 301;//登录失败
		public const int LOGIN_CANCEL = 302;//登录取消
		public const int INIT_SUCCESS = 400;//初始化成功
		public const int INIT_FAILED = 401;//初始化失败
		public const int EXIT_GAME = 500;//确认退出游戏
		public const int EXIT_GAME_DIALOG = 501;//退出游戏，渠道无对话框
		public const int SWITCH_USER_SUCCESS = 600;//切换用户成功
		public const int SWITCH_USER_FAILED = 601;//切换用户失败
		public const int PAY_HISTORY_SUCCESS = 700;//获取充值记录成功
		public const int PAY_HISTORY_FAILED = 701;//获取充值记录失败
		public const int GOODS_SUCCESS = 800;//获取商品列表成功
		public const int GOODS_FAILED = 801;//获取商品列表失败
		public const int HOST_SUCCESS = 1101;//获取网关成功
		public const int HOST_FAILED = 1102;//获取网关失败
		public const int AUTH_SUCCESS = 1103;//实名认证请求
        public const int PLUGIN_RESULT = 1107;//插件请求接口结果返回

		public const int MODE_RELEASE = 0;//SDK连接的正式biliing
		public const int MODE_DEBUG = 1;//测试
		public const int MODE_PRERELEAS = 2;//预发布

        //PC端专用新增代码
        public const int PAY_GET_QR_SUCCESS = 103;//第一步，返回等待扫描的二维码，研发展示二维码
        public const int PAY_SCANNED_OVER = 104;//第二步，返回已经扫过的二维码，研发更换二维码
        public const int PAY_CONFIRMED = 105;//第三步，已经确认，返回确认的二维码，研发更换二维码，关闭二维码，等待从游戏服更新元宝
        public const int PAY_SCAN_EXPIRED = 106;//扫描过期，研发需要重新调用pay接口
        public const int PAY_QR_ERROR = 107;//二维码错误

        public const int LOGIN_GET_QR_SUCCESS = 303;//第一步，返回等待扫描的二维码，研发展示二维码
        public const int LOGIN_SCANNED_OVER = 304;//第二步，返回已经扫过的二维码，研发更换二维码
        public const int LOGIN_CONFIRMED = 305;//第三步，已经确认，返回确认的二维码，研发更换二维码，关闭二维码，进入token验证流程
        public const int LOGIN_SCAN_EXPIRED = 306;//扫描过期，研发需要重新调用login接口
        public const int LOGIN_QR_ERROR = 307;//二维码错误

    }

	public class ApiCode{
		public const int LOGOUT = 0;
		public const int USER_CENTER = 2;
		public const int SWITCH_USER = 3;
		public const int SERVICE_CENTER = 5;
	}

    public class PluginCode {
        public const string BIND_SUCESS = "BIND_SUCESS";
        public const string CLOSE = "CLOSE";
        public const string COMPLAINT_SUCESS = "COMPLAINT_SUCESS";
        public const string FEEDBACK_SUCESS = "FEEDBACK_SUCESS";
        public const string TIMEOUT = "TIMEOUT";
        public const string UNBIND_SUCESS = "UNBIND_SUCESS";
        public const string GET_PLAYER_INFO = "GET_PLAYER_INFO";
        public const string RECEIVE_MESSAGES = "RECEIVE_MESSAGES";
        public const string SHOW_ICON = "SHOW_ICON";
    }

}
