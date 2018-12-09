using LightBuzz.Vitruvius;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Fall_detection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _sensor = null;
        private DepthFrameReader _depthReader = null;
        private BodyFrameReader _bodyReader = null;
        ColorFrameReader cfr;
        WriteableBitmap wbmp = new WriteableBitmap(512, 424, 96.0, 96.0, PixelFormats.Bgr32, null);
        BitmapSource bmpSource;
        private Body _body = null;
        private Floor _floor = null;
        byte[] colorData = new byte[512 * 424];
        ColorImageFormat format;
        double test;
        //設定smtp主機
        string smtpAddress = "smtp.gmail.com";
        //設定Port
        int portNumber = 587;
        bool enableSSL = true;
        //填入寄送方email和密碼
        string emailFrom = "user";
        string password = "passsword";
        //收信方email
        string emailTo = "user";
        //主旨
        string subject = "跌倒告知";
        //內容
        string body = "有人跌倒!";
        public MainWindow()
        {
            InitializeComponent();

            _sensor = KinectSensor.GetDefault();
            var fd = _sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            uint frameSize = fd.BytesPerPixel * fd.LengthInPixels;
            colorData = new byte[frameSize];
            format = ColorImageFormat.Bgra;
            if (_sensor != null)
            {
                _sensor.Open();

                _depthReader = _sensor.DepthFrameSource.OpenReader();
                _depthReader.FrameArrived += DepthReader_FrameArrived;
                _bodyReader = _sensor.BodyFrameSource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;
                cfr = _sensor.ColorFrameSource.OpenReader();
                cfr.FrameArrived += cfr_FrameArrived;
            }
        }
        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _floor = frame.Floor();
                    _body = frame.Body();

                    if (_floor != null && _body != null)
                    {
                        CameraSpacePoint wrist3D = _body.Joints[JointType.Head].Position;
                        Point wrist2D = wrist3D.ToPoint();

                        double distance = _floor.DistanceFrom(wrist3D);
                        int floorY = _floor.FloorY((int)wrist2D.X, (ushort)(wrist3D.Z * 1000));
                        test = Convert.ToDouble(distance.ToString("N2"));
                        TblDistance.Text = Convert.ToString(test);
                        /*string x = _floor.X.ToString();
                        string y = _floor.Y.ToString();
                        string z = _floor.Z.ToString();
                        string w = _floor.W.ToString();
                        xtext.Text = x;
                        ytext.Text = y;
                        ztext.Text = z;
                        wtext.Text = w;*/
                        //判斷
                        string x = "安全";
                        xtext.Text = x;
                        if (test<0.50)
                        {
                            xtext.Text = "不安全";
                            if(test<0.45)
                            {
                                if (this.wbmp != null)
                                {
                                    // create a png bitmap encoder which knows how to save a .png file
                                    BitmapEncoder encoder = new PngBitmapEncoder();

                                    // create frame from the writable bitmap and add to encoder
                                    encoder.Frames.Add(BitmapFrame.Create(this.wbmp));

                                    string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                                    string path = Path.Combine(myPhotos, "KinectScreenshot-Color-" + ".png");


                                    // FileStream is IDisposable
                                    using (FileStream fs = new FileStream(path, FileMode.Create))
                                    {
                                        encoder.Save(fs);
                                    }
                                }
                                mes();
                            }
                        }
                        Canvas.SetLeft(ImgHand, wrist2D.X - ImgHand.Width / 2.0);
                        Canvas.SetTop(ImgHand, wrist2D.Y - ImgHand.Height / 2.0);
                        Canvas.SetLeft(ImgFloor, wrist2D.X - ImgFloor.Width / 2.0);
                        Canvas.SetTop(ImgFloor, floorY - ImgFloor.Height / 2.0);
                    }
                }
            }
        }
        void mes()
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                // 若你的內容是HTML格式，則為True
                mail.IsBodyHtml = false;
                string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                string path = Path.Combine(myPhotos, "KinectScreenshot-Color-" + ".png");
                mail.Attachments.Add(new Attachment(path));
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }
        private void DepthReader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Camera.Source = frame.Bitmap();
                }
            }
        }

        private void cfr_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            if (e.FrameReference == null) return;

            using (ColorFrame cf = e.FrameReference.AcquireFrame())
            {
                if (cf == null) return;
                cf.CopyConvertedFrameDataToArray(colorData, format);
                var fd = cf.FrameDescription;

                // Creating BitmapSource
                var bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel) / 8;
                var stride = bytesPerPixel * cf.FrameDescription.Width;

                bmpSource = BitmapSource.Create(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgr32, null, colorData, stride);

                // WritableBitmap to show on UI
                wbmp = new WriteableBitmap(bmpSource);
                //image.Source = wbmp;
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_depthReader != null)
            {
                _depthReader.Dispose();
            }

            if (_bodyReader != null)
            {
                _bodyReader.Dispose();
            }

            if (_sensor != null && _sensor.IsOpen)
            {
                _sensor.Close();
            }

        }

    }

}
