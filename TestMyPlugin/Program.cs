﻿using System;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;

namespace TestMyPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引用添加项目Daylily.Common
            NewPlugin newPlugin = new NewPlugin();
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
