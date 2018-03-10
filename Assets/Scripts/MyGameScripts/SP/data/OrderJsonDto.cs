using System.Collections.Generic;

public class OrderJsonDto
{
	public int code;
	public string msg;
	public string orderId;
	public OrderExtraJsonDto extra;
}

public class OrderExtraJsonDto
{
	public int code;
	public string msg;
	public string tsiPayCburl;
	public string p;//平台ID
	public string vivoAccessKey;//vivoSDK 需要的参数
	public string vivoOrderAmount;//交易金额分
	public string vivoOrderNumber;//交易流水号
}

public class OrderItemsJsonDto
{
	public List<OrderItemJsonDto> items = new List<OrderItemJsonDto>();
}

public class OrderItemJsonDto
{
	public string id;
	public int cent;
	public int gold;
	public int goldOfAddition;
	public int gameShopItemId;
}