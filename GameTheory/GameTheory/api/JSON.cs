using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thjx
{
    /// <summary>
    /// A small JSON parser dedicated to THJX
    /// </summary>
    static internal class JSON
    {
        abstract public class Value
        {
            public static implicit operator Value(string value) { return (String)value; }
            public static implicit operator Value(int[] array) { return (Array)array; }
            public static implicit operator Value(int value) { return (Number)value; }
            public static implicit operator Value(bool value) { return (Boolean)value; }
        }

        public class Array : Value
        {
            List<Value> _Values = new List<Value>();
            public List<Value> Values { get { return _Values; } set { _Values = value; } }

            public static implicit operator Array(int[] Value)
            {
                Array a = new Array();
                for (int n = 0; n < Value.Length; n++) { a._Values.Add(Value[n]); }
                return a;
            }
            public static implicit operator int[](Array Array)
            {
                int[] iarray = new int[Array._Values.Count];
                for (int n = 0; n < iarray.Length; n++)
                {
                    Number number = Array._Values[n] as Number;
                    if (number == null) { iarray[n] = 0; }
                    else { iarray[n] = (int)number.Value; }
                }
                return iarray;
            }
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                bool first = true;
                builder.Append('[');
                foreach (Value value in _Values)
                {
                    if (!first)
                    { builder.Append(','); }
                    else
                    { first = false; }
                    builder.Append(value.ToString());
                }
                builder.Append(']');
                return builder.ToString();
            }
        }
        public class String : Value
        {
            string _Value = "";
            public string Value { get { return _Value; } set { _Value = value; } }
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append('"');
                builder.Append(_Value);
                builder.Append('"');
                return builder.ToString();
            }

            public static implicit operator String(string Value)
            {
                String str = new String();
                str.Value = Value;
                return str;
            }
        }
        public class Number : Value
        {
            Decimal _Value = 0;
            public Decimal Value { get { return _Value; } set { _Value = value; } }
            public static implicit operator Number(int Value) { Number n = new Number(); n._Value = Value; return n; }
            public static implicit operator int(Number Number) { return (int)Number._Value; }
            public override string ToString()
            {
                string result = _Value.ToString().Replace(",", ".");
                if (result.EndsWith(".0")) { return result.Replace(".0", ""); }
                return result;
            }
        }
        public class Boolean : Value
        {
            bool _State = false;
            public bool State { get { return _State; } set { _State = value; } }
            public static implicit operator Boolean(bool value) { Boolean b = new Boolean(); b.State = value; return b; }
            public override string ToString()
            {
                return _State ? "true" : "false";
            }
        }
        public class Null : Value
        {
            public override string ToString()
            {
                return "null";
            }
        }
        public class Object : Value
        {
            Dictionary<string, Value> _Properties = new Dictionary<string, Value>();

            public Value this[string Key] { get { return _Properties[Key]; } set { _Properties[Key] = value; } }

            public override string ToString()
            {
                bool first = true;
                StringBuilder Builder = new StringBuilder();
                Builder.Append('{');
                foreach (string property in _Properties.Keys)
                {
                    if (!first)
                    { Builder.Append(','); }
                    else
                    { first = false; }
                    Builder.Append("\"");
                    Builder.Append(property);
                    Builder.Append("\":");
                    Builder.Append(_Properties[property]);
                }
                Builder.Append('}');
                return Builder.ToString();
            }
        }

        public static Value Parse(string Data)
        {
            int Position = 0;
            return _ParseValue(Data, ref Position); 
        }
        private static Value _ParseValue(string Data, ref int Position)
        {
            if (Position >= Data.Length || Position < 0) { return null; }
            for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++);
            if (Data[Position] == '{')
            {
                Position++;
                StringBuilder chunk = new StringBuilder();
                int count = 1;
                for (; Position < Data.Length; Position++)
                {
                   if (Data[Position] == '{') { count++; }
                   if (Data[Position] == '}'){ count--; if (count == 0) { break; } }
                   chunk.Append(Data[Position]);
                }
                if (Data[Position] == '}') { Position++; }
                int SubPosition = 0;
                return _ParseObject(chunk.ToString(), ref SubPosition);   
            }
            if (Data[Position] == '"') { return _ParseString(Data, ref Position); }
            if (char.IsDigit(Data[Position])) { return _ParseNumber(Data, ref Position); }
            if (Data[Position] == 't' || Data[Position] == 'f') { return _ParseBoolean(Data, ref Position); }
            if (Data[Position] == '[') { return _ParseArray(Data, ref Position); }
            return new Null();
        }

        private static Array _ParseArray(string Data, ref int Position)
        {
            for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
            if (Position >= Data.Length) { return null; }
            if (Data[Position] != '[') { return null; }
            Position++;
            Array array = new Array();
            do
            {
                array.Values.Add(_ParseValue(Data, ref Position));
                for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
                if (Position < Data.Length && Data[Position] == ',') { Position++; }
                for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;           
            } while (Position < Data.Length && Data[Position] != ']');
            if (Position >= Data.Length) { return array; }
            if (Data[Position] == ']') { Position++; }
            return array;

        }

        private static Boolean _ParseBoolean(string Data, ref int Position)
        {
            bool result = false;
            for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
            if (Position >= Data.Length) { return false; }
            if (Data[Position] == 't') { result = true; }
            for (; Position < Data.Length && !char.IsWhiteSpace(Data[Position]) && Data[Position] != ','; Position++) ;
            return result;
        }
        private static Number _ParseNumber(string Data, ref int Position)
        {
            for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
            int value = 0;
            for (; Position < Data.Length; Position++)
            { 
                if (!char.IsDigit(Data[Position])) { return value; }
                value *= 10;
                value += (int)(Data[Position] - '0');
            }
            return value;
        }
        private static Object _ParseObject(string Data, ref int Position)
        {
            Object obj = new Object();
            if (Position >= Data.Length || Position < 0) { return null; }
            while (Position < Data.Length)
            {
                for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
                if (Position >= Data.Length) { break; }
                if (Data[Position] != '"') 
                {
                    return obj;
                }
                String key = _ParseString(Data, ref Position);
                for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
                if (Position >= Data.Length) { break; }

                if (Data[Position] == '=' || Data[Position] == ':') { Position++; }
                for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
                if (Position >= Data.Length) { break; }
                Value value = _ParseValue(Data, ref Position);
                obj[key.Value] = value;
                for (; Position < Data.Length && char.IsWhiteSpace(Data[Position]); Position++) ;
                if (Position >= Data.Length) { break; }
                if (Data[Position] == ',') { Position++; }
                
            }
            return obj;
        }

        private static String _ParseString(string Data, ref int Position)
        {
            try
            {
                if (Position >= Data.Length || Position < 0) { return null; }
                if (Data[Position] != '"') { return null; }
                Position++;

                StringBuilder builder = new StringBuilder();
                for (;
                    Position < Data.Length &&
                    Data[Position] != '"';
                    Position++)
                {
                    builder.Append(Data[Position]);
                }

                Position++;
                String result = new String();
                result.Value = builder.ToString();
                return result;
            }
            catch
            {
                throw new Exception("JSON Parse error");
            }
        }


    }
}

