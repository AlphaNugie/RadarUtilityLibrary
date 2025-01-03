using ArsLibrary.Core;
using CommonLib.Clients.Tasks;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArsLibrary.Tasks
{
    /// <summary>
    /// ARS40X系雷达的报文内容（例如空白报文）分发服务的任务类，ip为指定的本地ip，端口号默认为46334
    /// </summary>
    public class Ars40xContentServerTask : Task
    {
        private SocketTcpServer _server;

        /// <inheritdoc/>
        protected override void Init()
        {
            _server = new SocketTcpServer()
            {
                ServerIp = ArsConst.IpAddress_Local,
                ServerPort = ArsConst.ContentServerPort,
            };
            _server.Start();
            //Interval = 500; //默认循环间隔为500毫秒
            AddLog(string.Format("空白报文的分发服务{0}:{1}已启动", _server.ServerIp, _server.ServerPort));
        }

        /// <inheritdoc/>
        protected override void LoopContent()
        {
            _server.SendData(ArsConst.EmptyMessage, StringType.Hex); //不断向外发送空白报文字符串
        }

/// <inheritdoc/>
        protected override Task GetNewInstance()
        {
            throw new NotImplementedException();
        }
    }
}
