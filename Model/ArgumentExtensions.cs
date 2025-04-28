﻿namespace Model;

public static class ArgumentExtensions
{
    public static void ThrowIfNull<T>(this T obj, string message = null)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(message ?? $"Argument was null.");
        }
    }

    public static void ThrowIfNullOrEmpty(this string str, string message = null)
    {
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentNullException(message ?? $"Argument was null or empty.");
        }
    }

    public static async Task ThrowIfNotSuccessful(this Task<HttpStatusCode> statusCode)
    {
        if (await statusCode != HttpStatusCode.OK)
            throw new HttpRequestException($"Request failed with status code: {statusCode}");
    }
}
