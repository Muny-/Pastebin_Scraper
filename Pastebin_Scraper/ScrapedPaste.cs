// file:///usr/share/applications/virtualbox.desktop
using System;
using System.IO;
using System.Net;
using Gtk;
using Pastebin_Scraper.PasteTypes;

namespace Pastebin_Scraper
{
    public class ScrapedPaste : VBox
    {
        public string PastebinID;

        public string Content
        {
            get
            {
                return tb.Text;
            }
            set
            {
                tb.Text = value;

                if (value.StartsWith("#EXTM3U") || value.StartsWith("#EXTINF:"))
                {
                    // is an m3u

                    string[] parts = value.Split('\n');

                    if (parts[0].StartsWith("#EXTM3U") && parts[0].Trim() != "#EXTM3U")
                    {
                        // fix for some improperly formatted files
                        parts[0] = "#EXTM3U";
                    }

                    string _content = String.Join("\n", parts);

                    PasteType = new PasteTypeM3U(_content);
                }
            }
        }

        public void OpenExternal()
        {
            if (PasteType != null)
            {
                PasteType.OpenExternal();
            }
        }

        IPasteType PasteType;

        TextBuffer tb = new TextBuffer(null);

        public ScrapedPaste(string id)
        {
            this.PastebinID = id;
        }

        public void LazyLoadContent()
        {
            if (Content == String.Empty)
            {
                if (PastebinID == "testm3u")
                {
                    SetContent(File.ReadAllText("sample.m3u8"));
                }
                else
                {
                    Console.WriteLine($"Lazy loading {PastebinID}...");
                    WebClient wc = new WebClient();

                    string paste_content = wc.DownloadString($"https://pastebin.com/raw/{PastebinID}");

                    SetContent(paste_content);
                }
            }
        }

        void SetContent(string content)
        {
            Content = content;
            tb.Text = content;

            Label pasteTypeLabel = new Label("Unrecognized file format");

            if (PasteType != null)
            {
                Console.WriteLine($"recognized paste type: {PasteType.DisplayType}");
                pasteTypeLabel.Text = PasteType.DisplayType;
                pasteTypeLabel.OverrideBackgroundColor(StateFlags.Normal, PasteType.DisplayColor);
            }
            else
            {
                Console.WriteLine("didn't recognize paste type");
            }

            this.PackStart(pasteTypeLabel, false, true, 0);

            TextView tv = new TextView(tb);
            tv.Editable = false;

            ScrolledWindow sw = new ScrolledWindow();

            sw.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            sw.Add(tv);

            this.PackEnd(sw, true, true, 0);

            this.Parent.ShowAll();
        }
    }
}
