using Hepsiburada.Zipkin.Models.Endpoints;

namespace Hepsiburada.Zipkin.Models.Annotations
{
    public abstract class AnnotationBase
    {
        public Endpoint Host { get; set; }
    }
}
