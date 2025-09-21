namespace Application.DTOs
{
    public class TagDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int VideoCount { get; set; }
    }
}
