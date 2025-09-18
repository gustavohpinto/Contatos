using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Application.DTOs
{
    public class ContactResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string Sex { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
