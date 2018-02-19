using System.Collections.Generic;

namespace AppDto
{
    public partial class VideoSkillAction
    {
	  private List<Tuple<long, VideoTargetStateGroup>> _victimStateGroup;

	  public int GetVictimStateGroupCount()
	  {
		  CheckVictimStateGroups();
		  return _victimStateGroup.Count;
	  }
	  
	  public IEnumerable<Tuple<long, VideoTargetStateGroup>> GetVictimStateGroups()
	  {
		  CheckVictimStateGroups();
		  return _victimStateGroup;
	  }
	  
	  public IEnumerable<long> GetVictims()
	  {
		  CheckVictimStateGroups();
		  foreach (var tuple in _victimStateGroup)
		  {
			  yield return tuple.p1;
		  }
	  }

	  public long GetVictim(int index)
	  {
		  CheckVictimStateGroups();
		  if (0 <= index && index < _victimStateGroup.Count)
		  {
			  return _victimStateGroup[index].p1;
		  }

		  return 0;
	  }

	  private void CheckVictimStateGroups()
	  {
		  if (_victimStateGroup == null)
		  {
			  _victimStateGroup = new List<Tuple<long, VideoTargetStateGroup>>();
			  var callSoldierIds = new List<long>();
			  for (int i = 0, len = targetStateGroups.Count; i < len; i++)
			  {
				  var group = targetStateGroups[i];
				  var injureId = GetInjureId(group, callSoldierIds);
				  if (injureId > 0)
				  {
					  _victimStateGroup.Add(Tuple.Create(injureId,group));;
				  }
			  }
		  }
	  }

	  private static long GetInjureId(VideoTargetStateGroup group, List<long> callSoldierIds)
	  {
		  for (int i = 0, len = group.targetStates.Count; i < len; i++)
		  {
			  var state = group.targetStates[i];
			  if (state is VideoBuffAddTargetState)
			  {
				  var videoBuffAddTargetState = state as VideoBuffAddTargetState;
				  if (callSoldierIds.Contains(videoBuffAddTargetState.id))
				  {
					  continue;
				  }

				  //当只有buff添加state的时候， 才考虑加入受击者，如果不是， 就不需要加入受击者
				  if (group.targetStates.Count > 1)
				  {
					  continue;
				  }
			  }
			  if (state is VideoRetreatState)
			  {
				  continue;
			  }
			  if (state.id > 0)
			  {
				  return state.id;
			  }
		  }

		  return 0;
	  }

	  private List<VideoTargetStateGroup> _attackerStateGroup;

	  private void CheckAttackStateGroup()
	  {
		  if (_attackerStateGroup == null)
		  {
			  _attackerStateGroup=new List<VideoTargetStateGroup>();
			  for (int i = 0, len = targetStateGroups.Count; i < len; i++)
			  {
				  var group = targetStateGroups[i];
				  foreach (var state in @group.targetStates)
				  {
					  if (state.id == actionSoldierId)
					  {
						  _attackerStateGroup.Add(group);
						  break;
					  }
				  }
			  }
		  }
	  }

	  public int GetAttackerStateGroupCount()
	  {
		  CheckAttackStateGroup();
		  return _attackerStateGroup.Count;
	  }

	  public VideoTargetStateGroup GetAttackerStateGroup(int index)
	  {
		  CheckAttackStateGroup();
		  if (0 <= index && index < _attackerStateGroup.Count)
		  {
			  return _attackerStateGroup[index];
		  }

		  return null;
	  }
	  
	  public IEnumerable<VideoTargetStateGroup> GetAttackerStateGroups()
	  {
		  CheckAttackStateGroup();
		  return _attackerStateGroup;
	  }

    }
}