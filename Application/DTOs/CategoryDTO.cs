﻿namespace Application.DTOs
{
    public class CategoryDTO
    {
        public Guid Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public IEnumerable<VideoDTO> Videos { get; set; }
    }
}
