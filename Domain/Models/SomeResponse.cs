using Domain.Enums;
using System.Collections.Generic;

namespace Domain.Models
{
    public class SomeResponse
    {
        public SubDomainType Type { get; set; }

        public List<DocumentDetails> Documents { get; set; }

        public bool? IsValid { get; set; }

        // Error details etc
    }
}