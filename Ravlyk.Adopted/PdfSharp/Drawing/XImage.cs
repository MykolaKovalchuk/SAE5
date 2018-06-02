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
using System.Drawing;
using Ravlyk.Adopted.PdfSharp.Drawing.Internal;
using Ravlyk.Adopted.PdfSharp.Internal;
using Ravlyk.Adopted.PdfSharp.Pdf.Advanced;

// WPFHACK
#pragma warning disable 0169
#pragma warning disable 0649

namespace Ravlyk.Adopted.PdfSharp.Drawing
{
    [Flags]
    internal enum XImageState
    {
        UsedInDrawingContext = 0x00000001,

        StateMask = 0x0000FFFF,
    }

    /// <summary>
    /// Defines an object used to draw image files (bmp, png, jpeg, gif) and PDF forms.
    /// An abstract base class that provides functionality for the Bitmap and Metafile descended classes.
    /// </summary>
    public class XImage : IDisposable
    {
        // The hierarchy is adapted to WPF/Silverlight/WinRT
        //
        // XImage                           <-- ImageSource
        //   XForm
        //   PdfForm
        //   XBitmapSource               <-- BitmapSource
        //     XBitmapImage             <-- BitmapImage

        // ???
        //public bool Disposed
        //{
        //    get { return _disposed; }
        //    set { _disposed = value; }
        //}


        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class.
        /// </summary>
        protected XImage()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class from a GDI+ image.
        /// </summary>
        XImage(Image image)
        {
            _gdiImage = image;
            Initialize();
        }

        /// <summary>
        /// Conversion from Image to XImage.
        /// </summary>
        public static XImage FromGdiPlusImage(Image image)
        {
            return new XImage(image);
        }

        internal XImageState XImageState
        {
            get { return _xImageState; }
            set { _xImageState = value; }
        }
        XImageState _xImageState;

        internal void Initialize()
        {
            if (_importedImage != null)
            {
                ImportedImageJpeg iiJpeg = _importedImage as ImportedImageJpeg;
                // In PDF there are two formats: JPEG and PDF bitmap.
                if (iiJpeg != null)
                    _format = XImageFormat.Jpeg;
                else
                    _format = XImageFormat.Png;
                return;
            }

            if (_gdiImage != null)
            {
                // ImageFormat has no overridden Equals function.
                string guid;
                try
                {
                    Lock.EnterGdiPlus();
                    guid = _gdiImage.RawFormat.Guid.ToString("B").ToUpper();
                }
                finally { Lock.ExitGdiPlus(); }

                switch (guid)
                {
                    case "{B96B3CAA-0728-11D3-9D7B-0000F81EF32E}":  // memoryBMP
                    case "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}":  // bmp
                    case "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}":  // png
                        _format = XImageFormat.Png;
                        break;

                    case "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}":  // jpeg
                        _format = XImageFormat.Jpeg;
                        break;

                    case "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}":  // gif
                        _format = XImageFormat.Gif;
                        break;

                    case "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}":  // tiff
                        _format = XImageFormat.Tiff;
                        break;

                    case "{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}":  // icon
                        _format = XImageFormat.Icon;
                        break;

                    case "{B96B3CAC-0728-11D3-9D7B-0000F81EF32E}":  // emf
                    case "{B96B3CAD-0728-11D3-9D7B-0000F81EF32E}":  // wmf
                    case "{B96B3CB2-0728-11D3-9D7B-0000F81EF32E}":  // exif
                    case "{B96B3CB3-0728-11D3-9D7B-0000F81EF32E}":  // photoCD
                    case "{B96B3CB4-0728-11D3-9D7B-0000F81EF32E}":  // flashPIX

                    default:
                        throw new InvalidOperationException("Unsupported image format.");
                }
            }
        }

        /// <summary>
        /// Under construction
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes underlying GDI+ object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                _disposed = true;

            _importedImage = null;

