namespace Application.Storage
{
    public sealed class MinioOptions
    {
        public string ServiceUrl { get; init; } = default!;
        public string AccessKey { get; init; } = default!;
        public string SecretKey { get; init; } = default!;
        public string Bucket { get; init; } = default!;
        public bool ForcePathStyle { get; init; } = true;
    }
}
