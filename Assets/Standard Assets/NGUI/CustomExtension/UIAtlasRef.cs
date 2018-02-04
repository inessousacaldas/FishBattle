using AssetPipeline;
using UnityEngine;

public class UIAtlasRef : MonoBehaviour
{
    public UIAtlas Atlas
    {
        get { return atlas; }
        set
        {
            if (value != atlas)
            {
                AssetManager.RemoveAtlasRef(atlas, this);
                atlas = value;
                AssetManager.AddAtlasRef(atlas, this);

            }
        }
    }

    [SerializeField]
    private UIAtlas atlas;

    protected void Awake()
    {
        AssetManager.AddAtlasRef(atlas, this);
    }

    protected void OnDestroy()
    {
        AssetManager.RemoveAtlasRef(atlas, this);
    }
}
