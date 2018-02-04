using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using cn.sharesdk.unity3d;
using AssetPipeline;
using Pathfinding;
using Spine.Unity;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#region C#导出接口特殊处理
[assembly: CsExportedMethod(TargetType = typeof(NGUITools), TargetMethodName = "AddMissingComponent", JsCode = @"
        AddMissingComponent$1: function(T, go) { 
            var t = go.GetComponent$1(T);
            if (t == null){
                t = go.AddComponent$1(T);
            }
            return t; //Ret: T
        },")]
#endregion
public static class JSBCodeGenSettings
{
    /// <summary>
    /// 以下文件或者目录不参与编译转换为js脚本
    /// </summary>
    public static string[] PathsNotToJavaScript =
    {
        "GameResources/",
        "JSBinding/",
        "MyTemp/",
        "NGUI/",
        "Assets/Plugins/",
        "Assets/Preview/",
        "Assets/SkillEditor/",
        "Resources/",
        "Standard Assets/",
        "StreamingAssets/",
        "Assets/UI/",
        "WebPlayerTemplates/",
        "Scripts/MyGameScripts/Module/_ModuleName_Module",
        "Scripts/MyGameScripts/Proxy/Proxy_ModuleName_Module.cs",
        "Scripts/GameProtocol/app-clientservice/AppProtobuf",
        "Scripts/MyTestScripts/",
//        "Scripts/GameProtocol/",
//        "Scripts/MyGameScripts/",
    };

    /// <summary>
    /// 参与编译转换js脚本白名单
    /// </summary>
    public static string[] PathsToJavaScript =
    {
        "Scripts/GameProtocol/app-clientservice/AppProtobuf/ProtobufMap.cs",
    };

    /// <summary>
    ///     导出指定Assembly内的指定命名空间下的类型，若没有指定命名空间列表则导出这个dll的所有接口
    ///     导出dll内的根命名空间用null来表示
    /// </summary>
    public static readonly Dictionary<string, List<string>> CustomAssemblyConfig = new Dictionary<string, List<string>>
    {
        {"UnityEngine", new List<string> {"UnityEngine"}},
        //{"UnityEngine.UI", null},
        {"DOTween", new List<string> {"DG.Tweening","DG.Tweening.Core"}},
        {"Assembly-CSharp-firstpass", new List<string> {"AssetPipeline"}}
    };

    /// <summary>
    ///     导出指定类型
    /// </summary>
    public static readonly List<Type> CustomTypeConfig = new List<Type>
    {
        //JSBExportTest
        //typeof (TestExtensionMethod),
        //typeof (PerTest),
        //typeof (PerTest.RefObject),
        //JSBExportTest
        
        //mscorlib
        typeof (IEnumerator),
        typeof (ICollection),
        typeof (IDisposable),
        typeof (IConvertible),
        typeof (IList),
        typeof (IDictionary),
        typeof (List<>),
        typeof (Dictionary<,>),
        typeof (KeyValuePair<,>),
        typeof (Dictionary<,>.KeyCollection),
        typeof (Dictionary<,>.ValueCollection),
        typeof (HashSet<>),
        typeof (Hashtable),

        typeof (WeakReference),
        typeof (Stopwatch),
        //typeof (StringBuilder),
        //typeof (TimeSpan),
        //typeof (DateTime),
        typeof (Math),
        typeof (System.Random),

        typeof (Regex),
        typeof (Capture),
        typeof (Group),
        typeof (GroupCollection),
        typeof (Match),

        typeof (Convert),
        typeof (Encoding),

        typeof (File),
        typeof (FileInfo),
        typeof (Directory),
        typeof (DirectoryInfo),
        typeof (FileStream),
        typeof (Stream),
        //typeof (Path),

        typeof (ICloneable),
        typeof (IEnumerable),
        typeof (System.Runtime.Serialization.IDeserializationCallback),
        typeof (System.Runtime.Serialization.ISerializable),
        typeof (System.Runtime.InteropServices._Exception),

        //typeof (XmlNode),
        //typeof (System.Runtime.Serialization.ISurrogateSelector),
        //typeof (IXPathNavigable),
        //typeof (XmlDocument),
        //typeof (XmlNodeList),
        //typeof (XmlElement),
        //typeof (XmlLinkedNode),
        //typeof (XmlAttributeCollection),
        //typeof (XmlNamedNodeMap),
        //typeof (XmlAttribute),
        //mscorlib

        //UnityEngine

        //UnityEngine

        //NGUI
        //typeof(LanguageSelection),
        //typeof(TypewriterEffect),
        typeof (UIButton),
        typeof (UIButtonActivate),
        typeof (UIButtonColor),
        typeof (UIButtonMessage),
        typeof (UIButtonOffset),
        typeof (UIButtonRotation),
        typeof (UIButtonScale),
        typeof (UICenterOnChild),
        typeof (UICenterOnClick),
        typeof (UIDragCamera),
        typeof (UIDragDropContainer),
        typeof (UIDragDropItem),
        typeof (UIDragDropRoot),
        typeof (UIDraggableCamera),
        typeof (UIDragObject),
        //typeof(UIDragResize),
        typeof (UIDragScrollView),
        typeof (UIEventTrigger),
        //typeof(UIForwardEvents),
        typeof (UIGrid),
        //typeof(UIImageButton),
        //typeof(UIKeyBinding),
        //typeof(UIKeyNavigation),
        typeof (UIPlayAnimation),
        typeof (UIPlaySound),
        typeof (UIPlayTween),
        typeof (UIPopupList),
        typeof (UIProgressBar),
        //typeof(UISavedOption),
        typeof (UIScrollBar),
        typeof (UIScrollView),
        typeof (UISlider),
        typeof (UISoundVolume),
        typeof (UITable),
        typeof (UIToggle),
        //typeof(UIToggledComponents),
        //typeof(UIToggledObjects),
        typeof (UIWidgetContainer),
        typeof (UIWrapContent),
        typeof (ActiveAnimation),
        //typeof(AnimationOrTween<>),
        typeof (BetterList<>),
        typeof (BMFont),
        typeof (BMGlyph),
        typeof (BMSymbol),
        //typeof(ByteReader),
        typeof (EventDelegate),
        //typeof(Localization),
        typeof (NGUIDebug),
        typeof (NGUIMath),
        typeof (NGUIText),
        typeof (NGUITools),
        //typeof(PropertyBinding),
        //typeof(PropertyReference),
        typeof (RealTime),
        typeof (SpringPanel),
        typeof (UIBasicSprite),
        typeof (UIDrawCall), //modify by senkay at 2015-08-13
        typeof (UIEventListener),
        typeof (UIGeometry),
        typeof (UIRect),
        //typeof(UISnapshotPoint),
        typeof (UIWidget),
        //typeof(AnimatedAlpha),
        //typeof(AnimatedColor),
        //typeof(AnimatedWidget),
        typeof (SpringPosition),
        typeof (TweenAlpha),
        typeof (TweenColor),
        typeof (TweenFOV),
        typeof (TweenHeight),
        typeof (TweenOrthoSize),
        typeof (TweenPosition),
        typeof (TweenRotation),
        typeof (TweenScale),
        typeof (TweenTransform),
        typeof (TweenVolume),
        typeof (TweenWidth),
        typeof (UITweener),
        typeof (UI2DSprite),
        typeof (UI2DSpriteAnimation),
        typeof (UIAnchor),
        typeof (UIAtlas),
        typeof (UICamera),
        typeof (UICamera.MouseOrTouch),
        typeof (UIFont),
        typeof (UIInput), //modify by senkay at 2015-08-13
        //typeof(UIInputOnGUI),
        typeof (UILabel),
        //typeof(UILocalize),
        //typeof(UIOrthoCamera),
        typeof (UIPanel),
        typeof (UIRoot),
        typeof (UISprite),
        typeof (UISpriteAnimation),
        typeof (UISpriteData),
        //typeof(UIStretch),
        typeof (UITextList),
        typeof (UITexture),
        //typeof(UITooltip),
        typeof (UIViewport),
        typeof (UIRect.AnchorPoint),
        typeof (UIPageGrid),
        typeof (PageScrollView),
        typeof (UILabelHUD),
//        typeof (ScrollNum),
//        typeof (ScrollNum.DisplayConfig),
        //NGUI
        
        //GameFramework
        typeof(CSGameDebuger),
		//PigeonCoopToolkit
		typeof(FPSWeaponTrigger),
		typeof(AlwaysForward),
		typeof(TankController),
		typeof(MouseFollower),
		typeof(ConstForce),
		typeof(TankProjectile),
		typeof(Orbiter),
		typeof(DestroyAfterTime),
		typeof(TankTranslator),
		typeof(PitchShifter),
		typeof(FPSController),
		typeof(TankWeaponController),
		typeof(TankAlwaysForward),
		typeof(PigeonCoopToolkit.Effects.Trails.TrailRenderer_Base),
		typeof(PigeonCoopToolkit.Utillities.GizmosExtra),
		//PathologicalGames
		typeof(PathologicalGames.SpawnPool),
		typeof(PathologicalGames.PoolManager),
		typeof(PathologicalGames.PreRuntimePoolItem),
		typeof(PathologicalGames.InstanceHandler),
		typeof(PathologicalGames.SpawnPoolsDict),
		typeof(PathologicalGames.PrefabPool),
		typeof(PathologicalGames.PrefabsDict),
		//
        typeof (ARC4),
        typeof (HaStage),
        typeof (IConigurable),
        typeof (ServiceInfo),
        typeof (EventObject),
        typeof (DirectMessageInstruction),
        typeof (DirectMessageReceivedInstruction),
        typeof (GroupMessageReceivedInstruction),
        //typeof(InstructionDefine),
        typeof (JoinAckInstruction),
        typeof (JoinInstruction),
        typeof (KeepaliveAckInstruction),
        typeof (KeepaliveInstruction),
        typeof (LeaveEventInstruction),
        typeof (LeaveInstruction),
        typeof (Marshalable),
        typeof (MarshalableObject),
        typeof (ServiceQueryRequestInstruction),
        typeof (ServiceQueryResponseInstruction),
        typeof (SSLOpenRequestInstruction),
        typeof (SSLOpenResponseInstruction),
        typeof (StateEventNotifyInstruction),
        typeof (VerAckInstruction),
        typeof (VerInstruction),
        typeof (BroadcastMessageHeader),
        typeof (DirectMessageHeader),
        typeof (GroupMessageHeader),
        typeof (Packetable),
        typeof (PacketHeader),
        typeof (HaApplicationContext),
        typeof (HaConfiguration),
        typeof (HaConfigurationImpl),
        typeof (HaConnector),

        typeof (ProtoByteArray),
        typeof (ByteArray),

        typeof (HttpController),
        typeof (SimpleWWW),
        typeof (Request),
        typeof (ShortenUrl),

        //自定义Helper类
        typeof (MD5Hashing),
        typeof (CRC32Hashing),
        typeof (VectorHelper),
        typeof(TextureHelper),

        typeof (CSTimer),
        typeof (CSTimer.Task),
        typeof (CSTimer.CdTask),
        typeof (CSTimer.TimerTask),

        typeof (GameSetting),
        typeof (FrameworkVersion),
        typeof (GameLauncher),
        typeof (GameStopwatch),
        typeof (GameStaticConfigManager),
        typeof (CGPlayer),
        typeof (UpdateManager),
		//GameEvent legacy 放到业务层了
//		typeof (GameEventCenter),
//		typeof (GameEvents),
//		typeof (GameEvents.Event),
//		typeof (GameEvents.Event<>),
//		typeof (GameEvents.Event<,>),
//		typeof (GameEvents.Event<,,>),
//		typeof (GameEvents.Event<,,,>),
//		typeof (GameEventAgent),
//		typeof (GameEventAgent<>),
//		typeof (GameEventAgent<,>),
//		typeof (GameEventAgent<,,>),
//		typeof (GameEventAgent<,,,>),

        //自定义Component
        typeof (RendererFlicker),
        typeof (AutoRotation),
        typeof (NoRotation),
        typeof (EffectTime),
        typeof (ParticleScaler),
        typeof (CameraShake),
        typeof (SceneCameraController),
        typeof (ParticleRotationSync),
        typeof (ModelVisibleChecker),
        typeof (ModelHSV),
        //自定义Component

        //自定义UI组件
        typeof (UIFollowTarget),
        typeof (HUDText),
        typeof (BuiltInDialogueViewController),
        typeof (CostButton),
        typeof (SliderToggle),
        typeof (UIRecycledList),
        typeof (ButtonLabelSpacingAdjust),
        typeof (UIPageInfo),
        typeof (UIPageGroup),
        typeof (FloatTipText),
        typeof (UIEffectRenderQueueSync),
        typeof (NGUIJoystick),
//        typeof (LogoAutoLocation),
        //自定义UI组件

        typeof (ResGroup),
//        typeof (FileHelper),
        typeof (ZipLibUtils),
//        typeof (ZipLibUtils.ZipFileCreateHelper),
//        typeof (ZipLibUtils.ZipFileUpdateHelper),
        //GameFramework

        #region 第三方SDK，插件等
        // ShareSDK
        typeof (ShareSDKManager),
        typeof (ShareContent),
        typeof (ContentType),
		// UniWebView
		typeof (UniWebView),
        typeof (UniWebViewEdgeInsets),
        typeof (UniWebViewHelper),
        typeof (UniWebViewMessage),
		// 相册
		typeof(PhotoReaderManager),
        // C# 调用 其它平台 的一些通用接口
        typeof (IosBridge),
        typeof (BaoyugameSdk),
        typeof (iOSUtility),
        // app内购
        typeof(IOSInAppPurchaseManager),
        typeof(ISN_Result),
        typeof(ISN_Error),
        typeof(IOSProductTemplate),
        typeof(IOSStoreKitResult),
        typeof(IOSStoreKitRestoreResult),
        // Crasheye
//        typeof (Crasheye),
        //TalkingDataHelper
        typeof (TalkingDataHelper),
        typeof (BehaviorHelper),
        // AntaresQRCodeUtil
        typeof (AntaresQRCodeUtil),
        typeof (Antares.QRCode.Result),
        //SPSDK
        typeof (SPSDK),
        typeof(SdkMessageScript),
        //XinGe
        typeof (XinGeSdk),
        //CameraPath3
        typeof (CameraPathAnimator),
        typeof (CameraPath),
        //Voice
        typeof (BaiduVoiceRecognition),
        typeof (QiNiuFileExt),
        typeof (VoiceFinalResult),
        typeof (VoiceSaveResult),
        typeof (VoiceEndResult),
        typeof (VoiceSaveHelper),
        typeof (VoiceLoadHelper),
        typeof (VoiceHelper),
        typeof (Qiniu.RS.GetPolicy),
        typeof (Qiniu.Auth.digest.Mac),
        typeof (Qiniu.Conf.Config),
		// Thread
		typeof(ThreadManager),
        typeof(ThreadTask),
        typeof(QiNiuSaveFileThreadTask),
		// WebCamTexture
		typeof(WebCamTextureHelper),
		// 原生 Util
		typeof(SystemProcess),
        // tk2d
        typeof(tk2dCamera),
        typeof(tk2dSprite),
        typeof(tk2dSpriteCollectionData),
        // A*
        typeof(NNConstraint),
        typeof(AstarPath),
        typeof(AstarData),
        typeof(GridGraph),
		typeof(NNInfo),
		typeof(GraphNode),
		typeof(Pathfinding.GridNode),
		typeof(Pathfinding.Int3),
		typeof(Pathfinding.Int2),
		// GridWrapContent
		typeof(GridWrapContent),
		typeof(GridWrapContent.CellsConfig),
		//other
		typeof(UIAtlasRef),
        typeof(GridMapAgent),
        typeof(CameraMove2d),
        typeof(EditorRuntimeHelper),
        typeof(JSBindingFixHelper),
		//unirxa
		typeof (UniRx.ISubject<>),
		typeof (UniRx.ISubject<,>),
		typeof (UniRx.Subject<>),
		typeof (UniRx.IObserver<>),
		typeof (UniRx.IObservable<>),
		typeof (UniRx.IObservableExpand<>),
		typeof (UniRx.IOptimizedObservable<>),
		typeof (UniRx.SubjectExtensions),
		typeof (UniRx.ObservableExtensions),
        //Spine
        typeof(SkeletonAnimation),
        #endregion
    };

    // if uselist return a white list, don't check noUseList(black list) again
    /// <summary>
    ///     类型白名单，在白名单内的类型一定会导出，忽略所有过滤条件
    /// </summary>
    public static readonly HashSet<Type> TypeWhiteSet = new HashSet<Type>();

    /// <summary>
    ///     类型黑名单，忽略导出一下类型
    /// </summary>
    public static readonly List<string> TypeBlackSet = new List<string>
    {
        "Canvas",
        "CanvasRenderer",
        "RectTransform",
        "HideInInspector",
        "ExecuteInEditMode",
        "AddComponentMenu",
        "ContextMenu",
        "RequireComponent",
        "DisallowMultipleComponent",
        "SerializeField",
        "AssemblyIsEditorAssembly",
        "Attribute",
        "Types",
        "UnitySurrogateSelector",
        //"TrackedReference",
        "TypeInferenceRules",
        "FFTWindow",
        "RPC",
        "Network",
        "MasterServer",
        "BitStream",
        "HostData",
        "ConnectionTesterStatus",
        //"GUI",
        "EventType",
        "EventModifiers",
        "FontStyle",
        "TextAlignment",
        "TextEditor",
        "TextEditorDblClickSnapping",
        "TextGenerator",
        "TextClipping",
        "Gizmos",
        "ADBannerView",
        "ADInterstitialAd",
        "Android",
        "Tizen",
        "jvalue",
        "iPhone",
        "iOS",
        "CalendarIdentifier",
        "CalendarUnit",
        "CalendarUnit",
        "ClusterInput",
        "FullScreenMovieControlMode",
        "FullScreenMovieScalingMode",
        "Handheld",
        "LocalNotification",
        "NotificationServices",
        "RemoteNotificationType",
        "RemoteNotification",
        "SamsungTV",
        "TextureCompressionQuality",
        "TouchScreenKeyboardType",
        "TouchScreenKeyboard",
        "MovieTexture",
        "UnityEngineInternal",
        "Terrain",
        "Tree",
        "SplatPrototype",
        "DetailPrototype",
        "DetailRenderMode",
        "MeshSubsetCombineUtility",
        "AOT",
        "Social",
        "SendMouseEvents",
        "Cursor",
        "Flash",
        "ActionScript",
        "OnRequestRebuild",
        "Ping",
        "ShaderVariantCollection",
        "SimpleJson.Reflection",
        "CoroutineTween",
        "GraphicRebuildTracker",
        "Advertisements",
        "UnityEditor",
        "WSA",
        "EventProvider",
        "Apple",
        "ClusterInput"
    };

    /// <summary>
    ///     过滤指定类型的成员信息，大多数是移动平台不支持的接口
    /// </summary>
    public static readonly Dictionary<Type, HashSet<string>> TypeMemberFilterConfig = new Dictionary<Type, HashSet<string>>
    {
        {typeof (NGUITools), new HashSet<string> {"Draw"}},
        {typeof (StreamReader), new HashSet<string> {"CreateObjRef", "GetLifetimeService", "InitializeLifetimeService"}},
        {typeof (StreamWriter), new HashSet<string> {"CreateObjRef", "GetLifetimeService", "InitializeLifetimeService"}},
        {typeof (WWW), new HashSet<string> {"movie"}},
        {
            typeof (AnimationClip),
            new HashSet<string>
            {
                "averageDuration",
                "averageAngularSpeed",
                "averageSpeed",
                "apparentSpeed",
                "isLooping",
                "isAnimatorMotion",
                "isHumanMotion"
            }
        },
        {typeof (AnimatorOverrideController), new HashSet<string> {"PerformOverrideClipListCleanup"}},
        {typeof (Caching), new HashSet<string> {"SetNoBackupFlag", "ResetNoBackupFlag"}},
        {typeof (Light), new HashSet<string> {"areaSize"}},
        {typeof (Security), new HashSet<string> {"GetChainOfTrustValue"}},
        {typeof (Texture2D), new HashSet<string> {"alphaIsTransparency"}},
#if !UNITY_IPHONE
		{typeof (WebCamTexture), new HashSet<string> {"MarkNonReadable", "isReadable"}},
#endif
        {typeof (Application), new HashSet<string> {"ExternalEval"}},
        {typeof (GameObject), new HashSet<string> {"networkView"}},
        {typeof (Component), new HashSet<string> {"networkView"}},
        // unity5
        {typeof (UnityEngine.AnimatorControllerParameter), new HashSet<string> {"name"}},
        //{typeof (Resources), new HashSet<string> {"LoadAssetAtPath"}},
        {typeof (Input), new HashSet<string> {"IsJoystickPreconfigured"}},
#if UNITY_4_6 || UNITY_4_7
        {typeof (PointerEventData), new HashSet<string> {"lastPress"}},
        {typeof (InputField), new HashSet<string> {"onValidateInput"}},
        {typeof (Graphic), new HashSet<string> {"OnRebuildRequested"}},
        {typeof (Text), new HashSet<string> {"OnRebuildRequested"}},
        {
            typeof (Motion),
            new HashSet<string>
            {
                "ValidateIfRetargetable",
                "averageDuration",
                "averageAngularSpeed",
                "averageSpeed",
                "apparentSpeed",
                "isLooping",
                "isAnimatorMotion",
                "isHumanMotion"
            }
        },
#endif
        {typeof(CameraPath), new HashSet<string>
        {
                "ToXML",
                "FromXML",
            }
        },
        {typeof(CameraPathAnimator), new HashSet<string>
        {
                "ToXML",
                "FromXML",
            }
        },
        {typeof(AssetManager), new HashSet<string>
        {
                "EditorLoadDelay",
                "AbInfoDic",
            }
        },
        {typeof(UIDrawCall), new HashSet<string>
        {
                "isActive",
            }
        },
        {typeof(UIWidget), new HashSet<string>
        {
                "showHandlesWithMoveTool",
                "showHandles",
            }
        },
        {typeof(UIInput), new HashSet<string>
        {
                "ProcessEvent",
            }
        },
        {typeof(FileHelper), new HashSet<string>
        {
                "ReadFileAsyncByCoroutine",
            }
        },
        {typeof(GUIStyleState), new HashSet<string>
            {
                "scaledBackgrounds",
            }
        },
        {typeof(tk2dBaseSprite), new HashSet<string>
            {
                "EditMode__CreateCollider",
            }
        },
        {typeof(tk2dCamera), new HashSet<string>
            {
                "Editor__Inst",
                "Editor__gameViewReflectionError",
                "Editor__GetNativeProjectionMatrix",
                "Editor__GetFinalProjectionMatrix",
                "Editor__GetGameViewSize",
            }
        },
        {typeof(ComputeShader), new HashSet<string>
            {
                "SetTextureFromGlobal",
            }
        },
    };

    public static bool IsDiscardMemberInfo(Type type, MemberInfo memberInfo)
    {
        if (typeof(
            Delegate).IsAssignableFrom(type))
        {
            return true;
        }

        if (TypeMemberFilterConfig.ContainsKey(type))
        {
            var filterList = TypeMemberFilterConfig[type];
            if (filterList.Contains(memberInfo.Name))
                return true;
        }

        return false;
    }

    public static bool IsSupportByDotNet2SubSet(string functionName)
    {
        if (functionName == "Directory_CreateDirectory__String__DirectorySecurity" ||
            functionName == "Directory_GetAccessControl__String__AccessControlSections" ||
            functionName == "Directory_GetAccessControl__String" ||
            functionName == "Directory_SetAccessControl__String__DirectorySecurity" ||
            functionName == "DirectoryInfo_Create__DirectorySecurity" ||
            functionName == "DirectoryInfo_CreateSubdirectory__String__DirectorySecurity" ||
            functionName == "DirectoryInfo_GetAccessControl__AccessControlSections" ||
            functionName == "DirectoryInfo_GetAccessControl" ||
            functionName == "DirectoryInfo_SetAccessControl__DirectorySecurity" ||
            functionName == "File_Create__String__Int32__FileOptions__FileSecurity" ||
            functionName == "File_Create__String__Int32__FileOptions" ||
            functionName == "File_GetAccessControl__String__AccessControlSections" ||
            functionName == "File_GetAccessControl__String" ||
            functionName == "File_SetAccessControl__String__FileSecurity" ||
            functionName == "FileInfo_GetAccessControl__AccessControlSections" ||
            functionName == "FileInfo_GetAccessControl" ||
            functionName == "FileInfo_SetAccessControl__FileSecurity")
        {
            return false;
        }
        return true;
    }

    public static bool NeedGenDefaultConstructor(Type type)
    {
        if (typeof(Delegate).IsAssignableFrom(type))
            return false;

        if (type.IsInterface)
            return false;

        // don't add default constructor
        // if it has non-public constructors
        // (also check parameter count is 0?)
        if (type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Length != 0)
            return false;

        //foreach (var c in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance))
        //{
        //    if (c.GetParameters().Length == 0)
        //        return false;
        //}

        if (type.IsClass && (type.IsAbstract || type.IsInterface))
            return false;

        if (type.IsClass)
        {
            return type.GetConstructors().Length == 0;
        }
        foreach (var c in type.GetConstructors())
        {
            if (c.GetParameters().Length == 0)
                return false;
        }
        return true;
    }
}