using UnityEngine;
using System.Collections;

public class TeamConfirmHeadController:MonolessViewController<TeamConfirmHeadView>
{
    public string GetName() {
        return View.Name_UILabel.text;
    }
    public void Init(string IconName,string name)
    {
        UIHelper.SetPetIcon(View.Head_UISprite,IconName);
        View.Name_UILabel.text = name;
    }

    public void SetStaue(string IconName) {
        if(!View.Stats_UISprite.gameObject.activeSelf)
            View.Stats_UISprite.gameObject.SetActive(true);
        UIHelper.SetCommonIcon(View.Stats_UISprite,IconName);
    }

    /// <summary>
    /// 还原数据
    /// </summary>
    public void OnClose() {
        View.Head_UISprite.spriteName = "";
        View.Name_UILabel.text = "";
        View.Stats_UISprite.gameObject.SetActive(false);
        View.Stats_UISprite.spriteName = "";
    }

}
