using System;

namespace Dragos.Net.Client.Html.Tags
{
    public class Text :INode
    {
        public IParentTag Parent { get; private set; }
        public string Content { get; private set; }

        public DocInfo DocInfo { get; private set; }

        public Text(string content,DocInfo info)
        {
            Content = content;
            this.DocInfo = info;
        }
        public void SetParent(IParentTag parent)
        {
            this.Parent = parent;
        }

        public override string ToString()
        {
            return Content;
        }

        public void Apply(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
