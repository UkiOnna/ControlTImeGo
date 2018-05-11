using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RefreshApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer timer;
        private Uri iconUri;
        Application cur;
        private object lockObject=new object();
        public MainWindow()
        {
            InitializeComponent();
            iconUri = new Uri(@"C:\Users\СикировТ\Documents\Visual Studio 2017\Projects\RefreshApp\RefreshApp\Icons\icon.jpg", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            cur = Application.Current;
            timer = new Timer(CheckFiles, cur, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15)); 
        }



        public void CheckFiles(object obj)
        {

            Application mes = (Application)obj;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {

                 //   try
                   // {
                        // Only get files that begin with the letter "c."
                        string[] icons = Directory.GetFiles(@"C:\Users\СикировТ\Documents\files", "icon*");//хранятся пути 
                        string[] backgronds = Directory.GetFiles(@"C:\Users\СикировТ\Documents\files", "Background*");
                    
                        if (icons.Length > 0 && backgronds.Length > 0)
                        {
                        if (FileCompare(icons[0], @"C:\Users\СикировТ\Documents\Visual Studio 2017\Projects\RefreshApp\RefreshApp\Icons\icon.jpg") && FileCompare(backgronds[0], @"C:\Users\СикировТ\Documents\Visual Studio 2017\Projects\RefreshApp\RefreshApp\Backrounds\Backround.jpg"))
                        {


                            MessageBoxResult res = MessageBox.Show("Обнаружены обновления хотите загрузить?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (MessageBoxResult.No == res)
                            {
                                //do no stuff
                            }
                            else
                            {

                                FileChange(icons[0], backgronds[0]);
                                timer.Dispose();
                                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                                mes.Shutdown();


                                //do yes stuff
                            }
                        }
                        }

                  //  }
                  //  catch (Exception e)
                    //{
                     //   MessageBox.Show("The process failed: {0}", e.ToString());
                    //}
                }
                );
        }

        public void FileChange(string iconPathh,string backPathh)
        {
            string iconPath = @"C:\Users\СикировТ\Documents\Visual Studio 2017\Projects\RefreshApp\RefreshApp\Icons\icon.jpg";
            string backPath = @"C:\Users\СикировТ\Documents\Visual Studio 2017\Projects\RefreshApp\RefreshApp\Backrounds\Backround.jpg";
            FileInfo icoInf = new FileInfo(iconPath);
            FileInfo backInf = new FileInfo(backPath);
            if (icoInf.Exists && backInf.Exists)
            {
                icoInf.Delete();
                backInf.Delete();
                // альтернатива с помощью класса File
                // File.Delete(path);
            }

            string newIconPath = @"C:\Users\СикировТ\Documents\Visual Studio 2017\Projects\RefreshApp\RefreshApp\Icons\icon.jpg";

            string newBackPath = @"C:\Users\СикировТ\Documents\Visual Studio 2017\Projects\RefreshApp\RefreshApp\Backrounds\Backround.jpg";

            FileInfo icoFileInf = new FileInfo(iconPath);
            FileInfo backFileInf = new FileInfo(backPath);
            if (icoFileInf.Exists && backFileInf.Exists) ;
            {
                icoFileInf.MoveTo(newIconPath);
                backFileInf.MoveTo(newBackPath);
                // альтернатива с помощью класса File
                // File.Move(path, newPath);
            }

        }

        private bool FileCompare(string file1, string file2)
        {
            lock (lockObject)
            {
                int file1byte;
                int file2byte;
                FileStream fs1;
                FileStream fs2;

                if (file1 == file2)
                {
                    return true;
                }

                fs1 = new FileStream(file1, FileMode.Open);
                fs2 = new FileStream(file2, FileMode.Open);

                if (fs1.Length != fs2.Length)
                {
                    fs1.Close();
                    fs2.Close();

                    return false;
                }

                do
                {
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));

                fs1.Close();
                fs2.Close();

                return ((file1byte - file2byte) == 0);
            }
        }


    }
}


