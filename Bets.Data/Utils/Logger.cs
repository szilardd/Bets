using System;

namespace Bets.Data
{
    public static class Logger
    {
        public static void Log(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }

}
