namespace Dragos.Net.Client.Html.Tags
{
    public class Input : SingleTag, IEntry
    {
        public string Name => this.Attributes["name"];
        public string Value => this.Attributes["value"];
        public string Type => this.Attributes["type"];
        public Input(string tagName, IAttributes attributes, DocInfo docInfo) : base(tagName, attributes, docInfo)
        {
        }
    }
}
