// file:///usr/share/applications/virtualbox.desktop
using System;
using System.Diagnostics;
using Gdk;

namespace Pastebin_Scraper.PasteTypes
{
    public class PasteTypeM3U : IPasteType
    {
        public PasteTypeM3U(string content)
        {
            Content = content;
        }

        public string Content { get; set; }

        public string DisplayType {
            get {
                return "M3U audio/video/image playlist";
            }
        }

        public RGBA DisplayColor {
            get {
                return ColorHelper.Color8888(255, 57, 155, 124);
            }
        }

        public void OpenExternal()
        {
			try
			{
				Console.WriteLine("Opening mpv...");
				ProcessStartInfo vlcSi = new ProcessStartInfo("mpv", "-");
				vlcSi.RedirectStandardInput = true;
				vlcSi.UseShellExecute = false;

				Process vlcProc = new Process();
				vlcProc.StartInfo = vlcSi;
				vlcProc.Start();

				vlcProc.StandardInput.Write(Content);
				vlcProc.StandardInput.Flush();
				vlcProc.StandardInput.Close();
			}
			catch
			{
				Console.WriteLine("failed to open mpv");
			}

            /*string pwd = vlcProc.StandardOutput.ReadToEnd();

            Console.WriteLine(pwd);*/
        }
    }
}
