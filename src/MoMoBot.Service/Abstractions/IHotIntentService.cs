using MoMoBot.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IHotIntentService
    {
        /// <summary>
        /// 获取热门意图条形图数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<HotIntentBarViewModel>> GetHotIntentBarTopTen();
    }
}
