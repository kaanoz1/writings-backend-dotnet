using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace writings_backend_dotnet.Models
{
    public class Section
    {
        [Key, Column("id", TypeName = "smallint")]
        public short Id { get; set; }

        [Required, Column("section_name", TypeName = "varchar(100)")]
        public required string Name { get; set; }

        [Required, Column("section_number", TypeName = "smallint")]
        public required short Number { get; set; }

        [NotMapped]
        public short ChapterCount => (short)(Chapters?.Count ?? -1);

        [Required, Column("scripture_id", TypeName = "smallint")]
        public required short ScriptureId { get; set; }

        public required Scripture Scripture { get; set; }

        public List<Chapter> Chapters { get; set; } = [];

        public List<SectionMeaning> Meanings { get; set; } = [];

    }
}