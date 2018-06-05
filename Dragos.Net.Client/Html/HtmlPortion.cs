using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dragos.Net.Client.Html
{
    public class CharIndex
    {
        public CharIndex(char value, int index)
        {
            Value = value;
            Index = index;
        }

        public char Value { get;}
        public int Index { get;  }
    }

    public class HtmlPortion  
    {
        public string Value { get; }
        public int Current { get; private set; } = 0;

        public int Length => Value.Length;

        public char Char
        {
            get
            {
                return Value[Current];
            }
        }

        public char NextChar => Value[Current + 1];

        public char BeforeChar => Value[Current - 1];

        public bool IsEnd => Current >= (Value.Length -1);

        public bool HasNext => !IsEnd;

        public string NextWith
        {
            get { return GetText(20); }
        }

        public string BeforeWith
        {
            get
            {
                return this.Memento(Current - 20).GetText(20);
            }
        }

        public bool IsWhiteSpace
        {
            get { return char.IsWhiteSpace(Char); }
        }

        
        public HtmlPortion(string value,int current =0)
        {
            Value = value;
            Current = current;
        }

        public bool IsStartTagChar()
        {
            if (IsEnd) return false;
            return Char == '<';
        }

        public HtmlPortion Memento(int current)
        {
            return new HtmlPortion(Value, current);
        }

        public HtmlPortion Memento()
        {
            return Memento(Current);
        }


        public bool Is(char ch)
        {
            return Value[Current] == ch;
        }
   

        public void Jump(int index)
        {
            if(Current > index) return;
            var len = index;
            if (Value.Length <= len)
                Current = Value.Length - 1;
            if (index < 0)
                Current = 0;
            Current = len;
        }

        public void Jump()
        {
            Jump(NextNonWhiteSpace());
        }

        public int NextNonWhiteSpace()
        {
            var firsOrDefault = AsEnumerable(Current + 1).FirstOrDefault(x => !char.IsWhiteSpace(x.Value));
            if (firsOrDefault == null) return Length - 1;
            return firsOrDefault.Index;
        }

        public char NextNonWhiteSpaceChar()
        {
            return Value[NextNonWhiteSpace()];
        }

        public char Get(int index)
        {
            return Value[index];
        }

        public bool NextIfWhiteSpace()
        {
            if (IsWhiteSpace)
            {
                Next();
                return true;
            }
            return false;
        }

        //public void Jump(string str,StringComparison type)
        //{
        //    var result = string.Empty;
        //    for (var i = Current; i < Value.Length; i += str.Length)
        //    {
        //        result += Value[i];
        //        var index = result.IndexOf(str, type);
        //        if(index == -1)continue;
        //        Jump(Current + index);
        //        return;
        //    }
        //}

        public bool Is(string str,StringComparison type = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null) return false;
            if (MoreThan(Current + str.Length)) return false;
            var next = this.Substring(str.Length);
            return (next).Equals(str, type);
        }

        public bool MoreThan(int len)
        {
            return len > Length;
        }


        public bool IsNext(char ch,char? exceptional = null, int nextCount = 1)
        {
            if (MoreThan(Current + nextCount)) return false;
            var r = Value[Current + nextCount];
            if (exceptional == r)
                return IsNext(ch, exceptional, nextCount + 1);
            return r == ch;
        }

        //public bool IsNext(Regex regex)
        //{
        //    return regex.IsMatch(GetRightText());
        //}

        //public void Jump(Regex regex)
        //{
        //    Jump(IndexOf(regex));
        //}

        public void Next()
        {
            Next(1);
        }


        public void Next(int value)
        {
            if (value <= 0) return;
            Jump(Current + value);
        }

        public string GetWord()
        {
            return Value.Substring(LastWhiteSpaceIndex(), NextWhiteSpaceIndex());
        }

 
        public int LastWhiteSpaceIndex()
        {
            for (var i = Current; i > 0; i--)
            {
                if (char.IsWhiteSpace(Value[i]))
                    return i;
            }
            return -1;
        }

        public int NextWhiteSpaceIndex()
        {
            for(var i=Current;i<Value.Length;i++)
                if (char.IsWhiteSpace(Value[i]))
                    return i;
            return -1;
        }


        public int IndexOf(char value)
        {
            for(var i=Current;i<Value.Length;i++)
                if (value == Value[i])
                    return i;
            return -1;
        }

        public int IndexOf(string value)
        {
            return IndexOf(Current, value);
        }

        public int IndexOf(int startIndex, Regex regex)
        {
            if (startIndex == -1)
                startIndex = Current;
            var result = string.Empty;
            for (var i = startIndex; i < Value.Length; i++)
            {
                result += Value[i];
                if (!regex.IsMatch(result)) continue;
                return i;
            }
            return -1;
        }

        public int StartIndexOf(Regex regex)
        {
            return StartIndexOf(Current, regex);
        }
        public int StartIndexOf(int startIndex, Regex regex)
        {
            if (startIndex == -1)
                startIndex = Current;
            var result = string.Empty;
            for (var i = startIndex; i < Value.Length; i++)
            {
                result += Value[i];
                if (!regex.IsMatch(result)) continue;
                return i - regex.Match(result).Value.Length;
            }
            return -1;
        }

        public int IndexOf(Regex regex)
        {
            return IndexOf(Current, regex);
        }

        public string GetText(Regex regex)
        {
            var result = string.Empty;
            for (var i = Current; i < Value.Length; i++)
            {
                result += Value[i];
                if (!regex.IsMatch(result)) continue;
                return result;
            }
            return null;
        }



        //public int StartIndexOf(int start,Regex regex)
        //{
        //    if (start == -1)
        //        start = Current;
        //    var result = string.Empty;
        //    for (var i = start; i < Value.Length; i++)
        //    {
        //        result += Value[i];
        //        if (!regex.IsMatch(result)) continue;
        //        return i;
        //    }
        //    return -1;
        //}
        public int IndexOf(int startIndex, string value,StringComparison type = StringComparison.Ordinal)
        {
            if (startIndex == -1)
                startIndex = Current;
            var result = string.Empty;
            for (var i = startIndex; i < Value.Length; i++)
            {
                result += Value[i];
                var index = result.IndexOf(value, type);
                if (index == -1) continue;
                return i;
            }
            return -1;
        }

        public string GetText(int len)
        {
            var st = new StringBuilder();
            
            for (var i = Current; i < GetTotalLen(Current + len); i++)
            {
                st.Append(Value[i]);
            }
            return st.ToString();
        }

        private int GetTotalLen(int len)
        {
            if (MoreThan(len)) return Length;
            return len;
        }

        //public int IndexOf(Regex regex)
        //{
        //    var text = GetRightText();
        //    if (regex.IsMatch(text)) return -1;
        //    var index = regex.Match(text).Index;
        //    return Current + index;
        //}

        public string Substring(char ch)
        {
            var indexOf = IndexOf(ch);
            return GetText(indexOf-Current+1);
        }

        public string Substring(int start, int end)
        {
            var st = new StringBuilder();
            for (var i = start; i <= start + end; i++)
                st.Append(Value[i]);
            return st.ToString();
        }

        public string Substr(int start, int end)
        {
            return Substring(start, end- start);
        }

        public string Substr(int end)
        {
            return Substr(Current, end);
        }

        public string Substring(int end)
        {
            return GetText(end);
        }

        public bool BeforeThan(string value,char left, char right)
        {
            var leftIndex = value.IndexOf(left);
            if (leftIndex == -1) return false;
            var rightIndex = value.IndexOf(right);
            if (rightIndex == -1) return false;
            return leftIndex < rightIndex;
        }

        public IEnumerable<CharIndex> AsEnumerable()
        {
            return AsEnumerable(Current);
        }

        public IEnumerable<CharIndex> AsEnumerable(int start)
        {
            for (var i = start; i < Value.Length; i++)
                yield return new CharIndex(Value[i], i);
        }

        public char this[int index]
        {
            get { return Get(index); }
        }


        //public string GetRightText()
        //{
        //    var result = string.Empty;
        //    for (var i = Current; i < Value.Length; i++)
        //        result += Value[i];
            
        //    return result;
        //}

    }

}