            if (_gdiImage != null)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    _gdiImage.Dispose();
                    _gdiImage = null;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }
        bool _disposed;

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        [Obsolete("Use either PixelWidth or PointWidth. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelWidth, but will become PointWidth in future releases of PDFsharp.")]
        public virtual double Width
        {
            get
            {
                if (_importedImage != null)
                {
                    return _importedImage.Information.Width;
                }

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        [Obsolete("Use either PixelHeight or PointHeight. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelHeight, but will become PointHeight in future releases of PDFsharp.")]
        public virtual double Height
        {
            get
            {
                if (_importedImage != null)
                {
                    return _importedImage.Information.Height;
                }

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// The factor for conversion from DPM to PointWidth or PointHeight.
        /// 72 points per inch, 1000 mm per meter, 25.4 mm per inch => 72 * 1000 / 25.4.
        /// </summary>
        const decimal FactorDPM72 = 72000 / 25.4m;

        /// <summary>
        /// The factor for conversion from DPM to PointWidth or PointHeight.
        /// 1000 mm per meter, 25.4 mm per inch => 1000 / 25.4.
        /// </summary>
        const decimal FactorDPM = 1000 / 25.4m;

        /// <summary>
        /// Gets the width of the image in point.
        /// </summary>
        public virtual double PointWidth
        {
            get
            {
                if (_importedImage != null)
                {
                    if (_importedImage.Information.HorizontalDPM > 0)
                        return (double)(_importedImage.Information.Width * FactorDPM72 / _importedImage.Information.HorizontalDPM);
                    if (_importedImage.Information.HorizontalDPI > 0)
                        return (double)(_importedImage.Information.Width * 72 / _importedImage.Information.HorizontalDPI);
                    // Assume 72 DPI if information not available.
                    return _importedImage.Information.Width;
                }

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width * 72 / _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// Gets the height of the image in point.
        /// </summary>
        public virtual double PointHeight
        {
            get
            {
                if (_importedImage != null)
                {
                    if (_importedImage.Information.VerticalDPM > 0)
                        return (double)(_importedImage.Information.Height * FactorDPM72 / _importedImage.Information.VerticalDPM);
                    if (_importedImage.Information.VerticalDPI > 0)
                        return (double)(_importedImage.Information.Height * 72 / _importedImage.Information.VerticalDPI);
                    // Assume 72 DPI if information not available.
                    return _importedImage.Information.Width;
                }

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height * 72 / _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public virtual int PixelWidth
        {
            get
            {
                if (_importedImage != null)
                    return (int)_importedImage.Information.Width;

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public virtual int PixelHeight
        {
            get
            {
                if (_importedImage != null)
                    return (int)_importedImage.Information.Height;

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// Gets the size in point of the image.
        /// </summary>
        public virtual XSize Size
        {
            get { return new XSize(PointWidth, PointHeight); }
        }

        /// <summary>
        /// Gets the horizontal resolution of the image.
        /// </summary>
        public virtual double HorizontalResolution
        {
            get
            {
                if (_importedImage != null)
                {
                    if (_importedImage.Information.HorizontalDPI > 0)
                        return (double)_importedImage.Information.HorizontalDPI;
                    if (_importedImage.Information.HorizontalDPM > 0)
                        return (double)(_importedImage.Information.HorizontalDPM / FactorDPM);
                    return 72;
                }

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// Gets the vertical resolution of the image.
        /// </summary>
        public virtual double VerticalResolution
        {
            get
            {
                if (_importedImage != null)
                {
                    if (_importedImage.Information.VerticalDPI > 0)
                        return (double)_importedImage.Information.VerticalDPI;
                    if (_importedImage.Information.VerticalDPM > 0)
                        return (double)(_importedImage.Information.VerticalDPM / FactorDPM);
                    return 72;
                }

                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.VerticalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether image interpolation is to be performed. 
        /// </summary>
        public virtual bool Interpolate
        {
            get { return _interpolate; }
            set { _interpolate = value; }
        }
        bool _interpolate = true;

        /// <summary>
        /// Gets the format of the image.
        /// </summary>
        public XImageFormat Format
        {
            get { return _format; }
        }
        XImageFormat _format;

        internal void AssociateWithGraphics(XGraphics gfx)
        {
            if (_associatedGraphics != null)
                throw new InvalidOperationException("XImage already associated with XGraphics.");
            _associatedGraphics = null;
        }

        internal void DisassociateWithGraphics()
        {
            if (_associatedGraphics == null)
                throw new InvalidOperationException("XImage not associated with XGraphics.");
            _associatedGraphics.DisassociateImage();

            Debug.Assert(_associatedGraphics == null);
        }

        internal void DisassociateWithGraphics(XGraphics gfx)
        {
            if (_associatedGraphics != gfx)
                throw new InvalidOperationException("XImage not associated with XGraphics.");
            _associatedGraphics = null;
        }

        internal XGraphics AssociatedGraphics
        {
            get { return _associatedGraphics; }
            set { _associatedGraphics = value; }
        }
        XGraphics _associatedGraphics;

        internal ImportedImage _importedImage;

        internal Image _gdiImage;

        /// <summary>
        /// If path starts with '*' the image is created from a stream and the path is a GUID.
        /// </summary>
        internal string _path;

        /// <summary>
        /// Contains a reference to the original stream if image was created from a stream.
        /// </summary>
        internal Stream _stream;

        /// <summary>
        /// Cache PdfImageTable.ImageSelector to speed up finding the right PdfImage
        /// if this image is used more than once.
        /// </summary>
        internal PdfImageTable.ImageSelector _selector;
    }
}
