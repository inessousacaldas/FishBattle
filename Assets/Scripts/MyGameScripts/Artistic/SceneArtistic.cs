using UnityEngine;

public class SceneArtistic
{
    public static Vector3 shadowDir = new Vector3(-0.01266672f,-0.6694283f,-0.7427687f) * 2;

    public static void SetShadowDir(Vector3 dir)
    {
        dir.Normalize();
        if (dir != shadowDir)
        {
            shadowDir = dir;
            Shader.SetGlobalVector("_WorldShadowDir", new Vector4(shadowDir.x, shadowDir.y, shadowDir.z, 0));
        }
    }

    public static void SetShadowDir(string dir)
    {
        if (string.IsNullOrEmpty(dir))
            return;
        string[] tDirStrs = dir.Split(':');
        if (null == tDirStrs || tDirStrs.Length < 3)
            return;
        SetShadowDir(new Vector3(tDirStrs[0].ToFloat(), tDirStrs[1].ToFloat(), tDirStrs[2].ToFloat()));
    }
}
