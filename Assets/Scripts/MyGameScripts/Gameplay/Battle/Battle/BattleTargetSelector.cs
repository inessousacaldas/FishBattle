using System;
using System.Collections.Generic;
using AppDto;


public class BattleTargetSelector
{
	private class SkillConfigInfo
	{
		public TargetType  targetType; //目标类型
		public CharacterType  characterType; //角色类型
		public SelfType selfType; //是否包括自己

		public SkillConfigInfo (
			TargetType  _targetType
				, CharacterType  _characterType 
				, SelfType _selfType)
		{
			targetType = _targetType;
			characterType = _characterType;
			selfType = _selfType;
		}
	}
	
	public enum TargetType
	{
		ALL = 0,	 //0.全体
		PLAYER = 1,	 //1.我军
		ENEMY = 2,   //2.敌人 
		SELF = 3, //仅自己
		NONE,
	}
	
	public enum SelfType
	{
		Invalid,
		SELF, //包括自己
		NOTSELF, //不包括自己
	}	

	public enum CharacterType
	{
		Invalid,
		ALL = 0, //全部
		PET = 1, //宠物
		NOTPET = 2, //非宠物
		HERO = 3, //仅主角
		COUPLE = 4, //仅配偶
		PLAYER = 5, //仅玩家，不含宠物和伙伴
	}	

	public enum LifeState
	{
		ALL = 0,	 //0.全部
		ALIVE = 1,	 //1.存活
		DEAD = 2,   //2.死亡
	}

	public enum SelectorType
	{
		CAPTURE = 0,
		SKILL = 1,
		ITEM = 2,
	}
#pragma warning disable 0649
    private bool 		singleTarget; //是否单体
#pragma warning restore

	private LifeState 	lifeState; //0全部  1存活  2死亡
	private SelectorType selectorType; //选择类型

	private int _selectParam; //参数
    //技能附加参数
    public object SkillParam;

	private Skill normalSkill;
	private Skill skill;

	public Skill MagicOrCraftSkill {
		get { return skill; }
		set { skill = value; }
	}

	private MonsterController monsterSource;
	private long monsterTargets;

    private readonly Dictionary<Skill.UserTargetScopeType, SkillConfigInfo> SkillConfigDic = new Dictionary<Skill.UserTargetScopeType, SkillConfigInfo>(
	    )
    {
	    //		1、敌方全体；
	    {
		    Skill.UserTargetScopeType.Enemy, new SkillConfigInfo(
		    TargetType.ENEMY
		    , CharacterType.ALL
		    , SelfType.NOTSELF)
	    }
	    //		2、仅自身；
	    , {
		    Skill.UserTargetScopeType.Self, new SkillConfigInfo(
			    TargetType.SELF
			    , CharacterType.ALL
			    , SelfType.SELF)
	    }
	    //		3、己方除自身外的单位，包括己方全部人的宠物
	    , {
		    Skill.UserTargetScopeType.FriendsExceptSelfWithPet, new SkillConfigInfo(
			    TargetType.PLAYER
			    , CharacterType.ALL
			    , SelfType.NOTSELF)
	    }
	    //		4、己方所有单位，包括己方全部人的宠物
	    , {
		    Skill.UserTargetScopeType.FriendsWithPet, new SkillConfigInfo(
			    TargetType.PLAYER
			    , CharacterType.ALL
			    , SelfType.SELF)
	    }
	    //		5、仅己方全部宠物
	    , {
	    Skill.UserTargetScopeType.FriendPets, new SkillConfigInfo(
		    TargetType.PLAYER
		    , CharacterType.PET
		    , SelfType.NOTSELF)
    	}
	    //		6、场上除自身外所有单位
	    , {
		    Skill.UserTargetScopeType.ExceptSelf, new SkillConfigInfo(
			    TargetType.ALL
			    , CharacterType.ALL
			    , SelfType.NOTSELF)
	    }
	    //		7、身上宠物  无用
	    , {
		    Skill.UserTargetScopeType.PetsInBag, new SkillConfigInfo(
			    TargetType.ALL
			    , CharacterType.PET
			    , SelfType.NOTSELF)
	    }
	    //		8、伴侣
	    , {
		    Skill.UserTargetScopeType.Fere, new SkillConfigInfo(
			    TargetType.PLAYER
			    , CharacterType.COUPLE
			    , SelfType.NOTSELF)
	    }
	    //		9、敌方玩家
	    , {
		    Skill.UserTargetScopeType.EnemyPlayer, new SkillConfigInfo(
			    TargetType.ENEMY
			    , CharacterType.PLAYER
			    , SelfType.NOTSELF)
	    }
	    //		10、仅己方玩家
	    , {
		    Skill.UserTargetScopeType.MyTeamPlayer, new SkillConfigInfo(
			    TargetType.PLAYER
			    , CharacterType.PLAYER
			    , SelfType.SELF)
	    }
    };

