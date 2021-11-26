using System;

namespace ParkyAPI.Infrastructure.Interface
{
    public interface ILog
    {
        void Information(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(string message);
        void Error(Exception exception, string message);
    }
}
