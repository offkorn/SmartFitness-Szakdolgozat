

namespace SmartFitness.components
{
    public partial class DailyQuoteBlock : ContentView
    {
        private readonly List<string> _quotes = new();
        private readonly Random _rng = new();


        private readonly (Color, Color)[] _gradients =
        {
            (Colors.Blue, Colors.Purple),
            (Colors.Orange, Colors.Pink),
            (Colors.MintCream, Colors.Teal),
            (Colors.Indigo, Colors.Cyan),
            (Colors.Red, Colors.Orange),
            (Colors.Purple, Colors.Magenta)
        };

        public DailyQuoteBlock()
        {
            InitializeComponent();
            LoadQuotes();
            GenerateQuote();

        }

        //quotes beolvasas a quotes.txt -bol
        private void LoadQuotes()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("quotes.txt").Result;
            using var reader = new StreamReader(stream);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    _quotes.Add(line.Trim());
            }
        }

        private void GenerateQuote()
        {
            if (_quotes.Count == 0)
            {
                QuoteLabel.Text = "There is no quotes";
                return;
            }

            QuoteLabel.Text = _quotes[_rng.Next(_quotes.Count)];

            var gr = _gradients[_rng.Next(_gradients.Length)];

            QuoteFrame.Background = new LinearGradientBrush(
                new GradientStopCollection
                {
                    new GradientStop(gr.Item1, 0),
                    new GradientStop(gr.Item2, 1)
                },
                new Point(1, 1),
                new Point(0, 0)
            );
        }



        //kattintaskor frissul a quote
        private void OnQuoteTapped(object sender, TappedEventArgs e)
        {
            Console.WriteLine("TAPPED WORKS");
            GenerateQuote();
        }


        public void RefreshQuote()
        {
            GenerateQuote();
        }


    }

}
