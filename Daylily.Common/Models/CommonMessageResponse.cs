﻿using Daylily.Common.Models.CQRequest.Api;
using System;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models
{
    public class CommonMessageResponse
    {
        public bool EnableAt { get; set; }
        public string Message { get; set; }
        public MessageType MessageType { get; set; }

        public string UserId { get; set; }
        public string DiscussId { get; set; }
        public string GroupId { get; set; }

        public SendPrivateMsg Private { get; set; }
        public SendDiscussMsg Discuss { get; set; }
        public SendGroupMsg Group { get; set; }

        public CommonMessageResponse() { }

        /// <summary>
        /// 此为指定一个特定的群时再使用
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="enableAt"></param>
        public CommonMessageResponse(string message, string userId = null, bool enableAt = false)
        {
            UserId = userId;
            Message = message;
            EnableAt = enableAt;
        }

        public CommonMessageResponse(string message, CommonMessage commonMessage, bool enableAt = false)
        {
            Message = message;
            UserId = commonMessage.UserId;
            MessageType = commonMessage.MessageType;
            EnableAt = enableAt;
            switch (commonMessage.MessageType)
            {
                case MessageType.Private:
                    Private = new SendPrivateMsg(commonMessage.Private.UserId.ToString(),
                        commonMessage.Private.Message);
                    break;
                case MessageType.Discuss:
                    Discuss = new SendDiscussMsg(commonMessage.Discuss.DiscussId.ToString(),
                        commonMessage.Discuss.Message);
                    DiscussId = commonMessage.DiscussId;
                    break;
                case MessageType.Group:
                    Group = new SendGroupMsg(commonMessage.Group.GroupId.ToString(), commonMessage.Group.Message);
                    GroupId = commonMessage.GroupId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //public CommonMessageResponse(SendPrivateMsgResp privateMsg, string userId, bool enableAt = false)
        //{
        //    UserId = privateMsg.UserId.ToString();

        //    MessageType = MessageType.Private;
        //    Private = privateMsg;
        //}

        //public CommonMessageResponse(SendDiscussMsgResp discussMsg, string userId, bool enableAt = false)
        //{
        //    UserId = userId;
        //    DiscussId = discussMsg.DiscussId.ToString();

        //    MessageType = MessageType.Discuss;
        //    Discuss = discussMsg;
        //}

        //public CommonMessageResponse(SendGroupMsgResp groupMsg, string userId, bool enableAt = false)
        //{
        //    UserId = userId;
        //    DiscussId = groupMsg.GroupId.ToString();

        //    MessageType = MessageType.Group;
        //    Group = groupMsg;
        //}
    }
}
