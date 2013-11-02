using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.Automation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace active
{
    public partial class MainPage : UserControl
    {
        private String folderPath = "C:\\WayneTestSL4FSO";
        private String filePath = "C:\\WayneTestSL4Fso\\WayneTest.txt";

        public MainPage()
        {
            InitializeComponent();

            CheckOOB();
        }

        private void CheckOOB()
        {
            if (!Application.Current.IsRunningOutOfBrowser)
            {
                switch (Application.Current.InstallState)
                {
                    case InstallState.NotInstalled:
                        sp.Children.Clear();
                        Button installBtn = new Button()
                                                {
                                                    Content = "Install Me...",
                                                    Width = 100d,
                                                    Height = 23d,
                                                    Margin = new Thickness(0, 100, 0, 0)
                                                };

                        installBtn.Click += (o, e) => Application.Current.Install();

                        sp.Children.Add(installBtn);
                        break;
                    case InstallState.Installed:
                        sp.Children.Clear();
                        TextBlock tb = new TextBlock()
                                           {
                                               Text = "I've been installed already..."
                                           };

                        sp.Children.Add(tb);
                        break;
                    case InstallState.Installing:
                        MessageBox.Show("No hurry, I am installing:)...");
                        break;
                    case InstallState.InstallFailed:
                        MessageBox.Show("I just tried installed but failed, please check reason...");
                        break;
                    default:
                        break;
                }
            }
        }

        #region Button Click events
        private void btnCopyToIso_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Filter = "All files (*.*)|*.*", Multiselect = true };
            var dlgResult = dlg.ShowDialog();
            if (dlgResult != null && dlgResult.Value)
            {
                IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
                foreach (FileInfo file in dlg.Files)
                {
                    using (Stream fileStream = file.OpenRead())
                    {
                        using (IsolatedStorageFileStream isoStream =
                            new IsolatedStorageFileStream(file.Name, FileMode.Create, iso))
                        {
                            // Read and write the data block by block until finish
                            while (true)
                            {
                                byte[] buffer = new byte[100001];
                                int count = fileStream.Read(buffer, 0, buffer.Length);
                                if (count > 0)
                                {
                                    isoStream.Write(buffer, 0, count);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                ShowManipulationResult("Successfully copied the selected file(s) to Isolated file storage..");
            }
            else
            {
                MessageBox.Show("You canceled file selection.");
            }
        }

        private void btnReadIsolation_Click(object sender, RoutedEventArgs e)
        {
            var isoFiles = from files in IsolatedStorageFile.GetUserStoreForApplication().GetFileNames()
                           select files;

            lbIsoFiles.Visibility = Visibility.Visible;
            lbIsoFiles.ItemsSource = isoFiles;
        }

        private void btnWriteReg_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationFactory.IsAvailable)
            {
                using (dynamic wScript = AutomationFactory.CreateObject("WScript.Shell"))
                {
                    // Only has write permissin to HKCU
                        wScript.RegWrite(@"HKCU\Software\WayneTestRegValue",
                            "SomeStrValue", "REG_SZ");

                    ShowManipulationResult("Successfully wrote value to registry HKCU!");
                }
            }
        }
        private void btnReadReg_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationFactory.IsAvailable)
            {
                using (dynamic wScript = AutomationFactory.CreateObject("WScript.Shell"))
                {
                    string dotNetRoot =
                        wScript.RegRead(@"HKLM\SOFTWARE\Microsoft\.NETFramework\InstallRoot");

                    ShowManipulationResult("Successfully read value from HKLM! .NET InstallRoot is: " + dotNetRoot);
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Code below will throw System.UnauthorizedAccessException
            //using (StreamWriter writer = File.CreateText("C:\\Temp.txt"))
            //{
            //    writer.WriteLine("Some text");
            //}

            if (AutomationFactory.IsAvailable)
            {
                // Interesting, I can simply try to dispose a dynamic object without checking whether it has implemented IDisposible:)
                using (dynamic fso = AutomationFactory.CreateObject("Scripting.FileSystemObject"))
                {
                    if (!fso.FolderExists(folderPath)) fso.CreateFolder(folderPath);
                    dynamic txtFile = fso.CreateTextFile(filePath);
                    txtFile.WriteLine("Some text...");
                    txtFile.close();
                }

                ShowManipulationResult(filePath + " was created..");
            }
        }

        private void btnReadTxt_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationFactory.IsAvailable)
            {
                var fileContent = String.Empty;

                using (dynamic fso = AutomationFactory.CreateObject("Scripting.FileSystemObject"))
                {
                    dynamic file = fso.OpenTextFile(filePath);
                    fileContent = file.ReadAll();

                    file.Close();
                }

                ShowManipulationResult("The content of " + filePath + " is: " + Environment.NewLine + fileContent);
            }
        }

        private void btnRunExe_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationFactory.IsAvailable)
            {
                using (dynamic wScript = AutomationFactory.CreateObject("WScript.Shell"))
                {
                    //Refer WScript.Run at: http://msdn.microsoft.com/en-us/library/d5fk67ky(v=VS.85).aspx
                    //wScript.Run("iexplore http://wayneye.com", 1, true);
                    wScript.Run("C:\\Users\\yewei\\desktop\\Demo.js", 1, true);
                }

                ShowManipulationResult("Successfully launched IE and opened http://wayneye.com");
            }
        }

        private void btnPhonate_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationFactory.IsAvailable)
            {
                using (dynamic speechApi = AutomationFactory.CreateObject("Sapi.SpVoice"))
                {
                    speechApi.Speak(this.txtPhonateSource.Text);
                }

                ShowManipulationResult("I am speaking: " + this.txtPhonateSource.Text);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationFactory.IsAvailable)
            {
                using (var wScript = AutomationFactory.CreateObject("WScript.Shell"))
                {
                    wScript.Run(@"cmd /k taskkill /IM sllauncher.exe & exit", 0);
                }
            }
        }

        #endregion

        private void ShowManipulationResult(String msg)
        {
            this.rtb.Selection.Text += msg + Environment.NewLine;
        }
    }
}
