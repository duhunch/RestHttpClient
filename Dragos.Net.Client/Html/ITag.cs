using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html
{
    public interface INodeVisitor
    {
        void Visit(SingleTag host);
        void Visit(PairTag host);
        void Visit(Comment host);
        void Visit(Text host);
        void Visit(ScriptTag host);
        void Visit(StyleTag host);
        void Visit(INode host);
    }


    public interface INode 
    {
        DocInfo DocInfo { get; }
        IParentTag Parent { get; }
        void SetParent(IParentTag parent);
        void Apply(INodeVisitor visitor);

    }

    public interface ITag : INode
    {
      IAttributes Attributes { get; }
      string TagName { get; }
    }
}