	public void Dispose()
	{
		_selectedSkill = null;
		skill = null;
		_selectParam = -1;
	}

	private Skill selectedSkill{
        set{
            _selectedSkill = value; 
        }
        get{ return _selectedSkill;}
    }
    private Skill _selectedSkill;
	//------------------------------------------------------------------------------------------------------
	#region

	public static BattleTargetSelector Create(MonsterController source, Skill _normalSkill)
	{
		var s = new BattleTargetSelector
		{
			normalSkill = _normalSkill
			, monsterSource = source
		};
		return s;
	}

	public void SetItemSkill(Skill skill, BagItemDto itemDto)
	{
		SkillParam = itemDto;
		selectedSkill = skill;

		lifeState = LifeState.ALL;
		selectorType = SelectorType.ITEM;
	}

	public void SetSkill()
	{
		SkillParam = null;
		selectedSkill = skill;
		GameDebuger.TODO(@"if (normalSkill.battleType == EquipmentActiveSkill.BattleType_PVP)
        {
            characterType = CharacterType.HERO;
        }");

		lifeState = LifeState.ALL;
		selectorType = SelectorType.SKILL;
	}

	public void SetSkill( Skill _skill)
	{
		SkillParam = null;
		MagicOrCraftSkill = _skill;
		selectedSkill = skill;
		GameDebuger.TODO(@"if (normalSkill.battleType == EquipmentActiveSkill.BattleType_PVP)
        {
            characterType = CharacterType.HERO;
        }");

		lifeState = LifeState.ALL;
		selectorType = SelectorType.SKILL;
	}

	private SkillConfigInfo GetSkillConfigInfo(Skill skill)
	{
		var val = skill == null
			? Skill.UserTargetScopeType.Unknown
			: (Skill.UserTargetScopeType) skill.skillAiId;

		SkillConfigInfo info;
		SkillConfigDic.TryGetValue(val, out info);
		if (info == null)
			info = new SkillConfigInfo(
				TargetType.NONE
				, CharacterType.Invalid
				, SelfType.Invalid);

		return info;
	}

	public void SetTargets( long targets )
	{
		monsterTargets = targets;
	}
	#endregion

	public void SetNormalSkill()
	{
		selectedSkill = normalSkill;
		lifeState = LifeState.ALL;
		selectorType = SelectorType.SKILL;
	}

	public bool Set_S_Skill()
	{
		if (monsterSource == null)
			return false;
		var skill = DataCache.getDtoByCls<Skill>(monsterSource.videoSoldier.defaultSCraftsId);
		if (skill == null)
			return false;
        //selectedSkill = DataCache.getDtoByCls<Skill>(monsterSource.videoSoldier.defaultSCraftsId);
        selectedSkill = skill;
        lifeState = LifeState.ALL;
		selectorType = SelectorType.SKILL;
		return true;
	}

	public Skill GetCurSkill()
	{
		return selectedSkill;
	}

	public bool IsCoupleSKill()
	{
		GameDebuger.TODO(@"if (normalSkill != null)
        {
            return normalSkill.relationType == Skill.RelationTypeEnum_Couple;
        }
        else");
		{
			return false;
		}
	}

	public TargetType getTargetType()
	{
		return GetSkillConfigInfo(selectedSkill).targetType;
	}

	private bool IsTargetTypeMatch( MonsterController choosePet, MonsterController mc )
	{
		var targetType = getTargetType();
		return targetType == TargetType.ALL
		       || (targetType == TargetType.ENEMY && mc.side == BattlePosition.MonsterSide.Enemy)
		       || (targetType == TargetType.PLAYER && mc.side == BattlePosition.MonsterSide.Player)
		       || (targetType == TargetType.SELF && choosePet.GetId() == mc.GetId());
	}

	private bool IsLifeStateMatch( MonsterController mc )
	{
		GameDebuger.Log( lifeState.ToString() );
		return (lifeState == LifeState.ALL)
		       || (lifeState == LifeState.DEAD && mc.IsDead())
		       || (lifeState == LifeState.ALIVE && !mc.IsDead());
	}

	private bool IsSelfTypeMatch(MonsterController choosePet, MonsterController mc)
	{
		var selfType  = GetSkillConfigInfo(selectedSkill).selfType;
		if (selfType != SelfType.NOTSELF)
			return true;	
		return choosePet.GetId() != mc.GetId();
	}

	private bool IsCharacterTypeMatch(MonsterController mc)
	{
		var characterType = GetSkillConfigInfo(selectedSkill).characterType;
		switch (characterType)
		{
			case CharacterType.HERO:
				return mc.IsMainCharactor();
			case CharacterType.NOTPET:
				return !(mc.IsPet());
			case CharacterType.COUPLE:
				return mc.IsMyCouple();
			case CharacterType.PLAYER:
				return mc.IsPlayer();
			default:
				return true;
		}
	}

	public bool CanSetTarget( MonsterController choosePet, MonsterController targetMC )
	{

		if (selectedSkill != null && selectedSkill.id == BattleDataManager.GetDefenseSkillId()){		
			return false;
		}

		if ( !IsTargetTypeMatch( choosePet, targetMC ) )
			return false;
//
//		if ( !IsLifeStateMatch( targetMC ) )
//			return false;

		if ( !IsSelfTypeMatch( choosePet, targetMC ) )
			return false;		

		if ( !IsCharacterTypeMatch( targetMC ) )
			return false;	

		if (selectedSkill.id == BattleDataManager.GetCaptureSkillId())
		{
			GameDebuger.TODO(@"if (targetMC.IsMonster())
            {
                Pet pet = targetMC.videoSoldier.monster.pet;
                if (pet != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else");
			{
				return false;
			}
		}
//		if ( targetMC.GetMP() >= normalSkill.mp )
//			return true;

		return true;
	}

	public bool CanSetCaptureTarget(MonsterController choosePet, MonsterController mc)
	{
		if (selectedSkill.id == BattleDataManager.GetCaptureSkillId())
		{
			GameDebuger.TODO(@"if (targetMC.IsMonster())
            {
                Pet pet = targetMC.videoSoldier.monster.pet;
                         if (ModelManager.Player.GetPlayerLevel() >= pet.companyLevel)
                         {
                             return true;
                         }
                         else
                {
                    TipManager.AddTip(string.Format('捕捉{0}需要等级{1}级', pet.name.WrapColor(ColorConstant.Color_Tip_Item), pet.companyLevel.WrapColor(ColorConstantV3.Color_Green_Str)));
                    return false;
                }
            }
            else");
			{
				return false;
			}
		}
		else
		{
			return true;
		}
	}

	public bool IsMainCharactor()
	{
		return !(monsterSource.IsPet ());
	}

    public GeneralCharactor.CharactorType GetCharactorType()
    {
        return monsterSource.GetCharactorType();
    }

    public bool IsItemSkill()
	{
		return selectedSkill.id == BattleDataManager.GetUseItemSkillId();
	}

    public bool IsCommandSkill()
    {
        return selectedSkill.id == BattleDataManager.GetCommandSkillId();
    }

	public int GetSelectedSkillId()
	{
		return selectedSkill.id;
	}

	public long GetSourceSoldierId()
	{
		return monsterSource.GetId();
	}

	public long GetTargetSoldierId()
	{
		return monsterTargets;
	}

	public string getSelectParam()
	{
		return string.Format ("IsMainCharactor={0} GetSelectedSkillId={1} GetTargetSoldierId={2}", IsMainCharactor (), GetSelectedSkillId (), GetTargetSoldierId ());
	}

	public void ClearCurSkill()
	{
		selectedSkill = null;
		SkillParam = null;
	}
}
