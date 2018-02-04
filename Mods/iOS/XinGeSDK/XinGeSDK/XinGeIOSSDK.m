

#import "XGPush.h"
#import "XGSetting.h"

#import "XinGeIOSSDK.h"


#define _IPHONE80_ 80000


@implementation XinGeIOSSDK

static NSData *_deviceToken;


//发送消息给Unity type 类型   code 代号   data 内容
+ (void) sendMessageToUnity:(char *)type code:(int)code data:(NSString *)data
{
    NSString *message=[NSString stringWithFormat:@"{\"type\":\"%s\",\"code\":%d,\"data\":\"%@\"}",type,code,data];
    
    NSLog(@"sendMessageToUnity %@",message);
    
    UnitySendMessage("_SdkMessageHandler", "OnSdkCallback", [message cStringUsingEncoding:NSUTF8StringEncoding]);
}


+ (void) setup:(NSString *)appId AppKey:(NSString *)appKey
{
    NSLog(@"[XGPush Demo]setup:%@ %@", appId, appKey);

    uint32_t idValue = [appId longLongValue];
    [XGPush startApp:idValue appKey:appKey];
}


+ (void) enableDebug:(bool)enable
{
    [[XGSetting getInstance] enableDebug:enable];
}


+ (void) registerStart
{
    float sysVer = [[[UIDevice currentDevice] systemVersion] floatValue];
    if(sysVer < 8){
        [XinGeIOSSDK registerPush];
    }
    else{
        [XinGeIOSSDK registerPushForIOS8];
    }
}


+ (void) registerPushForIOS8
{
    //Types
    UIUserNotificationType types = UIUserNotificationTypeBadge | UIUserNotificationTypeSound | UIUserNotificationTypeAlert;
    
    //Actions
    UIMutableUserNotificationAction *acceptAction = [[UIMutableUserNotificationAction alloc] init];
    
    acceptAction.identifier = @"ACCEPT_IDENTIFIER";
    acceptAction.title = @"Accept";
    
    acceptAction.activationMode = UIUserNotificationActivationModeForeground;
    acceptAction.destructive = NO;
    acceptAction.authenticationRequired = NO;
    
    //Categories
    UIMutableUserNotificationCategory *inviteCategory = [[UIMutableUserNotificationCategory alloc] init];
    
    inviteCategory.identifier = @"INVITE_CATEGORY";
    
    [inviteCategory setActions:@[acceptAction] forContext:UIUserNotificationActionContextDefault];
    
    [inviteCategory setActions:@[acceptAction] forContext:UIUserNotificationActionContextMinimal];
    
    [acceptAction release];
    
    NSSet *categories = [NSSet setWithObjects:inviteCategory, nil];
    
    [inviteCategory release];
    
    
    UIUserNotificationSettings *mySettings = [UIUserNotificationSettings settingsForTypes:types categories:categories];
    
    [[UIApplication sharedApplication] registerUserNotificationSettings:mySettings];
    
    
    [[UIApplication sharedApplication] registerForRemoteNotifications];
}

+ (void) registerPush
{
    [[UIApplication sharedApplication] registerForRemoteNotificationTypes:(UIRemoteNotificationTypeAlert | UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeSound)];
}

+ (void) registerWithDeviceToken:(NSData *)deviceToken
{
    [_deviceToken release];
    _deviceToken = deviceToken;
    [_deviceToken retain];
    
    void (^successBlock)(void) = ^(void){
        //成功之后的处理
        NSLog(@"[XGPush Demo]register successBlock");
        
        [XinGeIOSSDK sendMessageToUnity:"XGRegisterResult" code:0 data:@"0"];
    };
    
    void (^errorBlock)(void) = ^(void){
        //失败之后的处理
        NSLog(@"[XGPush Demo]register errorBlock");
        
        [XinGeIOSSDK sendMessageToUnity:"XGRegisterResult" code:1 data:@"1"];
    };
    //    [XGPush setAccount:@"393830887"];
    //注册设备
    NSString *token = [XGPush registerDevice:_deviceToken successCallback:successBlock errorCallback:errorBlock];
    NSLog(@"%@", token);
}

+ (void) registerWithAccount:(NSString *)account
{
    NSLog(@"[XGPush Demo]registerWithAccount:%@", account);

    void (^successBlock)(void) = ^(void){
        //成功之后的处理
        NSLog(@"[XGPush Demo]register successBlock");
        
        [XinGeIOSSDK sendMessageToUnity:"XGRegisterWithAccountResult" code:0 data:@"0"];
    };
    
    void (^errorBlock)(void) = ^(void){
        //失败之后的处理
        NSLog(@"[XGPush Demo]register errorBlock");
        
        [XinGeIOSSDK sendMessageToUnity:"XGRegisterWithAccountResult" code:1 data:@"1"];
    };
    
    // 设置账号
    //    [XGPush setAccount:@"393830887"];
    //    [account retain];
    [XGPush setAccount:account];
    
    //注册设备
    NSString *token = [XGPush registerDevice:_deviceToken successCallback:successBlock errorCallback:errorBlock];
    NSLog(@"%@", token);
}

+ (void) setTag:(NSString *)tagName
{
    NSLog(@"[XGPush Demo]setTag:%@", tagName);

    [XGPush setTag:tagName];
}

+ (void) deleteTag:(NSString *)tagName
{
    NSLog(@"[XGPush Demo]deleteTag:%@", tagName);
    
    [XGPush delTag:tagName];
}

@end



void __xinGeSetup(const char *appId, const char *appKey)
{
    [XinGeIOSSDK setup:[NSString stringWithUTF8String:appId] AppKey:[NSString stringWithUTF8String:appKey]];
}


void __xinGeEnableDebug(bool enable)
{
    [XinGeIOSSDK enableDebug:enable];
}


void __xinGeRegister()
{
    [XinGeIOSSDK registerStart];
}


void __xinGeRegisterWithAccount(const char *account)
{
    [XinGeIOSSDK registerWithAccount:[NSString stringWithUTF8String:account]];
}


void __xinGeSetTag(const char *tagName)
{
    [XinGeIOSSDK setTag:[NSString stringWithUTF8String:tagName]];
}

void __xinGeDeleteTag(const char *tagName)
{
    [XinGeIOSSDK deleteTag:[NSString stringWithUTF8String:tagName]];
}