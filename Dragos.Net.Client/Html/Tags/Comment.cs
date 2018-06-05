using System;

namespace Dragos.Net.Client.Html.Tags
{
    public class Comment :INode
    {
        public IParentTag Parent { get; private set; }
        public string CommentText { get; private set; }

        public DocInfo DocInfo { get; private set; }

        public Comment(string comment,DocInfo docInfo)
        {
            CommentText = comment;
            DocInfo = docInfo;
        }

       
        public void SetParent(IParentTag parent)
        {
            Parent = parent;
        }

        public override string ToString()
        {
            return CommentText;
        }

        public void Apply(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
