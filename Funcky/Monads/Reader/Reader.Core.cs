using System;
using Funcky.Extensions;

namespace Funcky.Monads
{
    public delegate TResult Reader<in TEnvironment, out TResult>(TEnvironment environment);

    public static class Reader<TEnvironment>
    {
        public static Reader<TEnvironment, TResult> Return<TResult>(Func<TEnvironment, TResult> function)
            => environment
                => function(environment);

        public static Reader<TEnvironment, Unit> Return(Action<TEnvironment> action)
            => environment
                => action.ToUnitFunc()(environment);
    }
}
