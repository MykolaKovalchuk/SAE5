#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.IO;
using Ravlyk.Adopted.PdfSharp.Pdf.Internal;

namespace Ravlyk.Adopted.PdfSharp.Pdf.Content
{
    /// <summary>
    /// Represents a writer for generation of PDF streams. 
    /// </summary>
    class ContentWriter
    {
        public ContentWriter(Stream contentStream)
        {
            _stream = contentStream;
        }

        public void Close(bool closeUnderlyingStream)
        {
            if (_stream != null && closeUnderlyingStream)
            {
                _stream.Close();
                _stream = null;
            }
        }

        public void Close()
        {
            Close(true);
        }

        public int Position
        {
            get { return (int)_stream.Position; }
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(bool value)
        {
            //WriteSeparator(CharCat.Character);
            //WriteRaw(value ? bool.TrueString : bool.FalseString);
            //lastCat = CharCat.Character;
        }

        public void WriteRaw(string rawString)
        {
            if (String.IsNullOrEmpty(rawString))
                return;
            //AppendBlank(rawString[0]);
            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(rawString);
            _stream.Write(bytes, 0, bytes.Length);
            _lastCat = GetCategory((char)bytes[bytes.Length - 1]);
        }

        public void WriteLineRaw(string rawString)
        {
            if (String.IsNullOrEmpty(rawString))
                return;
            //AppendBlank(rawString[0]);
            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(rawString);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Write(new byte[] { (byte)'\n' }, 0, 1);
            _lastCat = GetCategory((char)bytes[bytes.Length - 1]);
        }

        public void WriteRaw(char ch)
        {
            Debug.Assert(ch < 256, "Raw character greater than 255 detected.");
            _stream.WriteByte((byte)ch);
            _lastCat = GetCategory(ch);
        }

        /// <summary>
        /// Gets or sets the indentation for a new indentation level.
        /// </summary>
        internal int Indent
        {
            get { return _indent; }
            set { _indent = value; }
        }
        protected int _indent = 2;
        protected int _writeIndent = 0;

        public void NewLine()
        {
            if (_lastCat != CharCat.NewLine)
                WriteRaw('\n');
        }

        CharCat GetCategory(char ch)
        {
            //if (Lexer.IsDelimiter(ch))
            //  return CharCat.Delimiter;
            //if (ch == Chars.LF)
            //  return CharCat.NewLine;
            return CharCat.Character;
        }

        enum CharCat
        {
            NewLine,
            Character
        };
        CharCat _lastCat;

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        internal Stream Stream
        {
            get { return _stream; }
        }
        Stream _stream;
    }
}
