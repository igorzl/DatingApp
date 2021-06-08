using System;

namespace API.Helpers
{
        public class ComparableByteArray
        {
            public static bool CompareData(byte[] a1, byte[] a2, string compareSign, out string errorMessage)
            {
                errorMessage = string.Empty;

                bool result = false;

                ComparableByteArray ca1 = new ComparableByteArray(a1);
                ComparableByteArray ca2 = new ComparableByteArray(a2);

                switch (compareSign.Trim())
                {
                    case ">":
                        result = ca1 > ca2; break;
                    case "<":
                        result = ca1 < ca2; break;
                    case ">=":
                        result = ca1 >= ca2; break;
                    case "<=":
                        result = ca1 <= ca2; break;
                    case "<>":
                        result = ca1 != ca2; break;
                    case "=":
                        result = ca1 == ca2; break;
                    default:
                        errorMessage = string.Format("Illegal relational operator found: [{0}]", compareSign);
                        return result;
                }

                if (!result)
                {
                    errorMessage = "Operands binary comparison result is negative";
                }

                return result;
            }
        
            private byte[] ar;
            ArraySegment<byte> arSeg;

            public ComparableByteArray(byte[] ba)
            {
                ar = ba;
                arSeg = new ArraySegment<byte>(ba);
            }

            public override int GetHashCode()
            {
                return arSeg.GetHashCode();
            }

            public override bool Equals(System.Object obj)
            {
                // If parameter is null return false.
                if (obj == null)
                {
                    return false;
                }

                // If parameter cannot be cast to ComparableByteArray return false.
                ComparableByteArray p = obj as ComparableByteArray;
                if ((System.Object)p == null)
                {
                    return false;
                }

                if (ar.Length != p.ar.Length)
                    return false;
                for (int i = 0; i < ar.Length; i++)
                    if (ar[i] != p.ar[i])
                        return false;
                return true;
            }

            public static bool operator ==(ComparableByteArray a, ComparableByteArray b)
            {
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }
                if (a.ar.Length != b.ar.Length)
                    return false;
                for (int i = 0; i < a.ar.Length; i++)
                    if (a.ar[i] != b.ar[i])
                        return false;
                return true;
            }

            public static bool operator !=(ComparableByteArray a, ComparableByteArray b)
            {
                return !(a == b);
            }

            public static bool operator <(ComparableByteArray a, ComparableByteArray b)
            {
                if (System.Object.ReferenceEquals(a, b))
                {
                    return false;
                }
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                if (a.ar.Length < b.ar.Length)
                    return true;
                else
                    if (a.ar.Length > b.ar.Length)
                        return false;

                for (int i = 0; i < a.ar.Length; i++)
                    if (a.ar[i] > b.ar[i])
                        return false;
                    else
                        if (a.ar[i] < b.ar[i])
                            return true;
                return false;
            }

            public static bool operator >(ComparableByteArray a, ComparableByteArray b)
            {
                if (System.Object.ReferenceEquals(a, b))
                {
                    return false;
                }
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                if (a.ar.Length > b.ar.Length)
                    return true;
                else
                    if (a.ar.Length < b.ar.Length)
                        return false;

                for (int i = 0; i < a.ar.Length; i++)
                    if (a.ar[i] < b.ar[i])
                        return false;
                    else
                        if (a.ar[i] > b.ar[i])
                            return true;
                return false;
            }

            public static bool operator <=(ComparableByteArray a, ComparableByteArray b)
            {
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                if (a.ar.Length < b.ar.Length)
                    return true;
                else
                    if (a.ar.Length > b.ar.Length)
                        return false;

                for (int i = 0; i < a.ar.Length; i++)
                    if (a.ar[i] > b.ar[i])
                        return false;
                    else
                        if (a.ar[i] < b.ar[i])
                            return true;
                // a == b
                return true;
            }

            public static bool operator >=(ComparableByteArray a, ComparableByteArray b)
            {
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                if (a.ar.Length > b.ar.Length)
                    return true;
                else
                    if (a.ar.Length < b.ar.Length)
                        return false;

                for (int i = 0; i < a.ar.Length; i++)
                    if (a.ar[i] < b.ar[i])
                        return false;
                    else
                        if (a.ar[i] > b.ar[i])
                            return true;
                return true;
            }

        }
}