/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * [2012-06-09 First Version]
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser now properly identifies types. So generating JSON with this framework should work.
 * - only double quotes (") are used for quoting strings.
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser tries to avoid errors, but if malformed JSON is parsed the result is more or less undefined
 * - It can serialize/deserialize a node tree into/from an experimental compact binary format. It might
 *   be handy if you want to store things in a file and don't want it to be easily modifiable
 * 
 * [2012-12-17 Update]
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * [2017-03-08 Update]
 * - Optimised parsing by using a StringBuilder for token. This prevents performance issues when large
 *   string data fields are contained in the json data.
 * - Finally refactored the badly named JSONClass into JSONObject.
 * - Replaced the old JSONData class by distict typed classes ( JSONString, JSONNumber, JSONBool, JSONNull ) this
 *   allows to propertly convert the node tree back to json without type information loss. The actual value
 *   parsing now happens at parsing time and not when you actually access one of the casting properties.
 * 
 * [2017-04-11 Update]
 * - Fixed parsing bug where empty string values have been ignored.
 * - Optimised "ToString" by using a StringBuilder internally. This should heavily improve performance for large files
 * - Changed the overload of "ToString(string aIndent)" to "ToString(int aIndent)"
 * 
 * [2017-11-29 Update]
 * - Removed the IEnumerator implementations on JSONArray & JSONObject and replaced it with a common
 *   struct Enumerator in JSONNode that should avoid garbage generation. The enumerator always works
 *   on KeyValuePair<string, JSONNode>, even for JSONArray.
 * - Added two wrapper Enumerators that allows for easy key or value enumeration. A JSONNode now has
 *   a "Keys" and a "Values" enumerable property. Those are also struct enumerators / enumerables
 * - A KeyValuePair<string, JSONNode> can now be implicitly converted into a JSONNode. This allows
 *   a foreach loop over a JSONNode to directly access the values only. Since KeyValuePair as well as
 *   all the Enumerators are structs, no garbage is allocated.
 * - To add Linq support another "LinqEnumerator" is available through the "Linq" property. This
 *   enumerator does implement the generic IEnumerable interface so most Linq extensions can be used
 *   on this enumerable object. This one does allocate memory as it's a wrapper class.
 * - The Escape method now escapes all control characters (# < 32) in strings as uncode characters
 *   (\uXXXX) and if the static bool JSONNode.forceASCII is set to true it will also escape all
 *   characters # > 127. This might be useful if you require an ASCII output. Though keep in mind
 *   when your strings contain many non-ascii characters the strings become much longer (x6) and are
 *   no longer human readable.
 * - The node types JSONObject and JSONArray now have an "Inline" boolean switch which will default to
 *   false. It can be used to serialize this element inline even you serialize with an indented format
 *   This is useful for arrays containing numbers so it doesn't place every number on a new line
 * - Extracted the binary serialization code into a seperate extension file. All classes are now declared
 *   as "partial" so an extension file can even add a new virtual or abstract method / interface to
 *   JSONNode and override it in the concrete type classes. It's of course a hacky approach which is
 *   generally not recommended, but i wanted to keep everything tightly packed.
 * - Added a static CreateOrGet method to the JSONNull class. Since this class is immutable it could
 *   be reused without major problems. If you have a lot null fields in your data it will help reduce
 *   the memory / garbage overhead. I also added a static setting (reuseSameInstance) to JSONNull
 *   (default is true) which will change the behaviour of "CreateOrGet". If you set this to false
 *   CreateOrGet will not reuse the cached instance but instead create a new JSONNull instance each time.
 *   I made the JSONNull constructor private so if you need to create an instance manually use
 *   JSONNull.CreateOrGet()
 * 
 * [2018-01-09 Update]
 * - Changed all double.TryParse and double.ToString uses to use the invariant culture to avoid problems
 *   on systems with a culture that uses a comma as decimal point.
 * 
 * [2018-01-26 Update]
 * - Added AsLong. Note that a JSONNumber is stored as double and can't represent all long values. However
 *   storing it as string would work.
 * - Added static setting "JSONNode.longAsString" which controls the default type that is used by the
 *   LazyCreator when using AsLong
 * 
 * [2018-04-25 Update]
 *  - Added support for parsing single values (JSONBool, JSONString, JSONNumber, JSONNull) as top level value.
 * 
 * [2019-02-18 Update]
 *  - Added HasKey(key) and GetValueOrDefault(key, default) to the JSONNode class to provide way to read
 *    values conditionally without creating a LazyCreator
 * 
 * [2019-03-25 Update]
 *  - Added static setting "allowLineComments" to the JSONNode class which is true by default. This allows
 *    "//" line comments when parsing json text as long as it's not within quoted text. All text after // up
 *    to the end of the line is completely ignored / skipped. This makes it easier to create human readable
 *    and editable files. Note that stripped comments are not read, processed or preserved in any way. So
 *    this feature is only relevant for human created files.
 *  - Explicitly strip BOM (Byte Order Mark) when parsing to avoid getting it leaked into a single primitive
 *    value. That's a rare case but better safe than sorry.
 *  - Allowing adding the empty string as key
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2012-2017 Markus Göbel (Bunny83)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleJSON
{
    public enum JSONNodeType
    {
        Array = 1,
        Object = 2,
        String = 3,
        Number = 4,
        NullValue = 5,
        Boolean = 6,
        None = 7,
        Custom = 0xFF,
    }
    public enum JSONTextMode
    {
        Compact,
        Indent
    }

    public class JsonQueue<T> where T : class
    {
        T[] Queue;
        volatile int nIn = 0;   // 入队数  单线程，多线程需实现原子计算
        volatile int nOut = 0;  // 出队数  单线程，多线程需实现原子计算
        public int Capacity
        {
            get { return Queue == null ? 0 : Queue.Length; }
            set
            {
                if (value == 0)
                {
                    Queue = null;
                    return;
                }
                if (Queue == null)
                {
                    Queue = new T[value];
                    return;
                }
                if (Queue.Length < value)
                {
                    int nInIdx = nIn % Queue.Length;
                    int nOutIdex = nOut % Queue.Length;
                    Array.Resize(ref Queue, value);
                    if (nInIdx >= nOutIdex)
                    {
                        nIn = nInIdx;
                        nOut = nOutIdex;
                    }
                    else
                    {
                        nIn = nInIdx + value;
                        nOut = nOutIdex;
                    }
                }
            }
        }

        public int Count
        {
            get { return nIn - nOut; }
        }

        public void Clear()
        {
            if (Queue != null)
            {
                Array.Clear(Queue, 0, Queue.Length);
            }
            nIn = 0;
            nOut = 0;
        }

        public bool Contains(T t)
        {
            if (nIn - nOut <= 0 || t == null)
            {
                return false;
            }

            int nLen = Capacity;
            for (int i = nOut; i < nIn; i++)
            {
                if (t == Queue[i % nLen])
                {
                    return true;
                }
            }
            return false;
        }

        public bool Push(T t)
        {
            if (Capacity == 0)
            {
                Capacity = 128;
            }
            int nLen = Capacity;
            if (nIn - nOut >= nLen)
            {
                return false;
            }
            Queue[nIn++ % nLen] = t;
            if (nIn >= int.MaxValue)
            {
                nIn = nLen;     // 不能直接置0 会导致出队问题
                nOut %= nLen;
            }
            return true;
        }

        public T Pop()
        {
            int nLen = Capacity;
            if (nLen == 0)
            {
                return null;
            }
            if (nOut >= nIn)
            {
                return null;
            }
            int idx = nOut++ % nLen;
            var ret = Queue[idx];
            Queue[idx] = null;
            return ret;
        }
    }

    public class JsonNodePool<T> where T : JSONNode, new()
    {
        private static readonly JsonQueue<T> s_Pool = new JsonQueue<T>();

        public JsonNodePool()
        {
            SetMaxCount(1024);
        }

        public static void SetMaxCount(int nCount)
        {
            s_Pool.Capacity = nCount;
        }

        public static T Pop()
        {
            T t = null;
            if (s_Pool.Count > 0)
            {
                t = s_Pool.Pop();
            }
            else
            {
                t = new T();
            }
            return t;
        }

        public static void Push(T data)
        {
            if (data == null)
            {
                return;
            }
            if (s_Pool.Contains(data))
            {
                return;
            }
            s_Pool.Push(data);
        }
    }

    public abstract partial class JSONNode : IDisposable
    {
        #region Enumerators
        public struct Enumerator
        {
            private enum Type { None, Array, Object }
            private Type type;
            private Dictionary<string, JSONNode>.Enumerator m_Object;
            private List<JSONNode>.Enumerator m_Array;
            public bool IsValid { get { return type != Type.None; } }
            public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
            {
                type = Type.Array;
                m_Object = default(Dictionary<string, JSONNode>.Enumerator);
                m_Array = aArrayEnum;
            }
            public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
            {
                type = Type.Object;
                m_Object = aDictEnum;
                m_Array = default(List<JSONNode>.Enumerator);
            }
            public KeyValuePair<string, JSONNode> Current
            {
                get {
                    if (type == Type.Array)
                        return new KeyValuePair<string, JSONNode>(string.Empty, m_Array.Current);
                    else if (type == Type.Object)
                        return m_Object.Current;
                    return new KeyValuePair<string, JSONNode>(string.Empty, null);
                }
            }
            public bool MoveNext()
            {
                if (type == Type.Array)
                    return m_Array.MoveNext();
                else if (type == Type.Object)
                    return m_Object.MoveNext();
                return false;
            }
        }
        public struct ValueEnumerator
        {
            private Enumerator m_Enumerator;
            public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            public ValueEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
            public JSONNode Current { get { return m_Enumerator.Current.Value; } }
            public bool MoveNext() { return m_Enumerator.MoveNext(); }
            public ValueEnumerator GetEnumerator() { return this; }
        }
        public struct KeyEnumerator
        {
            private Enumerator m_Enumerator;
            public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            public KeyEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
            public string Current { get { return m_Enumerator.Current.Key; } }
            public bool MoveNext() { return m_Enumerator.MoveNext(); }
            public KeyEnumerator GetEnumerator() { return this; }
        }

        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerable<KeyValuePair<string, JSONNode>>
        {
            private JSONNode m_Node;
            private Enumerator m_Enumerator;
            internal LinqEnumerator(JSONNode aNode)
            {
                m_Node = aNode;
                if (m_Node != null)
                    m_Enumerator = m_Node.GetEnumerator();
            }
            public KeyValuePair<string, JSONNode> Current { get { return m_Enumerator.Current; } }
            object IEnumerator.Current { get { return m_Enumerator.Current; } }
            public bool MoveNext() { return m_Enumerator.MoveNext(); }

            public void Dispose()
            {
                m_Node = null;
                m_Enumerator = new Enumerator();
            }

            public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
            {
                return new LinqEnumerator(m_Node);
            }

            public void Reset()
            {
                if (m_Node != null)
                    m_Enumerator = m_Node.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new LinqEnumerator(m_Node);
            }
        }

        #endregion Enumerators

        #region common interface

        public static bool forceASCII = false; // Use Unicode by default
        public static bool longAsString = false; // lazy creator creates a JSONString instead of JSONNumber
        public static bool allowLineComments = true; // allow "//"-style comments at the end of a line

        public abstract JSONNodeType Tag { get; }

        public virtual JSONNode this[int aIndex] { get { return null; } set { } }

        public virtual JSONNode this[string aKey] { get { return null; } set { } }

        public virtual string Value { get { return ""; } set { } }

        public virtual int Count { get { return 0; } }


        public virtual bool IsNumber { get { return false; } }
        public virtual bool IsString { get { return false; } }
        public virtual bool IsBoolean { get { return false; } }
        public virtual bool IsNull { get { return false; } }
        public virtual bool IsArray { get { return false; } }
        public virtual bool IsObject { get { return false; } }

        public virtual bool Inline { get { return false; } set { } }

        public void Dispose()
        {
            OnRecycle();
        }

        public abstract void OnRecycle();

        public virtual void Add(string aKey, JSONNode aItem)
        {
        }
        public virtual void Add(JSONNode aItem)
        {
            Add("", aItem);
        }

        public virtual JSONNode Remove(string aKey)
        {
            return null;
        }

        public virtual JSONNode Remove(int aIndex)
        {
            return null;
        }

        public virtual JSONNode Remove(JSONNode aNode)
        {
            return aNode;
        }

        public virtual IEnumerable<JSONNode> Children
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<JSONNode> DeepChildren
        {
            get
            {
                foreach (var C in Children)
                    foreach (var D in C.DeepChildren)
                        yield return D;
            }
        }

        public virtual bool HasKey(string aKey)
        {
            return false;
        }

        public virtual JSONNode GetValueOrDefault(string aKey, JSONNode aDefault)
        {
            return aDefault;
        }

        public override string ToString()
        {
            return "";
        }
        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, 0, JSONTextMode.Compact);
            return sb.ToString();
        }

        public virtual string ToJson(int aIndent)
        {
            StringBuilder sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, aIndent, JSONTextMode.Indent);
            return sb.ToString();
        }
        internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

        public abstract Enumerator GetEnumerator();
        public IEnumerable<KeyValuePair<string, JSONNode>> Linq { get { return new LinqEnumerator(this); } }
        public KeyEnumerator Keys { get { return new KeyEnumerator(GetEnumerator()); } }
        public ValueEnumerator Values { get { return new ValueEnumerator(GetEnumerator()); } }

        #endregion common interface

        #region typecasting properties


        public virtual double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(Value,NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                    return v;
                return 0.0;
            }
            set
            {
                Value = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public virtual int AsInt
        {
            get { return (int)AsDouble; }
            set { AsDouble = value; }
        }

        public virtual float AsFloat
        {
            get { return (float)AsDouble; }
            set { AsDouble = value; }
        }

        public virtual bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(Value, out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set
            {
                Value = (value) ? "true" : "false";
            }
        }

        public virtual long AsLong
        {
            get
            {
                long val = 0;
                if (long.TryParse(Value, out val))
                    return val;
                return 0L;
            }
            set
            {
                Value = value.ToString();
            }
        }

        public virtual JSONArray AsArray
        {
            get
            {
                return this as JSONArray;
            }
        }

        public virtual JSONObject AsObject
        {
            get
            {
                return this as JSONObject;
            }
        }


        #endregion typecasting properties

        #region operators

        public static implicit operator JSONNode(string s)
        {
            return JsonNodePool<JSONString>.Pop().InitData(s);
        }
        public static implicit operator string(JSONNode d)
        {
            return (d == null) ? null : d.Value;
        }

        public static implicit operator JSONNode(double n)
        {
            return JsonNodePool<JSONNumber>.Pop().InitData(n);
        }
        public static implicit operator double(JSONNode d)
        {
            return (d == null) ? 0 : d.AsDouble;
        }

        public static implicit operator JSONNode(float n)
        {
            return JsonNodePool<JSONNumber>.Pop().InitData(n);
        }
        public static implicit operator float(JSONNode d)
        {
            return (d == null) ? 0 : d.AsFloat;
        }

        public static implicit operator JSONNode(int n)
        {
            return JsonNodePool<JSONNumber>.Pop().InitData(n);
        }
        public static implicit operator int(JSONNode d)
        {
            return (d == null) ? 0 : d.AsInt;
        }

        public static implicit operator JSONNode(long n)
        {
            if (longAsString)
            {
                return JsonNodePool<JSONString>.Pop().InitData(n.ToString());
            }
            else
            {
                return JsonNodePool<JSONNumber>.Pop().InitData(n);
            }
        }
        public static implicit operator long(JSONNode d)
        {
            return (d == null) ? 0L : d.AsLong;
        }

        public static implicit operator JSONNode(bool b)
        {
            return JsonNodePool<JSONBool>.Pop().InitData(b);
        }

        public static implicit operator bool(JSONNode d)
        {
            return (d == null) ? false : d.AsBool;
        }

        public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
        {
            return aKeyValue.Value;
        }

        public static bool operator ==(JSONNode a, object b)
        {
            if (ReferenceEquals(a, b))
                return true;
            bool aIsNull = a is JSONNull || ReferenceEquals(a, null) || a is JSONLazyCreator;
            bool bIsNull = b is JSONNull || ReferenceEquals(b, null) || b is JSONLazyCreator;
            if (aIsNull && bIsNull)
                return true;
            return !aIsNull && a.Equals(b);
        }

        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion operators

        [ThreadStatic]
        private static StringBuilder m_EscapeBuilder;
        internal static StringBuilder EscapeBuilder
        {
            get {
                if (m_EscapeBuilder == null)
                    m_EscapeBuilder = new StringBuilder();
                return m_EscapeBuilder;
            }
        }
        internal static string Escape(string aText)
        {
            var sb = EscapeBuilder;
            sb.Length = 0;
            if (sb.Capacity < aText.Length + aText.Length / 10)
                sb.Capacity = aText.Length + aText.Length / 10;
            foreach (char c in aText)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (c < ' ' || (forceASCII && c > 127))
                        {
                            ushort val = c;
                            sb.Append("\\u").Append(val.ToString("X4"));
                        }
                        else
                            sb.Append(c);
                        break;
                }
            }
            string result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        private static JSONNode ParseElement(string token, bool quoted)
        {
            if (quoted)
                return token;
            string tmp = token.ToLower();
            if (tmp == "false" || tmp == "true")
                return tmp == "true";
            if (tmp == "null")
                return JSONNull.CreateOrGet();
            double val;
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
                return val;
            else
                return token;
        }

        static Stack<JSONNode> stack = new Stack<JSONNode>(16);
        static StringBuilder Token = new StringBuilder(4096);
        static StringBuilder TokenName = new StringBuilder(128);

        public static JSONNode Parse(string aJSON)
        {
            foreach (var Recycle in stack)
            {
                Recycle.Dispose();
            }
            stack.Clear();
            Token.Length = 0;
            TokenName.Length = 0;

            JSONNode ctx = null;
            int i = 0;
			bool QuoteMode = false;
            bool TokenIsQuoted = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        {
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }
                            stack.Push(JsonNodePool<JSONObject>.Pop());
                            if (ctx != null)
                            {
                                ctx.Add(TokenName.ToString(), stack.Peek());
                            }
                            TokenName.Length = 0;
                            Token.Length = 0;
                            ctx = stack.Peek();
                            break;
                        }
                            
                    case '[':
                        {
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }

                            stack.Push(JsonNodePool<JSONArray>.Pop());
                            if (ctx != null)
                            {
                                ctx.Add(TokenName.ToString(), stack.Peek());
                            }
                            TokenName.Length = 0;
                            Token.Length = 0;
                            ctx = stack.Peek();
                            break;
                        }
                    case '}':
                    case ']':
                        {
                            if (QuoteMode)
                            {

                                Token.Append(aJSON[i]);
                                break;
                            }
                            if (stack.Count == 0)
                                throw new Exception("JSON Parse: Too many closing brackets");
                            
                            stack.Pop();

                            if (Token.Length > 0 || TokenIsQuoted)
                                ctx.Add(TokenName.ToString(), ParseElement(Token.ToString(), TokenIsQuoted));

                            TokenIsQuoted = false;
                            TokenName.Length = 0;
                            Token.Length = 0;
                            if (stack.Count > 0)
                                ctx = stack.Peek();
                            break;
                        } 

                    case ':':
                        {
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }
                            TokenName.Append(Token.ToString());
                            Token.Length = 0;
                            TokenIsQuoted = false;
                            break;
                        }
                    case '"':
                        {
                            QuoteMode ^= true;
                            TokenIsQuoted |= QuoteMode;
                            break;
                        }
                    case ',':
                        {
                            if (QuoteMode)
                            {
                                Token.Append(aJSON[i]);
                                break;
                            }
                            if (Token.Length > 0 || TokenIsQuoted)
                            { 
                                ctx.Add(TokenName.ToString(), ParseElement(Token.ToString(), TokenIsQuoted));
                            }

                            TokenIsQuoted = false;
                            TokenName.Length = 0;
                            Token.Length = 0;
                            TokenIsQuoted = false;
                            break;
                        }

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        {
                            if (QuoteMode)
                                Token.Append(aJSON[i]);
                            break;
                        }
                    case '\\':
                        {
                            ++i;
                            if (QuoteMode)
                            {
                                char C = aJSON[i];
                                switch (C)
                                {
                                    case 't':
                                        Token.Append('\t');
                                        break;
                                    case 'r':
                                        Token.Append('\r');
                                        break;
                                    case 'n':
                                        Token.Append('\n');
                                        break;
                                    case 'b':
                                        Token.Append('\b');
                                        break;
                                    case 'f':
                                        Token.Append('\f');
                                        break;
                                    case 'u':
                                        {
                                            string s = aJSON.Substring(i + 1, 4);
                                            Token.Append((char)int.Parse(
                                                s,
                                                System.Globalization.NumberStyles.AllowHexSpecifier));
                                            i += 4;
                                            break;
                                        }
                                    default:
                                        Token.Append(C);
                                        break;
                                }
                            }
                            break;
                        }
                           
                    case '/':
                        {
                            if (allowLineComments && !QuoteMode && i + 1 < aJSON.Length && aJSON[i + 1] == '/')
                            {
                                while (++i < aJSON.Length && aJSON[i] != '\n' && aJSON[i] != '\r') ;
                                break;
                            }
                            Token.Append(aJSON[i]);
                            break;
                        }
                    case '\uFEFF': // remove / ignore BOM (Byte Order Mark)
                        break;

                    default:
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                }
                ++i;
            }
            if (QuoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            if (ctx == null)
                return ParseElement(Token.ToString(), TokenIsQuoted);
            return ctx;
        }

    }
    // End of JSONNode

    public partial class JSONArray : JSONNode
    {
        private List<JSONNode> m_List = new List<JSONNode>();
        private bool inline = false;
        public override bool Inline
        {
            get { return inline; }
            set { inline = value; }
        }

        public override JSONNodeType Tag { get { return JSONNodeType.Array; } }
        public override bool IsArray { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(m_List.GetEnumerator()); }

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                {
                    return JsonNodePool<JSONLazyCreator>.Pop().InitData(this);
                }
                return m_List[aIndex];
            }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= m_List.Count)
                    m_List.Add(value);
                else
                    m_List[aIndex] = value;
            }
        }

        public override JSONNode this[string aKey]
        {
            get 
            {
                return JsonNodePool<JSONLazyCreator>.Pop().InitData(this);
            }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                m_List.Add(value);
            }
        }

        public override int Count
        {
            get { return m_List.Count; }
        }

        public override void OnRecycle()
        {
            foreach (var iter in m_List)
            {
                iter.Dispose();
            }
            m_List.Clear();
            JsonNodePool<JSONArray>.Push(this);
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null)
                aItem = JSONNull.CreateOrGet();
            m_List.Add(aItem);
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return null;
            JSONNode tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }

        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (JSONNode N in m_List)
                    yield return N;
            }
        }
        
        public override string ToString()
        {
            return m_List.ToString();
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('[');
            int count = m_List.Count;
            if (inline)
                aMode = JSONTextMode.Compact;
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    aSB.Append(',');
                if (aMode == JSONTextMode.Indent)
                    aSB.AppendLine();

                if (aMode == JSONTextMode.Indent)
                    aSB.Append(' ', aIndent + aIndentInc);
                m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent)
                aSB.AppendLine().Append(' ', aIndent);
            aSB.Append(']');
        }
    }
    // End of JSONArray

    public partial class JSONObject : JSONNode
    {
        private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>(16);

        private bool inline = false;
        public override bool Inline
        {
            get { return inline; }
            set { inline = value; }
        }

        public override JSONNodeType Tag { get { return JSONNodeType.Object; } }
        public override bool IsObject { get { return true; } }

        public override Enumerator GetEnumerator() { return new Enumerator(m_Dict.GetEnumerator()); }


        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey))
                    return m_Dict[aKey];
                else
                    return new JSONLazyCreator(this, aKey);
            }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = value;
                else
                    m_Dict.Add(aKey, value);
            }
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return new JSONLazyCreator(this, aIndex.ToString());
				return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (value == null)
                    value = JSONNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return;
                string key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }


		public KeyValuePair<string,JSONNode> GetKeyValue(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_Dict.Count)
				return default(KeyValuePair<string, JSONNode>);
			return m_Dict.ElementAt(aIndex);
		}

        public override int Count
        {
            get { return m_Dict.Count; }
        }

        public override void OnRecycle()
        {
            foreach (var iter in m_Dict)
            {
                iter.Value.Dispose();
            }
            m_Dict.Clear();
            JsonNodePool<JSONObject>.Push(this);
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null)
            {
                aItem = JSONNull.CreateOrGet();
            }

            if (aKey != null)
            {
                if (m_Dict.TryGetValue(aKey, out var old))
                {
                    if (old != aItem)
                    {
                        old.Dispose();
                        m_Dict[aKey] = aItem;
                    }
                }
                else
                {
                    m_Dict.Add(aKey, aItem);
                }
            }
            else
            {
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
            }
        }

        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
                return null;
            JSONNode tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            var item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                var item = m_Dict.Where(k => k.Value == aNode).First();
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override bool HasKey(string aKey)
        {
            return m_Dict.ContainsKey(aKey);
        }

        public override JSONNode GetValueOrDefault(string aKey, JSONNode aDefault)
        {
            JSONNode res;
            if (m_Dict.TryGetValue(aKey, out res))
                return res;
            return aDefault;
        }

        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                    yield return N.Value;
            }
        }

        public override string ToString()
        {
            return m_Dict.ToString();
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('{');
            bool first = true;
            if (inline)
                aMode = JSONTextMode.Compact;
            foreach (var k in m_Dict)
            {
                if (!first)
                    aSB.Append(',');
                first = false;
                if (aMode == JSONTextMode.Indent)
                    aSB.AppendLine();
                if (aMode == JSONTextMode.Indent)
                    aSB.Append(' ', aIndent + aIndentInc);
                aSB.Append('\"').Append(Escape(k.Key)).Append('\"');
                if (aMode == JSONTextMode.Compact)
                    aSB.Append(':');
                else
                    aSB.Append(" : ");
                k.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent)
                aSB.AppendLine().Append(' ', aIndent);
            aSB.Append('}');
        }

    }
    // End of JSONObject

    public partial class JSONString : JSONNode
    {
        private string m_Data;

        public override JSONNodeType Tag { get { return JSONNodeType.String; } }
        public override bool IsString { get { return true; } }

        public override Enumerator GetEnumerator() { return new Enumerator(); }

        static char src = (char)2;
        static char dest = (char)34;
        public override string Value
        {
            get { return m_Data; }
            set
            {
                m_Data = value;
            }
        }

        public JSONString()
        {

        }

        public JSONNode InitData(string aData)
        {
            if (aData.Contains(src))
                aData = aData.Replace(src, dest);
            Value = aData;
            return this;
        }

        public override void OnRecycle()
        {
            m_Data = "";
            JsonNodePool<JSONString>.Push(this);
        }

        public JSONString(string aData)
        {
            if (aData.Contains(src))
                aData = aData.Replace(src, dest);
            Value = aData;
        }

        public override string ToString()
        {
            return m_Data;
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append('\"').Append(Escape(m_Data)).Append('\"');
        }
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;
            string s = obj as string;
            if (s != null)
                return m_Data == s;
            JSONString s2 = obj as JSONString;
            if (s2 != null)
                return m_Data == s2.m_Data;
            return false;
        }
        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }
    }
    // End of JSONString

    public partial class JSONNumber : JSONNode
    {
        private double m_Data;

        public override JSONNodeType Tag { get { return JSONNodeType.Number; } }
        public override bool IsNumber { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get { return m_Data.ToString(CultureInfo.InvariantCulture); }
            set
            {
                double v;
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                    m_Data = v;
            }
        }

        public override double AsDouble
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
        public override long AsLong
        {
            get { return (long)m_Data; }
            set { m_Data = value; }
        }

        public JSONNumber()
        {

        }
        
        public JSONNode InitData(double d)
        {
            m_Data = d;
            return this;
        }

        public override void OnRecycle()
        {
            m_Data = 0;
            JsonNodePool<JSONNumber>.Push(this);
        }

        public JSONNumber(double aData)
        {
            m_Data = aData;
        }

        public JSONNumber(string aData)
        {
            Value = aData;
        }

        public override string ToString()
        {
            return Value;
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(ToString());
        }
        private static bool IsNumeric(object value)
        {
            return value is int || value is uint
                || value is float || value is double
                || value is decimal
                || value is long || value is ulong
                || value is short || value is ushort
                || value is sbyte || value is byte;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (base.Equals(obj))
                return true;
            JSONNumber s2 = obj as JSONNumber;
            if (s2 != null)
                return m_Data == s2.m_Data;
            if (IsNumeric(obj))
                return Convert.ToDouble(obj) == m_Data;
            return false;
        }
        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }
    }
    // End of JSONNumber

    public partial class JSONBool : JSONNode
    {
        private bool m_Data;

        public override JSONNodeType Tag { get { return JSONNodeType.Boolean; } }
        public override bool IsBoolean { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get { return m_Data.ToString(); }
            set
            {
                bool v;
                if (bool.TryParse(value, out v))
                    m_Data = v;
            }
        }
        public override bool AsBool
        {
            get { return m_Data; }
            set { m_Data = value; }
        }


        public JSONBool()
        {

        }

        public JSONNode InitData(bool b)
        {
            m_Data = b;
            return this;
        }

        public override void OnRecycle()
        {
            m_Data = false;
            JsonNodePool<JSONBool>.Push(this);
        }

        public JSONBool(bool aData)
        {
            m_Data = aData;
        }

        public JSONBool(string aData)
        {
            Value = aData;
        }

        public override string ToString()
        {
            return (m_Data) ? "true" : "false";
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(ToString());
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is bool)
                return m_Data == (bool)obj;
            return false;
        }
        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }
    }
    // End of JSONBool

    public partial class JSONNull : JSONNode
    {
        static JSONNull m_StaticInstance = new JSONNull();
        public static bool reuseSameInstance = true;
        public static JSONNull CreateOrGet()
        {
            if (reuseSameInstance)
                return m_StaticInstance;
            return JsonNodePool<JSONNull>.Pop();
        }
        public JSONNull() { }

        public override JSONNodeType Tag { get { return JSONNodeType.NullValue; } }
        public override bool IsNull { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get { return "null"; }
            set { }
        }
        public override bool AsBool
        {
            get { return false; }
            set { }
        }

        public override void OnRecycle()
        {
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;
            return (obj is JSONNull);
        }
        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "null";
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(ToString());
        }
    }
    // End of JSONNull

    internal partial class JSONLazyCreator : JSONNode
    {
        private JSONNode m_Node = null;
        private string m_Key = null;
        public override JSONNodeType Tag { get { return JSONNodeType.None; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public JSONLazyCreator()
        {

        }

        public JSONNode InitData(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
            return this;
        }

        public JSONNode InitData(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
            return this;
        }

        public override void OnRecycle()
        {
            if (m_Node != null)
            {
                m_Node.Dispose();
                m_Node = null;
            }
            m_Key = null;
            JsonNodePool<JSONLazyCreator>.Push(this);
        }

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }

        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        private T Set<T>(T aVal) where T : JSONNode
        {
            if (m_Key == null)
                m_Node.Add(aVal);
            else
                m_Node.Add(m_Key, aVal);
            m_Node = null; // Be GC friendly.
            return aVal;
        }

        public override JSONNode this[int aIndex]
        {
            get { return JsonNodePool<JSONLazyCreator>.Pop().InitData(this); }
            set { Set(JsonNodePool<JSONArray>.Pop()).Add(value); }
        }

        public override JSONNode this[string aKey]
        {
            get {
                return JsonNodePool<JSONLazyCreator>.Pop().InitData(this, aKey);

            }
            set { Set(JsonNodePool<JSONObject>.Pop()).Add(aKey, value); }
        }

        public override void Add(JSONNode aItem)
        {
            Set(JsonNodePool<JSONArray>.Pop()).Add(aItem);
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            Set(JsonNodePool<JSONObject>.Pop()).Add(aKey, aItem);
        }

        public static bool operator ==(JSONLazyCreator a, object b)
        {
            if (b == null)
                return true;
            return System.Object.ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return true;
            return System.Object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override int AsInt
        {
            get { Set(JsonNodePool<JSONNumber>.Pop().InitData(0)); return 0; }
            set { Set(JsonNodePool<JSONNumber>.Pop().InitData(value)); }
        }

        public override float AsFloat
        {
            get { Set(JsonNodePool<JSONNumber>.Pop().InitData(0.0f)); return 0.0f; }
            set { Set(JsonNodePool<JSONNumber>.Pop().InitData(value)); }
        }

        public override double AsDouble
        {
            get { Set(JsonNodePool<JSONNumber>.Pop().InitData(0.0)); return 0.0; }
            set { Set(JsonNodePool<JSONNumber>.Pop().InitData(value)); }
        }

        public override long AsLong
        {
            get
            {
                if (longAsString)
                    Set(JsonNodePool<JSONString>.Pop().InitData("0"));
                else
                    Set(JsonNodePool<JSONNumber>.Pop().InitData(0.0));
                return 0L;
            }
            set
            {
                if (longAsString)
                    Set(JsonNodePool<JSONString>.Pop().InitData(value.ToString()));
                else
                    Set(JsonNodePool<JSONNumber>.Pop().InitData(value));
            }
        }

        public override bool AsBool
        {
            get { Set(JsonNodePool<JSONBool>.Pop().InitData(false)); return false; }
            set { Set(JsonNodePool<JSONBool>.Pop().InitData(value)); }
        }

        public override JSONArray AsArray
        {
            get { return Set(JsonNodePool<JSONArray>.Pop()); }
        }

        public override JSONObject AsObject
        {
            get { return Set(JsonNodePool<JSONObject>.Pop()); }
        }
        public override string ToString()
        {
            return "";
        }
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(ToString());
        }
    }
    // End of JSONLazyCreator

    public static class JSON
    {
        public static JSONNode Parse(string aJSON)
        {
#if USE_DBC_PROFILER
            using (var _ = rkt.DBCProfiler.Start("JSONNode.Parse"))
#endif
                return JSONNode.Parse(aJSON);
        }
    }
}
