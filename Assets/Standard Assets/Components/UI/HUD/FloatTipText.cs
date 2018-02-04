using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class FloatTipText : MonoBehaviour
{

    protected class Entry
    {
        public Transform root;
        public float time;          // Timestamp of when this entry was added
        public float stay = 0f;     // How long the text will appear to stay stationary on the screen
        public float offset = 0f;   // How far the object has moved based on time
        public UILabel label;       // Label on the game object
        public UISprite sprite;
        public UISprite itemSprite;
        public string prefabName;

        public float movementStart { get { return time + stay; } }
    }

    /// <summary>
    /// Sorting comparison function.
    /// </summary>

    static int Comparison(Entry a, Entry b)
    {
        if (a.movementStart < b.movementStart) return -1;
        if (a.movementStart > b.movementStart) return 1;
        return 0;
    }

    //	public Vector2 defaultSize;

    public List<GameObject> prefab = new List<GameObject>();
    /// <summary>
    /// Curve used to move entries with time.
    /// </summary>

    public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 80f) });

    /// <summary>
    /// Curve used to fade out entries with time.
    /// </summary>

    public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(1f, 0f) });

    /// <summary>
    /// Curve used to scale the entries.
    /// </summary>

    //	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.25f, 1f) });

    Queue<Entry> mList = new Queue<Entry>();
    Queue<Entry> mUnused = new Queue<Entry>();
    float counter = 0;
    private Dictionary<string, Queue<Entry>> testDic = new Dictionary<string, Queue<Entry>>();

    /// <summary>
    /// Whether some HUD text is visible.
    /// </summary>

    public bool isVisible { get { return mList.Count != 0; } }

    /// <summary>
    /// Create a new entry, reusing an old entry if necessary.
    /// </summary>
    Entry Create(string prefabName = "")
    {
        //test
        if (string.IsNullOrEmpty(prefabName)) return null;
        if (testDic.ContainsKey(prefabName))
        {
            var que = testDic[prefabName];
            if(que.Count > 0)
            {
                Entry ent = que.Dequeue();
                ent.time = Time.realtimeSinceStartup;
                //			if( ent.label != null ) ent.label.depth = NGUITools.CalculateNextDepth(gameObject);
                //			ent.sprite.depth = NGUITools.CalculateNextDepth(gameObject);
                //			NGUITools.SetActive(ent.root.gameObject, true);
                ent.offset = 0f;
                mList.Enqueue(ent);
                return ent;
            }
        }

        // See if an unused entry can be reused
        //if (mUnused.Count > 0)
        //{
        //    Entry ent = mUnused.Dequeue();
        //    ent.time = Time.realtimeSinceStartup;
        //    //			if( ent.label != null ) ent.label.depth = NGUITools.CalculateNextDepth(gameObject);
        //    //			ent.sprite.depth = NGUITools.CalculateNextDepth(gameObject);
        //    //			NGUITools.SetActive(ent.root.gameObject, true);
        //    ent.offset = 0f;
        //    mList.Enqueue(ent);
        //    return ent;
        //}
        
        // New entry
        Entry ne = new Entry();
        ne.time = Time.realtimeSinceStartup;
        prefab.ForEach(e =>
        {
            if (e != null && e.name == prefabName)
            {
                ne.root = NGUITools.AddChild(gameObject, e).transform;
            }
        });
        if (ne.root == null) return null;
        ne.root.name = string.Format("_tip_" + prefabName + "_{0}", counter.ToString());
        ne.prefabName = prefabName;
        //ne.label = ne.root.GetComponentInChildren<UILabel>();
        //ne.sprite = ne.root.GetComponentInChildren<UISprite>();
        ne.label = ne.root.Find("Label").GetComponent<UILabel>();
        ne.sprite = ne.root.Find("Sprite").GetComponent<UISprite>();
        ne.itemSprite = ne.root.Find("ItemSprite").GetComponent<UISprite>();

        // Make it small so that it's invisible to start with
        NGUITools.SetActive(ne.root.gameObject, false);
        mList.Enqueue(ne);
        ++counter;
        return ne;
    }

    void Delete(Entry ent)
    {
        TweenPosition comp = ent.root.gameObject.GetComponent<TweenPosition>();
        if (comp != null)
        {
            comp.to.y = 0;
            GameObject.Destroy(comp);
        }

        mList.Dequeue();
       // mUnused.Enqueue(ent);
        if (!testDic.ContainsKey(ent.prefabName))
        {
            testDic.Add(ent.prefabName, new Queue<Entry>());
        }
        testDic[ent.prefabName].Enqueue(ent);
        NGUITools.SetActive(ent.root.gameObject, false);
    }

    /// <summary>
    /// Add a new scrolling text entry.
    /// </summary>
    public void Add(string txt,UIAtlas atlas, string icon, Color c, float stayDuration,string prefabName = "")
    {
        if (!enabled) return;
        if (prefab.Count == 0) return;
        bool changeRaw = false;
        int count = 0;

        if (txt.Contains("\n"))
        {
            changeRaw = true;
        }
        else
        {
            for (int i = 0; i < txt.Length; i++)
            {
                string sTemp = txt.Substring(i, 1);
                byte[] byte_len = System.Text.Encoding.UTF8.GetBytes(sTemp);
                if (byte_len.Length > 1)
                {
                    count++;
                }
                if (count > 20)
                {
                    txt = txt.Insert(i, "\n");
                    count = 0;
                    changeRaw = true;
                }
            }
        }

        //		txt = "[b]" + txt;

        // Create a new entry
        Entry ne = Create(prefabName);
        //		int factor = mList.Count > 5 ? 5:mList.Count;
        if (ne == null) return;
        ne.stay = stayDuration;// * factor;

        if (ne.label != null)
        {
            ne.label.color = c;
            ne.label.text = txt;
            ne.label.alignment = changeRaw ? NGUIText.Alignment.Left : NGUIText.Alignment.Center;
        }

        if (ne.itemSprite != null)
        {
            ne.itemSprite.atlas = atlas;
            if (!string.IsNullOrEmpty(icon))
            {
                ne.itemSprite.spriteName = icon;
                ne.sprite.leftAnchor.absolute = -92;
                ne.root.transform.localPosition = new Vector3(21, ne.root.transform.localPosition.y, ne.root.transform.localPosition.z);
            }
            else
            {
                ne.itemSprite.spriteName = "";
                ne.sprite.leftAnchor.absolute = -50;
                ne.root.transform.localPosition = new Vector3(0, ne.root.transform.localPosition.y, ne.root.transform.localPosition.z);
            }
        }

        //		AdaptToContent(ne);

        // Sort the list
        //mList.Sort(Comparison);
    }

    /// <summary>
    /// Disable all labels when this script gets disabled.
    /// </summary>
    void OnDisable()
    {
        //		for (int i = mList.Count; i > 0; )
        //		{
        //			Entry ent = mList[--i];
        //			if (ent.root !=null)
        //			{
        //				NGUITools.SetActive(ent.root.gameObject,false);
        //			}
        //			else 
        //				mList.RemoveAt(i);
        //		}
    }

    public void Clean()
    {
        //		for (int i = mList.Count; i > 0; )
        //		{
        //			Entry ent = mList[--i];
        //			if (ent.root !=null)
        //			{
        //				NGUITools.SetActive(ent.root.gameObject,false);
        //				if( ent.label != null ) ent.label.text = "";
        //			}
        //			else 
        //				mList.RemoveAt(i);
        //		}		
    }

    /// <summary>
    /// Update the position of all labels, as well as update their size and alpha.
    /// </summary>

    void Update()
    {
        if (mList.Count == 0)
        {
            return;
        }

        float time = Time.realtimeSinceStartup;

        Keyframe[] offsets = offsetCurve.keys;
        Keyframe[] alphas = alphaCurve.keys;

        float offsetEnd = offsets[offsets.Length - 1].time;
        float alphaEnd = alphas[alphas.Length - 1].time;
        float totalEnd = Mathf.Max(offsetEnd, alphaEnd);

        float offset = 0f;

        Entry[] list = mList.ToArray();
        for (int i = 0, len = list.Length; i < len; i++)
        {
            offset = 0;

            Entry ent = list[i];

            //	Scale End TODO
            float currentTime = time - ent.movementStart;
            ent.offset = offsetCurve.Evaluate(currentTime) + (len - i - 1) * 35;
            //			float curAlpha = alphaCurve.Evaluate(currentTime);
            //			if( ent.label != null ) ent.label.alpha = curAlpha;
            //			ent.sprite.alpha = curAlpha;

            // Delete the entry when needed
            if (currentTime > totalEnd)
            {
                Delete(ent);
                continue;
            }
            else
            {
                NGUITools.SetActive(ent.root.gameObject, true);
            }

            //	move
            offset = Mathf.Max(offset, ent.offset);

            if (i < len - 1)
            {
                float lastY = 35f + (len - i - 1) * 35f;
                ent.root.DOLocalMoveY(lastY, 0.2f).SetDelay(0.05f);
            }
            else
            {
                if (string.IsNullOrEmpty(ent.itemSprite.spriteName))
                    ent.root.localPosition = new Vector3(0f, offset, -1 * i);
                else
                    ent.root.localPosition = new Vector3(21f, offset, -1 * i);
            }
        }

        /*// Adjust alpha and delete old entries
		for (int i = mList.Count; i > 0; )
		{
			Entry ent = mList[--i];
			float currentTime = time - ent.movementStart;
			ent.offset = offsetCurve.Evaluate(currentTime);
			float curAlpha = alphaCurve.Evaluate(currentTime);
			if( ent.label != null ) ent.label.alpha = curAlpha;
			ent.sprite.alpha = curAlpha;

			// Delete the entry when needed
			if (currentTime > totalEnd) Delete(ent);
			else NGUITools.SetActive(ent.root.gameObject,true);
		}

		float offset = 0f;

		// Move the entries
		for (int i = mList.Count; i > 0; )
		{
			Entry ent = mList[--i];
			offset = Mathf.Max(offset, ent.offset);
			ent.root.localPosition = new Vector3(0f, offset, 0f);
			//offset += Mathf.Round(ent.sprite.cachedTransform.localScale.y)+5f;
		}*/
    }
}
