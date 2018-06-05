using System;
using System.Collections.Generic;
using System.Linq;

namespace Dragos.Net.Client.Html.Tags
{
    public class PairTag : ITag, IParentTag
    {
        public IAttributes Attributes { get; }
        public string TagName { get; }
        public IParentTag Parent { get; private set; }
        private readonly List<INode> _nodes = new List<INode>();

        public INode[] Nodes => _nodes.ToArray();

        public string InnerText
        {
            get
            {
                var result = string.Empty;
                foreach (var t in Elements<Text>())
                {
                    result += t.Content;
                }
                return result;
            }
        }

        public DocInfo DocInfo { get; }

        public PairTag(string tagName, IAttributes attributes,DocInfo docInfo)
        {
            TagName = tagName;
            Attributes = attributes;
            DocInfo = docInfo;
        }
        public virtual void Append(INode node)
        {
           
            if (node is IIgnoreElement) return;
            node.SetParent(this);
            this._nodes.Add(node);
        }

        public void SetParent(IParentTag parent)
        {
            Parent = parent;
        }

        public IEnumerable<Type> Children<Type>() where Type:INode
        {
            return Children<Type>(x => true);
        }

        public IEnumerable<Type> Children<Type>(System.Func<Type,bool> selector) where Type:INode
        {
         
            foreach (var node in Nodes)
            {
                var typeIsTag = typeof(Type).IsAssignableFrom(node.GetType());
                if (typeIsTag && selector((Type)node))
                    yield return (Type) node;
            }
        }

        public Type Element<Type>() where Type : INode
        {
            return this.Elements<Type>().FirstOrDefault();
        }

        public Type Element<Type>(System.Func<Type,bool> selector) where Type:INode
        {
            return this.Elements<Type>(selector).FirstOrDefault();
        }

       

        public IEnumerable<ITag> Children(System.Func<ITag, bool> selector) 
        {
            return Children<ITag>(selector);
        }

        public IEnumerable<Type> Elements<Type>(System.Func<Type, bool> selector) where Type : INode
        {
            foreach (var node in Nodes)
            {
                var typeIsTag = typeof(Type).IsAssignableFrom(node.GetType());
                if (typeIsTag && selector((Type)node))
                    yield return (Type)node;
                var isPair = node as PairTag;
                if (isPair != null)
                    foreach (var item in isPair.Elements<Type>(selector))
                        yield return item;
               
            }
        }

        public IEnumerable<Type> Elements<Type>() where Type : INode
        {
            return Elements<Type>(x => true);
        }

        public bool Exists<Type>() where Type : INode
        {
            return Element<Type>() != null;
        }

        public bool Exists<Type>(System.Func<Type, bool> selector) where Type : INode
        {
            return Element<Type>(selector) != null;
        }

        public bool Exists(System.Func<ITag, bool> selector)
        {
            return Exists<ITag>(selector);
        }

        public IEnumerable<ITag> Elements(System.Func<ITag, bool> selector)
        {
            return Elements<ITag>(selector);
        }

        public override string ToString()
        {
            var result = "<" + TagName;
            foreach (var attribute in Attributes)
            {
                result += " " + attribute.Key;
                if (!string.IsNullOrWhiteSpace(attribute.Key))
                    result += "='" + attribute.Value + "'";
            }
            result += ">";
            foreach (var node in Nodes)
            {
                result += Environment.NewLine + node;
            }

            result += "</" + TagName + ">";
            return result;
        }

        public void Apply(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
