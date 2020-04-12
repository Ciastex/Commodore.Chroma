using System.Xml.Linq;

namespace Commodore.Engine.FrameworkExtensions
{
    public static class XDocumentExtensions
    {
        public static bool HasAttribute(this XElement xElement, string name)
        {
            return xElement.Attribute(name) != null;
        }

        public static bool HasElement(this XElement xElement, string name)
        {
            return xElement.Element(name) != null;
        }
    }
}
