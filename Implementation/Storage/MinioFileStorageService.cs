using Application.Storage;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Implementation.Storage
{
    public class MinioFileStorageService : IFileStorageService
    {
        private readonly IMinioClient _client;
        private readonly string _bucket;

        public MinioFileStorageService(IMinioClient client, IOptions<MinioOptions> options)
        {
            _client = client;
            _bucket = options.Value.Bucket;
        }

        public async Task UploadAsync(string objectName, Stream data, long size, string contentType, CancellationToken ct = default)
        {
            var args = new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(size)
                .WithContentType(contentType);

            await _client.PutObjectAsync(args, ct);
        }

        public async Task<Stream> DownloadAsync(string objectName, CancellationToken ct = default)
        {
            var stream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
                .WithCallbackStream(s => s.CopyTo(stream));

            await _client.GetObjectAsync(args, ct);
            stream.Position = 0;
            return stream;
        }

        public async Task DeleteAsync(string objectName, CancellationToken ct = default)
        {
            var args = new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName);

            await _client.RemoveObjectAsync(args, ct);
        }

        public async Task<string> GetPresignedUrlAsync(string objectName, int expirySeconds = 3600, CancellationToken ct = default)
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
                .WithExpiry(expirySeconds);

            return await _client.PresignedGetObjectAsync(args);
        }
    }
}
