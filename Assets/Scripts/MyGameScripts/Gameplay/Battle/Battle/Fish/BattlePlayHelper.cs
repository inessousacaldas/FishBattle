public static class BattlePlayHelper
{
    public static ModelHelper.AnimType GetAnimType(this string name,ModelHelper.AnimType defaultValue=ModelHelper.AnimType.hit)
    {
        return EnumParserHelper.TryParse(name, defaultValue);
    }
}
