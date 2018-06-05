namespace Dragos.Net.Client.Html
{
    public interface IParentTag
    {
        string TagName { get; }
        void Append(INode tag);
    }
}
