using Infrastructure.Entities;

namespace ApplicationCore.Models.Data
{
    public class TermNotes : BaseDocument
    {
        public int SubjectId { get; set; }

        public int TermId { get; set; }

        public string? RQIds { get; set; } = string.Empty;//歷屆試題

        public string? QIds { get; set; } = string.Empty;//普通試題
    }
}
