
using System;

namespace Dragos.Net.Client.Html.Tags
{
    public class SingleTag : ITag
    {
        public IAttributes Attributes { get; }
        public string TagName { get; }
        public IParentTag Parent { get; private set; }
        public DocInfo DocInfo { get; private set; }

        public SingleTag(string tagName, IAttributes attributes,DocInfo docInfo)
        {
            TagName = tagName;
            Attributes = attributes;
            this.DocInfo = docInfo;
        }

        public void SetParent(IParentTag parent)
        {
            Parent = parent;
        }

        public static string[] SingleElements()
        {
            return new[] { "input", "meta", "br", "hr","img","link" };
        }

        public override string ToString()
        {
            var result = "<" + TagName;
            foreach (var attribute in Attributes)
            {
                result += " " + attribute.Key;
                if (!string.IsNullOrWhiteSpace(attribute.Key))
                    result += "='" + attribute.Value + "'";
            }
            result += " />";
            return result;
        }

        public void Apply(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
