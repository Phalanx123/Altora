namespace Altora.Utilities;

public static class SafeTailUtil
{
   public static string SafeTail(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "<empty>" : value[^4..];
}