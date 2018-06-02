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
using System.Globalization;
using System.Text;
using Ravlyk.Adopted.PdfSharp.Drawing;
using Ravlyk.Adopted.PdfSharp.Pdf.Security;

namespace Ravlyk.Adopted.PdfSharp.Pdf.Internal
{
    /// <summary>
    /// Groups a set of static encoding helper functions.
    /// </summary>
    static class PdfEncoders
    {
        /// <summary>
        /// Gets the raw encoding.
        /// </summary>
        public static Encoding RawEncoding
        {
            get { return _rawEncoding ?? (_rawEncoding = new RawEncoding()); }
        }
        static Encoding _rawEncoding;

        /// <summary>
        /// Gets the raw Unicode encoding.
        /// </summary>
        public static Encoding RawUnicodeEncoding
        {
            get { return _rawUnicodeEncoding ?? (_rawUnicodeEncoding = new RawUnicodeEncoding()); }
        }
        static Encoding _rawUnicodeEncoding;

        /// <summary>
        /// Gets the Windows 1252 (ANSI) encoding.
        /// </summary>
        public static Encoding WinAnsiEncoding
        {
            get
            {
                if (_winAnsiEncoding == null)
                {
                    // Use .net encoder if available.
                    _winAnsiEncoding = Encoding.GetEncoding(1252);
                }
                return _winAnsiEncoding;
            }
        }
        static Encoding _winAnsiEncoding;

        /// <summary>
        /// Gets the PDF DocEncoding encoding.
        /// </summary>
        public static Encoding DocEncoding
        {
            get { return _docEncoding ?? (_docEncoding = new DocEncoding()); }
        }
        static Encoding _docEncoding;

        /// <summary>
        /// Gets the UNICODE little-endian encoding.
        /// </summary>
        public static Encoding UnicodeEncoding
        {
            get { return _unicodeEncoding ?? (_unicodeEncoding = Encoding.Unicode); }
        }
        static Encoding _unicodeEncoding;

        /// <summary>
        /// Converts a raw string into a raw string literal, possibly encrypted.
        /// </summary>
        public static string ToStringLiteral(string text, PdfStringEncoding encoding, PdfStandardSecurityHandler securityHandler)
        {
            if (String.IsNullOrEmpty(text))
                return "()";

            byte[] bytes;
            switch (encoding)
            {
                case PdfStringEncoding.RawEncoding:
                    bytes = RawEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.WinAnsiEncoding:
                    bytes = WinAnsiEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.PDFDocEncoding:
                    bytes = DocEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.Unicode:
                    bytes = RawUnicodeEncoding.GetBytes(text);
                    break;

                default:
                    throw new NotImplementedException(encoding.ToString());
            }
            byte[] temp = FormatStringLiteral(bytes, encoding == PdfStringEncoding.Unicode, true, false, securityHandler);
            return RawEncoding.GetString(temp, 0, temp.Length);
        }

        /// <summary>
        /// Converts a raw string into a raw string literal, possibly encrypted.
        /// </summary>
        public static string ToStringLiteral(byte[] bytes, bool unicode, PdfStandardSecurityHandler securityHandler)
        {
            if (bytes == null || bytes.Length == 0)
                return "()";

            byte[] temp = FormatStringLiteral(bytes, unicode, true, false, securityHandler);
            return RawEncoding.GetString(temp, 0, temp.Length);
        }

        /// <summary>
        /// Converts a raw string into a raw hexadecimal string literal, possibly encrypted.
        /// </summary>
        public static string ToHexStringLiteral(string text, PdfStringEncoding encoding, PdfStandardSecurityHandler securityHandler)
        {
            if (String.IsNullOrEmpty(text))
                return "<>";

            byte[] bytes;
            switch (encoding)
            {
                case PdfStringEncoding.RawEncoding:
                    bytes = RawEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.WinAnsiEncoding:
                    bytes = WinAnsiEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.PDFDocEncoding:
                    bytes = DocEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.Unicode:
                    //bytes = UnicodeEncoding.GetBytes(text);
                    bytes = RawUnicodeEncoding.GetBytes(text);
                    break;

                default:
                    throw new NotImplementedException(encoding.ToString());
            }

            byte[] agTemp = FormatStringLiteral(bytes, encoding == PdfStringEncoding.Unicode, true, true, securityHandler);
            return RawEncoding.GetString(agTemp, 0, agTemp.Length);
        }

        /// <summary>
        /// Converts a raw string into a raw hexadecimal string literal, possibly encrypted.
        /// </summary>
        public static string ToHexStringLiteral(byte[] bytes, bool unicode, PdfStandardSecurityHandler securityHandler)
        {
            if (bytes == null || bytes.Length == 0)
                return "<>";

            byte[] agTemp = FormatStringLiteral(bytes, unicode, true, true, securityHandler);
            return RawEncoding.GetString(agTemp, 0, agTemp.Length);
        }

