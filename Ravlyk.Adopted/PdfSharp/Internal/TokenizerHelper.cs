#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Microsoft
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
using System.Globalization;

namespace Ravlyk.Adopted.PdfSharp.Internal
{
    // Reflected from WPF to ensure compatibility
    // Use netmassdownloader -d "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.0" -output g:\cachetest -v
    static class TokenizerHelper
    {
        public static char GetNumericListSeparator(IFormatProvider provider)
        {
            char numericSeparator = ',';
            NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);
            if (numberFormat.NumberDecimalSeparator.Length > 0 && numericSeparator == numberFormat.NumberDecimalSeparator[0])
                numericSeparator = ';';
            return numericSeparator;
        }
    }
}