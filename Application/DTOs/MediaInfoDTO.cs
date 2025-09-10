namespace Application.DTOs
{
    public class MediaInfoDTO
    {
        /// <summary>
        /// Video duration in seconds
        /// </summary>
        public int Duration { get; set; }
        
        /// <summary>
        /// Video's Codec
        /// </summary>
        public string Codec { get; set; } = string.Empty;
        
        /// <summary>
        /// Video's width in pixel
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Video's height in pixel
        /// </summary>
        public int Height { get; set; }

        public string FormatName { get; set; } = string.Empty;
    }
}
