using System;
using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.DataTypes
{
    /// <remarks>
    /// EitherOrBoth values constructed using <c>default</c> are in an invalid state.
    /// Any attempt to perform actions on such a value will throw a <see cref="NotSupportedException"/>.
    /// </remarks>
    public readonly struct EitherOrBoth<TLeft, TRight> : IEquatable<EitherOrBoth<TLeft, TRight>>
        where TLeft : notnull
        where TRight : notnull
    {
        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly Side _side;

        private EitherOrBoth(TLeft left, TRight right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
            _side = Side.Both;
        }

        private EitherOrBoth(TLeft left)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = default!;
            _side = Side.Left;
        }

        private EitherOrBoth(TRight right)
        {
            _left = default!;
            _right = right ?? throw new ArgumentNullException(nameof(right));
            _side = Side.Right;
        }

        private enum Side : byte
        {
            Uninitialized,
            Both,
            Left,
            Right,
        }

        [Pure]
        public static bool operator ==(EitherOrBoth<TLeft, TRight> lhs, EitherOrBoth<TLeft, TRight> rhs) => lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(EitherOrBoth<TLeft, TRight> lhs, EitherOrBoth<TLeft, TRight> rhs) => !lhs.Equals(rhs);

        [Pure]
        public static EitherOrBoth<TLeft, TRight> Both(TLeft left, TRight right) => new(left, right);

        [Pure]
        public static EitherOrBoth<TLeft, TRight> Left(TLeft left) => new(left);

        [Pure]
        public static EitherOrBoth<TLeft, TRight> Right(TRight right) => new(right);

        [Pure]
        public TMatchResult Match<TMatchResult>(Func<TLeft, TMatchResult> left, Func<TRight, TMatchResult> right, Func<TLeft, TRight, TMatchResult> both)
            => _side switch
            {
                Side.Left => left(_left),
                Side.Right => right(_right),
                Side.Both => both(_left, _right),
                Side.Uninitialized => throw new NotSupportedException(
                    "EitherOrBoth constructed via default instead of a factory function (EitherOrBoth.Left, EitherOrBoth.Right or EitherOrBoth.Both)"),
                _ => throw new ArgumentOutOfRangeException(nameof(_side), $"Internal error: Enum variant {_side} is not handled"),
            };

        public void Match(Action<TLeft> left, Action<TRight> right, Action<TLeft, TRight> both)
        {
            switch (_side)
            {
                case Side.Left:
                    left(_left);
                    break;
                case Side.Right:
                    right(_right);
                    break;
                case Side.Both:
                    both(_left, _right);
                    break;
                case Side.Uninitialized:
                    throw new NotSupportedException("EitherOrBoth constructed via default instead of a factory function (EitherOrBoth.Left, EitherOrBoth.Right or EitherOrBoth.Both)");
                default:
                    throw new ArgumentOutOfRangeException(nameof(_side), $"Internal error: Enum variant {_side} is not handled");
            }
        }

        [Pure]
        public override bool Equals(object? obj)
            => obj is EitherOrBoth<TLeft, TRight> other && Equals(other);

        [Pure]
        public bool Equals(EitherOrBoth<TLeft, TRight> other)
            => Equals(_side, other._side)
               && Equals(_right, other._right)
               && Equals(_left, other._left);

        [Pure]
        public override int GetHashCode()
            => Match(HashFromLeft, HashFromRight, HashFromBoth);

        [Pure]
        private static int HashFromLeft(TLeft left)
            => left?.GetHashCode() ?? 0;

        [Pure]
        private static int HashFromRight(TRight right)
            => right?.GetHashCode() ?? 0;

        [Pure]
        private static int HashFromBoth(TLeft left, TRight right)
            => HashCode.Combine(left, right);
    }

    public static class EitherOrBoth
    {
        [Pure]
        public static Option<EitherOrBoth<TLeft, TRight>> FromOptions<TLeft, TRight>(Option<TLeft> leftElement, Option<TRight> rightElement)
            where TLeft : notnull
            where TRight : notnull
            => (leftElement, rightElement).Match(
                left: Left<TLeft, TRight>,
                right: Right<TLeft, TRight>,
                leftAndRight: Both,
                none: Option.None<EitherOrBoth<TLeft, TRight>>);

        private static Option<EitherOrBoth<TLeft, TRight>> Left<TLeft, TRight>(TLeft left)
            where TLeft : notnull
            where TRight : notnull
            => EitherOrBoth<TLeft, TRight>.Left(left);

        private static Option<EitherOrBoth<TLeft, TRight>> Right<TLeft, TRight>(TRight right)
            where TLeft : notnull
            where TRight : notnull
            => EitherOrBoth<TLeft, TRight>.Right(right);

        private static Option<EitherOrBoth<TLeft, TRight>> Both<TLeft, TRight>(TLeft left, TRight right)
            where TLeft : notnull
            where TRight : notnull
            => EitherOrBoth<TLeft, TRight>.Both(left, right);
    }
}
