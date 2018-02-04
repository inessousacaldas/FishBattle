using System.Collections.Generic;

public class LoginAccountDto
{
	public int code;
	public string msg;
	public string token;
	public string uid;
    public bool firstRegister;
    public long accountId;
    public LITJson.JsonData sdkResData;
    public List<AccountPlayerDto> players = new List<AccountPlayerDto>();

}