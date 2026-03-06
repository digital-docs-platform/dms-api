namespace Application.Storage
{
    public interface IFileStorageService
    {
        Task UploadAsync(string objectName, Stream data, long size, string contentType, CancellationToken ct = default);
        Task<Stream> DownloadAsync(string objectName, CancellationToken ct = default);
        Task DeleteAsync(string objectName, CancellationToken ct = default);
        Task<string> GetPresignedUrlAsync(string objectName, int expirySeconds = 3600, CancellationToken ct = default);
    }
}
