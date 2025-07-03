using System.Collections.Concurrent;
using UKHO.ADDS.Mocks.Files;
using UKHO.ADDS.Mocks.Headers;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class FileDownloadEndpoint : ServiceEndpointMock
    {
        // Static object for locking
        private static readonly object FileLock = new object();
        
        // Cache for file contents to improve performance and reduce file access
        private static readonly ConcurrentDictionary<string, CachedFile> FileCache = new();
        
        // Expiration time for cached files (5 minutes)
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapGet("/batch/{batchId}/files/{fileName}", (string batchId, string fileName, HttpRequest request, HttpResponse response) =>
            {
                EchoHeaders(request, response, [WellKnownHeader.CorrelationId]);
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:
                        var pathResult = GetFile("readme.txt");

                        if (pathResult.IsSuccess(out var file))
                        {
                            try
                            {
                                // Get cached file or read from disk
                                var cachedFile = GetCachedFile(file);
                                
                                // Return the file content from memory
                                return Results.File(cachedFile.Content, cachedFile.MimeType, fileName);
                            }
                            catch (IOException)
                            {
                                return Results.Problem(
                                    detail: "Error accessing file. Please try again later.",
                                    statusCode: 500,
                                    title: "Internal Server Error"
                                );
                            }
                        }

                        return Results.NotFound("Could not find the path in the /files GET method");

                    case WellKnownState.BadRequest:
                        return Results.Json(new
                        {
                            correlationId = request.Headers[WellKnownHeader.CorrelationId],
                            errors = new[]
                            {
                                    new
                                    {
                                        source = "File Download",
                                        description = "Invalid batchId."
                                    }
                                }
                        }, statusCode: 400);

                    case WellKnownState.NotFound:
                        return Results.Json(new
                        {
                            correlationId = request.Headers[WellKnownHeader.CorrelationId],
                            details = "Not Found"
                        }, statusCode: 404);

                    case WellKnownState.UnsupportedMediaType:
                        return Results.Json(new
                        {
                            type = "https://example.com",
                            title = "Unsupported Media Type",
                            status = 415,
                            traceId = "00-012-0123-01"
                        }, statusCode: 415);

                    case WellKnownState.InternalServerError:
                        return Results.Json(new
                        {
                            correlationId = request.Headers[WellKnownHeader.CorrelationId],
                            details = "Internal Server Error"
                        }, statusCode: 500);

                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
            .Produces<string>()
            .WithEndpointMetadata(endpoint, d =>
            {
                d.Append(new MarkdownHeader("Download a file", 3));
                d.Append(new MarkdownParagraph("Downloads readme.txt."));
            });
        }
        
        /// <summary>
        /// Get cached file content or read it from disk if not in cache or expired
        /// </summary>
        private static CachedFile GetCachedFile(IMockFile file)
        {
            // Use the file name as the cache key
            var cacheKey = file.Name;
            
            // Check if we have a valid cached version
            if (FileCache.TryGetValue(cacheKey, out var cachedFile))
            {
                // Return cached content if it's not expired
                if (DateTime.UtcNow - cachedFile.LastModified < CacheExpiration)
                {
                    return cachedFile;
                }
                
                // Remove expired entry
                FileCache.TryRemove(cacheKey, out _);
            }
            
            // Need to read the file - use lock to prevent concurrent access
            lock (FileLock)
            {
                // Double-check the cache in case another thread updated it
                if (FileCache.TryGetValue(cacheKey, out cachedFile) && 
                    DateTime.UtcNow - cachedFile.LastModified < CacheExpiration)
                {
                    return cachedFile;
                }
                
                // Read file into memory
                using var fileStream = file.Open();
                using var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                
                // Create cached file object
                cachedFile = new CachedFile(
                    memoryStream.ToArray(),
                    file.MimeType,
                    DateTime.UtcNow
                );
                
                // Store in cache
                FileCache[cacheKey] = cachedFile;
                
                return cachedFile;
            }
        }
        
        /// <summary>
        /// Class to store cached file data
        /// </summary>
        private class CachedFile
        {
            public byte[] Content { get; }
            public string MimeType { get; }
            public DateTime LastModified { get; }
            
            public CachedFile(byte[] content, string mimeType, DateTime lastModified)
            {
                Content = content;
                MimeType = mimeType;
                LastModified = lastModified;
            }
        }
    }
}
