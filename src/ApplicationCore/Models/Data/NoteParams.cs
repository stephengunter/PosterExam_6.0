using Infrastructure.Entities;

namespace ApplicationCore.Models.Data 
{ 
    public class NoteParams : BaseDocument
    {
        public string? UserId { get; set; } = string.Empty;
    }
}
