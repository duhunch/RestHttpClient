namespace Dragos.Net.Client.Html.Tags
{
    public class Select : PairTag, IEntry
    {
 
        public string Name =>  this.Attributes["name"];
        public string Value => this.Attributes["value"];

        public Select(string tagName, IAttributes attributes, DocInfo docInfo) : base(tagName, attributes, docInfo)
        {
        }

    }
}
