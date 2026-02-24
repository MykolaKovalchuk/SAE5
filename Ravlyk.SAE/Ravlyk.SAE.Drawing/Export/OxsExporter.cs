using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Ravlyk.SAE.Drawing.Export
{
    /// <summary>
    /// Exports scheme image to Open Cross Stitch File Format (.oxs).
    /// </summary>
    public static class OxsExporter
    {
        public static void ExportToOxs(
            CodedImage image,
            string fileName,
            IList<CodedColor> orderedColors)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("chart");

                // ===== format =====
                writer.WriteStartElement("format");
                writer.WriteAttributeString("comments01", "Exported from SAE");
                writer.WriteEndElement();

                // ===== properties =====
                writer.WriteStartElement("properties");
                writer.WriteAttributeString("oxs_version", "1.0");
                writer.WriteAttributeString("software", "SAE");
                writer.WriteAttributeString("software_version", "1.0");
                writer.WriteAttributeString("chartwidth", image.Size.Width.ToString());
                writer.WriteAttributeString("chartheight", image.Size.Height.ToString());
                writer.WriteAttributeString("palettecount", orderedColors.Count.ToString());
                writer.WriteEndElement();

                // ===== palette =====
                writer.WriteStartElement("palette");

                var colorIndex = new Dictionary<CodedColor, int>();

                for (int i = 0; i < orderedColors.Count; i++)
                {
                    var c = orderedColors[i];
                    int index = i + 1;

                    colorIndex[c] = index;

                    var rgbHex = ToRgbHex(c);

                    writer.WriteStartElement("palette_item");
                    writer.WriteAttributeString("index", index.ToString());
                    writer.WriteAttributeString("number", c.ColorCode ?? "");
                    writer.WriteAttributeString("name", c.ColorName ?? "");
                    writer.WriteAttributeString("color", rgbHex);
                    writer.WriteAttributeString("printcolor", rgbHex);
                    writer.WriteAttributeString("symbol", c.SymbolChar.ToString());
                    writer.WriteAttributeString("strands", "2");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // palette

                // ===== fullstitches =====
                writer.WriteStartElement("fullstitches");

                for (int y = 0; y < image.Size.Height; y++)
                {
                    for (int x = 0; x < image.Size.Width; x++)
                    {
                        var codedColor = image[x, y];

                        if (!colorIndex.TryGetValue(codedColor, out int palIndex))
                            continue;

                        writer.WriteStartElement("stitch");
                        writer.WriteAttributeString("x", (x + 1).ToString());
                        writer.WriteAttributeString("y", (y + 1).ToString());
                        writer.WriteAttributeString("palindex", palIndex.ToString());
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement(); // fullstitches

                // ===== empty required sections =====
                writer.WriteStartElement("backstitches");
                writer.WriteEndElement();

                writer.WriteStartElement("partstitches");
                writer.WriteEndElement();

                writer.WriteStartElement("commentboxes");
                writer.WriteEndElement();

                writer.WriteEndElement(); // chart
                writer.WriteEndDocument();
            }
        }

        private static string ToRgbHex(CodedColor color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}