//using CommonLib.Helpers;
using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 雷达数据写入单元
    /// </summary>
    public class CommUnit
    {
        //private readonly string _baseDir = @"D:\AntiCollDataLogs\";
        private readonly string _baseDir = @"AntiCollDataLogs\";
        //private FileStream _fileStream;
        private StreamWriter _streamWriter;
        private bool _firstCycle; //首次写入

        /// <summary>
        /// IP地址
        /// </summary>
        public string RemoteIp { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// 名称（同时作为保存文件名）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        private string _path = string.Empty/*, _pathPrev = string.Empty*/;
        /// <summary>
        /// 日志所在路径
        /// </summary>
        public string Path
        {
            get { return _path; }
            set
            {
                //假如路径没有变化，不进行文件完整路径的刷新
                if (value.Equals(_path))
                    return;
                //_pathPrev = _path;
                _path = value;
                //_path += FileSystemHelper.DirSeparatorChar + DateTime.Now.ToString("HHmmss"); //添加时间文件夹
                string pathSec = _path + FileSystemHelper.DirSeparatorChar + DateTime.Now.ToString("HH0000"); //添加时间文件夹
                if (!Directory.Exists(pathSec))
                    Directory.CreateDirectory(pathSec);
                //FullFilePath = string.Format("{0}{1}{2}_{3}.csv", FileSystemHelper.TrimFilePath(_path), FileSystemHelper.DirSeparatorChar, Name, DateTimeHelper.GetTimeStampBySeconds());
                FullFilePath = string.Format("{1}{0}{2}_{3:HHmmssfff}.csv", FileSystemHelper.DirSeparatorChar, FileSystemHelper.TrimFilePath(pathSec), Name, DateTime.Now);
                _streamWriter = new StreamWriter(FullFilePath, true, Encoding.GetEncoding("GB2312")) { AutoFlush = true }; //使用ANSI编码，防止中文乱码
                _firstCycle = true;
            }
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullFilePath { get; private set; }

        /// <summary>
        /// 是否开始
        /// </summary>
        public bool Started { get; set; }

        /// <summary>
        /// 是否处于暂停状态
        /// </summary>
        public bool OnHold { get; set; }

        /// <summary>
        /// 标题行内容
        /// </summary>
        public string Titles { get; set; }

        /// <summary>
        /// 用于写入的数据源
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 通讯单元
        /// </summary>
        /// <param name="remote_ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="name">名称</param>
        /// <param name="note">备注</param>
        public CommUnit(string remote_ip, ushort port, string name, string note)
        {
            var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            _baseDir = dirInfo.Root.ToString() + _baseDir;
            RemoteIp = remote_ip;
            Port = port;
            Name = name;
            Note = note;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <returns>返回是否成功</returns>
        public bool Start()
        {
            if (Started)
                return false;

            //Path = _baseDir + DateTime.Now.Month + DateTime.Now.Day;
            //_streamWriter = new StreamWriter(FullFilePath, true) { AutoFlush = true };
            _firstCycle = true;
            Started = true;
            return true;
        }

        /// <summary>
        /// 结束
        /// </summary>
        /// <returns>返回是否成功</returns>
        public bool Stop()
        {
            if (!Started)
                return false;

            Started = false;
            _firstCycle = false;
            _streamWriter.Close();
            return true;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        public bool Suspend()
        {
            OnHold = true;
            return true;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <returns></returns>
        public bool Resume()
        {
            OnHold = false;
            return true;
        }

        /// <summary>
        /// 写入数据源字段的值
        /// </summary>
        public void Write()
        {
            Write(DataSource);
        }

        /// <summary>
        /// 写入特定信息
        /// </summary>
        /// <param name="info"></param>
        public void Write(string info)
        {
            if (!Started || OnHold || string.IsNullOrWhiteSpace(info))
                return;

            Path = _baseDir + DateTime.Now.Month + DateTime.Now.Day;
            //首次写入时添加标题行
            if (_firstCycle && !string.IsNullOrWhiteSpace(Titles))
            {
                _streamWriter.WriteLine(Titles);
                _firstCycle = false;
            }
            _streamWriter.WriteLine(info);
            if (!_streamWriter.AutoFlush)
                _streamWriter.Flush();
        }
    }
}
