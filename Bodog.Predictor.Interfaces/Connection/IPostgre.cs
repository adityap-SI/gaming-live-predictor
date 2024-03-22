using System;

namespace Bodog.Predictor.Interfaces.Connection
{
    public interface IPostgre
    {
        String Schema { get; }
        String ConnectionString { get; }
    }
}