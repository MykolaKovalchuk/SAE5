#region License
/* TrueTypeSharp
   Copyright (c) 2010 Illusory Studios LLC

   TrueTypeSharp is available at zer7.com. It is a C# port of Sean Barrett's
   C library stb_truetype, which was placed in the public domain and is
   available at nothings.org.

   Permission to use, copy, modify, and/or distribute this software for any
   purpose with or without fee is hereby granted, provided that the above
   copyright notice and this permission notice appear in all copies.

   THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
   WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
   MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
   ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
   WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
   ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
   OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/
#endregion

using System;

namespace Ravlyk.Adopted.TrueTypeSharp
{
    #pragma warning disable 0660, 0661 // This never goes into a collection...
    struct FakePtr<T>
    {
        public T[] Array; public int Offset;

        public T[] GetData(int length)
        {
            var t = new T[length];
            if (Array != null) { global::System.Array.Copy(Array, Offset, t, 0, length); }
            return t;
        }

        public void MakeNull()
        {
            Array = null; Offset = 0;
        }

        public T this[int index]
        {
            get
            {
                try { return Array[Offset + index]; }
                catch (IndexOutOfRangeException) { return default(T); } // Sometimes accesses are made out of range, it appears.
                                                                        // In particular, to get all the way to char.MaxValue, this was needed.
                                                                        // Probably bad data in the font. Also, is bounds checking done?
                                                                        // I don't see it... Either way, it's not a problem for us here.
            }
            set { Array[Offset + index] = value; }
        }

        public T Value
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        public bool IsNull
        {
            get { return Array == null; }
        }

        public static FakePtr<T> operator +(FakePtr<T> p, int offset)
        {
            return new FakePtr<T>() { Array = p.Array, Offset = p.Offset + offset };
        }

        public static FakePtr<T> operator -(FakePtr<T> p, int offset)
        {
            return p + -offset;
        }

        public static FakePtr<T> operator +(FakePtr<T> p, uint offset)
        {
            return p + (int)offset;
        }

        public static FakePtr<T> operator ++(FakePtr<T> p)
        {
            return p + 1;
        }

        public static bool operator ==(FakePtr<T> p1, FakePtr<T> p2)
        {
            return p1.Array == p2.Array && p1.Offset == p2.Offset;
        }

        public static bool operator !=(FakePtr<T> p1, FakePtr<T> p2)
        {
            return !(p1 == p2);
        }
    }
}
