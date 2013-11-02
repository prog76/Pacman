using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace pacman
{

	/*
	 
    mediaElement = new MediaElement();
    mediaElement.Width = 400;
    mediaElement.Height = 300;
    mediaElement.Source = new Uri("test.wmv", UriKind.Relative);
    mediaElement.AutoPlay = true;

    LayoutRoot.Children.Add(mediaElement);
	
	 * * */

	public class TSoundPlayer
	{

		class playItem:IEquatable<playItem>
		{
			public Uri uri;
			public bool loop;
			public bool Equals(playItem item)
			{
				return item.uri.OriginalString.Equals(uri.OriginalString);
			}
			public playItem(Uri _uri, bool _loop)
			{
				uri = _uri;
				loop = _loop;
			}
		}

		MediaElement soundElem;
		Queue<playItem> uriList;

		void soundElem_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("error : {0}", e.ErrorException));
		}

		void soundElem_MediaOpened(object sender, RoutedEventArgs e)
		{
			stopped = false;
			soundElem.Play();
		}

		void soundElem_MediaEnded(object sender, RoutedEventArgs e)
		{
			if (fLoop) replay();
			else stopped = !doPlayNext();
		}

		bool doPlayNext()
		{
			if (uriList.Count > 0)
			{
				playItem item = uriList.Dequeue();
				fLoop = item.loop;
				soundElem.Source = item.uri;
				replay();
				return true;
			}
			else return false;
		}

		public TSoundPlayer(MediaElement element)
		{
			soundElem = element;
			uriList = new Queue<playItem>();
			soundElem.MediaOpened += new RoutedEventHandler(soundElem_MediaOpened);
			soundElem.MediaEnded += new RoutedEventHandler(soundElem_MediaEnded);
			soundElem.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(soundElem_MediaFailed);
		}

		bool stopped = true, fLoop=false;

		void replay()
		{
			stopped = false;
			soundElem.Position = TimeSpan.FromSeconds(0);
			soundElem.Play();
		}

		public void playUri(Uri AudioUri)
		{
			playUri(false, false, AudioUri);
		}

		public void playUri(bool stop, bool _loop, Uri AudioUri){
			if (stop) uriList.Clear();
			else
			{
				playItem item = new playItem(AudioUri, _loop);
				if (uriList.Contains(item)) return;
			}
			uriList.Enqueue(new playItem(AudioUri,_loop));
			if (stopped||stop||fLoop)doPlayNext(); 
		}

		public void playUri(bool stop, Uri AudioUri)
		{
			playUri(stop, false, AudioUri);
		}

		public void mute(bool mute)
		{
			soundElem.IsMuted = mute;
			if (mute)
			{
				soundElem.Pause();
			}
			else
			{
				soundElem.Play();
			}
		}
	}
}
