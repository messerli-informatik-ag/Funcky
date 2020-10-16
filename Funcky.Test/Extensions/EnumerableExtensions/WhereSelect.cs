using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class WhereSelect
    {
        [Fact]
        public void WhereSelectFiltersEmptyValues()
        {
            var input = Enumerable.Range(0, 10);
            var expectedResult = new[] { 0, 4, 16, 36, 64 };
            var result = input.WhereSelect(SquareEvenNumbers);
            Assert.Equal(expectedResult, result);
        }

        private static Option<int> SquareEvenNumbers(int number)
            => IsEven(number) ? Option.Some(number * number) : Option<int>.None();

        private static bool IsEven(int number)
            => number % 2 == 0;
    }
}
