using System.Collections.Generic;
using UnityEngine;
public class UILabelHUD : MonoBehaviour, UpdateManager.ILateUpdateObj
{
    [SerializeField]
    Font mTrueTypeFont;
    [SerializeField]
    UIFont mFont;

    //public UILabel.Crispness keepCrispWhenShrunk = UILabel.Crispness.OnDesktop;

    [SerializeField]
    string mText = "";
    [SerializeField]
    int mFontSize = 16;
    [SerializeField]
    FontStyle mFontStyle = FontStyle.Normal;
    [SerializeField]
    NGUIText.Alignment mAlignment = NGUIText.Alignment.Automatic;
    [SerializeField]
    bool mEncoding = true;
    [SerializeField]
    int mMaxLineCount = 0; // 0 denotes unlimited
    [SerializeField]
    UILabel.Effect mEffectStyle = UILabel.Effect.None;
    [SerializeField]
    Color mEffectColor = Color.black;
    [SerializeField]
    NGUIText.SymbolStyle mSymbols = NGUIText.SymbolStyle.Normal;
    [SerializeField]
    Vector2 mEffectDistance = Vector2.one;
    [SerializeField]
    UILabel.Overflow mOverflow = UILabel.Overflow.ResizeFreely;
    [SerializeField]
    Material mMaterial;
    [SerializeField]
    bool mApplyGradient = false;
    [SerializeField]
    Color mGradientTop = Color.white;
    [SerializeField]
    Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField]
    int mSpacingX = 0;
    [SerializeField]
    int mSpacingY = 0;
    [SerializeField]
    bool mUseFloatSpacing = false;
    [SerializeField]
    float mFloatSpacingX = 0;
    [SerializeField]
    float mFloatSpacingY = 0;
    [SerializeField]
    bool mOverflowEllipsis = false;
    [SerializeField]
    UIKeyBinding.Modifier mModifier = UIKeyBinding.Modifier.None;
    [SerializeField]
    Color mColor = Color.white;
    [SerializeField]
    protected int mWidth = 1000;
    [SerializeField]
    protected int mHeight = 50;

    [System.NonSerialized]
    int mFinalFontSize = 0;
    [System.NonSerialized]
    readonly float mScale = 1f;
    [System.NonSerialized]
    protected Vector4 mDrawRegion = new Vector4(0f, 0f, 1f, 1f);
    [System.NonSerialized]
    bool mPremultiply = false;
    [System.NonSerialized]
    Vector2 mCalculatedSize = Vector2.zero;
    public string text
    {
        get { return mText; }
        set
        {
            if (mText != value)
            {
                mText = value;
                markChange = true;
            }
        }
    }
    public UIFont bitmapFont
    {
        get
        {
            return mFont;
        }
        set
        {
            if (mFont != value)
            {
                mFont = value;
                mTrueTypeFont = null;
            }
        }
    }
    public Font trueTypeFont
    {
        get
        {
            if (mTrueTypeFont != null) return mTrueTypeFont;
            return (mFont != null ? mFont.dynamicFont : null);
        }
        set
        {
            if (mTrueTypeFont != value)
            {
                mTrueTypeFont = value;
                mFont = null;
            }
        }
    }
    /// <summary>
    /// Convenience property to get the used y spacing.
    /// </summary>

    public float effectiveSpacingY
    {
        get
        {
            return mUseFloatSpacing ? mFloatSpacingY : mSpacingY;
        }
    }

    /// <summary>
    /// Convenience property to get the used x spacing.
    /// </summary>

    public float effectiveSpacingX
    {
        get
        {
            return mUseFloatSpacing ? mFloatSpacingX : mSpacingX;
        }
    }

    //    bool keepCrisp
    //    {
    //        get
    //        {
    //            if (trueTypeFont != null && keepCrispWhenShrunk != UILabel.Crispness.Never)
    //            {
    //#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY
    //				return (keepCrispWhenShrunk == Crispness.Always);
    //#else
    //                return true;
    //#endif
    //            }
    //            return false;
    //        }
    //    }
    public NGUIText.Alignment alignment
    {
        get
        {
            return mAlignment;
        }
        set
        {
            if (mAlignment != value)
            {
                mAlignment = value;
                markChange = true;
            }
        }
    }

    public int fontSize
    {
        get { return mFontSize; }
        set
        {
            if (mFontSize != value)
            {
                mFontSize = value;
                markChange = true;
            }
        }
    }

    public UILabel.Effect effectStyle
    {
        get { return mEffectStyle; }
        set
        {
            if (mEffectStyle != value)
            {
                mEffectStyle = value;
                markChange = true;
            }
        }
    }

    public Color effectColor
    {
        get { return mEffectColor; }
        set
        {
            if (mEffectColor != value)
            {
                mEffectColor = value;
                markChange = true;
            }
        }
    }

    public Vector2 effectDistance
    {
        get { return mEffectDistance; }
        set
        {
            if (mEffectDistance != value)
            {
                mEffectDistance = value;
                markChange = true;
            }
        }
    }

    public bool markChange
    {
        get { return _markChange; }
        set
        {
            if (_markChange != value)
            {
                _markChange = value;
                if (_markChange)
                {
                    UpdateManager.Add(this);
                }
            }
        }
    }
    [System.NonSerialized]
    public bool _markChange = false;
    LinkedListNode<UpdateManager.ILateUpdateObj> UpdateManager.ILateUpdateObj.node { get; set; }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private BetterList<Vector3> verts;
    private BetterList<Vector2> uvs;
    private BetterList<Color> colors;
    void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;

        verts = new BetterList<Vector3>();
        uvs = new BetterList<Vector2>();
        colors = new BetterList<Color>();
        Font.textureRebuilt += OnFontTextureRebuild;

    }


    void OnEnable()
    {
        markChange = true;
    }
    void UpdateManager.ILateUpdateObj.CustomLateUpdate()
    {
        //先Remove 再生成，防止在生成过程中再次触发动态字体材质的更新但直接Remove了导致事件丢失
        UpdateManager.Remove(this);
        if (markChange)
        {
            GenerateMesh();
        }
    }

    void OnDisable()
    {
        meshRenderer.enabled = false;
        markChange = false;
        UpdateManager.Remove(this);
    }


    private void OnFontTextureRebuild(Font obj)
    {
        if (this.isActiveAndEnabled)
        {
            markChange = true;
        }
    }

    void OnDestroy()
    {
        Font.textureRebuilt -= OnFontTextureRebuild;
        UpdateManager.Remove(this);
    }

    public void GenerateMesh()
    {
        if (mFont == null || mFont.material == null)
            return;
        UIPanel parent = GetComponentInParent<UIPanel>();
        int renderQueue = 2450;
        if (parent != null && parent.renderQueue == UIPanel.RenderQueue.StartAt)
        {
            renderQueue = parent.startingRenderQueue;
        }
        markChange = false;
        Material material = GetMaterial(mFont, renderQueue);
        meshRenderer.sharedMaterial = material;
        meshRenderer.enabled = true;
        mesh.Clear(false);
        verts.Clear();
        uvs.Clear();
        colors.Clear();
        OnFill(verts, uvs, colors);
        OffsetWidthAndHeight(verts);
        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        int indexCount = (verts.size >> 1) * 3;
        int count = verts.size;
        mesh.triangles = UIDrawCall.GenerateCachedIndexBuffer(count, indexCount);
        mesh.RecalculateBounds();

    }
    void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color> cols)
    {
        Color col = mColor;
        int offset = verts.size;
        int start = verts.size;
        UpdateNGUIText();
        NGUIText.tint = col;
        NGUIText.Print(text, verts, uvs, cols, null);
        NGUIText.bitmapFont = null;
        NGUIText.dynamicFont = null;
        NGUIText.emojiFont = null;

        Vector2 pos = ApplyOffset(verts, start);
        // Effects don't work with packed fonts
        if (mFont != null && mFont.packedFontShader) return;

        // Apply an effect if one was requested
        if (mEffectStyle != UILabel.Effect.None)
        {
            int end = verts.size;
            pos.x = mEffectDistance.x;
            pos.y = mEffectDistance.y;

            ApplyShadow(verts, uvs, cols, offset, end, pos.x, -pos.y);

            if ((mEffectStyle == UILabel.Effect.Outline) || (mEffectStyle == UILabel.Effect.Outline8))
            {
                offset = end;
                end = verts.size;

                ApplyShadow(verts, uvs, cols, offset, end, -pos.x, pos.y);

                offset = end;
                end = verts.size;

                ApplyShadow(verts, uvs, cols, offset, end, pos.x, pos.y);

                offset = end;
                end = verts.size;

                ApplyShadow(verts, uvs, cols, offset, end, -pos.x, -pos.y);

                if (mEffectStyle == UILabel.Effect.Outline8)
                {
                    offset = end;
                    end = verts.size;

                    ApplyShadow(verts, uvs, cols, offset, end, -pos.x, 0);

                    offset = end;
                    end = verts.size;

                    ApplyShadow(verts, uvs, cols, offset, end, pos.x, 0);

                    offset = end;
                    end = verts.size;

                    ApplyShadow(verts, uvs, cols, offset, end, 0, pos.y);

                    offset = end;
                    end = verts.size;

                    ApplyShadow(verts, uvs, cols, offset, end, 0, -pos.y);
                }
            }
        }
    }

    void UpdateNGUIText()
    {
        Font ttf = trueTypeFont;

        mFinalFontSize = Mathf.Abs(mFontSize);
        NGUIText.fontSize = mFinalFontSize;
        NGUIText.fontStyle = mFontStyle;
        NGUIText.rectWidth = mWidth;
        NGUIText.rectHeight = mHeight;
        NGUIText.regionWidth = Mathf.RoundToInt(mWidth * (mDrawRegion.z - mDrawRegion.x));
        NGUIText.regionHeight = Mathf.RoundToInt(mHeight * (mDrawRegion.w - mDrawRegion.y));
        NGUIText.gradient = mApplyGradient && (mFont == null || !mFont.packedFontShader);
        NGUIText.gradientTop = mGradientTop;
        NGUIText.gradientBottom = mGradientBottom;
        NGUIText.encoding = mEncoding;
        NGUIText.premultiply = mPremultiply;
        NGUIText.symbolStyle = mSymbols;
        NGUIText.maxLines = mMaxLineCount;
        NGUIText.spacingX = effectiveSpacingX;
        NGUIText.spacingY = effectiveSpacingY;
        NGUIText.fontScale = ((float)mFontSize / mFont.defaultSize) * mScale;

        if (mFont != null)
        {
            NGUIText.bitmapFont = mFont;
            NGUIText.emojiFont = mFont;

            for (;;)
            {
                UIFont fnt = NGUIText.bitmapFont.replacement;
                if (fnt == null) break;
                NGUIText.bitmapFont = fnt;
                NGUIText.emojiFont = fnt;
            }
            if (NGUIText.bitmapFont.isDynamic)
            {
                NGUIText.dynamicFont = NGUIText.bitmapFont.dynamicFont;
                NGUIText.emojiFont = NGUIText.bitmapFont.emojiFont;
                NGUIText.bitmapFont = null;
            }
            else
            {
                NGUIText.dynamicFont = null;
            }
        }
        else
        {
            NGUIText.dynamicFont = ttf;
            NGUIText.bitmapFont = null;
            NGUIText.emojiFont = null;
        }

        NGUIText.pixelDensity = 1f;
        NGUIText.Update();

        ProcessText(false, false);

        NGUIText.rectWidth = mWidth;
        NGUIText.rectHeight = mHeight;
        NGUIText.regionWidth = Mathf.RoundToInt(mWidth * (mDrawRegion.z - mDrawRegion.x));
        NGUIText.regionHeight = Mathf.RoundToInt(mHeight * (mDrawRegion.w - mDrawRegion.y));

        if (alignment == NGUIText.Alignment.Automatic)
        {
            NGUIText.alignment = NGUIText.Alignment.Center;
        }
        else NGUIText.alignment = alignment;

        NGUIText.Update();
    }
    void ProcessText(bool legacyMode, bool full)
    {
        float regionX = mDrawRegion.z - mDrawRegion.x;
        float regionY = mDrawRegion.w - mDrawRegion.y;

        NGUIText.rectWidth = legacyMode ? 1000000 : mWidth;
        NGUIText.rectHeight = legacyMode ? 1000000 : mHeight;
        NGUIText.regionWidth = (regionX != 1f) ? Mathf.RoundToInt(NGUIText.rectWidth * regionX) : NGUIText.rectWidth;
        NGUIText.regionHeight = (regionY != 1f) ? Mathf.RoundToInt(NGUIText.rectHeight * regionY) : NGUIText.rectHeight;

        mFinalFontSize = Mathf.Abs(mFontSize);
        NGUIText.fontSize = mFinalFontSize;

        //if (full) UpdateNGUIText();

        if (mOverflow == UILabel.Overflow.ResizeFreely)
        {
            NGUIText.rectWidth = 1000000;
            NGUIText.regionWidth = 1000000;
        }

        if (mOverflow == UILabel.Overflow.ResizeFreely || mOverflow == UILabel.Overflow.ResizeHeight)
        {
            NGUIText.rectHeight = 1000000;
            NGUIText.regionHeight = 1000000;
        }
        if (mOverflow == UILabel.Overflow.ResizeFreely)
        {
            mCalculatedSize = NGUIText.CalculatePrintedSize(mText);
            int w = Mathf.RoundToInt(mCalculatedSize.x);
            if (regionX != 1f) w = Mathf.RoundToInt(w / regionX);
            int h = Mathf.RoundToInt(mCalculatedSize.y);
            if (regionY != 1f) h = Mathf.RoundToInt(h / regionY);

            if ((w & 1) == 1) ++w;
            if ((h & 1) == 1) ++h;

            if (mWidth != w || mHeight != h)
            {
                mWidth = w;
                mHeight = h;
            }
        }
        NGUIText.Update(false);

    }
    public void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color> cols, int start, int end, float x, float y)
    {
        Color c = mEffectColor;
        if (bitmapFont != null && bitmapFont.premultipliedAlphaShader) c = NGUITools.ApplyPMA(c);
        Color col = c;

        for (int i = start; i < end; ++i)
        {
            //if (verts [i].z == NGUIText.SYMBOLS_Z)
            //	continue;

            verts.Add(verts.buffer[i]);
            uvs.Add(uvs.buffer[i]);
            cols.Add(cols.buffer[i]);

            Vector3 v = verts.buffer[i];
            v.x += x;
            v.y += y;
            verts.buffer[i] = v;

            Color uc = cols.buffer[i];

            if (uc.a == 1f)
            {
                cols.buffer[i] = col;
            }
            else
            {
                Color fc = c;
                fc.a = uc.a * c.a;
                cols.buffer[i] = fc;
            }
        }
    }

    public Vector2 ApplyOffset(BetterList<Vector3> verts, int start)
    {
        Vector2 po = Vector2.zero;

        float fx = Mathf.Lerp(0f, -mWidth, po.x);
        float fy = Mathf.Lerp(mHeight, 0f, po.y) + Mathf.Lerp((mCalculatedSize.y - mHeight), 0f, po.y);

        fx = Mathf.Round(fx);
        fy = Mathf.Round(fy);

        for (int i = start; i < verts.size; ++i)
        {
            verts.buffer[i].x += fx;
            verts.buffer[i].y += fy;
        }
        return new Vector2(fx, fy);
    }

    private void OffsetWidthAndHeight(BetterList<Vector3> list)
    {
        Vector3 offset = Vector3.zero;
        if (alignment == NGUIText.Alignment.Center || alignment == NGUIText.Alignment.Automatic)
        {
            offset.x -= mWidth / 2f;
            offset.y -= mHeight / 2f;
        }
        for (int i = 0; i < list.size; i++)
        {
            list[i] += offset;
        }
    }
    #region 材质缓存

    private static Dictionary<UIFont, Dictionary<int, SharedMaterial>> sharedMaterialCache = new Dictionary<UIFont, Dictionary<int, SharedMaterial>>();

    private class SharedMaterial
    {
        public Material material;

    }

    private static Material GetMaterial(UIFont uiFont, int renderQueue)
    {
        Dictionary<int, SharedMaterial> item;
        SharedMaterial sharedMaterial;
        if (sharedMaterialCache.TryGetValue(uiFont, out item) == false)
        {
            item = new Dictionary<int, SharedMaterial>();
            sharedMaterial = CreateMaterial(uiFont, renderQueue);
            item.Add(renderQueue, sharedMaterial);
            sharedMaterialCache.Add(uiFont, item);
        }
        else
        {
            if (item.TryGetValue(renderQueue, out sharedMaterial) == false)
            {
                sharedMaterial = CreateMaterial(uiFont, renderQueue);
                item.Add(renderQueue, sharedMaterial);
            }
        }
        return sharedMaterial.material;
    }

    private static SharedMaterial CreateMaterial(UIFont uiFont, int renderQueue)
    {
        SharedMaterial sharedMaterial = new SharedMaterial();
        sharedMaterial.material = new Material(uiFont.material);
        string shaderName = sharedMaterial.material.shader.name;
        shaderName = shaderName.Replace("GUI/Text Shader", "Unlit/Text");
        Shader shader = Shader.Find(shaderName);
        sharedMaterial.material.shader = shader;
        sharedMaterial.material.renderQueue = renderQueue;
        return sharedMaterial;
    }
    #endregion
}
