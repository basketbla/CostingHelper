using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.IO;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using AngleSharp.Text;

namespace CostingHelper
{
    /// This is the core of the program. Scrapes the website for the selected number
    /// of results for each item. Outputs the average prices as well as the individual prices
    /// to the specified output file.
    public partial class Window1 : Window
    {
        bool usingFile = MainWindow.usingFile;
        string readFile = "";
        string writeFile = MainWindow.writeFile;

        //Things to get with scraper
        //private string Url { get; set; }
        //private string siteUrl = "https://shopping.google.com/u/0/";
        private string siteUrl = "";
        public List<string> QueryTerms = new List<string>();


        public Window1()
        {
            InitializeComponent();
            ReadFileLabel.Content = "Doing some scraping and such...";
            
            if(usingFile)
            {
                readFile = MainWindow.readFile;
            }
            else
            {
                readFile = DirectEntryWindow.directText;
            }

            List<int> selected = MainWindow.selectedSites;
            List<int> numResults = MainWindow.numResultsList;

            ReadInput();
            if (selected.Contains(0))
            {
                foreach (string term in QueryTerms)
                {
                    siteUrl = "https://www.google.com/search?psb=1&tbm=shop&q=" + term + "&ved=0CAQQr4sDKAJqFwoTCI2Vw6OSyesCFVoeswAdo2EPURAD";
                    ScrapeGoogle(term, numResults[0] + 1);
                }
            }
            /*
            if (selected.Contains(1))
            {
                foreach (string term in QueryTerms)
                {
                    //If i add more sites, also need to fix the 'done' output and put a label. 
                    siteUrl = "https://www.walgreens.com/search/results.jsp?Ntt=" + term;
                    ScrapeWalgreens(term, numResults[1] + 1);
                }
            }
            if (selected.Contains(2))
            {
                foreach (string term in QueryTerms)
                {
                    siteUrl = "https://www.amazon.com/s?k=" + term + "&ref=nb_sb_noss%2Ferrors%2FvalidateCaptcha";
                    ScrapeAmazon(term, numResults[2] + 1);
                }
            }
            if (selected.Contains(3))
            {
                foreach (string term in QueryTerms)
                {
                    siteUrl = "https://www.wellrx.com/prescriptions/" + term + "/72703/?freshSearch=true";
                    //ScrapeWellRx(term, numResults[3]+1);
                }
            }
            if (selected.Contains(4))
            {
                foreach (string term in QueryTerms)
                {
                    siteUrl = "https://www.google.com/search?psb=1&tbm=shop&q=" + term + "&ved=0CAQQr4sDKAJqFwoTCI2Vw6OSyesCFVoeswAdo2EPURAD";
                    //ScrapeGoogle(term, numResults[4]+1);
                }
            }*/


        }

        //for reading from .txt file
        public void ReadInput()
        {
            resultsTextBox.Text += "\nStarting readInput";
            if(usingFile)
            {
                StreamReader sr = new StreamReader(readFile);
                string line = sr.ReadLine();
                while (line != null)
                {
                    QueryTerms.Add(line);
                    line = sr.ReadLine();
                }
            }
            else
            {
                StringReader strr = new StringReader(readFile);
                string line = strr.ReadLine();
                while (line != null)
                {
                    QueryTerms.Add(line);
                    line = strr.ReadLine();
                }
            }
        }

