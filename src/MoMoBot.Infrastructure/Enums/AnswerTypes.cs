using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure.Enums
{
    public enum AnswerTypes
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text = 1,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 2,
        /// <summary>
        /// 音频
        /// </summary>
        Voice = 3,
        /// <summary>
        /// 视频
        /// </summary>
        Video = 4,
        /// <summary>
        /// HTML
        /// </summary>
        Html = 5,
        /// <summary>
        /// 流程
        /// </summary>
        ProcessFlow = 6
    }
}
