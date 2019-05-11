using ELIZA.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace XamEliza
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Line> _dialog = new ObservableCollection<Line>();
        Session _session;

        Random _random;

        public MainPage()
        {
            Device.SetFlags(new List<string>(Device.Flags ?? new List<string>()) { "CollectionView_Experimental" });

            InitializeComponent();

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            ResourceManager rm = new ResourceManager("XamEliza.doctor", Assembly.GetExecutingAssembly());
            String json = rm.GetString("json");
            var eliza = new ELIZA.NET.ELIZALib(json);
            _session = eliza.Session;

            Conversation.ItemsSource = _dialog;

            _dialog.Add(new Line(Speaker.Eliza, _session.GetGreeting()));

            _random = new Random(DateTime.Now.Second);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var userText = UserEntry.Text;

            UserEntry.Text = string.Empty;

            _dialog.Add(new Line(Speaker.User, userText));

            var delay = (int)(1000 * _random.Next(0, 3) + (_random.NextDouble() * 1000));

            await Task.Delay(delay);

            _dialog.Add(new Line(Speaker.Eliza, _session.GetResponse(userText)));
        }
    }

    public class SpeakerSelector : DataTemplateSelector
    {
        public DataTemplate ElizaTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var line = (Line)item;

            return line.Speaker == Speaker.User 
                ? UserTemplate
                : ElizaTemplate;
        }
    }

    public enum Speaker
    {
        Eliza,
        User
    }

    public class Line
    {
        public Line(Speaker speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }

        public Speaker Speaker { get; set; }
        public string Text { get; set; }
    }
}
