using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Middleware
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ShouldNotModifyAttribute : Attribute
    {

    }
}
