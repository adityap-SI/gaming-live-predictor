using System;

namespace ICC.Predictor.Interfaces.Connection
{
    public interface IPostgre
    {
        string Schema { get; }
        string ConnectionString { get; }
    }
}