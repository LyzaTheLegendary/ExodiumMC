using ExodiumMC.Core;
using Gtk;

namespace UserInterface
{
    public class Display
    {
        private static Window wnd = new("Exodium Server");
        private static Grid grid = new Grid();
        private static ScrolledWindow scWnd = new();
        private static Entry cmdInput = new();
        private static TextView textView = new();
        public static void Construct()
        {
            Application.Init();

            cmdInput.Activated += delegate
            {
                string? response = CommandParser.HandleCMD(cmdInput.Text);
                cmdInput.Text = string.Empty;
                if(response != null)
                    Write(response);
            };

            cmdInput.PlaceholderText = "Run your command here!";
            textView.Editable = false;
            textView.WrapMode = WrapMode.WordChar;
            wnd.DeleteEvent += (s, e) => { Application.Quit(); };
            wnd.Resizable = false;
            wnd.SetSizeRequest(800, 600);
            scWnd.SetSizeRequest(800, 580);
            grid.Attach(scWnd, 0, 0, 800, 600);
            grid.AttachNextTo(cmdInput, scWnd, PositionType.Bottom, 800, 20);
            scWnd.Add(textView);
            wnd.Add(grid);

            wnd.ShowAll();
            Application.Run();
        }
        public static void Write(string text) => Application.Invoke(delegate { textView.Buffer.Text += text + "\n"; });
        
    }
}
