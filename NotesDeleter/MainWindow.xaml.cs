using System.Windows;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Windows.Documents;

namespace NotesDeleter
{
    public partial class MainWindow : Window
    {
        private string pathFrom;
        private string pathTo;

        private string currentParagraph;
        private string matchStr;

        private Regex reg;
        private Regex regN;
        private MatchCollection matches;
        private int countProcessedMatches;

        private StreamReader reader;
        private StreamWriter writer;

        public MainWindow()
        {
            InitializeComponent();
            reg = new Regex(@"\[.+\]|\(.+\)"); // Из-за точки, вместо которой может подставиться скобка, охватывается лишнее
            regN = new Regex(@"(\[|\()\d+ - ");
            countProcessedMatches = 0;
        }

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                pathFrom = openFileDialog.FileName;
                pathTo = openFileDialog.FileName.Split("\\").Last();

                reader = new StreamReader(pathFrom);
                writer = new StreamWriter(pathTo);

                Process();
            }
        }

        private void Process()
        {
            while (!reader.EndOfStream)
            {
                currentParagraph = reader.ReadLine();

                matches = reg.Matches(currentParagraph);
                if (matches.Count > 0)
                {
                    LoadForm();
                    break;
                }

                writer.WriteLine(currentParagraph);
            }

            writer.Flush();
        }

        private void LoadForm()
        {
            matches = reg.Matches(currentParagraph);
            matchStr = matches[0].Value;

            SetTextBlock(currentParagraph, matchStr);

            buttonDelete.IsEnabled = true;
            if (regN.IsMatch(matchStr))
            {
                buttonRemoveNumber.IsEnabled = true;
            }
            if (matchStr[0] == '[')
            {
                buttonReplaceStoR.IsEnabled = true;
            }
            else
            {
                buttonReplaceRtoS.IsEnabled = true;
            }
        }

        private void SetTextBlock(string text, string current)
        {
            string before = currentParagraph.Split(matchStr)[0];
            string after = currentParagraph.Split(matchStr).Length > 1 ? currentParagraph.Split(matchStr)[1] : "";

            textBlock.Inlines.Clear();
            textBlock.Inlines.Add(new Run(before));
            textBlock.Inlines.Add(new Bold(new Run(matchStr)));
            textBlock.Inlines.Add(new Run(after));
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = textBlock.Text.Replace(matchStr, "");

            buttonDelete.IsEnabled = false;
            buttonRemoveNumber.IsEnabled = false;
            buttonReplaceRtoS.IsEnabled = false;
            buttonReplaceStoR.IsEnabled = false;
        }

        private void buttonRemoveNumber_Click(object sender, RoutedEventArgs e)
        {
            Match match = regN.Match(matchStr);
            string newMatchStr = matchStr.Replace(match.Value, match.Value[0].ToString());
            currentParagraph = currentParagraph.Replace(matchStr, newMatchStr);
            matchStr = newMatchStr;
            SetTextBlock(currentParagraph, matchStr);
            buttonRemoveNumber.IsEnabled = false;
        }

        private void buttonReplaceRtoS_Click(object sender, RoutedEventArgs e)
        {
            string newBrackets = matchStr.Replace('(', '[').Replace(')', ']');
            currentParagraph = currentParagraph.Replace(matchStr, newBrackets);
            matchStr = newBrackets;
            SetTextBlock(currentParagraph, matchStr);
            buttonReplaceRtoS.IsEnabled = false;
            buttonReplaceStoR.IsEnabled = true;
        }

        private void buttonReplaceStoR_Click(object sender, RoutedEventArgs e)
        {
            string newBrackets = matchStr.Replace('[', '(').Replace(']', ')');
            currentParagraph = currentParagraph.Replace(matchStr, newBrackets);
            matchStr = newBrackets;
            SetTextBlock(currentParagraph, matchStr);
            buttonReplaceRtoS.IsEnabled = true;
            buttonReplaceStoR.IsEnabled = false;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            countProcessedMatches++;
            matches = reg.Matches(currentParagraph);
            if (matches.Count > countProcessedMatches)
            {
                LoadForm();
            }
            else
            {
                countProcessedMatches = 0;
                writer.WriteLine(textBlock.Text);
                Process();
            }
        }
    }
}
