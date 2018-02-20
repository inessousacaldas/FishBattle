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

/*
{
      "$type": "SkillConfigInfo, Assembly-CSharp",
      "id": 1329,
      "name": "普攻",
      "attackerActions": [
        {
          "$type": "MoveActionInfo, Assembly-CSharp",
		  var totalDis = Vector3.Distance(_mTrans.position, position);
		  var time = totalDis /(catchMode ? ModelHelper.DefaultBattleCatchSpeed * (turn ? 2f : 1f) : ModelHelper.DefaultBattleModelSpeed);
          "distance": 1.8,
          "type": "move",
          "name": "forward",
          "effects": []
        },
        {
          "$type": "NormalActionInfo, Assembly-CSharp",
		  anim.GetClipLength(action.ToString())//actack animation
		  should config Dao Guang
		  PlayDaoGuangEffect(_mc, tActionName);
          "type": "normal",
          "name": "attack",
          "effects": [
            {
              "$type": "TakeDamageEffectInfo, Assembly-CSharp",
			  PlayInjureHandle(ids[i], i, mAttacker);
			  no timing
              "type": "TakeDamage"
            },
            {
              "$type": "NormalEffectInfo, Assembly-CSharp",
            var normalEffectInfo = (NormalEffectInfo)node;
            if (_isAttack == false)
            {
                bool hasDodge = HasVideoDodgeTargetState(_stateGroup.targetStates);
                if (hasDodge && normalEffectInfo.hitEff)
                    return;
            }
            PlayNormalEffect(normalEffectInfo);
			
			PlaySpecialEffect(node, skillName, _mc, mc, clientSkillScale);
			
			default time of effect is 5
              "name": "skill_eff_1329_att",
              "mount": "Mount_Shadow",
              "faceToTarget": true,
              "type": "Normal"
            }
          ]
        },
        {
          "$type": "MoveBackActionInfo, Assembly-CSharp",
          var totalDis = Vector3.Distance(_mTrans.position, position);
          time = totalDis / (catchMode ? ModelHelper.DefaultBattleCatchSpeed : ModelHelper.DefaultBattleModelSpeed);

          "type": "moveBack",
          "name": "forward",
          "effects": []
        }
      ],
      "injurerActions": [
        {
          "$type": "NormalActionInfo, Assembly-CSharp",
			  float delayTime = node.delayTime;
			  if (actionName == ModelHelper.AnimType.hit)
              {
              //这里要特殊处理，因为防御动作结束后不需要播放hit， 需要直接回到battle
              delayTime += 0.3f;
              }
          "startTime": 0.8,
          "delayTime": 0.166666,受击后恢复到原始站位的时间
          "type": "normal",
          "effects": [
            {
              "$type": "ShowInjureEffectInfo, Assembly-CSharp",
			  ShowInjureEffect(ShowInjureEffectInfo node)
			  no timing
              "type": "ShowInjure",
              "playTime": 0.8
            },
            {
              "$type": "NormalEffectInfo, Assembly-CSharp",
              "name": "skill_eff_1329_hit",
              "mount": "Mount_Hit",
              "hitEff": true,
              "type": "Normal",
              "playTime": 0.8
            }
          ]
        }
      ]
    }
 */