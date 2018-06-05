namespace Dragos.Net.Client.Html.Tags
{
    public interface IEntry :INode
    {
        string Name { get; }

        string Value { get; }

        IAttributes Attributes { get; }
    }
}
