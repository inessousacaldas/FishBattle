using Newtonsoft.Json;

public class CorrectSkillConfig
{
    public int id = 0;
    public string name = "";
    
    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
    public BattlePhraseBase battlePhrase;
}

public static class CorrectSkillConfigExt
{
    public static BattlePhraseBase Chain(this BattlePhraseBase first, BattlePhraseBase second)
    {
        if (first == null) return second;
        if (second == null) return first;
        return SeqPhrase.Create(new[] {first, second});
    }

    public static BattlePhraseBase Parall(this BattlePhraseBase first, BattlePhraseBase second)
    {
        if (first == null) return second;
        if (second == null) return first;
        return ParPhrase.Create(new[] {first, second});
    }

    public static BattlePhraseBase Branch(this BattlePhraseBase main, BattlePhraseBase other)
    {
        if (other == null) return main;
        return BranchPhrase.Create(main,other);
    }
}

public abstract class BattlePhraseBase
{
    [JsonIgnore]
    public abstract float Duaration { get; }
}

public class SeqPhrase : BattlePhraseBase
{
    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
    private BattlePhraseBase[] _lst;
    private float _duration;

    [JsonConstructor]
    private SeqPhrase(){}
    
    private SeqPhrase(BattlePhraseBase[] lst)
    {
        _lst = lst;
        foreach (var battlePhraseBase in lst)
        {
            _duration += battlePhraseBase.Duaration;
        }
    }

    public static SeqPhrase Create(BattlePhraseBase[] lst)
    {
        return new SeqPhrase(lst);
    }

    public override float Duaration {
        get { return _duration; }
    }
}

public class ParPhrase : BattlePhraseBase
{
    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
    private BattlePhraseBase[] _lst;
    private float _duration;

    [JsonConstructor]
    private ParPhrase(){}
    
    private ParPhrase(BattlePhraseBase[] lst)
    {
        _lst=lst;
        foreach (var battlePhraseBase in lst)
        {
            if (_duration < battlePhraseBase.Duaration)
                _duration = battlePhraseBase.Duaration;
        }
    }

    public static ParPhrase Create(BattlePhraseBase[] lst)
    {
        return new ParPhrase(lst);
    }

    public override float Duaration {
        get { return _duration; }
    }
}

public class BranchPhrase : BattlePhraseBase
{
    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
    private BattlePhraseBase _main;
    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
    private BattlePhraseBase _other;
    private float _duration;

    [JsonConstructor]
    private BranchPhrase(){}
    
    private BranchPhrase(BattlePhraseBase main, BattlePhraseBase other)
    {
        _main=main;
        _other = other;
        _duration = main.Duaration;
    }

    public static BranchPhrase Create(BattlePhraseBase first,BattlePhraseBase second)
    {
        return new BranchPhrase(first,second);
    }

    public override float Duaration {
        get { return _duration; }
    }
}

public class WaitPhrase : BattlePhraseBase
{
    [JsonProperty]
    private float _duration;

    [JsonConstructor]
    private WaitPhrase(){}
    
    private WaitPhrase(float duaration)
    {
        _duration=duaration;
    }

    public static WaitPhrase Create(float duaration)
    {
        return new WaitPhrase(duaration);
    }

    public override float Duaration
    {
        get { return _duration; }
    }
}

public class ActionPhrase : BattlePhraseBase
{
    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
    private BaseActionInfo _actInfo;
    private float _duration;
    
    [JsonConstructor]
    private ActionPhrase(){}

    private ActionPhrase(BaseActionInfo actionInfo)
    {
        _actInfo=actionInfo;
    }

    public static ActionPhrase Create(BaseActionInfo actionInfo)
    {
        return new ActionPhrase(actionInfo);
    }

    public override float Duaration
    {
        get { return _duration; }
    }
}

public class EffectPhrase : BattlePhraseBase
{
    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
    private BaseEffectInfo _effCfg;
    private float _duration;

    [JsonConstructor]
    private EffectPhrase(){}
    
    private EffectPhrase(BaseEffectInfo effectInfo)
    {
        _effCfg = effectInfo;
    }

    public static EffectPhrase Create(BaseEffectInfo effectInfo)
    {
        return new EffectPhrase(effectInfo);
    }

    public override float Duaration
    {
        get { return _duration; }
    }
}