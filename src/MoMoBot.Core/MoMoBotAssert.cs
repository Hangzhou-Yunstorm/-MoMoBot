using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core
{
    public static class MoMoBotAssert
    {
        public static void KeyNotNullOrEmpty(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
        }

        public static void NotNull(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }

        public static void ValueNullOrWhiteSpace(string dialogId)
        {
            if (string.IsNullOrWhiteSpace(dialogId))
            {
                throw new ArgumentNullException(dialogId);
            }
        }

        public static void NameNullOrWhiteSpace(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
        }

        public static void ValueNotNull(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
