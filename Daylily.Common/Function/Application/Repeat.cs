﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Function.Application
{
    [Name("复读")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Beta)]
    [Help("按一定条件触发复读")]
    public class Repeat : ApplicationApp
    {
        private static readonly ConcurrentDictionary<string, GroupSettings> GroupDic = new ConcurrentDictionary<string, GroupSettings>();
        private const int MaxNum = 10;

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private)
                return null;
            string groupId = messageObj.GroupId ?? messageObj.DiscussId;

            if (!GroupDic.ContainsKey(groupId))
            {
                GroupDic.GetOrAdd(groupId, new GroupSettings
                {
                    GroupId = groupId,
                });

                GroupDic[groupId].Task = Task.Run(DecreaseQueue);
                //GroupDic[groupId].Thread.Start();
            }

            if (GroupDic[groupId].IntQueue >= MaxNum && !GroupDic[groupId].Locked)
            {
                GroupDic[groupId].Locked = true;
                Logger.Debug(groupId + " locked");
                Logger.Success(groupId + "的" + messageObj.UserId + "触发了复读");
                Thread.Sleep(Rnd.Next(1000, 8000));
                return new CommonMessageResponse(messageObj.Message, messageObj);
            }

            GroupDic[groupId].IntQueue++;
            //Logger.Debug(groupId + " incresed to " + GroupDic[groupId].IntQueue);
            return null;

            Task DecreaseQueue()
            {
                while (true)
                {
                    Thread.Sleep(Rnd.Next(1000, 10000));
                    if (GroupDic[groupId].IntQueue <= 0)
                    {
                        if (GroupDic[groupId].Locked)
                        {
                            GroupDic[groupId].Locked = false;
                            Logger.Debug(groupId + " unlocked");
                        }

                        continue;
                    }

                    if (Rnd.NextDouble() < 0.02) Thread.Sleep(Rnd.Next(30000, 45000));

                    GroupDic[groupId].IntQueue--;
                    //Logger.Debug(groupId + " decresed to " + GroupDic[groupId].IntQueue);
                }
            }
        }

        private class GroupSettings
        {
            public string GroupId { get; set; }
            public int IntQueue { get; set; }
            public Task Task { get; set; }
            public bool Locked { get; set; } = false;
        }

    }
}
