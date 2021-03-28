using Domain.Enums;
using Domain.Models;

namespace Domain.Subdomain1.Models
{
    public class Subdomain1Document : DocumentDetails
    {
        public override DocumentType Type { get; } = DocumentType.Document1;

        public string AdditionalInfo { get; set; }
    }
}