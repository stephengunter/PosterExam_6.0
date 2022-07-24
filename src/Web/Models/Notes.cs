using ApplicationCore.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class NotesIndexModel
    {
        public List<NoteCategoryViewModel> Categories { get; set; }

        public NoteParamsViewModel Params { get; set; }
    }
}
