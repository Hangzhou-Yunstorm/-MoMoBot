using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.ML.Exceptions
{
    public class MoMoBotMLException : Exception
    {
        public MoMoBotMLException(string message) : base(message)
        {
        }
    }
}
