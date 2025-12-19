namespace Application.DTOs
{
    public class ThumbnailFileData
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }

        public ThumbnailFileData(Stream stream, string fileName, long length)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Length = length;
        }
    }
}
