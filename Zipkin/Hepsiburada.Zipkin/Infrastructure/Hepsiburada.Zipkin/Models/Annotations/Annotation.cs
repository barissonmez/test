using System;

namespace Hepsiburada.Zipkin.Models.Annotations
{
    public class Annotation: AnnotationBase
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        public string Value { get; set; }

        public int DurationMilliseconds { get; set; }
    }
}
