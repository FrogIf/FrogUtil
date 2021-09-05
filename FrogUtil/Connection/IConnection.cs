using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frog.Util.Connection
{
    public interface IConnection
    {
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes">发送的字节</param>
        /// <returns>是否发送成功: true - 成功; false - 失败</returns>
        bool SendData(byte[] bytes);

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns>是否打开成功: true - 成功; false - 失败</returns>
        bool Open(IConfiguration config);

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="closeSuccessCallback">关闭成功后回调</param>
        void Close(CloseSuccessCallback closeSuccessCallback);

        /// <summary>
        /// 连接是否已打开
        /// </summary>
        /// <returns>true - 已打开; false - 未打开</returns>
        bool IsOpen();

        /// <summary>
        /// 获取当前可用的连接目标
        /// </summary>
        /// <returns>可用的连接目标</returns>
        string[] GetAvailableTargets();

    }

    /// <summary>
    /// 连接关闭委托
    /// </summary>
    public delegate void CloseSuccessCallback();

    public interface IConfiguration
    {
        /// <summary>
        /// 目标
        /// </summary>
        /// <returns></returns>
        string Target();
    }
}
