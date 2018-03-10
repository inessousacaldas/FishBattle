using AppDto;
using System.Collections.Generic;

//任务日志数据管理类
public class MissionRecordClient  {
    
}

public partial interface IMissionData
{
    Dictionary<string, List<MissionRecord>> RecordDic { get; }
    IEnumerable<int> RecordsList { get; }
}
public sealed partial class MissionDataMgr
{
    public sealed partial class MissionData
    {
        //地区管理事件
        private Dictionary<string, List<MissionRecord>> recordDic = new Dictionary<string, List<MissionRecord>>();
        private void InitRecorList()
        {
            List<MissionRecord> recordList = DataCache.getArrayByCls<MissionRecord>();
            recordList.ForEach(_ =>
            {
                string id = _.area;
                if (!recordDic.ContainsKey(id))
                {
                    recordDic.Add(id, new List<MissionRecord>());
                }
                recordDic[id].Add(_);
            });
        }

        public Dictionary<string, List<MissionRecord>> RecordDic
        {
            get { return recordDic; }
        }

        public void UpdateRecordList(int evtID)
        {
            _playerMissionListDto.records.Add(evtID);
        }
        public IEnumerable<int> RecordsList { get { return _playerMissionListDto.records; } }
    }
}
