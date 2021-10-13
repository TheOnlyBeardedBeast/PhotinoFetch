
namespace Photino.Access
{
    public class PhotinoRequest<T>
    {
        public string Id { get; set; }
        public T Body { get; set; }
    }
}
