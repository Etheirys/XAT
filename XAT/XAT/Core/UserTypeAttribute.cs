using System;
using System.Runtime.CompilerServices;

namespace XAT.Core;

public class UserTypeAttribute : Attribute
{
    public int Order { get; set; }

    public UserTypeAttribute([CallerLineNumber] int order = -1)
    {
        this.Order = order;
    }
}
