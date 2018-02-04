using System;
using System.Collections;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using UnityEngine;

namespace AssetPipeline
{
    public class ZipManager : MonoBehaviour
    {
        private static ZipManager _instance;
        private static bool _isQuit;

        public static ZipManager Instance
        {
            get
            {
                if (_instance == null && !_isQuit)
                {
                    var type = typeof(ZipManager);
                    var objects = FindObjectsOfType<ZipManager>();

                    if (objects.Length > 0)
                    {
                        _instance = objects[0];
                        if (objects.Length > 1)
                        {
                            Debug.LogWarning("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first. Destroying the others.");
                            for (var i = 1; i < objects.Length; i++)
                                DestroyImmediate(objects[i].gameObject);
                        }
                        return _instance;
                    }

                    GameObject go = new GameObject(type.Name);
                    _instance = go.AddComponent<ZipManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        void OnApplicationQuit()
        {
            _isQuit = true;
        }

        public float Progress
        {
            get;
            private set;
        }

        public bool HasTask
        {
            get
            {
                return ZipProxy.HasTask();
            }
        }

        private Coroutine _checkCoroutine;

        void Awake()
        {
            ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;
        }

        public void StarWork()
        {
            if (_checkCoroutine != null)
                return;

            _checkCoroutine = StartCoroutine(ZipUpdate());
        }

        public void StopWork()
        {
            if (_checkCoroutine != null)
            {
                if (!HasTask)
                {
                    StopCoroutine(_checkCoroutine);
                    _checkCoroutine = null;
                }
                else
                {
                    Debug.Log("当前还存在解压中的任务，无法停止");
                }
            }
        }

        //Unity主线程每0.2秒检查一下，ZipProxy情况
        IEnumerator ZipUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                Progress = ZipProxy.CheckOutZipProxyList();
            }
        }

        public ZipProxy Extract(string zipFile, string outFolder,
                                 Action<ZipProxy> finishCallback,
                                 Action<Exception> errorCallback = null)
        {
            return ZipProxy.Extract(zipFile, outFolder, finishCallback, errorCallback);
        }

        public ZipProxy Extract(string zipFile, string outFolder, Stream inputStream,
                                 Action<ZipProxy> finishCallback,
                                 Action<Exception> errorCallback = null)
        {
            return ZipProxy.Extract(zipFile, outFolder, inputStream, finishCallback, errorCallback);
        }

        public static void CompressFile(string inputFile, string outputFile, int compressLevel = Deflater.BEST_COMPRESSION, string password = "")
        {
            FileInfo fileInfo = new FileInfo(inputFile);
            string dir = Path.GetDirectoryName(outputFile);
            Directory.CreateDirectory(dir);
            FileStream fsOut = File.Create(outputFile);

            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(compressLevel); //0-9, 9 being the highest level of compression
            zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

            string entryName = Path.GetFileName(inputFile);
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fileInfo.LastWriteTime;
            newEntry.Size = fileInfo.Length;
            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(inputFile))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        public static void CompressFiles(string outputFile, string[] files, int compressLevel = Deflater.BEST_COMPRESSION,
            string password = "")
        {
            string dir = Path.GetDirectoryName(outputFile);
            Directory.CreateDirectory(dir);
            FileStream fsOut = File.Create(outputFile);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(compressLevel); //0-9, 9 being the highest level of compression
            zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = fi.Name; // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        public static void CompressFolder(string outputFile, string folderName, string searchPattern, int compressLevel = Deflater.BEST_COMPRESSION, string password = "")
        {
            string dir = Path.GetDirectoryName(outputFile);
            Directory.CreateDirectory(dir);
            FileStream fsOut = File.Create(outputFile);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(compressLevel); //0-9, 9 being the highest level of compression
            zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            CompressFolder(folderName, searchPattern, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        private static void CompressFolder(string folderName, string searchPattern, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(folderName, searchPattern, SearchOption.TopDirectoryOnly);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(folderName);
            foreach (string folder in folders)
            {
                CompressFolder(folder, searchPattern, zipStream, folderOffset);
            }
        }
    }
}
