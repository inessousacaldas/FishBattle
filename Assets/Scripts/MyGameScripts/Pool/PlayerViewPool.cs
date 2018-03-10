using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
public class PlayerViewPool
{
    private Queue<PlayerView> _playerViewPool;
    public readonly WorldView master;
    private bool UsePool
    {
        get
        { return WorldView.UsePool; }
    }
    public PlayerViewPool(WorldView _master)
    {
        _playerViewPool = new Queue<PlayerView>(GameDisplayManager.MaxPlayerDataCount);
        master = _master;
    }

    public GameObject CreatePlayerViewGo()
    {
        GameObject playerGo = new GameObject();
        GameObjectExt.AddPoolChild(LayerManager.Root.WorldActors, playerGo);
        playerGo.tag = GameTag.Tag_Player;
        playerGo.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

        CharacterController characterController = playerGo.GetMissingComponent<CharacterController>();
        characterController.center = new Vector3(0f, 0.75f, 0f);
        characterController.radius = 0.4f;
        characterController.height = 2f;

        return playerGo;
    }

    public void DespawnPlayerView(PlayerView playerView)
    {
        if (playerView == null) return;
        if (playerView.IsHero)
        {
            playerView.SetUnitActive(false);
            return;
        }
        if (UsePool && _playerViewPool.Count < GameDisplayManager.MaxPlayerDataCount)
        {
            playerView.SetUnitActive(false);
            playerView.cachedGameObject.name = "playerViewX";
            _playerViewPool.Enqueue(playerView);
        }
        else
        {
            //超过当前场景最大玩家数量,直接删除
            Object.Destroy(playerView.cachedGameObject);
        }
    }

    public PlayerView SpawnPlayerView()
    {
        if (_playerViewPool.Count > 0)
        {
            var playerView = _playerViewPool.Dequeue();
            playerView.SetUnitActive(true);
            return playerView;
        }

        var playerGo = CreatePlayerViewGo();
        return playerGo.GetMissingComponent<PlayerView>();
    }
}
