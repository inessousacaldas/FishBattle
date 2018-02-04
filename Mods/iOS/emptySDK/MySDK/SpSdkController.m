//
//  SpSdkController.m
//  Unity-iPhone
//
//

#import "SpSdkController.h"

IMPL_APP_CONTROLLER_SUBCLASS (SpSdkController)


@implementation SpSdkController
{
}

//通用代码开始......

//游戏初始化完成自动调用
- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
     NSLog(@"SpSdkController didFinishLaunchingWithOptions");
    //这里可增加一些SDK要求加入到这个过程的处理

    return YES;
}

//发送消息给Unity type 类型   code 代号   data 内容
- (void)sendMessageToUnity:(char *)type code:(int)code data:(NSString *)data
{
    NSString *message=[NSString stringWithFormat:@"{\"type\":\"%s\",\"code\":%d,\"data\":\"%@\"}",type,code,data];
    
    NSLog(@"sendMessageToUnity %@",message);
    
    UnitySendMessage("_SdkMessageHandler", "OnSdkCallback", [message cStringUsingEncoding:NSUTF8StringEncoding]);
}

//通用代码结束......

//初始化
-(void)initsdk
{
    NSLog(@"初始化 init");
    //一般要在这里增加回调监听
    
    [self sendMessageToUnity:"init" code:0 data:@""];
}

//注销
- (void)logout
{
    NSLog(@"注销 logout");
}

//绑定
- (void)bindAccount
{
    NSLog(@"绑定 bindAccount");
}


//登陆
- (void)login
{
    NSLog(@"登陆 login");
    
}

//帐号管理
- (void)userCenter
{
    NSLog(@"帐号管理 userCenter");
}


/**
 *支付
 * orderSerial 订单号
 * pid 商品ID
 * name 商品名称，如XX元宝
 * price 商品价格RMB，充值金额
 * count 商品数量，一般为1
 * serverId 充值服务器ID
 */
- (void) pay:(NSString *) orderSerial
													productId:(NSString *) pid
													productName:(NSString *) name
													productPrice:(NSString *) price
													productCount:(NSString *) count
													appServerId:(NSString *) serverId
{
    NSLog(@"支付 amount=%@ serverId=%@ orderSerial=%@", price, serverId, orderSerial);

    
}


#if !__has_feature(objc_arc)
-(void)dealloc
{
    [super dealloc];
}
#endif

//各种回调结束......



//必须添加代码开始....

- (UIInterfaceOrientationMask)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window
{
    return UIInterfaceOrientationMaskAll;
}

//必须添加代码结尾....

@end
