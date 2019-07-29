namespace SparkTech.SDK.Clipper
{
    internal struct Int128
    {
        #region Fields

        private long hi;

        private ulong lo;

        #endregion

        #region Constructors and Destructors

        public Int128(long _lo)
        {
            this.lo = (ulong)_lo;
            if (_lo < 0) this.hi = -1;
            else this.hi = 0;
        }

        public Int128(long _hi, ulong _lo)
        {
            this.lo = _lo;
            this.hi = _hi;
        }

        public Int128(Int128 val)
        {
            this.hi = val.hi;
            this.lo = val.lo;
        }

        #endregion

        #region Public Methods and Operators

        //nb: Constructing two new Int128 objects every time we want to multiply longs
        //is slow. So, although calling the Int128Mul method doesn't look as clean, the
        //code runs significantly faster than if we'd used the * operator.

        public static Int128 Int128Mul(long lhs, long rhs)
        {
            var negate = lhs < 0 != rhs < 0;
            if (lhs < 0) lhs = -lhs;
            if (rhs < 0) rhs = -rhs;
            var int1Hi = (ulong)lhs >> 32;
            var int1Lo = (ulong)lhs & 0xFFFFFFFF;
            var int2Hi = (ulong)rhs >> 32;
            var int2Lo = (ulong)rhs & 0xFFFFFFFF;

            //nb: see comments in clipper.pas
            var a = int1Hi * int2Hi;
            var b = int1Lo * int2Lo;
            var c = int1Hi * int2Lo + int1Lo * int2Hi;

            ulong lo;
            long hi;
            hi = (long)(a + (c >> 32));

            unchecked
            {
                lo = (c << 32) + b;
            }
            if (lo < b) hi++;
            var result = new Int128(hi, lo);
            return negate ? -result : result;
        }

        public static Int128 operator +(Int128 lhs, Int128 rhs)
        {
            lhs.hi += rhs.hi;
            lhs.lo += rhs.lo;
            if (lhs.lo < rhs.lo) lhs.hi++;
            return lhs;
        }

        public static bool operator ==(Int128 val1, Int128 val2)
        {
            if ((object)val1 == (object)val2) return true;
            else if ((object)val1 == null || (object)val2 == null) return false;

            return val1.hi == val2.hi && val1.lo == val2.lo;
        }

        public static explicit operator double(Int128 val)
        {
            const double shift64 = 18446744073709551616.0; //2^64
            if (val.hi < 0)
            {
                if (val.lo == 0) return (double)val.hi * shift64;
                else return -(double)(~val.lo + ~val.hi * shift64);
            }
            else return (double)(val.lo + val.hi * shift64);
        }

        public static bool operator >(Int128 val1, Int128 val2)
        {
            if (val1.hi != val2.hi) return val1.hi > val2.hi;
            else return val1.lo > val2.lo;
        }

        public static bool operator !=(Int128 val1, Int128 val2)
        {
            return !(val1 == val2);
        }

        public static bool operator <(Int128 val1, Int128 val2)
        {
            if (val1.hi != val2.hi) return val1.hi < val2.hi;
            else return val1.lo < val2.lo;
        }

        public static Int128 operator -(Int128 lhs, Int128 rhs)
        {
            return lhs + -rhs;
        }

        public static Int128 operator -(Int128 val)
        {
            if (val.lo == 0) return new Int128(-val.hi, 0);
            else return new Int128(~val.hi, ~val.lo + 1);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Int128)) return false;

            var i128 = (Int128)obj;
            return i128.hi == this.hi && i128.lo == this.lo;
        }

        public override int GetHashCode()
        {
            return this.hi.GetHashCode() ^ this.lo.GetHashCode();
        }

        public bool IsNegative()
        {
            return this.hi < 0;
        }

        #endregion
    };
}