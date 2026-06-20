using System.Diagnostics;
using static System.Guid;

namespace NetBoilerplate.Shared.Extensions;

public static class StringExtensions
{
    [DebuggerStepThrough]
    public static Guid ToGuid(this string value)
    {
        TryParse(value, out var result);
        return result;
    }
}