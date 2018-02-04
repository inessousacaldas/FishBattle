//
//  SpSdkController.h
//  Unity-iPhone
//
//

#import <UIKit/UIKit.h>
#import "UnityAppController.h"

@interface SpSdkController : UnityAppController<UIApplicationDelegate>
- (void)initsdk;
- (void)login;
- (void)bindAccount;
- (void)logout;
- (void)userCenter;
//支付
- (void) pay:(NSString *) orderSerial
   productId:(NSString *) pid
 productName:(NSString *) name
productPrice:(NSString *) price
productCount:(NSString *) count
 appServerId:(NSString *) serverId;
@end
