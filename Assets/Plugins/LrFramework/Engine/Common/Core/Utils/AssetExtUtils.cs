using System.Collections.Generic;

namespace Table
{
    /// <summary>
    /// 资源格式
    /// @author hannibal
    /// @time 2019-10-14
    /// </summary>
    public class AssetExtUtils
    {
        /// <summary>
        /// 是否视频格式
        /// </summary>
        public static bool IsVideo(string ext)
        {
            if (ext.StartsWith(".")) ext = ext.Substring(1);
            ext = ext.ToLower();

            List<string> exts = new List<string>() { "mov", "mpg", "mpeg", "mp4", "avi", "asf" };
            return exts.Contains(ext);
        }
        /// <summary>
        /// 是否音频格式
        /// WAV：微软公司开发的一种声音文件格式，特点：简单的编/解码、普遍的认同/支持以及无损耗存储，目前Windows上最流行的声音文件格式
        /// AIFF：是APPLE公司开发的一种音频文件格式，被MACHINTOSH平台及其应用程序所支持，属于QuickTime技术的一部分
        /// MP3：是MPEG标准中的音频部分，大小只有WAV文件的1/10，是一种有损压缩数字音频格式
        /// MIDI：用于记录声音的信息，主要用处是在电脑作曲领域
        /// WMA：音质要强于MP3格式，压缩率可以达到1：18，由微软开发，具有强的保护版权的能力
        /// OGG：是一种新的音频压缩格式，有点类似MP3等现有的音乐格式，但有一点不同的是，它是完全免费、开放和没有专利限制的。
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsAudio(string ext)
        {
            if (ext.StartsWith(".")) ext = ext.Substring(1);
            ext = ext.ToLower();

            List<string> exts = new List<string>() { "wav", "aiff", "mp3", "midi", "wma", "ogg" };
            return exts.Contains(ext);
        }
    }
}
