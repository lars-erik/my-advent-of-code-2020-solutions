using System;
using System.IO;
using System.Linq;

namespace AoCSharp.Common
{
    public class Resources
    {
        public static string[] GetResourceLines<T>(T instance, string path, string splitter = null)
        {
            return GetResourceLines(instance.GetType(), path, splitter);
        }

        public static string[] GetResourceLines(Type type, string path, string splitter = null)
        {
            splitter ??= Environment.NewLine;
            using var inputStream = type
                .Assembly
                .GetManifestResourceStream(path);
            var input = new StreamReader(inputStream)
                .ReadToEnd()
                .Split(splitter)
                .ToArray();
            return input;
        }
    }
}
