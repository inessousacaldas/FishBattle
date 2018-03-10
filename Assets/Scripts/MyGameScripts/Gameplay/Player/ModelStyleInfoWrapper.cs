using AppDto;

public static class ModelStyleInfoWrapper
{
    public static ModelStyleInfo CreateModelStyleInfo(PlayerModel player, bool showFashion = false)
    {
        ModelStyleInfo _modelStyleInfo = null;
        var playerDto = player.GetPlayer();
        var transformModelId = player.TransformModelId;

        if (transformModelId != 0)
        {
            _modelStyleInfo = new ModelStyleInfo();
            _modelStyleInfo.TransformModelId = transformModelId;
        }
        else
        {
            _modelStyleInfo = ModelStyleInfo.ToInfo(playerDto.charactor);
//            _modelStyleInfo.weaponId = ModelManager.Backpack.GetCurrentWeaponModel();
//            _modelStyleInfo.weaponEffId = playerDto.dressInfoDto.weaponEffect;
//            _modelStyleInfo.hallowSpriteId = ModelManager.Backpack.GetCurrentHallowSpriteId();
            _modelStyleInfo.mutateTexture = 0;
//            _modelStyleInfo.mutateColorParam = PlayerModel.GetDyeColorParams(playerDto.dressInfoDto);
//            GameEventCenter.AddListener(GameEvent.Backpack_OnWeaponModelChange, UpdateWeapon);
//            GameEventCenter.AddListener(GameEvent.Backpack_OnHallowSpriteChange, UpdateHallowSprite);

//            if (showFashion && playerDto.dressInfoDto != null)
            {
//                _modelStyleInfo.SetupFashionIds(playerDto.dressInfoDto.fashionDressIds);
            }
        }
        return _modelStyleInfo;
    }

    public static ModelStyleInfo CreateModelStyleInfo(GeneralCharactor charactor, PlayerDressInfoDto dressInfo)
    {
        var _modelStyleInfo = ModelStyleInfo.ToInfo(charactor);
        _modelStyleInfo.weaponId = dressInfo.wpmodel;
        _modelStyleInfo.weaponEffId = dressInfo.weaponEffect;
        _modelStyleInfo.hallowSpriteId = dressInfo.hallowSpriteId;
        _modelStyleInfo.mutateTexture = 0;
        _modelStyleInfo.mutateColorParam = PlayerModel.GetDyeColorParams(dressInfo);
        return _modelStyleInfo;
    }
}