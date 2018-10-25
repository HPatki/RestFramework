using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Exceptions;

namespace RestFramework.Serde
{
    public class BodyBinaryExtractor : IConvertible 
    {
        internal String m_ParamName;
        internal Byte[] m_FileContent;

        public String ParamName
        {
            get { return m_ParamName; }
            set { m_ParamName = value; }
        }

        public Byte[] FileContent
        {
            get { return m_FileContent; }
            set { m_FileContent = value; }
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(m_FileContent[0]);
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(m_FileContent[0]);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(m_FileContent[0]);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new MethodNotImplemented("Method ToDateTime is not implemented");
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new MethodNotImplemented("Method ToDecimal is not implemented");
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Double.Parse(System.Text.Encoding.UTF8.GetString(m_FileContent));
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Int16.Parse(System.Text.Encoding.UTF8.GetString(m_FileContent));
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Int32.Parse(System.Text.Encoding.UTF8.GetString(m_FileContent));
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Int64.Parse(System.Text.Encoding.UTF8.GetString(m_FileContent));
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            sbyte outval;
            SByte.TryParse(System.Text.Encoding.UTF8.GetString(m_FileContent), out outval);
            return outval;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return float.Parse(System.Text.Encoding.UTF8.GetString(m_FileContent));
        }

        public string ToString(IFormatProvider provider)
        {
            return System.Text.Encoding.UTF8.GetString(m_FileContent);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new MethodNotImplemented("Method ToType is not implemented");
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            UInt16 outval;
            UInt16.TryParse (System.Text.Encoding.UTF8.GetString(m_FileContent), out outval);
            return outval;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            UInt32 outval;
            UInt32.TryParse(System.Text.Encoding.UTF8.GetString(m_FileContent), out outval);
            return outval;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            UInt64 outval;
            UInt64.TryParse(System.Text.Encoding.UTF8.GetString(m_FileContent), out outval);
            return outval;
        }

    }
}
