using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Base
{
    /// <summary>
    /// App入口
    /// </summary>
    public interface IAppEntry
    {
        /// <summary>
        /// 在初始化之前
        /// </summary>
        /// <returns></returns>
        IEnumerator OnBeforeInit();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        IEnumerator OnGameStart();
    }
}
