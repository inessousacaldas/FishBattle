using System.Collections.Generic;

namespace SdkAccountDto
{
    public class LoginSessionDto
    {
        //充值相关
        public string name;
        public string currencyName;
        public float currencyRate;

        public string sid;
        public string uid;
        public string nickname;
        public string accountId;
        public string accountName;
        public string accountSession;
        public bool accountBound;
    }


    public class LoginResponseDto: ResponseDto
    {
        public LoginSessionDto item;
    }

    //客户端使用
    public class AccountDto
    {
        //0自由；1手机；2邮箱；3设备；4QQ登录；5微信登录
        public enum AccountType
        {
            free,
            phone,//自平台（手机）
            mail,
            device,//设备号
            qq,
            weixin,
        }
        private string _name;

        public string name {
            get
            {
                if (!string.IsNullOrEmpty(_name)) return _name;

                return seesionDto.accountName;
            }
            set { _name = value;}
        }

        //password(md5)
        public string password;

        //AccountType
        public AccountType type;
    
        public LoginSessionDto seesionDto;

        public AccountDto() { }

        public AccountDto(LoginResponseDto dto)
        {
            this.seesionDto = dto.item;
        }
    }
}
