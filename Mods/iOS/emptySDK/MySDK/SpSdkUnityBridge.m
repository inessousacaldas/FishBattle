//
//  SpSdkUnityBridge.m
//  Unity-iPhone
//
//  Created by senkay on 12/14/15.
//
//

#import "SpSdkUnityBridge.h"

#if defined (__cplusplus)
extern "C" {
#endif

    /**
     *	@brief	初始化
     */
    void __spsdk_init ()
    {
    }
    
    char* __spsdk_getChannelId()
    {
        //这里要返回渠道定义的ID
        const char* string = "-2";
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
    /**
     *	@brief	账号登录
     */
    void __spsdk_login ()
    {
    }

    /**
     *  @brief  提交角色数据
     *  uid 用户唯一id
     *  newRole 是否新角色
     *  playerId 角色id
     *  playerName 角色昵称
     *  playerLv 角色等级
     *  serverId 服务器id
     *  serverName 服务器名字
     */
    extern void __spsdk_submitRoleData(const char* uid,
                                       const bool newRole,
                                       const char* playerId,
                                       const char* playerName,
                                       const char* playerLv,
                                       const char* serverId,
                                       const char* serverName
                                       )
    {
    }
    
    /**
     *	@brief	账号绑定
     */
    void __spsdk_bind ()
    {
    }
    
    /**
     *	@brief	账号登出
     */
    void __spsdk_logout ()
    {
    }
    
    /**
     *	@brief	是否支持用户中心
     */
    bool __spsdk_isSupportUserCenter ()
    {
        return false;
    }
    
    /**
     *	@brief	进入用户中心
     */
    void __spsdk_enterUserCenter (){
    }
    
    /**
     *	@brief	是否支持论坛
     */
    bool __spsdk_isSupportBBS (){
        return false;
    }
    
    /**
     *	@brief	进入论坛
     */
    void __spsdk_enterSdkBBS ()
    {
        
    }
    
    /**
     *	@brief	支付
     * orderSerial 订单号
     * productId 商品ID
     * productName 商品名称，如XX元宝
     * productPrice 商品价格RMB，充值金额
     * productCount 商品数量，一般为1
     * serverId 充值服务器ID
     */
    void __spsdk_pay (const char* orderSerial,
                      const char* productId,
                      const char* productName,
                      const char* productPrice,
                      const char* productCount,
                      const char* serverId
                      )
    {
    }
    
#if defined (__cplusplus)
}
#endif
@implementation SpSdkUnityBridge

@end