using System;

namespace Dragos.Net.Client.Html.Tags
{
    public class IgnoreElement : IIgnoreElement, IParseBreak
    {
        public IParentTag Parent { get; } = null;

        public DocInfo DocInfo
        {
            get { throw new NotImplementedException(); }
        }

        public void Apply(INodeVisitor visitor)
        {
            
        }

        public void SetParent(IParentTag parent)
        {

        }
    }
}
