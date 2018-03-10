using System;
using AppDto;

public class MailNotifyListener : BaseDtoListener<PlayerMailDto>
{
    protected override void HandleNotify(PlayerMailDto dto)
    {
        EmailDataMgr.DataMgr.AddMailDto(dto);
    }
}
