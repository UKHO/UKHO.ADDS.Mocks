using UKHO.ADDS.Mocks.Domain;
using UKHO.ADDS.Mocks.Domain.Markdown;
using UKHO.ADDS.Mocks.Domain.Mocks;
using UKHO.ADDS.Mocks.Domain.States;

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

                            var jpegPathResult = GetFile("messier-78.jpg");

                            if (jpegPathResult.IsSuccess(out var jpegFile))
                            {
                                var newFileName = $"new-file-{Guid.NewGuid():N}.jpg";

                                using (var s = jpegFile.Open())
                                {
                                    CreateFile(newFileName);

                                    var content = Array.Empty<byte>();

                                    do
                                    {
                                        var chunk = ReadNextChunk(s, 8192);
                                        if (chunk.Length == 0)
                                        {
                                            break;
                                        }
                                        content = content.Concat(chunk).ToArray();
                                        AppendFile(newFileName, chunk);

                                    } while (content.Length > 0);
                                }

                                var newFileResult = GetFile(newFileName);

                                if (newFileResult.IsSuccess(out var newFile))
                                {
                                    return Results.File(newFile.Open(), newFile.MimeType);
                                }
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
