using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.QueryModel
{
    public interface IQueryHandler<T, Tq> where Tq : IQuery
    {
        T Get(Tq query);
    }
}
