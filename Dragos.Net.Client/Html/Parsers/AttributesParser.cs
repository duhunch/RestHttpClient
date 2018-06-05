
using System.Linq;
using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Exception;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{

    internal enum AttributeState
    {
        EndSingle,
        End,
    }

    internal enum AttributeNameValue
    {
        Key = 1,
        Value =2
    }

    internal class AttributeStringProcess
    {
        private readonly HtmlPortion _portion;
        internal AttributeStringProcess(HtmlPortion portion)
        {
            _portion = portion;
        }

        private char Start(HtmlPortion portion)
        {
            while (portion.HasNext)
            {
                if (portion.IsWhiteSpace)
                {
                    portion.Next();
                    continue;
                }
                if (!IsValueStart(portion))
                    throw new HtmlParseException("attribute value is not valid");
                var result = portion.Char;
                portion.Next();
                return result;
            }
            throw new HtmlParseException("attribute value is not valid");
        }

      

        public string GetValue()
        {
            Start(_portion);
            var result = string.Empty;
            while (_portion.HasNext)
            {
                if (IsValueStart(_portion))
                {
                    return result;
                }
                else
                {
                    result += _portion.Char;
                    _portion.Next();
                }

            }
            throw new HtmlParseException("attribute value is not valid");
        }

        public static bool IsValueStart(HtmlPortion portion)
        {
            if ((portion.Char == '\'' || portion.Char == '"') && portion.BeforeChar != '\\') return true;
            return false;
        }

        public static bool IsValueStart(char ch)
        {
            return (ch == '\'' || ch == '"');
        }

        


    }

    public class AttributeParseResult
    {
        public IAttributes Attributes { get; }
        public string TagName { get; }

        public bool LocalSingle { get; private set; }

        public bool IsSingle
        {
            get
            {

                var isSingle = SingleTag.SingleElements().Contains(TagName.ToLower());
                if (isSingle) return true;
                return LocalSingle;
            }
        }

        public AttributeParseResult(bool isSingle,string tagName,IAttributes attribute)
        {
            this.LocalSingle = isSingle;
            this.TagName = tagName;
            this.Attributes = attribute;
        }


  
    }

    internal class AttributeSingle
    {
        public string Key { get; private set; } = null;
        public string Value { get; private set; } = null;

        private AttributeNameValue _type = AttributeNameValue.Key;

        public bool ValueEmpty
        {
            get { return string.IsNullOrWhiteSpace(Value); }
        }

        public bool KeyEmpty
        {
            get { return string.IsNullOrWhiteSpace(Key); }
        }

        public bool Empty
        {
            get { return ValueEmpty && KeyEmpty; }
        }

        public void SetType(AttributeNameValue type)
        {
            _type = type;
        }

        public void Insert(char ch)
        {
            if (_type == AttributeNameValue.Key)
                Key += ch;
            if (_type == AttributeNameValue.Value)
                Value += ch;
        }

        public void Insert(string ch)
        {
            if (_type == AttributeNameValue.Key)
                Key += ch;
            if (_type == AttributeNameValue.Value)
                Value += ch;
        }

        public bool TagNameIsValid()
        {
            return IsValidTagName(Key);
        }

        private static bool IsValidTagName(string tagName)
        {
            return new Regex(@"^[a-zA-Z]\w*$").IsMatch(tagName);
        }

        public Attribute Pull()
        {
            if (Key == null) return null;
            var result = new Attribute(Key, Value);
            Key = null;
            Value = null;
            return result;
        }
    }

   public class AttributesParser 
    {
        public AttributeParseResult Parse(HtmlPortion current)
        {
            if (!IsValid(current)) throw new HtmlParseException("tag is not valid");
            current.Next();
            var attributes = new Attributes();
            var attribute = new AttributeSingle();
            while (current.HasNext)
            {
                if (current.IsWhiteSpace)
                {
                    if (attribute.ValueEmpty && !attribute.KeyEmpty)
                        attributes.Add(attribute.Pull());
                    current.Jump();
                    continue;
                }

                if (current.Char == '/' && current[current.NextNonWhiteSpace()] == '>')
                {
                    var next = current.NextNonWhiteSpace();
                    current.Jump(next);
                    if (!attributes.Any())
                    {
                        if (attribute.ValueEmpty && !attribute.KeyEmpty)
                            attributes.Add(attribute.Pull());
                        if (!attributes.Any())
                            throw new HtmlParseException("tag is not valid");
                    }
                    var tagName = attributes.First().Key;
                    return new AttributeParseResult(true, tagName, new Attributes(attributes.Skip(1).ToArray()));
                }

                if (current.Char == '>')
                {
                    if (attributes.IsEmpty)
                    {
                        if (!attribute.Empty)
                            attributes.Add(attribute.Pull());
                    }
                    if (attributes.IsEmpty) throw new HtmlParseException("tag is not valid");
                    var tagName = attributes.First().Key;
                    return new AttributeParseResult(false,tagName, new Attributes(attributes.Skip(1).ToArray()));
                }

                if (current.Is('=') && AttributeStringProcess.IsValueStart(current.NextNonWhiteSpaceChar()))
                {
                    current.Jump();
                    attribute.SetType(AttributeNameValue.Value);
                    attribute.Insert(new AttributeStringProcess(current).GetValue());
                    attributes.Add(attribute.Pull());
                    attribute.SetType(AttributeNameValue.Key);
                    current.Jump();
                    continue;
                }
                attribute.Insert(current.Char);
                current.Next();
            }
            throw new HtmlParseException("tag is not valid");
        }



        private static bool IsSingleEnd(HtmlPortion portion)
        {
            return portion.IndexOf(new Regex(@"^\s*/>")) >= 0;
        }

        private static bool IsEnd(HtmlPortion portion)
        {
            return portion.IndexOf(new Regex(@"^\s*>")) >= 0;
        }

        private static int GetEndIndex(HtmlPortion portion)
        {
            return portion.IndexOf(new Regex(@"^\s*>"));
        }

        private static int GetEndSingleIndex(HtmlPortion portion)
        {
            return portion.IndexOf(new Regex(@"^\s*/>"));
        }

        private bool IsValid(HtmlPortion current)
        {
            return current.Char == '<' && current.NextChar !='/';
        }
    }
}
