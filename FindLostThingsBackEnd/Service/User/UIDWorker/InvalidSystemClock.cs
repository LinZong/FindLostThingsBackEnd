using System;

namespace ChargeScheduler.Services.User.UIDWorker

{
    public class InvalidSystemClock : Exception
    {      
        public InvalidSystemClock(string message) : base(message) { }
    }
}