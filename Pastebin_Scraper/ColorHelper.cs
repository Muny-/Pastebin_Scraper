using System;
namespace Pastebin_Scraper
{
    public static class ColorHelper
    {
        public static Gdk.RGBA Color8888(int a, int r, int g, int b)
        {
            return new Gdk.RGBA() { Alpha = a / 255.0, Red = r / 255.0, Green = g / 255.0, Blue = b / 255.0 };
        }
    }
}
