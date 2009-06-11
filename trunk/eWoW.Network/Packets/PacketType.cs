using System;
using eWoW.Network.OpCodes;

namespace eWoW.Network.Packets
{
    public enum PacketHandleType
    {
        None,
        Logon,
        World,
    }

    public class PacketType : IEquatable<PacketType>
    {
        public PacketType(PacketHandleType handleType, ushort opCode)
        {
            Type = handleType;
            OpCode = opCode;
        }

        public PacketType(AuthServerOpCode opCode) : this(PacketHandleType.Logon, (ushort) opCode)
        {}

        public PacketType(RealmServerOpCode opCode) : this(PacketHandleType.World, (ushort) opCode)
        {}

        public PacketHandleType Type { get; private set; }
        public ushort OpCode { get; private set; }

        #region IEquatable<PacketType> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(PacketType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Type, Type) && other.OpCode == OpCode;
        }

        #endregion

        public static implicit operator PacketType(AuthServerOpCode opCode)
        {
            return new PacketType(opCode);
        }

        public static implicit operator PacketType(RealmServerOpCode opCode)
        {
            return new PacketType(opCode);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case PacketHandleType.Logon:
                    return ((AuthServerOpCode) OpCode).ToString();
                case PacketHandleType.World:
                    return ((RealmServerOpCode) OpCode).ToString();
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(PacketType))
            {
                return false;
            }
            return Equals((PacketType) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ OpCode.GetHashCode();
            }
        }

        public static bool operator ==(PacketType left, PacketType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PacketType left, PacketType right)
        {
            return !Equals(left, right);
        }
    }
}