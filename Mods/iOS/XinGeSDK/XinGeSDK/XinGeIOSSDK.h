

@interface XinGeIOSSDK: NSObject

+ (void) setup:(NSString *)appId AppKey:(NSString *)appKey;
+ (void) enableDebug:(bool)enable;
+ (void) registerStart;
+ (void) registerWithDeviceToken:(NSData *)deviceToken;
+ (void) registerWithAccount:(NSString *)account;
+ (void) setTag:(NSString *)tagName;
+ (void) deleteTag:(NSString *)tagName;

@end