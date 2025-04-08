// Project.cs

namespace LightEditor2.Core.Models
{
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<SubGroup> SubGroups { get; set; } = new List<SubGroup>();
    }
}
