using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using FileHelpers;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;
using System.Windows.Controls.Primitives;
using Microsoft.VisualBasic;

namespace PrvniWPF
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        ObservableCollection<ExeSoubory> vypis = new ObservableCollection<ExeSoubory>();
        string jakySoubor ="";
        string newdef = "";
        string defaultCesta = @"A:\VS-projekty";
        List<string> ListCest = new List<string>();
        List<string> ListExe = new List<string>();

        public MainWindow()
        {
            
            InitializeComponent();
            nacteniDat(defaultCesta);
            
        }

        [DelimitedRecord(",")]
        public class CSVFile
        {
            public string NazevProjektu;

            public string Verze;

            public string Poznamka;

        }
        private void personsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                jakySoubor = personsList.SelectedItem.ToString();
            }

            catch (Exception oo)
            {
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int overeni = JeVybrano(jakySoubor);
            if(overeni==0)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Musíte vybrat soubor", "Upozornění", MessageBoxButton.OK);
            }
            else
            {
                Process.Start(jakySoubor);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int overeni = JeVybrano(jakySoubor);
            if (overeni == 0)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Musíte vybrat soubor", "Upozornění", MessageBoxButton.OK);
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = fbd.SelectedPath;
                    KopirovaniSlozek(ListCest[personsList.SelectedIndex], path);
                    /*if (!File.Exists(path))
                    {

                        File.Copy(@"A:\VS-projekty\Launcher1\PrvniWPF\bin\Debug\Output.txt", path+@"\Output.txt");
                    }*/
                }
            }
            
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int overeni = JeVybrano(jakySoubor);
            if (overeni == 0)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Musíte vybrat soubor", "Upozornění", MessageBoxButton.OK);
            }
            else
            {
                //KopirovaniSouboru( @"C:\Users\Ondra\AppData\Roaming\MAK\Output.txt", @"A:\VS-projekty\Launcher1\PrvniWPF\bin\Debug\Output.txt");
                //Console.WriteLine(ListCest[0]);
                Directory.Delete(ListCest[personsList.SelectedIndex], true);
                nacteniDat(newdef);
            }
            
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                newdef = fbd.SelectedPath;
                string path = fbd.SelectedPath;
                nacteniDat(path);
            }

        }
        public void vypsat_csproj()
        {
            string fullPathName = @"A:\VS-projekty\Launcher1\PrvniWPF\Launcher.csproj";
            XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
            XDocument projDefinition = XDocument.Load(fullPathName);
            IEnumerable<string> references = projDefinition
                .Element(msbuild + "Project")
                .Elements(msbuild + "ItemGroup")
                .Elements(msbuild + "Reference")
                .Select(refElem => refElem.Value);
            foreach (string reference in references)
            {
                Console.WriteLine("DALSI TEST: " + reference);
            }
        }
        public void KontrolaBinu(FileInfo item)
        {
            string myString = System.IO.Path.GetFullPath(item.FullName);
            string answer = myString.Replace("\\", "/");
            var splitted = answer.Split('/');
            var values = splitted.Skip(1).Take(splitted.Length - 2).ToArray();
            if (values[values.Length - 2] == "bin")
            {
                vypis.Add(new ExeSoubory(System.IO.Path.GetFullPath(item.FullName)));
                ListExe.Add(item.Directory.ToString());
            }
        }
        public void KopirovaniSlozek(string sourceFolder, string destinationFolder)
        {
            if (Directory.Exists(sourceFolder))
            {
                // Copy folder structure
                foreach (string sourceSubFolder in Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(sourceSubFolder.Replace(sourceFolder, destinationFolder));
                }
                // Copy files
                foreach (string sourceFile in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
                {
                    string destinationFile = sourceFile.Replace(sourceFolder, destinationFolder);
                    File.Copy(sourceFile, destinationFile, true);
                }
            }
        }
        public void KopirovaniSouboru(string pathCO,string pathKam)
        {
            if (!File.Exists(pathCO))
            {
                File.Copy(pathKam, pathCO);
            }
            else
            {
                File.Delete(pathCO);
                File.Copy(pathKam, pathCO);
            }
        }

        

        public int JeVybrano(string soubor)
        {
            if (string.IsNullOrEmpty(soubor))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public void nacteniDat(string cesticka)
        {
            ListCest.Clear();
            int ctr1 = 0;
            jakySoubor = "";
            vypis.Clear();
            txtCesta.Text = cesticka;
            var engine = new FileHelperEngine<CSVFile>();
            var orders = new List<CSVFile>();
            personsList.ItemsSource = vypis;
            string[] x = Directory.GetDirectories(cesticka);
            foreach (var test in x)
            {
                var dalsiFile = Directory.GetFiles(test.ToString());
                var fileList = new DirectoryInfo(test.ToString()).GetFiles("*.exe", SearchOption.AllDirectories);
                var slnList = new DirectoryInfo(test.ToString()).GetFiles("*.sln", SearchOption.AllDirectories);
                foreach (var item in fileList)
                {
                    KontrolaBinu(item);
                }
                foreach (var item in slnList)
                {
                    ListCest.Add(item.Directory.ToString());
                    
                    //string pathCO = @"A:\VS-projekty\Launcher1\Output.txt";
                    //string pathKam = @"A:\VS-projekty\Launcher1\PrvniWPF\bin\Debug\Output.txt";
                    //Console.WriteLine("CO: "+ListExe[ctr1]+@"\Output.txt"+" KAM: "+ListCest[ctr1] + @"\Output.txt");
                    if (!File.Exists(ListCest[ctr1]))
                    {
                        orders.Add(new CSVFile()
                        {
                            NazevProjektu = "NewProject",
                            Verze = "0.0",
                            Poznamka = ""
                        });
                        engine.WriteFile(ListCest[ctr1] + @"\Info.txt", orders);
                        
                    }
                    else
                    {
                    }
                    ctr1++;
                    

                }
            }
        }
    }
}
