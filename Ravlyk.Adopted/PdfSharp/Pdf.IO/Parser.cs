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

namespace Ravlyk.Adopted.PdfSharp.Pdf.IO
{
    /// <summary>
    /// Provides the functionality to parse PDF documents.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses a PDF date string.
        /// </summary>
        internal static DateTime ParseDateTime(string date, DateTime errorValue)  // TODO: TryParseDateTime
        {
            DateTime datetime = errorValue;
            try
            {
                if (date.StartsWith("D:"))
                {
                    // Format is
                    // D:YYYYMMDDHHmmSSOHH'mm'
                    //   ^2      ^10   ^16 ^20
                    int length = date.Length;
                    int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, hh = 0, mm = 0;
                    char o = 'Z';
                    if (length >= 10)
                    {
                        year = Int32.Parse(date.Substring(2, 4));
                        month = Int32.Parse(date.Substring(6, 2));
                        day = Int32.Parse(date.Substring(8, 2));
                        if (length >= 16)
                        {
                            hour = Int32.Parse(date.Substring(10, 2));
                            minute = Int32.Parse(date.Substring(12, 2));
                            second = Int32.Parse(date.Substring(14, 2));
                            if (length >= 23)
                            {
                                if ((o = date[16]) != 'Z')
                                {
                                    hh = Int32.Parse(date.Substring(17, 2));
                                    mm = Int32.Parse(date.Substring(20, 2));
                                }
                            }
                        }
                    }
                    // There are miserable PDF tools around the world.
                    month = Math.Min(Math.Max(month, 1), 12);
                    datetime = new DateTime(year, month, day, hour, minute, second);
                    if (o != 'Z')
                    {
                        TimeSpan ts = new TimeSpan(hh, mm, 0);
                        if (o == '-')
                            datetime = datetime.Add(ts);
                        else
                            datetime = datetime.Subtract(ts);
                    }
                    // Now that we converted datetime to UTC, mark it as UTC.
                    DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
                }
                else
                {
                    // Some libraries use plain English format.
                    datetime = DateTime.Parse(date, CultureInfo.InvariantCulture);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception ex)
            {
                // If we cannot parse datetime, just eat it, but give a hint in DEBUG build.
                Debug.Assert(false, ex.Message);
            }
            return datetime;
        }
    }
}