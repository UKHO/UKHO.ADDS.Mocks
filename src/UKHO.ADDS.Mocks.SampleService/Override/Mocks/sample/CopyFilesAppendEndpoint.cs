using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.SampleService.Override.Mocks.sample
{
    public class CopyFilesAppendEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/files/copy/append", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            // ADDS Mock will have the 'default' state unless we have told it otherwise
                            return Results.Ok("This is a result, just needed this text with the 200 response");

                        case "get-jpeg":

                            var fs = GetFileSystem();
                            if (fs.FileExists("/subpath/messier-78.jpg"))
                            {
                                var newFileName = $"/new-file-{Guid.NewGuid():N}.jpg";

                                using (var s = fs.OpenFile("/subpath/messier-78.jpg", FileMode.Open, FileAccess.Read))
                                using (var destination = fs.OpenFile(newFileName, FileMode.CreateNew, FileAccess.Write))
                                {
                                    while (true)
                                    {
                                        var chunk = ReadNextChunk(s, 8192);
                                        if (chunk.Length == 0)
                                        {
                                            break;
                                        }
                                        destination.Write(chunk, 0, chunk.Length);
                                    }
                                }

                                return Results.File(fs.OpenFile(newFileName, FileMode.Open, FileAccess.Read), "image/jpeg");
                            }

                            return Results.NotFound("Could not find the JPEG path in the /files GET method");


                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Copies a file", 3));
                    d.Append(new MarkdownParagraph("For the get-jpeg state, copies the file to a new file and returns the new file"));
                });

        private static byte[] ReadNextChunk(Stream stream, int bufferSize = 8192)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream must be readable.");
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be greater than 0.");
            }

            var buffer = new byte[bufferSize];
            var bytesRead = stream.Read(buffer, 0, bufferSize);

            if (bytesRead == 0)
            {
                return Array.Empty<byte>();
            }

            if (bytesRead == bufferSize)
            {
                return buffer;
            }

            // Last chunk - trim buffer
            var trimmed = new byte[bytesRead];
            Array.Copy(buffer, trimmed, bytesRead);
            return trimmed;
        }
    }
}
