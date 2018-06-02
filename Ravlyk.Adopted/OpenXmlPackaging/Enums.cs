using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    [Flags]
    public enum FontStyles {
        Bold = 1,
        Italic = 2,
        Regular = 4,
        Strikeout = 8,
        Underline = 16,
        DoubleUnderline = 32,
        Superscript = 64,
        Subscript = 128,
    }

    public enum BorderType {
        [Element("right")]
        Right,
        [Element("left")]
        Left,
        [Element("top")]
        Top,
        [Element("bottom")]
        Bottom,
        [Element("diagonal")]
        Diagonal
    }

    public enum BorderStyles {
        [Element("thin")]
        Thin,
        [Element("thick")]
        Thick,
        [Element("hair")]
        Hair,
        [Element("dotted")]
        Dotted,
        [Element("dashDotDot")]
        DashDotDot,
        [Element("dashDot")]
        DashDot,
        [Element("dashed")]
        Dashed,
        [Element("mediumDashDotDot")]
        MediumDashDotDot,
        [Element("slantDashDot")]
        SlantDashDot,
        [Element("mediumDashDot")]
        MediumDashDot,
        [Element("medium")]
        Medium,
        [Element("mediumDashed")]
        MediumDashed,
        [Element("double")]
        Double,
        [Element("none")]
        None
    }

    [Element("vertical")]
    public enum VerticalAlignment {
        [Element("top")]
        Top = 1,
        [Element("middle")]
        Middle = 2,
        [Element("bottom")]
        Bottom = 3,
        [Element("")]
        None
    }

    [Element("horizontal")]
    public enum HorizontalAlignment {
        [Element("left")]
        Left = 1,
        [Element("center")]
        Center = 2,
        [Element("right")]
        Right = 3,
        [Element("")]
        None
    }    
}
