using System.Collections.Generic;
using AppDto;


public interface IEmailData
{
    IEnumerable<PlayerMailDto> MailDtoList { get; }

    PlayerMailDto CurMailDto { get; set; }
    int CurMailIdx { get; set; }
    bool IsOutTime { get; }
    IEnumerable<long> DeleteMailIds { get;}
    bool IsDeleteAll { get; set; }
    bool IsSort { get; set; }
}

public sealed partial class EmailDataMgr
{
    public sealed partial class EmailData:IEmailData
    {
        public EmailData()
        {

        }

        public void InitData()
        {
        }

        public static int ATTACHMENT_MAXCOUNT = 3;    //可携带附件的最大值

        public List<PlayerMailDto> _mailList = new List<PlayerMailDto>();    //用于客户端缓存数据

        public IEnumerable<PlayerMailDto> MailDtoList     //客户端缓存
        {
            get
            {
                return _mailList;
            }
        }
        
        public bool IsSort { get; set; }

        private PlayerMailDto curMailDto = null;
        public PlayerMailDto CurMailDto
        {
            get
            {
                if (curMailDto == null)
                    curMailDto = _mailList.TryGetValue(0);
                return curMailDto;
            }

            set
            {
                curMailDto = value;
            }
        }

        private int curMailIdx = -1;
        public int CurMailIdx
        {
            get { return curMailIdx; }
            set { curMailIdx = value; }
        }

        public List<long> deleteMailIds = new List<long>();

        public bool isOutTime;
        public bool IsOutTime {
            get
            {
                return isOutTime;
            }
        }

        public IEnumerable<long> DeleteMailIds
        {
            get
            {
                return deleteMailIds;
            }
        }

        public bool isDeleteAll = false;
        public bool IsDeleteAll
        {
            get { return isDeleteAll; }
            set { isDeleteAll = value; }
        }

        public void Dispose()
        {

        }
    }
}
