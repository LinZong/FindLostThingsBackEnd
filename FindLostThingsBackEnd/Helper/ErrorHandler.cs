using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Helper
{
    public static class ErrorHandler
    {
        public static void FormatError<TSource>(ILogger Logger,string Message)
        {
            Logger.LogError($"{typeof(TSource).Name} -- {Message}");
        }

        public static void FormatError<TSource>(ILogger Logger, Exception e)
        {
            Logger.LogError($"{typeof(TSource).Name} -- {e.Source} -- {e.Message}");
        }


    }
}
