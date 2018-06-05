using System;

namespace Dragos.Net.Client.Html.Tags
{
    public class StyleTag :ITag
    {
        public IParentTag Parent { get; private set; }
        public IAttributes Attributes { get; }
        public string TagName { get; }
        public string InnerText { get; }

        public DocInfo DocInfo { get; private set; }

        public StyleTag(string tagName, IAttributes attributes, string innerText,DocInfo docInfo)
        {
            Attributes = attributes;
            InnerText = innerText;
            TagName = tagName;
            DocInfo = docInfo;
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
            result += ">";

            result += System.Environment.NewLine + InnerText;
            result += "</" + TagName + ">";
            return result;
        }

        public void SetParent(IParentTag parent)
        {
            Parent = parent;
        }

        public void Apply(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
