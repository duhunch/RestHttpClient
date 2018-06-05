using System;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Selector
{
    public class NodeEachVisitor : INodeVisitor
    {
        public ITag Result { get; private set; }

        public void Visit(SingleTag host)
        {
           
        }

        public void Visit(PairTag host)
        {
            throw new NotImplementedException();
        }

        public void Visit(Comment host)
        {
            throw new NotImplementedException();
        }

        public void Visit(Text host)
        {
            throw new NotImplementedException();
        }

        public void Visit(ScriptTag host)
        {
            throw new NotImplementedException();
        }

        public void Visit(StyleTag host)
        {
            throw new NotImplementedException();
        }

        public void Visit(INode host)
        {
            throw new NotImplementedException();
        }
    }
}
