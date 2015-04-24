using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace HA.COSMOS.DAContracts
{
    public sealed class UpdateExpression<T> : Dictionary<Expression<Func<T, object>>, object> { }
}