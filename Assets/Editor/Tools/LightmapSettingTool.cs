using UnityEngine;
using UnityEditor;

public class LightmapHelper : MonoBehaviour
{
	[MenuItem("Tools/LightmappingBake_256")]
	static void Init256()
	{
		LightmapEditorSettings.maxAtlasHeight = 256;
		LightmapEditorSettings.maxAtlasWidth = 256;
		Lightmapping.Clear();
		Lightmapping.Bake();
	}

	[MenuItem("Tools/LightmappingBake_512")]
	static void Init512()
	{
		LightmapEditorSettings.maxAtlasHeight = 512;
		LightmapEditorSettings.maxAtlasWidth = 512;
		Lightmapping.Clear();
		Lightmapping.Bake();
	}

	[MenuItem("Tools/LightmappingBake_1024")]
	static void Init1024()
	{
		LightmapEditorSettings.maxAtlasHeight = 1024;
		LightmapEditorSettings.maxAtlasWidth = 1024;
		Lightmapping.Clear();
		Lightmapping.Bake();
	}
}

