namespace Application.UseCases.Commands.Requests.Document
{
    public class DocumentFileInput
    {
        public Stream Content { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long SizeInBytes { get; set; }
        public int Order { get; set; }
    }
}
