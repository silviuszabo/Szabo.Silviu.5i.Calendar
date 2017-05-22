using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Szabo.Silviu._5i.Calendar.Models;

namespace Szabo.Silviu._5i.Calendar
{
    /// <summary>
    ///     Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string ApplicationName = "Google Calendar API .NET Quickstart";
        private static readonly string[] Scopes = { CalendarService.Scope.CalendarReadonly };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserCredential credential;

            using (FileStream stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //  Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            CalendarService service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            if (events.Items != null && events.Items.Count > 0)
            {
                List<Evento> listEventi = new List<Evento>(events.Items.Count);
                listEventi.AddRange(
                    events.Items.Select(
                        eventItem =>
                            new Evento { Description = eventItem.Summary, Date = eventItem.Start.DateTime.ToString() }));
                MainFinestra.ItemsSource = listEventi;
                NumeroEventi.Content = "Numero Eventi: " + listEventi.Count;
            }
            else
            {
                MessageBox.Show("Your calendar hasn't any events ;)", "Calendar", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
                Close();
            }
        }
    }
}
