using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EngineLayer;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileDialogFilter = "Proteoform Results(*.csv;*.tsv;*.txt)|*.csv;*.tsv;*.txt";
        public List<DataForDataGrid> ValidationFilePath = new List<DataForDataGrid>();
        public List<DataForDataGrid> ResultFilePaths = new List<DataForDataGrid>();

        public MainWindow()
        {
            InitializeComponent();
            Title = "Proteoform Classifier";
            LoadParams();
            dataGridValidationResults.DataContext = ValidationFilePath;
            dataGridProteoformResults.DataContext = ResultFilePaths;

            WriteOutput.NotificationHandler += NotificationHandler;

        }

        private void LoadParams()
        {
            proteoformFormatDelimitedRadioButtonv.IsChecked = true;
            proteoformFormatDelimitedRadioButton.IsChecked = true;
            proteoformFormaParentheticalRadioButtonv.IsChecked = false;
            proteoformFormaParentheticalRadioButton.IsChecked = false;
            proteoformAndGeneDelimiterTextBoxv.Text = "|";
            proteoformAndGeneDelimiterTextBox.Text = "|";
            UpdateExample();
        }

        private void NotificationHandler(object sender, StringEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => NotificationHandler(sender, e)));
            }
            else
            {
                notificationsTextBox.AppendText(e.S);
                notificationsTextBox.AppendText(Environment.NewLine);
            }
        }

        private void AddValidationResults_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = StartOpenFileDialog(fileDialogFilter, false);

            if (openPicker.ShowDialog() == true)
            {
                ValidationFilePath.Clear();
                ValidationFilePath.Add(new DataForDataGrid(openPicker.FileNames.First()));
            }
            RefreshFileGrid();
        }

        private OpenFileDialog StartOpenFileDialog(string filter, bool multiselect)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                Multiselect = multiselect
            };

            return openFileDialog;
        }

        private void AnalyzeValidationResults_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationFilePath.Count == 1)
            {
                ToggleButtons(false);
                Validate.ValidateResults(ValidationFilePath.First().FilePath);
                ToggleButtons(true);
            }
            else
            {
                WriteOutput.Notify("Attempted to validate, but no result file was found. Please add a file and try again.");
            }
        }

        private void RefreshFileGrid()
        {
            dataGridValidationResults.CommitEdit(DataGridEditingUnit.Row, true);
            dataGridProteoformResults.CommitEdit(DataGridEditingUnit.Row, true);

            dataGridValidationResults.Items.Refresh();
            dataGridProteoformResults.Items.Refresh();
        }

        private string GetPathOfItem(object sender, RoutedEventArgs e)
        {
            DataForDataGrid item = (DataForDataGrid)sender;
            return item.FilePath;
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            ValidationFilePath.Clear();
            ResultFilePaths.Clear();
            RefreshFileGrid();
        }

        private void AddProteoformResults_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = StartOpenFileDialog(fileDialogFilter, true);

            if (openPicker.ShowDialog() == true)
            {
                foreach (string filename in openPicker.FileNames)
                {
                    ResultFilePaths.Add(new DataForDataGrid(filename));
                }

                RefreshFileGrid();
            }
        }

        /// <summary>
        /// Opens the requested URL with the user's web browser.
        /// </summary>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string path = e.Uri.ToString();
            OpenFile(path);
        }

        private void OpenFile(string path)
        {
            var p = new Process();

            p.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = path
            };

            p.Start();
        }

        /// <summary>
        /// Event fires when a file is dragged-and-dropped into the GUI.
        /// </summary>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
            {
                var selectedItem = (TabItem)MainWindowTabControl.SelectedItem;
                if (selectedItem.Header.Equals("Validate Input"))
                {
                    foreach (string file in files)
                    {
                        ValidationFilePath.Clear();
                        ValidationFilePath.Add(new DataForDataGrid(file));
                    }
                }
                else if (selectedItem.Header.Equals("Classify PrSMs"))
                {
                    foreach (string file in files)
                    {
                        ResultFilePaths.Add(new DataForDataGrid(file));
                    }
                }
                else
                {
                    WriteOutput.Notify("Please select a tab on the left side before dragging and dropping files.");
                }
                RefreshFileGrid();
            }
        }

        //needed to build, doesn't serve a function
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AnalyzeProteoformResults_Click(object sender, RoutedEventArgs e)
        {
            if (ResultFilePaths.Count > 0)
            {
                ToggleButtons(false);
                Classifier.ClassifyResultFiles(ResultFilePaths.Select(x => x.FilePath).ToList(), aggregateOutputCheckBox.IsChecked.Value);
                ToggleButtons(true);
            }
            else
            {
                WriteOutput.Notify("Attempted to classify, but no result files were found. Please add a file and try again.");
            }
        }

        private void ToggleButtons(bool enabled)
        {
            aggregateOutputCheckBox.IsEnabled = enabled;
            AnalyzeProteoformResults.IsEnabled = enabled;
            AnalyzeValidationResults.IsEnabled = enabled;
        }

        private void Delimited_Click(object sender, RoutedEventArgs e)
        {
            proteoformAndGeneDelimiterTextBox.IsEnabled = proteoformFormatDelimitedRadioButton.IsChecked.Value;
            ReadResults.ModifyProteoformFormat(proteoformFormatDelimitedRadioButton.IsChecked.Value ? ProteoformFormat.Delimited : ProteoformFormat.Parenthetical);
            proteoformAndGeneDelimiterTextBoxv.IsEnabled = proteoformFormatDelimitedRadioButton.IsChecked.Value;
            proteoformFormatDelimitedRadioButtonv.IsChecked = proteoformFormatDelimitedRadioButton.IsChecked;
            UpdateExample();
        }

        private void Delimited_Clickv(object sender, RoutedEventArgs e)
        {
            proteoformAndGeneDelimiterTextBoxv.IsEnabled = proteoformFormatDelimitedRadioButtonv.IsChecked.Value;
            ReadResults.ModifyProteoformFormat(proteoformFormatDelimitedRadioButtonv.IsChecked.Value ? ProteoformFormat.Delimited : ProteoformFormat.Parenthetical);
            proteoformAndGeneDelimiterTextBox.IsEnabled = proteoformFormatDelimitedRadioButtonv.IsChecked.Value;
            proteoformFormatDelimitedRadioButton.IsChecked = proteoformFormatDelimitedRadioButtonv.IsChecked;
            UpdateExample();
        }

        private void Delimiter_TextChange(object sender, TextChangedEventArgs e)
        {
            if (proteoformAndGeneDelimiterTextBox.Text.Length != 0)
            {
                proteoformAndGeneDelimiterTextBox.Text = proteoformAndGeneDelimiterTextBox.Text[0].ToString();//only use first char
                proteoformAndGeneDelimiterTextBoxv.Text = proteoformAndGeneDelimiterTextBox.Text;
                ReadResults.ModifySequenceAndGeneDelimiter(proteoformAndGeneDelimiterTextBox.Text[0]);
            }
            else
            {
                proteoformAndGeneDelimiterTextBox.Text = ReadResults.GetProteoformDelimiter().ToString();
                proteoformAndGeneDelimiterTextBoxv.Text = ReadResults.GetProteoformDelimiter().ToString();
            }
            UpdateExample();
        }

        private void Delimiter_TextChangev(object sender, TextChangedEventArgs e)
        {
            if (proteoformAndGeneDelimiterTextBoxv.Text.Length != 0)
            {
                proteoformAndGeneDelimiterTextBoxv.Text = proteoformAndGeneDelimiterTextBoxv.Text[0].ToString();
                proteoformAndGeneDelimiterTextBox.Text = proteoformAndGeneDelimiterTextBoxv.Text;
                ReadResults.ModifySequenceAndGeneDelimiter(proteoformAndGeneDelimiterTextBoxv.Text[0]);
            }
            else
            {
                proteoformAndGeneDelimiterTextBoxv.Text = ReadResults.GetProteoformDelimiter().ToString();
                proteoformAndGeneDelimiterTextBox.Text = ReadResults.GetProteoformDelimiter().ToString();
            }
            UpdateExample();
        }

        private void UpdateExample()
        {
            const string a = "M[Oxidation]AM";
            const string b = "MAM[Oxidation]";
            string thingy = "(MAM)[Oxidation]";
            if (ReadResults.GetProteoformFormat() == ProteoformFormat.Delimited)
            {
                thingy = a + ReadResults.GetProteoformDelimiter().ToString() + b;
            }
            exampleTextBox.Text = thingy;
            exampleTextBoxv.Text = thingy;
            exampleTextBox.FontSize = 10;
            exampleTextBoxv.FontSize = 10;
        }
    }
}