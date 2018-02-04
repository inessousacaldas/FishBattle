using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

namespace AssetPipeline
{
    //zip proxy
    public class ZipProxy
    {
#if UNITY_EDITOR
        public static bool LogMessage = true;
#else
		public static bool LogMessage = false;
#endif
        public static System.Text.StringBuilder LogBuilder = new System.Text.StringBuilder();
        //static member
        private static readonly List<ZipProxy> AllProxyList = new List<ZipProxy>();
        private string mZipFile;
        private Stream mInputStream;
#if UNITY_IPHONE
        private readonly List<string> mOutFileList = new List<string>();
#endif

        public string ZipFile
        {
            get
            {
                return mZipFile;
            }
        }

        private string mOutFolder;
        private bool mIsDone;

        public bool IsDone
        {
            get
            {
                return mIsDone;
            }
        }

        private bool mIsError;

        public bool IsError
        {
            get
            {
                return mIsError;
            }
        }

        private long mTotalCount;
        private long mCompletedCount;

        public float Progress
        {
            get
            {
                if (mTotalCount == 0)
                    return 0f;
                return mCompletedCount * 1f / mTotalCount;
            }
        }

        private System.Action<ZipProxy> mFinishCallback;
        private System.Action<System.Exception> mErrorCallback;
        private System.Exception mException;

        public static ZipProxy Extract(string zipFile, string outFolder,
                                        System.Action<ZipProxy> finishCallback, System.Action<System.Exception> errorCallback)
        {
            ZipProxy proxy = new ZipProxy();
            proxy.mZipFile = zipFile;
            proxy.mOutFolder = outFolder;
            proxy.mIsDone = false;
            proxy.mIsError = false;
            proxy.mFinishCallback = finishCallback;
            proxy.mErrorCallback = errorCallback;
            proxy.mException = null;
            proxy.mTotalCount = 0;
            proxy.mCompletedCount = 0;

            //加锁加入全局列表中
            lock (((ICollection)AllProxyList).SyncRoot)
            {
                AllProxyList.Add(proxy);
            }

            ThreadPool.QueueUserWorkItem(ExtractProc, proxy);

            return proxy;
        }

        public static ZipProxy Extract(string zipFile, string outFolder, Stream inputStream,
                                        System.Action<ZipProxy> finishCallback, System.Action<System.Exception> errorCallback)
        {
            ZipProxy proxy = new ZipProxy();
            proxy.mZipFile = zipFile;
            proxy.mOutFolder = outFolder;
            proxy.mInputStream = inputStream;
            proxy.mIsDone = false;
            proxy.mIsError = false;
            proxy.mFinishCallback = finishCallback;
            proxy.mErrorCallback = errorCallback;
            proxy.mException = null;
            proxy.mTotalCount = 0;
            proxy.mCompletedCount = 0;


            //加锁加入全局列表中
            lock (((ICollection)AllProxyList).SyncRoot)
            {
                AllProxyList.Add(proxy);
            }

            ThreadPool.QueueUserWorkItem(ExtractProc, proxy);

            return proxy;
        }

        private static void ExtractProc(object state)
        {
            //			Thread.Sleep (5000);	//休眠当前解压线程便于测试观察效果
            System.Exception exception = null;
            ZipProxy zipProxy = (ZipProxy)state;

            ZipFile zf = null;
            try
            {
                if (zipProxy.mInputStream != null)
                {
                    zf = new ZipFile(zipProxy.mInputStream);
                }
                else
                {
                    FileStream fs = File.OpenRead(zipProxy.mZipFile);
                    zf = new ZipFile(fs);
                }

                zipProxy.mTotalCount = zf.Count;
                zipProxy.mCompletedCount = 0;
                foreach (ZipEntry zipEntry in zf)
                {
                    try
                    {
                        if (zipEntry.IsFile)
                        {
                            string entryFileName = zipEntry.Name;

                            byte[] buffer = new byte[4096];     // 4K is optimum
                            Stream zipStream = zf.GetInputStream(zipEntry);

                            string outputPath = Path.Combine(zipProxy.mOutFolder, entryFileName);
                            string dir = Path.GetDirectoryName(outputPath);
                            if (!string.IsNullOrEmpty(dir))
                                Directory.CreateDirectory(dir);

                            // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                            // of the file, but does not waste memory.
                            // The "using" will close the stream even if an exception occurs.
                            using (FileStream streamWriter = File.Create(outputPath))
                            {
                                StreamUtils.Copy(zipStream, streamWriter, buffer);
                            }
#if UNITY_IPHONE
                            zipProxy.mOutFileList.Add(outputPath); 
#endif
                        }
                    }
                    catch (System.Exception e)
                    {
                        exception = e;
                        break;
                    }

                    zipProxy.mCompletedCount++;
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }

            //complete unzip delete the zip file
            // File.Delete(arg.m_zipPath);
            //			lock (((ICollection)AllProxyList).SyncRoot) {
            if (exception != null)
            {
                zipProxy.mIsError = true;
                zipProxy.mException = exception;
            }
            zipProxy.mIsDone = true;
            //			}
        }

        public static bool HasTask()
        {
            return AllProxyList.Count > 0;
        }

        //主线程每隔一段时间轮询一下，更新AllProxyList状态
        public static float CheckOutZipProxyList()
        {
            if (AllProxyList.Count == 0)
                return 0f;

            float totalProgress = 0f;
            lock (((ICollection)AllProxyList).SyncRoot)
            {
                List<ZipProxy> removeList = new List<ZipProxy>();
                for (int i = 0; i < AllProxyList.Count; ++i)
                {
                    ZipProxy proxy = AllProxyList[i];
                    if (proxy.mIsDone)
                    {
                        if (proxy.mIsError)
                        {
                            if (proxy.mErrorCallback != null)
                            {
                                proxy.mErrorCallback(proxy.mException);
                            }
                        }
                        else
                        {
                            if (proxy.mFinishCallback != null)
                            {
                                proxy.mFinishCallback(proxy);
                            }
                        }
#if UNITY_IPHONE
                        proxy.mOutFileList.ForEach(iOSUtility.ExcludeFromBackupUrl);
#endif
                        removeList.Add(proxy);
                        AddLogMessage(string.Format("Name:{0}\nFinish", proxy.ZipFile));
                        totalProgress += 1f;
                    }
                    else
                    {
                        float progress = proxy.Progress;
                        totalProgress += progress;
                        AddLogMessage(string.Format("Name:{0}\nProgress:{1}", proxy.ZipFile, progress));
                    }
                }
                //计算总进度值
                totalProgress = totalProgress / AllProxyList.Count;
                AddLogMessage(string.Format("Total Progress:{0}", totalProgress));

                for (int i = 0; i < removeList.Count; ++i)
                {
                    AllProxyList.Remove(removeList[i]);
                }
                PrintLogMessage();
            }

            return totalProgress;
        }

        public static void AddLogMessage(string log)
        {
            if (!LogMessage)
                return;
            LogBuilder.AppendLine(log);
        }

        public static void PrintLogMessage()
        {
            if (!LogMessage)
                return;
            Debug.Log(LogBuilder.ToString());
            LogBuilder.Length = 0;
        }
    }
}
