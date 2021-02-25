using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core
{
    public interface IPropertyManager
    {
        IStatePropertyAccessor<T> CreateProperty<T>(string name);
    }
}
