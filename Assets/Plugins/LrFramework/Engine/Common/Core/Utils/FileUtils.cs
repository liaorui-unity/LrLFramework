using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System;
using LogInfo;

namespace Table
{
    /// <summary>
    /// 文件+目录操作
    /// @author hannibal
    /// @time 2014-11-20
    /// </summary>
    public class FileUtils
    {
        #region 目录
        /// <summary>
        /// 遍历目录，获取所有文件
        /// </summary>
        /// <param name="dir">查找的目录</param>
        /// <param name="listFiles">文件列表</param>
        static public void GetDirectoryFiles(string dir_path, ref List<string> list_files)
        {
            if (!Directory.Exists(dir_path)) return;

            DirectoryInfo dir = new DirectoryInfo(dir_path);
            RecursiveDirectoryFiles(dir, dir_path + '/', ref list_files);
        }
        static private void RecursiveDirectoryFiles(DirectoryInfo dir, string parent_path, ref List<string> list_files)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                string ext = fi.Extension.ToLower();
                if (ext == ".meta" || ext == ".manifest" || ext == ".svn") continue;
                if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                list_files.Add(parent_path + fi.Name);
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                if (d.Name == "." || d.Name == "..") continue;
                if ((d.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                RecursiveDirectoryFiles(d, parent_path + d.Name + '/', ref list_files);
            }
        }
        /// <summary>
        /// 获得所有目录
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="list_dirs"></param>
        static public void GetDirectorys(string dir_path, ref List<string> list_dirs)
        {
            if (!Directory.Exists(dir_path)) return;

            DirectoryInfo dir = new DirectoryInfo(dir_path);
            RecursiveDirectory(dir, dir_path + '/', ref list_dirs);
        }
        static private void RecursiveDirectory(DirectoryInfo dir, string parent_path, ref List<string> list_dirs)
        {
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                if (d.Name == "." || d.Name == "..") continue;
                if ((d.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                list_dirs.Add(d.Name);
                RecursiveDirectory(d, parent_path + d.Name + '/', ref list_dirs);
            }
        }
        /// <summary>
        /// 拷贝目录
        /// </summary>
        static public void CopyXMLFolderTo(string srcDir, string dstDir)
        {
            if (!Directory.Exists(dstDir))
            {
                FileUtils.CreateDirectory(dstDir);
            }

            DirectoryInfo info = new DirectoryInfo(srcDir);
            FileInfo[] files = info.GetFiles();
            foreach (FileInfo file in files)
            {
                string srcFile = srcDir + file.Name;
                string dstFile = dstDir + file.Name;
                if (srcFile.Contains(".xml"))
                {
                    Info.Log("srcFile:" + srcFile + " dstFile:" + dstFile);
                    File.Copy(srcFile, dstFile, true);
                }
            }
        }
        /// <summary>
        /// 拷贝目录
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                Info.LogError("Source directory does not exist or could not be found: "+ sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                FileUtils.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static bool CreateDirectory(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return false;
            if (Directory.Exists(folder)) return true;

            int num = 0;
            bool result = false;
            while(num < 3)
            {
                try
                {
                    Directory.CreateDirectory(folder);
                    result = true;
                    break;
                }
                catch (Exception e)
                {
                    Info.LogException(e);
                }
                finally
                {
                    num++;
                }
            }
            return result;
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static bool DeleteDirectory(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return false;
            if (!Directory.Exists(folder)) return true;

            int num = 0;
            bool result = false;
            while (num < 3)
            {
                try
                {
                    Directory.Delete(folder, true);
                    result = true;
                    break;
                }
                catch (Exception e)
                {
                    Info.LogException(e);
                }
                finally
                {
                    num++;
                }
            }
            return result;
        }
        #endregion

        #region 读写文件
        /// <summary>
        /// 读文本文件
        /// </summary>
        /// <param name="path">目录</param>
        /// <returns>文件内容</returns>
        public static string ReadFileText(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Info.LogWarning("路径错误");
                return string.Empty;
            }
            if (!File.Exists(path))
            {
                Info.LogWarning("未找到文件:" + path);
                return string.Empty;
            }
            try
            {
                return File.ReadAllText(path, System.Text.Encoding.UTF8);
            }
            catch(System.Exception e)
            {
                Info.LogException(e);
                return string.Empty;
            }
        }
        /// <summary>
        /// 写入文本文件
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="content">内容</param>
        /// <param name="append">是否追加</param>
        public static void WriteFileText(string path, string content, bool append = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                Info.LogWarning("路径错误");
                return;
            }
            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                FileUtils.CreateDirectory(folder);

            StreamWriter sw = null;
            try
            {
                if (!File.Exists(path))
                {
                    using (sw = File.CreateText(path))
                    {
                        sw.Write(content);
                        sw.Flush();
                    }
                }
                else
                {
                    using (sw = new StreamWriter(path, append, System.Text.Encoding.UTF8))
                    {
                        sw.Write(content);
                        sw.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Info.LogException(ex);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
        /// <summary>
        /// 读二进制文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] ReadFileByte(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Info.LogWarning("路径错误");
                return null;
            }
            if (!File.Exists(path))
            {
                Info.LogWarning("未找到文件:" + path);
                return null;
            }
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (System.Exception e)
            {
                Info.LogException(e);
                return null;
            }
        }
        /// <summary>
        /// 写二进制文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        public static void WriteFileByte(string path, byte[] data)
        {
            if (string.IsNullOrEmpty(path))
            {
                Info.LogWarning("路径错误");
                return;
            }

            FileStream fs = null;
            try
            {
                string folder = Path.GetDirectoryName(path);
                if (!Directory.Exists(folder))
                    FileUtils.CreateDirectory(folder);
                using (fs = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
            catch (System.Exception e)
            {
                Info.LogException(e);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            int num = 0;
            bool result = false;
            while (num < 3)
            {
                try
                {
                    File.Delete(path);
                    result = true;
                    break;
                }
                catch (Exception e)
                {
                    num++;
                    if (num >= 3)
                    {
                        Info.LogException(e);
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
#endregion

        #region md5
        /// <summary>
        /// 计算文件md5值
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        private static System.Security.Cryptography.MD5 md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
        static public string GetFileMD5(string pathName)
        {
            try
            {
                byte[] retVal = FileUtils.ReadFileByte(pathName);
                return BitConverter.ToString(md5Provider.ComputeHash(retVal)).Replace("-", string.Empty);
            }
            catch (Exception ex)
            {
                Info.LogException(ex);
            }
            return string.Empty;
        }
        static public string GetMD5(string content)
        {
            try
            {
                return BitConverter.ToString(md5Provider.ComputeHash(Encoding.UTF8.GetBytes(content))).Replace("-", string.Empty);
            }
            catch (Exception ex)
            {
                Info.LogException(ex);
            }
            return content;
        }
        static public string GetMD5(byte[] content)
        {
            try
            {
                return BitConverter.ToString(md5Provider.ComputeHash(content)).Replace("-", string.Empty);
            }
            catch (Exception ex)
            {
                Info.LogException(ex);
            }
            return string.Empty;
        }
        #endregion

        #region 其他
        /// <summary>
        /// 获得文件占用磁盘大小
        /// </summary>
        /// <param name="asset_path">绝对路径</param>
        /// <returns>单位M</returns>
        public static float GetFileSize(string full_path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(full_path);
                long size = fileInfo.Length;
                float mem = size / 1024f / 1024f;
                return mem;
            }
            catch(System.Exception e)
            {
                Info.LogException(e);
                return 0;
            }
        }
        #endregion
    }
}