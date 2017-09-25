using System.Collections.Generic;
using Hepsiburada.Zipkin.Models.Annotations;

namespace Hepsiburada.Zipkin.Models.Spans
{
    public class Span
    {
        public string TraceId { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public IList<AnnotationBase> Annotations { get; } = new List<AnnotationBase>();
    }
}
