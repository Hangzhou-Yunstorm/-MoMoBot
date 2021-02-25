using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class MessageResponse
    {
        public MessageResponse(bool success, string message) : this(success, message, false)
        {
        }
        public MessageResponse(bool success, string message, bool isFlow) : this(success, message, isFlow, null)
        {
        }
        public MessageResponse(bool success, string message, object data) : this(success, message, false, data)
        {
        }
        public MessageResponse(bool success, string message, bool isFlow, object data)
        {
            Success = success;
            IsFlow = isFlow;
            Message = message;
            Data = data;
        }

        public bool Success { get; set; }
        public bool IsFlow { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
