using System;

namespace Domain.Models
{
    public class Document
    {
        public string FileName { get; init; }

        public Guid FileId { get; init; }
    }
}