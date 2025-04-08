// Components/Models/Setting.cs
using System.ComponentModel.DataAnnotations; // Für [Key]

namespace LightEditor2.Core.Models
{
    public class Setting
    {
        [Key] // Macht 'Key' zum Primärschlüssel
        public required string Key { get; set; }

        public string? Value { get; set; } // Wert kann null sein
    }
}