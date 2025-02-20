using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ADDSMock.Applications.Interactive.Controls
{
    internal static class IconPathFactory
    {
        private static readonly ConcurrentDictionary<string, string> _icons;

        static IconPathFactory()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "ADDSMock.Applications.Interactive.Assets.icon-stream.dat";

            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new BinaryReader(stream);

            var bytes = reader.ReadBytes((int)stream.Length);
            var jsonBytes = Decompress(bytes);

            var json = Encoding.UTF8.GetString(jsonBytes);
            var icons = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

            _icons = new ConcurrentDictionary<string, string>(icons);
        }

        public static string GetIconPath(string iconKey)
        {
            return _icons.GetValueOrDefault(iconKey, "Help - 02");
        }

        private static byte[] Decompress(byte[] bytes)
        {
            using var memoryStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();

            using (var decompressStream = new BrotliStream(memoryStream, CompressionMode.Decompress))
            {
                decompressStream.CopyTo(outputStream);
            }

            return outputStream.ToArray();
        }
    }
}
