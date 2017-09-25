using Hepsiburada.Zipkin.Enumeration;
using Hepsiburada.Zipkin.Extensions;

namespace Hepsiburada.Zipkin.Models.Annotations
{
    public class BinaryAnnotation: AnnotationBase
    {
        public string Key { get; set; }

        public object Value { get; set; }

        public AnnotationType AnnotationType => Value.GetType().AsAnnotationType();
    }
}
