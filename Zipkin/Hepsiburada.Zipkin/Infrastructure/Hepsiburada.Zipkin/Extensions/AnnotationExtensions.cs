using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hepsiburada.Zipkin.Enumeration;
using Hepsiburada.Zipkin.Models.Annotations;
using Hepsiburada.Zipkin.Models.Spans;

namespace Hepsiburada.Zipkin.Extensions
{
    public static class AnnotationExtensions
    {
        private static readonly Dictionary<Type, AnnotationType> annotationTypeMappings =
              new Dictionary<Type, AnnotationType>()
              {
                { typeof(bool), AnnotationType.Boolean },
                { typeof(byte[]), AnnotationType.ByteArray },
                { typeof(short), AnnotationType.Int16 },
                { typeof(int), AnnotationType.Int32 },
                { typeof(long), AnnotationType.Int64 },
                { typeof(double), AnnotationType.Double },
                { typeof(string), AnnotationType.String }
              };

        public static AnnotationType AsAnnotationType(this Type type)
        {
            return annotationTypeMappings.ContainsKey(type) ? annotationTypeMappings[type] : AnnotationType.String;
        }

        public static IEnumerable<TAnnotation> GetAnnotationsByType<TAnnotation>(this Span span)
            where TAnnotation : AnnotationBase
        {
            return span.Annotations.OfType<TAnnotation>();
        }
    }
}
