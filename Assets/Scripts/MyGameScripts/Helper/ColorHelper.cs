/// <summary>
/// Helper class for color
/// </summary>
public class ColorHelper
{
	//格式化为主界面聊天框的显示样式
	/// <summary>
	/// 把文本中的颜色更新为深度背景下对应的颜色
	/// </summary>
	/// <see cref="http://oa.cilugame.com/redmine/issues/10918"/>
	/// <returns>The color in deep style.</returns>
	/// <param name="str">String.</param>
	public static string UpdateColorInDeepStyle(string str)
	{
		return str.Replace("[0081AB]", "[2DC6F8]")
			.Replace("[A52D00]", "[FFF9E3]")
				.Replace("[A64E00]", "[F7E423]")
				.Replace("[1D8E00]", "[0FFF32]")
				.Replace("[8130A7]", "[C368E9]")
				.Replace("[0081AA]", "[FFF9E2]")
				.Replace("[C30000]", "[FD614C]");
	}
	
	/// <summary>
	/// 把文本中的颜色更新为浅度背景下对应的颜色
	/// </summary>
	/// <see cref="http://oa.cilugame.com/redmine/issues/10918"/>
	/// <returns>The color in light style.</returns>
	/// <param name="str">String.</param>
	public static string UpdateColorInLightStyle(string str)
	{
		return str.Replace("[2DC6F8]", "[0081AB]")
			.Replace("[FFF9E3]", "[A52D00]")
				.Replace("[F7E423]", "[A64E00]")
				.Replace("[0FFF32]", "[1D8E00]")
				.Replace("[C368E9]", "[8130A7]")
				.Replace("[D08D05]", "[E7BD37]")
				.Replace("[FFF9E2]", "[0081AA]")
				.Replace("[FD614C]", "[C30000]");
	}

	public static string UpdateColorInStyledBG(string pOrignLabelStr,bool pIsDeepBG)
	{
		return pIsDeepBG?UpdateColorInDeepStyle(pOrignLabelStr):UpdateColorInLightStyle(pOrignLabelStr);
	}
}