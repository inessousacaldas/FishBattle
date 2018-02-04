using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class EmailDataMgr
{
    public static class EmailNetMsg
    {
        //登录获取邮件
        public static void ReqMailList(Action onSuccess = null, Action onFail = null)
        {
            GameUtil.GeneralReq(Services.Mail_Check(), delegate(GeneralResponse res)
            {
                DataMgr.AcquireEmailDtosSuccess(res);
                FireData();
            }
            , onSuccess
            , error =>
            {
                TipManager.AddTip("获取邮件列表失败".WrapColor(ColorConstantV3.Color_Red));
                GameUtil.SafeRun(onFail);
            }
        );
        }

        //一键领取所有附件
        public static void ReqExtractAll(Action onSuccess =null, Action<ErrorResponse> onFail =null)
        {
            GameUtil.GeneralReq(Services.Mail_ExtractAll(), delegate(GeneralResponse resp)
            {
                TipManager.AddTip("一键领取成功");
            }
            , onSuccess, onFail);
        }

        //删除所有已读邮件
        public static void ReqDelAllRead(Action onSuccess = null,Action<ErrorResponse> onFail = null)
        {
            GameUtil.GeneralReq(Services.Mail_DeleteAllRead(), null, () =>
            {
                if (onSuccess != null) onSuccess();
                DataMgr.RemoveAllReadedMail();
            }
            , onFail);
        }

        //阅读邮件
        public static void ReadMailRequest(long mailId,Action onSuccess = null,Action<ErrorResponse> onFail = null)
        {
            GameUtil.GeneralReq(Services.Mail_Read(mailId), null, () =>
            {
                if (onSuccess != null) onSuccess();
                DataMgr.ReadMail();
            }
            , onFail);
        }

        //领取邮件
        public static void ReqExtract(long mailId,Action onSuccess = null,Action<ErrorResponse> onFail = null)
        {
            GameUtil.GeneralReq(Services.Mail_Extract(mailId), null, () =>
            {
                if (onSuccess != null) onSuccess();
                DataMgr.GetAttachmentSuccess();
            }
            , onFail);
        }

        //删除邮件
        public static void ReqDelMail(long mailId, Action onSuccess = null, Action<ErrorResponse> onFail = null)
        {
            GameUtil.GeneralReq(Services.Mail_Delete(mailId), null, () =>
            {
                if (onSuccess != null) onSuccess();
                DataMgr.RemoveCurrentMail();
            }
            , onFail);
        }

    }
}
