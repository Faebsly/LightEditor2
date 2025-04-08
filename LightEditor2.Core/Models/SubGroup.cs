// SubGroup.cs

namespace LightEditor2.Core.Models
{
    public class SubGroup
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public List<Slide> Slides { get; set; } = new List<Slide>();
    }
}
