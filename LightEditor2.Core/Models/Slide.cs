// Slide.cs

namespace LightEditor2.Core.Models
{
    public class Slide
    {
        public int Id {  get; set; }
        public int SubGroupId { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string Prompt { get; set; }
    }
}
