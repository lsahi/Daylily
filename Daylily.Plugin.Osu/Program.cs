﻿using System;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;
using Daylily.Plugin.Osu.Command.Subscribes;

namespace Daylily.Plugin.Osu
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引用添加项目Daylily.Common
            Subscribe newPlugin = new Subscribe();
            newPlugin.Initialize(args);
            CommonMessage cm = new CommonMessage()
            {
                GroupId = "123456788",
                UserId = "2241521134",
                Message = "SB",
                MessageType = MessageType.Group,
                Group = new GroupMsg(),
            };

            Logger.Success("收到：" + newPlugin.Message_Received(cm).Message);
            Console.ReadKey();
        }
    }
}
