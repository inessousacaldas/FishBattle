#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Fish;
using NJson = Newtonsoft.Json;
using JsonC = Newtonsoft.Json.JsonConvert;

public class SimulateCorrectSkillConfig
{
    public static CorrectSkillConfig SimulateNormalAttack()
    {
        var movePhrase = ActionPhrase.Create(new MoveActionInfo
        {
            type = MoveActionInfo.TYPE,
            name = "forward",
            distance = 1.8f,
            time=1f,
            initiator = ActionInitiator.Attacker,
        });

        var attackPhrase = ActionPhrase.Create(new NormalActionInfo
        {
            type = NormalActionInfo.TYPE,
            name = "attack",
            initiator = ActionInitiator.Attacker,
            effects = new List<BaseEffectInfo>
            {
                new NormalEffectInfo
                {
                    type = NormalEffectInfo.TYPE,
                    name = "skill_eff_1329_att",
                    mount = "Mount_Shadow",
                    faceToTarget = true,
                },
            }
        });

        var movebackPhrase = ActionPhrase.Create(new MoveBackActionInfo
        {
            type = MoveBackActionInfo.TYPE,
            name = "forward",
            time=1f,
            initiator = ActionInitiator.Attacker,
        });

        var injurePhrase = ActionPhrase.Create(new NormalActionInfo
        {
            type = NormalActionInfo.TYPE,
            name = "hit",
            //startTime = 0.8f,
            delayTime = 0.167f,
            initiator = ActionInitiator.Victim,
            effects = new List<BaseEffectInfo>
            {
                new NormalEffectInfo
                {
                    type = NormalEffectInfo.TYPE,
                    name = "skill_eff_1329_hit",
                    mount = "Mount_Hit",
                    hitEff = true,
                    playTime = 0.8f
                },
                new ShowInjureEffectInfo
                {
                    type = ShowInjureEffectInfo.TYPE,
                    playTime = 0.8f
                }
            }
        });

        var attackAndHitPhrase = attackPhrase.Parall(injurePhrase);
        var wholePhrase = SeqPhrase.Create(new[]
            {movePhrase, attackAndHitPhrase, movebackPhrase, WaitPhrase.Create(0.5f)});

        return new CorrectSkillConfig
        {
            id = 1329,
            name = "normal attack",
            battlePhrase = wholePhrase
        };
    }

    public static CorrectSkillConfig SimulateNormalAttack1609()
    {
        var attackPhrase = ActionPhrase.Create(new NormalActionInfo
        {
            type = NormalActionInfo.TYPE,
            name = "attack",
            initiator = ActionInitiator.Attacker,
            effects = new List<BaseEffectInfo>
            {
                new NormalEffectInfo
                {
                    type = NormalEffectInfo.TYPE,
                    name = "skill_eff_1609_att",
                    mount = "Mount_Shadow",
                    faceToTarget = true,
                    playTime = 0.05f,
                },
                new NormalEffectInfo
                {
                    type = NormalEffectInfo.TYPE,
                    name = "skill_eff_1609_fly",
                    delayTime=0.2333f,
                    mount = "Mount_Hit",
                    faceToTarget = true,
                    fly = true,
                    playTime = 0.25f,
                },
            }
        });

        var injurePhrase = ActionPhrase.Create(new NormalActionInfo
        {
            type = NormalActionInfo.TYPE,
            name = "hit",
            startTime = 0.55f,
            delayTime = 0.3f,
            initiator = ActionInitiator.Victim,
            effects = new List<BaseEffectInfo>
            {
                new NormalEffectInfo
                {
                    type = NormalEffectInfo.TYPE,
                    name = "skill_eff_1609_hit",
                    mount = "Mount_Hit",
                    hitEff = true,
                    //playTime = 0.55f
                },
                new ShowInjureEffectInfo
                {
                    type = ShowInjureEffectInfo.TYPE,
                    //playTime = 0.55f
                }
            }
        });

        var attackAndHitPhrase = attackPhrase.Parall(injurePhrase);

        return new CorrectSkillConfig
        {
            id = 1609,
            name = "normal attack",
            battlePhrase = attackAndHitPhrase
        };
    }
    
    public static void SaveSimulatedNormalAttack()
    {
        var newjson = SimulateNormalAttack();
        var serializeObject = newjson.ToBattleJsonStr();
        File.WriteAllText("CorrectBattleConfig.json", serializeObject);
    }

    public static void TestSimulatedConfig()
    {
        string path = "CorrectBattleConfig.json";
        var newjson = JsonC.DeserializeObject<CorrectSkillConfig>(File.ReadAllText(path));
        System.Console.ReadLine();
    }
}
#endif
