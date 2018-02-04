using System;


public class QRCodeHelper
{
    public static string EncodeLoginQRCode(string sid, int spVersionCode)
    {
        var code = new LoginQRCode { Sid = sid, SpVersionCode = spVersionCode };

        return JsHelper.ToJson(code);
    }


    public static string EncodeProductQRCode(OrderItemJsonDto itemDto, int quantity, OrderJsonDto orderDto)
    {
        var code = new ProductQRCode { ItemDto = itemDto, Quantity = quantity, OrderDto = orderDto };

        return JsHelper.ToJson(code);
    }


    public static T Decode<T>(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                if (typeof (T) == typeof (LoginQRCode) && !json.Contains("Sid"))
                {
                    return default(T);
                }
                else if (typeof (T) == typeof (ProductQRCode) && (!json.Contains("ProductIdentifier") || !json.Contains("Quantity") || !json.Contains("OrderId")))
                {
                    return default(T);
                }

                var obj = JsHelper.ToObject<T>(json);
                return obj;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        return default(T);
    }
}


public class ProductQRCode
{
    public int Quantity;

	public OrderItemJsonDto ItemDto;
	public OrderJsonDto OrderDto;
}

public class LoginQRCode
{
    public string Sid;
	public int SpVersionCode;
}
