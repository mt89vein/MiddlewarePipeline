using System.Collections.Generic;

namespace Domain.Models
{
    public class SomeRequest
    {
        public IReadOnlyList<Document> Documents { get; init; } = new List<Document>();
    }
}