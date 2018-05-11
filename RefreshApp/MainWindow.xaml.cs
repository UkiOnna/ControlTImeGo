using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Uri imageUri;
        Application cur;
        private object lockObject = new object();
        public string updateIconPath { get; set; }
        public string updateImagePath { get; set; }
        private bool update;
        private bool updateIc;
        private bool updateIm;
        public MainWindow()
        {
            InitializeComponent();
            update = false;
            updateIc = false;
            updateIm = false;

            Directory.CreateDirectory("D:/programRestart");
            Directory.CreateDirectory("D:/programUpdate");
            try
            {

                var icoInf = new FileInfo("D:/programRestart/mainIcon.png");
                var backInf = new FileInfo("D:/programRestart/mainImage.png");

                if (icoInf.Exists && backInf.Exists)
                {
                    icoInf.Delete();
                    backInf.Delete();
                    // альтернатива с помощью класса File
                    // File.Delete(path);
                }



                File.Copy(Directory.GetCurrentDirectory() + @"\Backrounds\Background.png", "D:/programRestart/mainImage.png");
                File.Copy(Directory.GetCurrentDirectory() + @"\Icons\Icon.png", "D:/programRestart/mainIcon.png");

            }
            catch (Exception a)
            {
                //MessageBox.Show(a.Message);
            }

            iconUri = new Uri("D:/programRestart/mainIcon.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            imageUri = new Uri("D:/programRestart/mainImage.png", UriKind.RelativeOrAbsolute);
            mainImage.ImageSource = BitmapFrame.Create(imageUri);


            try
            {
                var icoInf = new FileInfo("D:/programRestart/currentIcon.png");
                var backInf = new FileInfo("D:/programRestart/currentImage.png");

                if (icoInf.Exists && backInf.Exists)
                {
                    icoInf.Delete();
                    backInf.Delete();
                    // альтернатива с помощью класса File
                    // File.Delete(path);
                }



                File.Copy("D:/programRestart/mainIcon.png", "D:/programRestart/currentIcon.png");
                File.Copy("D:/programRestart/mainImage.png", "D:/programRestart/currentImage.png");

            }
            catch
            {

            }



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
                    string[] icons = Directory.GetFiles("D:/programUpdate", "icon*");//хранятся пути НАЗВАНИЯ НАЧИНАЮТСЯ НА icon И image
                    string[] backgronds = Directory.GetFiles("D:/programUpdate", "image*");
                    string updateIcon = "";
                    string updateImage = "";


                    if (icons.Length > 0 || backgronds.Length > 0)
                    {
                        foreach (string o in icons)
                        {
                            if (!FileCompare(o, "D:/programRestart/currentIcon.png"))
                            {
                                updateIcon = o;
                                break;
                            }
                        }

                        foreach (string o in backgronds)
                        {
                            if (!FileCompare(o, "D:/programRestart/currentImage.png"))
                            {
                                updateImage = o;
                                break;
                            }
                        }

                        if (updateImage.Length > 0)
                        {

                            updateImagePath = updateImage;
                            update = true;
                            updateIm = true;
                        }
                        if (updateIcon.Length > 0)
                        {
                            updateIconPath = updateIcon;
                            update = true;
                            updateIc = true;
                        }
                        if (update)
                        {
                            MessageBoxResult res = MessageBox.Show("Обнаружены обновления хотите загрузить?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (MessageBoxResult.No == res)
                            {
                                //do no stuff
                            }
                            else
                            {

                                Restart(mes);
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

        public void Restart(Application mes)
        {
            Thread thread = new Thread(FileChange);
            //FileChange();

            timer.Dispose();

            thread.Start();

            mes.Shutdown();

        }

        public void FileChange()
        {
            lock (lockObject)
            {
                string iconPath = Directory.GetCurrentDirectory() + @"\Icons\icon.png";
                string backPath = Directory.GetCurrentDirectory() + @"\Backrounds\Background.png";

                FileInfo icoInf = new FileInfo(iconPath);
                FileInfo backInf = new FileInfo(backPath);
                if (icoInf.Exists && backInf.Exists)
                {
                    if (updateIc)
                    {
                        icoInf.Delete();
                    }
                    if (updateIm)
                    {
                        backInf.Delete();
                    }
                    // альтернатива с помощью класса File
                    // File.Delete(path);
                }





;
                if (updateIc)
                {
                    FileInfo icoFileInf = new FileInfo(updateIconPath);
                    if (icoFileInf.Exists)
                    {
                        icoFileInf.MoveTo(iconPath);
                    }
                }
                if (updateIm)
                {
                    FileInfo backFileInf = new FileInfo(updateImagePath);
                    if (backFileInf.Exists)
                    {



                        backFileInf.MoveTo(backPath);

                        // альтернатива с помощью класса File
                        // File.Move(path, newPath);
                    }
                }

               


                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                // System.Diagnostics.Process.Start("RefreshApp"); 
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


