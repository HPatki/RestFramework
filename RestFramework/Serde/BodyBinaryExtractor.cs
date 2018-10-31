using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using RestFramework.Exceptions;

namespace RestFramework.Serde
{
    public class BodyBinaryExtractor : IConvertible 
    {
        internal String m_ParamName;
        internal Byte[] m_FileContent;
        private Int64 m_StartPos;
        private Int64 m_EndPos;

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

        public Int64 StartPos
        {
            get { return m_StartPos; }
            set { m_StartPos = value; }
        }

        public Int64 EndPos
        {
            get { return m_EndPos; }
            set { m_EndPos = value; }
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

        virtual public object ToType(Type conversionType, IFormatProvider provider)
        {
            Object toReturn = null;
            ConstructorInfo[] infor = conversionType.GetConstructors();
            foreach (ConstructorInfo info in infor)
            {
                var ParamInfo = info.GetParameters();
                if (3 == ParamInfo.Length && ParamInfo[0].ParameterType == typeof(Byte[]) &&
                    ParamInfo[1].ParameterType == typeof(Int32) && ParamInfo[2].ParameterType == typeof(Int32) )
                {
                    Object[] arguments = new Object[3];
                    arguments[0] = m_FileContent;
                    arguments[1] = (Int32)this.StartPos;
                    arguments[2] = (Int32)this.EndPos;
                    toReturn = info.Invoke(arguments);
                    break;
                }
            }

            return toReturn;
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
