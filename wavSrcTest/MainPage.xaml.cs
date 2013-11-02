using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Resources;
using WaveMSS;

namespace wavSrcTest
{
	public partial class MainPage : UserControl

	{

		WaveMediaStreamSource source;
		 public MainPage()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

		 void MainPage_Loaded(object sender, RoutedEventArgs e)
		 {
			 Uri uri = new Uri("files/111.mp3", UriKind.RelativeOrAbsolute);
			 StreamResourceInfo sr = Application.GetResourceStream(uri);
			 source = new WaveMediaStreamSource(sr.Stream);
			 TestMediaElement.SetSource(source);
		 }

		 private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		 {
			 
		 }

		 private void TestMediaElement_MediaEnded(object sender, RoutedEventArgs e)
		 {
			 TestMediaElement.Position = TimeSpan.FromSeconds(0);
			 TestMediaElement.Play();
/*			 Uri uri = new Uri("files/111.mp3", UriKind.RelativeOrAbsolute);
			 StreamResourceInfo sr = Application.GetResourceStream(uri);
			 source = new WaveMediaStreamSource(sr.Stream, slider.Value);*/
//			 TestMediaElement.SetSource(source);
		 }
	}
}
