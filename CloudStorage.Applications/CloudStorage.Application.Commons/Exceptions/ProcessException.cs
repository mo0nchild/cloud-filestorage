namespace CloudStorage.Application.Commons.Exceptions;

public class ProcessException(string message) : Exception(message)
{
    public static void ThrowIf(Func<bool> predicate, string message)
    {
        if (predicate.Invoke()) throw new ProcessException(message);
    }
}