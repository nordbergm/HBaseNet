/**
 * Autogenerated by Thrift Compiler (0.7.0-dev)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 */
using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;

namespace Thrift.HBase
{
    [Serializable]
    public class BatchMutation : TBase
    {
        public Isset __isset;
        private List<Mutation> _mutations;
        private byte[] _row;

        public byte[] Row
        {
            get { return this._row; }
            set
            {
                this.__isset.row = true;
                this._row = value;
            }
        }

        public List<Mutation> Mutations
        {
            get { return this._mutations; }
            set
            {
                this.__isset.mutations = true;
                this._mutations = value;
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
                            this.Row = iprot.ReadBinary();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;
                    case 2:
                        if (field.Type == TType.List)
                        {
                            {
                                this.Mutations = new List<Mutation>();
                                TList _list0 = iprot.ReadListBegin();
                                for (int _i1 = 0; _i1 < _list0.Count; ++_i1)
                                {
                                    var _elem2 = new Mutation();
                                    _elem2 = new Mutation();
                                    _elem2.Read(iprot);
                                    this.Mutations.Add(_elem2);
                                }
                                iprot.ReadListEnd();
                            }
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
            var struc = new TStruct("BatchMutation");
            oprot.WriteStructBegin(struc);
            var field = new TField();
            if (this.Row != null && this.__isset.row)
            {
                field.Name = "row";
                field.Type = TType.String;
                field.ID = 1;
                oprot.WriteFieldBegin(field);
                oprot.WriteBinary(this.Row);
                oprot.WriteFieldEnd();
            }
            if (this.Mutations != null && this.__isset.mutations)
            {
                field.Name = "mutations";
                field.Type = TType.List;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                {
                    oprot.WriteListBegin(new TList(TType.Struct, this.Mutations.Count));
                    foreach (Mutation _iter3 in this.Mutations)
                    {
                        _iter3.Write(oprot);
                    }
                    oprot.WriteListEnd();
                }
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder("BatchMutation(");
            sb.Append("Row: ");
            sb.Append(this.Row);
            sb.Append(",Mutations: ");
            sb.Append(this.Mutations);
            sb.Append(")");
            return sb.ToString();
        }

        #region Nested type: Isset

        [Serializable]
        public struct Isset
        {
            public bool mutations;
            public bool row;
        }

        #endregion
    }
}