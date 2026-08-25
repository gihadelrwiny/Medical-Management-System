using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Common
{
    public sealed record Result<T>(T? Value, string? Error)
    {
        public bool IsSuccess => Error is null;

        public static Result<T> Success(T value)
            => new(value, null);

        public static Result<T> Failure(string error)
            => new(default, error);
    }
}
