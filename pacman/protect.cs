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
using System.Runtime.InteropServices.Automation;
using System.Windows.Browser;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel;


namespace pacman
{
	public class Protect : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}

		#endregion
		protected bool fRegistered;
		protected string baseUrl, keyPath, subPass;
		static Protect protect;
		static public Protect getInstance()
		{
			if (protect == null) protect = new Protect();
			return protect;
		}

		virtual protected void init()
		{
			baseUrl = @"file\:\/\/\/T\:;http\:\/\/pacman.comyr.com;http://www.silverarcade.com/";
			subPass = "PaCmAn";
			Assembly asm = Assembly.GetExecutingAssembly();
			string company = "NONE", product = "NONE";

			foreach (Attribute attr in Attribute.GetCustomAttributes(asm))
			{
				if (attr.GetType() == typeof(AssemblyCompanyAttribute)) company = ((AssemblyCompanyAttribute)attr).Company;
				if (attr.GetType() == typeof(AssemblyProductAttribute)) product = ((AssemblyProductAttribute)attr).Product;
			}
			keyPath = @"HKLM\SOFTWARE\" + company + "\\" + product + "\\";
		}

		protected bool checkUrl(string url)
		{
			foreach (string pattern in baseUrl.Split(';'))
			{
				if (Regex.IsMatch(url, pattern)) return true;
			}
			return false;
		}

		public bool registered
		{
			get
			{
				return fRegistered;
			}
			set
			{
				if (fRegistered != value)
				{
					fRegistered = value;
					NotifyPropertyChanged("registered");
					if(Tick!=null)
					foreach (EventHandler nextDel in Tick.GetInvocationList())
						try
						{
							nextDel.Invoke(this,null);
						}
						catch (Exception xx) { ;}
				}
			}
		}

		public bool canRegister
		{
			get;
			set;
		}

		protected class RegKeys
		{
			string keyPath, subPass;
			static RegKeys regKeys;
			/*		static public RegKeys getInstance()
					{
						if (regKeys == null) regKeys = new RegKeys();
						return regKeys;
					}*/
			protected dynamic shell;
			static string GetSHA1Hash(string val)
			{
				System.Text.Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
				byte[] data = System.Text.Encoding.UTF8.GetBytes(val);
				System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1Managed();
				byte[] res = sha.ComputeHash(data);
				return System.BitConverter.ToString(res).Replace("-", "").ToUpper();
			}

			public RegKeys(string _keyPath, string _subPass)
			{
				keyPath = _keyPath;
				subPass = _subPass;
				shell = AutomationFactory.CreateObject("WScript.Shell");
			}

			public string getPass(string code)
			{
				return GetSHA1Hash(code);
			}
			public string getCode()
			{
				return GetSHA1Hash(readCode());
			}
			private string readCode()
			{
				try
				{
					return shell.RegRead(keyPath + "code");
				}
				catch (Exception e)
				{
					shell.RegWrite(keyPath + "code", Guid.NewGuid().ToString(), "REG_SZ");
					return shell.RegRead(keyPath + "code");
				}
			}
			public string readKey()
			{
				try
				{
					return shell.RegRead(keyPath + "key");
				}
				catch (Exception e)
				{
					return "";
				}
			}
			public bool checkKey()
			{
				return readKey() == getPass(getCode());
			}
		}

		public string getCode()
		{
			return getRegKey().getCode();
		}
		protected RegKeys getRegKey()
		{
			return new RegKeys(keyPath, subPass);
		}

		public void checkKey()
		{

		}

		public void addKey(string key)
		{
			using (dynamic shell = AutomationFactory.CreateObject("WScript.Shell"))
			{
				shell.RegWrite(keyPath + "key", key, "REG_SZ");
			}
			registered = getRegKey().checkKey();
		}

		public Protect()
		{
			init();
			if (Application.Current.IsRunningOutOfBrowser)
			{
				if ((Application.Current.HasElevatedPermissions) && (AutomationFactory.IsAvailable))
				{
					canRegister = true;
					fRegistered = getRegKey().checkKey();
				}
				else
				{
					MessageBox.Show("Unable to access registry or COM services. If you want to register, please, reinstall.", "Error", MessageBoxButton.OK);
					registered = false;
					canRegister = false;
				}
			}
			else
			{
				registered = checkUrl(HtmlPage.Document.DocumentUri.ToString());
				if (!registered)
				{
					MessageBox.Show("Sorry. This application should not be used on that site", "Error", MessageBoxButton.OK);
					Application.Current.MainWindow.Close();
				}
			}
		}

		public EventHandler Tick { get; set; }
	}
}
