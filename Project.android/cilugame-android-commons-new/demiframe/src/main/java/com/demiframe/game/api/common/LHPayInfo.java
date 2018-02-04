package com.demiframe.game.api.common;

import android.text.TextUtils;

import com.demiframe.game.api.connector.IHandleCallback;

public class LHPayInfo
{
  private String appName;

  private String orderSerial;
  private IHandleCallback payCallback;
  //支付回掉信息
  private String payCustomInfo;
  private String payNotifyUrl;

  private String productCount;
  private String productDes;
  private String productId;
  private String productName;
  private String productPrice;
  private String serverId;
  private String gainGold;
  private String balance;

  private String appId;
  private String sid;
  private String playerId;

  private String extraJson;

  public String getAppName()
  {
    return this.appName;
  }

  public String getOrderSerial()
  {
    return this.orderSerial;
  }

  public IHandleCallback getPayCallback()
  {
    return this.payCallback;
  }

  public String getPayCustomInfo()
  {
    return this.payCustomInfo;
  }

  public String getPayNotifyUrl()
  {
    return this.payNotifyUrl;
  }

  public String getProductCount()
  {
    return this.productCount;
  }

  public String getProductDes()
  {
    return this.productDes;
  }

  public String getProductId()
  {
    return this.productId;
  }

  public String getProductName()
  {
    return this.productName;
  }

  //isCent 是否是分
  //如果需要返回元，则这里不保留小数部分
  public String getProductPrice(boolean isCent)
  {
    if(isCent){
      return this.productPrice;
    }

    return Integer.toString(Integer.parseInt(this.productPrice)/100);
  }

  //获取总价格，单位分
  public int getTotalPriceCent(){
    return Integer.parseInt(getProductCount()) * Integer.parseInt(getProductPrice(true));
  }

  public String getServerId()
  {
    return this.serverId;
  }

  public void setAppName(String paramString)
  {
    this.appName = paramString;
  }

  public void setOrderSerial(String paramString)
  {
    this.orderSerial = paramString;
  }

  public void setPayCallback(IHandleCallback handleCallback)
  {
    this.payCallback = handleCallback;
  }
//支付回调
  public void setPayCustomInfo(String paramString)
  {
    this.payCustomInfo = paramString;
  }

  public void setPayNotifyUrl(String paramString)
  {
    this.payNotifyUrl = paramString;
  }

  public void setProductCount(String paramString)
  {
    this.productCount = paramString;
  }

  public void setProductDes(String paramString)
  {
    this.productDes = paramString;
  }

  public void setProductId(String paramString)
  {
    this.productId = paramString;
  }

  public void setProductName(String paramString)
  {
    this.productName = paramString;
  }

  public void setProductPrice(String paramString)
  {
    this.productPrice = paramString;
  }

  public void setServerId(String paramString)
  {
    this.serverId = paramString;
  }

  public void setAppId(String appId){
    this.appId = appId;
  }

  public String getAppId(){
    return this.appId;
  }

  public void setSid(String sid){
    this.sid = sid;
  }

  public String getSid(){
    return this.sid;
  }

  public void setPlayerId(String playerId){
    this.playerId = playerId;
  }

  public String getPlayerId(){
    return this.playerId;
  }

  //充值成功后可获得的元宝
  public void setGainGold(String gainGold){
    this.gainGold = gainGold;
  }

  public String getGainGold(){
    return this.gainGold;
  }

  public void setBalance(String balance){
    this.balance = balance;
  }

  public String getBalance(){
    return this.balance;
  }

  public void setExtraJson(String extra){
    this.extraJson = extra;
  }

  public String getExtraJson(){
    return this.extraJson;
  }
}