/**
 * Autogenerated by Thrift Compiler (0.7.0-dev)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 */
using System;
using System.Text;
using Thrift.Protocol;

namespace Thrift.HBase
{
    [Serializable]
    public class TCell : TBase
    {
        public Isset __isset;
        private long _timestamp;
        private byte[] _value;

        public byte[] Value
        {
            get { return this._value; }
            set
            {
                this.__isset.value = true;
                this._value = value;
            }
        }

        public long Timestamp
        {
            get { return this._timestamp; }
            set
            {
                this.__isset.timestamp = true;
                this._timestamp = value;
            }
        }

        #region TBase Members

        public void Read(TProtocol iprot)
        {
            TField field;
            iprot.ReadStructBegin();
            while (true)
            {
                field = iprot.ReadFieldBegin();
                if (field.Type == TType.Stop)
                {
                    break;
                }
                switch (field.ID)
                {
                    case 1:
                        if (field.Type == TType.String)
                        {
                            this.Value = iprot.ReadBinary();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;
                    case 2:
                        if (field.Type == TType.I64)
                        {
                            this.Timestamp = iprot.ReadI64();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;
                    default:
                        TProtocolUtil.Skip(iprot, field.Type);
                        break;
                }
                iprot.ReadFieldEnd();
            }
            iprot.ReadStructEnd();
        }

        public void Write(TProtocol oprot)
        {
            var struc = new TStruct("TCell");
            oprot.WriteStructBegin(struc);
            var field = new TField();
            if (this.Value != null && this.__isset.value)
            {
                field.Name = "value";
                field.Type = TType.String;
                field.ID = 1;
                oprot.WriteFieldBegin(field);
                oprot.WriteBinary(this.Value);
                oprot.WriteFieldEnd();
            }
            if (this.__isset.timestamp)
            {
                field.Name = "timestamp";
                field.Type = TType.I64;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                oprot.WriteI64(this.Timestamp);
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder("TCell(");
            sb.Append("Value: ");
            sb.Append(this.Value);
            sb.Append(",Timestamp: ");
            sb.Append(this.Timestamp);
            sb.Append(")");
            return sb.ToString();
        }

        #region Nested type: Isset

        [Serializable]
        public struct Isset
        {
            public bool timestamp;
            public bool value;
        }

        #endregion
    }
}