        //scraping
        internal async void ScrapeGoogle(string term, int numResults)
        {
            resultsTextBox.Text += "\nStarting ScrapeGoogle";
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage request = await httpClient.GetAsync(siteUrl);
            cancellationToken.Token.ThrowIfCancellationRequested();

            Stream response = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(response);

            //GetScrapeResults(document);

            /*var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(siteUrl);*/
            //string text = document.DocumentElement.OuterHtml;
            //System.IO.File.WriteAllText(@"C:\Users\Surface\Desktop\canehtml.txt", text);

            //resultsTextBox.Text += "\nStarting GetScrapeResults";
            IEnumerable<IElement> items = null;
            IEnumerable<IElement> prices = null;
            IEnumerable<IElement> shipping = null;
            IEnumerable<IElement> sellers = null;

            items = document.All.Where(x => x.ClassName == "rgHvZc");
            prices = document.All.Where(x => x.ClassName == "HRLxBb");
            shipping = document.All.Where(x => x.LocalName == "span" && x.ClassName == "dD8iuc");
            sellers = document.All.Where(x => x.LocalName == "div" && x.ClassName == "dD8iuc");


            int startingVal = numResults;
            numResults = Math.Min(numResults, items.ToList().Count);
            numResults = Math.Min(numResults, prices.ToList().Count);
            numResults = Math.Min(numResults, shipping.ToList().Count);
            numResults = Math.Min(numResults, sellers.ToList().Count);

            if (numResults < startingVal)
            {
                resultsTextBox.Text += "Didn't have enough results to get the number that was asked for.";
                //Pull up some kind of warning or something
            }


            List<string> itemList = CleanUpGoogle(items, "items", numResults);
            List<string> pricesList = CleanUpGoogle(prices, "prices", numResults);
            List<string> shippingList = CleanUpGoogle(shipping, "shipping", numResults);
            List<string> sellersList = CleanUpGoogle(sellers, "sellers", numResults);


            string output = "";

            for (int i = 0; i < numResults; i++)
            {
                output += $"\nItem {(i + 1)}: " + itemList[i].Replace("<b>", "").Replace("</b>", "") + $"\nPrice {(i + 1)}: " +
                pricesList[i] + $"\nShipping {(i + 1)}: " + shippingList[i] + $"\nSeller {(i + 1)}: " + sellersList[i] + "\n\n";
            }

            double priceSum = 0.0;
            string num = "";
            foreach (string price in pricesList)
            {
                num = price.Substring(1);
                priceSum += num.ToDouble();
            }
            foreach (string price in shippingList)
            {
                if (!shippingList.Contains("free"))
                {
                    num = price.Substring(2);
                    priceSum += num.ToDouble();
                }
            }
            double avg = priceSum / numResults;
            avg = Math.Round(avg, 2);


            System.IO.File.AppendAllText(writeFile, $"{term}");
            System.IO.File.AppendAllText(writeFile, $"\nAverage price (including shipping):${avg}\n");
            System.IO.File.AppendAllText(writeFile, output);
            System.IO.File.AppendAllText(writeFile, "\n\n\n");

            ReadFileLabel.Content = "All done!";

        }

        public List<string> CleanUpGoogle(IEnumerable<IElement> articleLink, string type, int numResults)
        {
            List<string> output = new List<string>();
            resultsTextBox.Text += "\nStarting CleanUpGoogle";
            int count = 0;
            string sentence = "";

            if (type == "items")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf("href"));
                        resultsTextBox.Text += $"\n{sentence}";
                        sentence = sentence.Substring(sentence.IndexOf(">") + 1, sentence.IndexOf("</a") - sentence.IndexOf(">") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            else if (type == "prices")
            {

                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf(">$") + 1, sentence.IndexOf("</") - sentence.IndexOf(">$") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        //resultsTextBox.Text += $"\nINDEX OF >$ IS {sentence.IndexOf(">$")}, INDEX OF </ IS {sentence.IndexOf("</")}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }

            }
            else if (type == "shipping")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf(">") + 1, sentence.IndexOf("ship") - sentence.IndexOf(">") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            else if (type == "sellers")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf("</span> from") + 12, sentence.IndexOf("</div") - sentence.IndexOf("</span> from") - 12);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }

