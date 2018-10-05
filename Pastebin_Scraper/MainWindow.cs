// file:///usr/share/applications/virtualbox.desktop
using System;
using System.Net;
using Gtk;
using HtmlAgilityPack;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pastebin_Scraper;
using System.Net.Http;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        /*MessageDialog md = new MessageDialog(this,
                DialogFlags.DestroyWithParent, MessageType.Info,
                ButtonsType.Close, "Download completed");
        md.Run();
        md.Destroy();*/

        notebook1.SwitchPage += notebook1_SwitchPage;

        FetchPastebin();

        GLib.Timeout.Add(60000, new GLib.TimeoutHandler(FetchPastebin));
    }

    void notebook1_SwitchPage(object o, SwitchPageArgs args)
    {
        GetSelectedPaste((paste) => {
            paste.LazyLoadContent();
        });
    }

    void GetSelectedPaste(System.Action<ScrapedPaste> doThis)
    {
        if (notebook1.Children[notebook1.Page].GetType() == typeof(ScrapedPaste))
        {
            ScrapedPaste paste = (ScrapedPaste)notebook1.Children[notebook1.Page];

            doThis(paste);
        }
        else
        {
            // is probably home page
        }
    }

    public Dictionary<string, ScrapedPaste> RecentPastes = new Dictionary<string, ScrapedPaste>();

    bool FetchPastebin()
    {
        HttpClient client = new HttpClient();

        string response = client.GetStringAsync("http://pastebin.com/archive").Result;

        HtmlDocument document = new HtmlDocument();

        document.LoadHtml(response);

        var query_results = document.DocumentNode.Descendants("table").Where(node => node.HasClass("maintable"));

        if (query_results.Count() > 0)
        {
            var recent_pastes_table_node = query_results.First();

            foreach (var row_node in recent_pastes_table_node.Descendants("tr"))
            {
                var children = row_node.Descendants("td");

                if (children.Count() > 0)
                {
                    var a_link = children.First().Descendants("a").First();
                    string paste_id = a_link.Attributes["href"].Value.Remove(0, 1);
                    string paste_title = a_link.InnerText;

                    if (!RecentPastes.ContainsKey(paste_id))
                    {
                        Console.WriteLine($"id: {paste_id}, title: {paste_title}");

                        ScrapedPaste paste = new ScrapedPaste(paste_id);

                        RecentPastes.Add(paste_id, paste);

                        notebook1.AppendPage(paste, new Label($"{paste_title} [{paste_id}]"));

                        notebook1.ShowAll();

                        //Thread.Sleep(1000);
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("No results...");
        }

        /*ScrapedPaste paste = new ScrapedPaste("testm3u");

        notebook1.AppendPage(paste, new Label("testm3u"));

        notebook1.ShowAll();*/

        return true;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnDefaultActivated(object sender, EventArgs e)
    {

    }

    protected void OnFocusActivated(object sender, EventArgs e)
    {

    }

    protected void OnButton4Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("clicked..");

        TextBuffer tb = new TextBuffer(null);
        tb.Text = "lol this is a test";
        TextView tv = new TextView(tb);
        tv.Editable = false;

        notebook1.AppendPage(tv, null);

        this.ShowAll();
    }

    [GLib.ConnectBefore]
    protected void Window_KeyPress(object o, KeyPressEventArgs args)
    {
        Console.WriteLine($"Caught keypress: {args.Event.Key.ToString()}");
        if (args.Event.Key == Gdk.Key.o)
        {
            GetSelectedPaste((paste) => {
                paste.OpenExternal();
            });
        }

    }
}