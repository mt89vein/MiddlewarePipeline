using Domain.Enums;
using System;

namespace Domain.Models
{
    public abstract class DocumentDetails
    {
        public string FileName { get; init; }

        public Guid FileId { get; init; }

        public abstract DocumentType Type { get; }

        // other attributes...
    }
}