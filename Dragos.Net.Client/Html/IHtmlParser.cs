namespace Dragos.Net.Client.Html
{
    public interface IHtmlParser
    {
        INode Parse(HtmlParser parser,HtmlPortion current);
    }

    public interface IIgnoreElement :INode
    {
        
    }

    public interface IParseBreak
    {
        
    }


}
