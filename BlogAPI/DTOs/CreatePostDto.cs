using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs
{
    public class CreatePostDto
    {
        [MinLength(3)]
        [MaxLength(100)]
        public required string Title { get; set; }
        
        [MinLength(10)]
        public required string Content { get; set; }
    }
}