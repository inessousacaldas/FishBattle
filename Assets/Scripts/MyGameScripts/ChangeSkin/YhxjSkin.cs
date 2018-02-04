using UnityEngine;
using AssetPipeline;


public static class YhxjSkin
{
	public const string WorldMapUIAtlas_Yhxj = "WorldMapUIAtlas_Yhxj";
	public const string WorldMap_Yhxj = "WorldMap_Yhxj";

	public const string GoldCell_Yhxj = "GoldCell_Yhxj";

	public const string MainUIAltas_Yhxj = "MainUIAltas_Yhxj";
    /*
	public static void ChangeMiniMapSkin(MiniMapController controller)
	{
		if (controller != null)
		{
			ChangeSkinHelper.ChangeAtlas(controller.View.ChangeMapBtn_UIButton.sprite, GetWorldMapUIAtlas_Yhxj());
		}
	}*/

    //legacy 2017-02-22 17:13:58
//	public static void ChangeGoldBuyWinSkin(GoldBuyWinUIController controller)
//	{
//		if (controller != null)
//		{
//			controller.View.GoldGrid_UIGrid.maxPerLine = 2;
//			controller.View.GoldGrid_UIGrid.cellWidth = 464;
//			controller.View.GoldGrid_UIGrid.cellHeight = 174;
//			controller.View.GoldGrid_UIGrid.transform.localPosition = new Vector3(-254, 125, 0);
//		}
//	}

//	public static void ChangeMainUISkin(MainUIViewController controller)
//	{
//		if (controller != null)
//		{
//			var mainUIAtlas = GetMainUIAltas_Yhxj();
//
//			var sprites = controller.GetComponentsInChildren<UISprite>(true);
//			for (int i = 0; i < sprites.Length; i++)
//			{
//				ChangeSkinHelper.ChangeAtlas(sprites[i], mainUIAtlas);
//			}
//		}
//	}


	private static UIAtlas GetWorldMapUIAtlas_Yhxj()
	{
		return
            (AssetManager.Instance.LoadAsset(WorldMapUIAtlas_Yhxj, ResGroup.UIAtlas) as GameObject).GetComponent<UIAtlas>();
    }


	private static UIAtlas GetMainUIAltas_Yhxj()
	{
		return
            (AssetManager.Instance.LoadAsset(MainUIAltas_Yhxj, ResGroup.UIAtlas) as GameObject).GetComponent<UIAtlas>();
    }
}
