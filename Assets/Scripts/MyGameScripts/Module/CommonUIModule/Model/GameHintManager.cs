using UnityEngine;
using AppDto;

/// <summary>
/// Game hint manager.
/// 用于显示游戏中的帮助提示
/// </summary>


public static class GameHintManager
{

    public const float FADEOUT_TIME = 10f;
    private const string VIEWNAME = "GameHintView";

    private static GameHintViewController _instance;

    public static void Open(GameObject target, string hint, UIAnchor.Side side = UIAnchor.Side.Top, int maxWidth = -1, bool needTimeClose = true, int spacingY = 5)
    {
        if (string.IsNullOrEmpty(hint))
            return;

        InitGameHintManager();
        _instance.Open(target, hint, side, maxWidth, needTimeClose, spacingY);
    }

    public static void Open(GameObject target, int hintId, UIAnchor.Side side = UIAnchor.Side.Top, int maxWidth = -1, bool needTimeClose = true, int spacingY = 5)
    {
        if (hintId == 0)
            return;

        string hint = GetHintIDString(hintId);
        Open(target, hint, side, maxWidth, needTimeClose, spacingY);
    }

    private static void InitGameHintManager()
    {
        GameObject view = UIModuleManager.Instance.OpenFunModule(VIEWNAME, UILayerType.FloatTip, false);
        if (_instance == null)
        {
            _instance = view.GetMissingComponent<GameHintViewController>();
            _instance.hintLbl = view.GetComponentInChildren<UILabel>();
            _instance.hintBg = view.GetComponentInChildren<UISprite>();
            _instance.posAnchor = view.GetComponentInChildren<UIAnchor>();
            _instance.panel = view.GetComponent<UIPanel>();
            _instance.panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
            _instance.panel.updateAnchors = UIRect.AnchorUpdate.OnEnable;
            _instance.panel.SetAnchor(LayerManager.Root.UICamera.gameObject, 20, 20, -20, -20);
        }
    }

    public static void Close()
    {
        UIModuleManager.Instance.HideModule(VIEWNAME);
    }

    public static void Dispose()
    {
        UIModuleManager.Instance.CloseModule(VIEWNAME);
        _instance = null;
    }

    public static string GetHintIDString(int hintId)
    {
        switch (hintId)
        {
            case 1:
                return "战斗中受到物理攻击和法术攻击则减少，为0时死亡";
            case 2:
                return "战斗中使用法术时消耗，为0时法术无法使用";
            case 3:
                return "物理攻击";
            case 4:
                return "物理防御";
            case 5:
                return "出手速度";
            case 6:
                return "法术攻击和法术防御";
            case 7:
                return "1点体质增加10点气血、0.1点速度、0.1点灵力";
            case 8:
                return "1点魔力增加0.7点灵力";
            case 9:
                return "1点力量增加0.7点攻击、0.1点速度、0.4点灵力";
            case 10:
                return "1点耐力增加1.5点防御、0.1点速度、0.1点灵力";
            case 11:
                return "1点敏捷增加0.7点速度";
            case 12:
                return "当前可支配属性点";
            case 13:
                return "影响攻击";
            case 14:
                return "影响防御";
            case 15:
                return "影响气血上限";
            case 16:
                return "影响灵力";
            case 17:
                return "影响速度";
            case 18:
                return "寿命低于50点将无法出战；每次出战会消耗1点，如战斗内死亡\n则会扣除50点；可使用长寿果、长寿面、糯米桂花丸提升寿命";
            case 19:
                return "影响宠物物理攻击力";
            case 20:
                return "影响宠物物理防御力";
            case 21:
                return "影响宠物气血上限";
            case 22:
                return "影响宠物灵力";
            case 23:
                return "影响宠物速度";
            case 24:
                return "主要影响气血上限，对灵力和速度有一定影响";
            case 25:
                return "主要影响灵力，对速度有一定影响";
            case 26:
                return "主要影响攻击，对灵力和速度有一定影响";
            case 27:
                return "主要影响防御，对灵力和速度有一定影响";
            case 28:
                return "影响速度";
            case 29:
                return "影响潜力增加属性的效果";
            case 30:
                return "影响宠物的加点效果，越高越好";
            default:
                return "";
        }
    }

    public static string GetFactionName(int factionId)
    {
        Faction tFaction = DataCache.getDtoByCls<Faction>(factionId);
        if (tFaction == null)
            return "(门派)";

        return tFaction.name;
    }

    public static string GetFactionHintString(Faction.FactionType factionId)
    {
        switch (factionId)
        {
            //暂时这样处理
            case Faction.FactionType.Swordman:
                return "重剑士";
            default:
                {
                    GameDebuger.TODO(@"case Faction.FactionType.DaTang:
                return '攻击敌方宠物时伤害增加5%';
            case Faction.FactionType.HuaSheng:
                return '每回合有10%几率自动解除自己身上的封印状态';
            case Faction.FactionType.FangCun:
                return '受到法术治疗时，效果增加20%；对带有傀儡类技能的敌人伤害提高100%';
            case Faction.FactionType.TianGong:
                return '15%几率躲避负面法术（不包括龙宫的门派法术）';
            case Faction.FactionType.LongGong:
                return '门派法术必定命中';
            case Faction.FactionType.PuTuo:
                return '倒地时有20%的几率出现涅槃效果，气血恢复15%。如果存在其他涅槃效果，则其几率增加10%';
            case Faction.FactionType.MoWang:
                return '10%几率躲避普通攻击，5%几率躲避物理技能';
            case Faction.FactionType.ShiTuo:
                return '宠物寿命消耗减半，并且战斗中不会逃跑';
            case Faction.FactionType.PanSi:
                return '被NPC击倒时，100%几率复活并恢复1点气血，\n每场战斗最多触发1次；被非NPC击倒时，触发几率为50%';
           
                case Faction.FactionType.JiLe:
                return '物理暴击提高2%';
            case Faction.FactionType.PengLai:
                return '法术暴击提高2%';
            case Faction.FactionType.Difu:
                return '夜战能力（夜晚物理攻击和防御不会降低），封印抵抗率增加20%';
                    ");
                }
                return "";
        }
    }
}
