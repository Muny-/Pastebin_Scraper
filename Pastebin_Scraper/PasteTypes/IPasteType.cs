// file:///usr/share/applications/virtualbox.desktop
using System;
namespace Pastebin_Scraper.PasteTypes
{
    public interface IPasteType
    {
        string Content { get; set; }

        string DisplayType { get; }

        Gdk.RGBA DisplayColor { get; }

        void OpenExternal();
    }
}