        /// <summary>
        /// Converts the specified byte array into a byte array representing a string literal.
        /// </summary>
        /// <param name="bytes">The bytes of the string.</param>
        /// <param name="unicode">Indicates whether one or two bytes are one character.</param>
        /// <param name="prefix">Indicates whether to use Unicode prefix.</param>
        /// <param name="hex">Indicates whether to create a hexadecimal string literal.</param>
        /// <param name="securityHandler">Encrypts the bytes if specified.</param>
        /// <returns>The PDF bytes.</returns>
        public static byte[] FormatStringLiteral(byte[] bytes, bool unicode, bool prefix, bool hex, PdfStandardSecurityHandler securityHandler)
        {
            if (bytes == null || bytes.Length == 0)
                return hex ? new byte[] { (byte)'<', (byte)'>' } : new byte[] { (byte)'(', (byte)')' };

            Debug.Assert(!unicode || bytes.Length % 2 == 0, "Odd number of bytes in Unicode string.");

            bool encrypted = false;
            if (securityHandler != null)
            {
                bytes = (byte[])bytes.Clone();
                bytes = securityHandler.EncryptBytes(bytes);
                encrypted = true;
            }

            int count = bytes.Length;
            StringBuilder pdf = new StringBuilder();
            if (!unicode)
            {
                if (!hex)
                {
                    pdf.Append("(");
                    for (int idx = 0; idx < count; idx++)
                    {
                        char ch = (char)bytes[idx];
                        if (ch < 32)
                        {
                            switch (ch)
                            {
                                case '\n':
                                    pdf.Append("\\n");
                                    break;

                                case '\r':
                                    pdf.Append("\\r");
                                    break;

                                case '\t':
                                    pdf.Append("\\t");
                                    break;

                                case '\b':
                                    pdf.Append("\\b");
                                    break;

                                // Corrupts encrypted text.
                                //case '\f':
                                //  pdf.Append("\\f");
                                //  break;

                                default:
                                    // Don't escape characters less than 32 if the string is encrypted, because it is
                                    // unreadable anyway.
                                    encrypted = true;
                                    if (!encrypted)
                                    {
                                        pdf.Append("\\0");
                                        pdf.Append((char)(ch % 8 + '0'));
                                        pdf.Append((char)(ch / 8 + '0'));
                                    }
                                    else
                                        pdf.Append(ch);
                                    break;
                            }
                        }
                        else
                        {
                            switch (ch)
                            {
                                case '(':
                                    pdf.Append("\\(");
                                    break;

                                case ')':
                                    pdf.Append("\\)");
                                    break;

                                case '\\':
                                    pdf.Append("\\\\");
                                    break;

                                default:
                                    pdf.Append(ch);
                                    break;
                            }
                        }
                    }
                    pdf.Append(')');
                }
                else
                {
                    pdf.Append('<');
                    for (int idx = 0; idx < count; idx++)
                        pdf.AppendFormat("{0:X2}", bytes[idx]);
                    pdf.Append('>');
                }
            }
            else
            {
            Hex:
                if (hex)
                {
                    pdf.Append(prefix ? "<FEFF" : "<");
                    for (int idx = 0; idx < count; idx += 2)
                    {
                        pdf.AppendFormat("{0:X2}{1:X2}", bytes[idx], bytes[idx + 1]);
                        if (idx != 0 && (idx % 48) == 0)
                            pdf.Append("\n");
                    }
                    pdf.Append(">");
                }
                else
                {
                    // TODO non hex literals... not sure how to treat linefeeds, '(', '\' etc.
                    hex = true;
                    goto Hex;
                }
            }
            return RawEncoding.GetBytes(pdf.ToString());
        }

        /// <summary>
        /// ...because I always forget CultureInfo.InvariantCulture and wonder why Acrobat
        /// cannot understand my German decimal separator...
        /// </summary>
        public static string Format(string format, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Converts a float into a string with up to 3 decimal digits and a decimal point.
        /// </summary>
        public static string ToString(double val)
        {
            return val.ToString(Config.SignificantFigures3, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an XColor into a string with up to 3 decimal digits and a decimal point.
        /// </summary>
        public static string ToString(XColor color, PdfColorMode colorMode)
        {
            const string format = Config.SignificantFigures3;

            // If not defined let color decide
            if (colorMode == PdfColorMode.Undefined)
                colorMode = color.ColorSpace == XColorSpace.Cmyk ? PdfColorMode.Cmyk : PdfColorMode.Rgb;

            switch (colorMode)
            {
                case PdfColorMode.Cmyk:
                    return String.Format(CultureInfo.InvariantCulture, "{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "}",
                      color.C, color.M, color.Y, color.K);

                default:
                    return String.Format(CultureInfo.InvariantCulture, "{0:" + format + "} {1:" + format + "} {2:" + format + "}",
                      color.R / 255.0, color.G / 255.0, color.B / 255.0);
            }
        }

        /// <summary>
        /// Converts an XMatrix into a string with up to 4 decimal digits and a decimal point.
        /// </summary>
        public static string ToString(XMatrix matrix)
        {
            const string format = Config.SignificantFigures4;
            return String.Format(CultureInfo.InvariantCulture,
                "{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "}",
                matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
        }
    }
}