            return output;
        }

        /*
        internal async void ScrapeWalgreens(string term, int numResults)
        {
            resultsTextBox.Text += "\nStarting ScrapeWalgreens";
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage request = await httpClient.GetAsync(siteUrl);
            cancellationToken.Token.ThrowIfCancellationRequested();

            Stream response = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(response);


            string text = document.DocumentElement.OuterHtml;
            System.IO.File.WriteAllText(writeFile, text);

            IEnumerable<IElement> items = null;
            IEnumerable<IElement> prices = null;
            IEnumerable<IElement> shipping = null;
            IEnumerable<IElement> sellers = null;

            items = document.All.Where(x => x.ClassName == "rgHvZc");
            prices = document.All.Where(x => x.ClassName == "HRLxBb");
            shipping = document.All.Where(x => x.LocalName == "span" && x.ClassName == "dD8iuc");
            sellers = document.All.Where(x => x.LocalName == "div" && x.ClassName == "dD8iuc");


            int startingVal = numResults;
            numResults = Math.Min(numResults, items.ToList().Count);
            numResults = Math.Min(numResults, prices.ToList().Count);
            numResults = Math.Min(numResults, shipping.ToList().Count);
            numResults = Math.Min(numResults, sellers.ToList().Count);

            if (numResults < startingVal)
            {
                resultsTextBox.Text += "Didn't have enough results to get the number that was asked for.";
                //Pull up some kind of warning or something
            }


            List<string> itemList = CleanUpWalgreens(items, "items", numResults);
            List<string> pricesList = CleanUpWalgreens(prices, "prices", numResults);
            List<string> shippingList = CleanUpWalgreens(shipping, "shipping", numResults);
            List<string> sellersList = CleanUpWalgreens(sellers, "sellers", numResults);


            string output = "";

            for (int i = 0; i < numResults; i++)
            {
                output += $"\nItem {(i + 1)}: " + itemList[i].Replace("<b>", "").Replace("</b>", "") + $"\nPrice {(i + 1)}: " +
                pricesList[i] + $"\nShipping {(i + 1)}: " + shippingList[i] + $"\nSeller {(i + 1)}: " + sellersList[i] + "\n\n";
            }

            double priceSum = 0.0;
            string num = "";
            foreach (string price in pricesList)
            {
                num = price.Substring(1);
                priceSum += num.ToDouble();
            }
            foreach (string price in shippingList)
            {
                if (!shippingList.Contains("free"))
                {
                    num = price.Substring(2);
                    priceSum += num.ToDouble();
                }
            }
            double avg = priceSum / numResults;
            avg = Math.Round(avg, 2);


            System.IO.File.AppendAllText(writeFile, $"{term}");
            System.IO.File.AppendAllText(writeFile, $"\nAverage price: ${avg}\n");
            System.IO.File.AppendAllText(writeFile, output);
            System.IO.File.AppendAllText(writeFile, "\n\n\n");

            ReadFileLabel.Content = "All done!";

        }

        public List<string> CleanUpWalgreens(IEnumerable<IElement> articleLink, string type, int numResults)
        {
            List<string> output = new List<string>();
            resultsTextBox.Text += "\nStarting CleanUpWalgreens";
            int count = 0;
            string sentence = "";

            if (type == "items")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf("href"));
                        resultsTextBox.Text += $"\n{sentence}";
                        sentence = sentence.Substring(sentence.IndexOf(">") + 1, sentence.IndexOf("</a") - sentence.IndexOf(">") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            else if (type == "prices")
            {

                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf(">$") + 1, sentence.IndexOf("</") - sentence.IndexOf(">$") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        //resultsTextBox.Text += $"\nINDEX OF >$ IS {sentence.IndexOf(">$")}, INDEX OF </ IS {sentence.IndexOf("</")}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }

            }
            else if (type == "shipping")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf(">") + 1, sentence.IndexOf("ship") - sentence.IndexOf(">") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            else if (type == "sellers")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf("</span> from") + 12, sentence.IndexOf("</div") - sentence.IndexOf("</span> from") - 12);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }

            return output;
        }

        internal async void ScrapeAmazon(string term, int numResults)
        {
            resultsTextBox.Text += "\nStarting ScrapeTarget";
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage request = await httpClient.GetAsync(siteUrl);
            cancellationToken.Token.ThrowIfCancellationRequested();

            Stream response = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(response);


            string text = document.DocumentElement.OuterHtml;
            System.IO.File.WriteAllText(writeFile, text);

            IEnumerable<IElement> items = null;
            IEnumerable<IElement> prices = null;
            IEnumerable<IElement> shipping = null;
            IEnumerable<IElement> sellers = null;

            items = document.All.Where(x => x.ClassName == "rgHvZc");
            prices = document.All.Where(x => x.ClassName == "HRLxBb");
            shipping = document.All.Where(x => x.LocalName == "span" && x.ClassName == "dD8iuc");
            sellers = document.All.Where(x => x.LocalName == "div" && x.ClassName == "dD8iuc");


            int startingVal = numResults;
            numResults = Math.Min(numResults, items.ToList().Count);
            numResults = Math.Min(numResults, prices.ToList().Count);
            numResults = Math.Min(numResults, shipping.ToList().Count);
            numResults = Math.Min(numResults, sellers.ToList().Count);

            if (numResults < startingVal)
            {
                resultsTextBox.Text += "Didn't have enough results to get the number that was asked for.";
                //Pull up some kind of warning or something
            }


            List<string> itemList = CleanUpAmazon(items, "items", numResults);
            List<string> pricesList = CleanUpAmazon(prices, "prices", numResults);
            List<string> shippingList = CleanUpAmazon(shipping, "shipping", numResults);
            List<string> sellersList = CleanUpAmazon(sellers, "sellers", numResults);


            string output = "";

            for (int i = 0; i < numResults; i++)
            {
                output += $"\nItem {(i + 1)}: " + itemList[i].Replace("<b>", "").Replace("</b>", "") + $"\nPrice {(i + 1)}: " +
                pricesList[i] + $"\nShipping {(i + 1)}: " + shippingList[i] + $"\nSeller {(i + 1)}: " + sellersList[i] + "\n\n";
            }

            double priceSum = 0.0;
            string num = "";
            foreach (string price in pricesList)
            {
                num = price.Substring(1);
                priceSum += num.ToDouble();
            }
            foreach (string price in shippingList)
            {
                if (!shippingList.Contains("free"))
                {
                    num = price.Substring(2);
                    priceSum += num.ToDouble();
                }
            }
            double avg = priceSum / numResults;
            avg = Math.Round(avg, 2);


            System.IO.File.AppendAllText(writeFile, $"{term}");
            System.IO.File.AppendAllText(writeFile, $"\nAverage price: ${avg}\n");
            System.IO.File.AppendAllText(writeFile, output);
            System.IO.File.AppendAllText(writeFile, "\n\n\n");

            ReadFileLabel.Content = "All done!";

        }

        public List<string> CleanUpAmazon(IEnumerable<IElement> articleLink, string type, int numResults)
        {
            List<string> output = new List<string>();
            resultsTextBox.Text += "\nStarting CleanUpTarget";
            int count = 0;
            string sentence = "";

            if (type == "items")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf("href"));
                        resultsTextBox.Text += $"\n{sentence}";
                        sentence = sentence.Substring(sentence.IndexOf(">") + 1, sentence.IndexOf("</a") - sentence.IndexOf(">") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            else if (type == "prices")
            {

                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf(">$") + 1, sentence.IndexOf("</") - sentence.IndexOf(">$") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        //resultsTextBox.Text += $"\nINDEX OF >$ IS {sentence.IndexOf(">$")}, INDEX OF </ IS {sentence.IndexOf("</")}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }

            }
            else if (type == "shipping")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf(">") + 1, sentence.IndexOf("ship") - sentence.IndexOf(">") - 1);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            else if (type == "sellers")
            {
                foreach (IElement result in articleLink)
                {
                    if (count < numResults)
                    {
                        sentence = result.OuterHtml;
                        sentence = sentence.Substring(sentence.IndexOf("</span> from") + 12, sentence.IndexOf("</div") - sentence.IndexOf("</span> from") - 12);
                        resultsTextBox.Text += $"\n{sentence}";
                        output.Add(sentence);
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }

            return output;
        }*/
    }
}