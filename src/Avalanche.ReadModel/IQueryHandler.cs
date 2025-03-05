using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.ReadModel
{
    public interface IQueryHandler<T, Tq> where Tq : IQuery
    {
        T Get(Tq query);
    }
}
