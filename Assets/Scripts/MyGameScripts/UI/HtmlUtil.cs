using Assets.Scripts.MyGameScripts.UI;
using System.Text.RegularExpressions;

namespace Assets.Scripts.MyGameScripts.UI
{
    public class HtmlUtil {
        public static string Font2(object msg, string color) {
            return string.Format("[{0}]{1}[-]", color, msg);
        }

        public static string NormalColor(object msg) {
            return Font2(msg, GameColor.PANEL_EXPLAIN_NORMAL_POP);
        }

        public static string Font2(object msg, uint color) {
            return string.Format("[{0}]{1}[-]", GameColor.getHtmlColorByIndex(color), msg);
        }

        public static string LinkColor(object msg, object link, string color = "00ff00", bool line = true) {
            if (line) {
                return string.Format("[{0}][url={1}][u]{2}[/u][/url][-]", color, link, msg);
            }
            return string.Format("[{0}][url={1}]{2}[/url][-]", color, link, msg);
        }

        public static string Link(string msg, string link, bool line = true) {
            if (line) {
                return string.Format("[url={0}][u]{1}[/u][/url]", link, msg);
            }
            return string.Format("[url={0}]{1}[/url]", link, msg);
        }

        public static string RoleName(string name, string color = GameColor.LINK_ROLE_NAME) {
            name = name ?? ModelManager.Player.GetPlayerName();
            return Font2("[" + name + "]", color);
        }

        public static string LinkRoleName(long roleID, string name, string color = GameColor.LINK_ROLE_NAME) {
            name = name ?? ModelManager.Player.GetPlayerName();
            return LinkColor("[" + name + "]", "roleID#" + name + "#" + roleID, color);
        }

        public static string LinkRoleName2(long roleID, string name, int cate, string color = GameColor.LINK_ROLE_NAME) {
            name = name ?? ModelManager.Player.GetPlayerName();
            return LinkColor("[" + name + "]", string.Format("roleID2#{0}#{1}#{2}", name, roleID, cate), color);
        }

        /// <summary>
        /// !!包含[]格式的都会被过滤，因此，角色尽量使用中文方括号包围【】,或遇到此种情况不使用该方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FilterHtml(string str) {
            return Regex.Replace(str, @"\[.*?\]", "", RegexOptions.Multiline);
        }

        public static string LinkPos(int mapID, string mapName, long pos, string color = GameColor.LINK_ROLE_NAME) {
            //            Vector3 rpos = MoveMath.GetPosVector(pos);
            return string.Format("pos#{0}#{1}#{2}", mapID, mapName, pos);
        }

        public static string FontCenter(string msg) {
            return "\n[c]" + msg + "\n[/c]";
        }

    }
}
