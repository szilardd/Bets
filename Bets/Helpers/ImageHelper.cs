using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Bets.Data;
using Svg;

namespace Bets
{
    public static class ImageHelper
    {
        public static void GeneratePngFromSvg()
        {
            GenerateFlagPngFromSvg();

            var bonusPath = HostingEnvironment.MapPath("~/content/img/icons/bonus.svg");
            SvgToPng(bonusPath, 16, 16);
        }

        private static void GenerateFlagPngFromSvg()
        {
            var flagPath = HostingEnvironment.MapPath("~/" + UIExtensions.TeamFlagUrl);

            var paths = Directory.GetFiles(flagPath, "*.svg");

            foreach (var path in paths)
            {
                try
                {
                    SvgToPng(path, 21, 16);
                }
                catch (Exception ex)
                {
                    Logger.Log(new Exception("Failed to convert svg to png " + path));
                    Logger.Log(ex);
                }
            }
        }

        /// <summary>
        /// Converts svg to png and saves next to svg
        /// </summary>
        private static void SvgToPng(string imagePath, int width, int height)
        {
            var pngPath = imagePath.Replace(".svg", ".png");

            if (File.Exists(pngPath))
            {
                File.Delete(pngPath);
            }

            var svgFileContents = File.ReadAllText(imagePath);

            var byteArray = Encoding.ASCII.GetBytes(svgFileContents);

            using (var stream = new MemoryStream(byteArray))
            {
                var svgDocument = SvgDocument.Open(stream);
                
                using (var bitmap = svgDocument.Draw())
                {
                    using (var image = bitmap.GetThumbnailImage(width, height, null, IntPtr.Zero))
                    {
                        image.Save(pngPath, ImageFormat.Png);
                    }
                }
            }
        }
    }
}
