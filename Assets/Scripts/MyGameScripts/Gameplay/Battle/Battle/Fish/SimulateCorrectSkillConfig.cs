using System.IO;
using NJson = Newtonsoft.Json;
using JsonC = Newtonsoft.Json.JsonConvert;

public class SimulateCorrectSkillConfig
{
    public static CorrectSkillConfig SimulateNormalAttack()
    {
        var movePhrase = ActionPhrase.Create(new MoveActionInfo {type = MoveActionInfo.TYPE});
        var attackPhrase = ActionPhrase.Create(new NormalActionInfo {type = NormalActionInfo.TYPE});
        var movebackPhrase = ActionPhrase.Create(new MoveBackActionInfo {type = MoveBackActionInfo.TYPE});

        var injurePhrase = ActionPhrase.Create(new NormalActionInfo {type = NormalActionInfo.TYPE});
        var damagePhrase = EffectPhrase.Create(new NormalEffectInfo {type = NormalEffectInfo.TYPE});

        var hitPhrase = injurePhrase.Branch(damagePhrase);

        var attackAndHitPhrase = attackPhrase.Parall(hitPhrase);
        var wholePhrase = SeqPhrase.Create(new[]
            {movePhrase, attackAndHitPhrase, movebackPhrase, WaitPhrase.Create(0.5f)});
        return new CorrectSkillConfig
        {
            id = 1329,
            name = "normal attack",
            battlePhrase = wholePhrase
        };
    }

    public static void SaveSimulatedNormalAttack()
    {
        var newjson = SimulateNormalAttack();
        var serializeObject = JsonC.SerializeObject(newjson, NJson.Formatting.Indented,
            new NJson.JsonSerializerSettings {DefaultValueHandling = NJson.DefaultValueHandling.Ignore});
        File.WriteAllText("CorrectBattleConfig.json", serializeObject);
    }

    public static void TestSimulatedConfig()
    {
        string path = "CorrectBattleConfig.json";
        var newjson = JsonC.DeserializeObject<CorrectSkillConfig>(File.ReadAllText(path));
        System.Console.ReadLine();
    }
}