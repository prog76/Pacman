using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Runtime.Serialization;

namespace GeniusPacman.Core
{

	[DataContract]
	public class TTskSet
	{
		[DataMember]
		public int minLen,maxLen,freq;
		[DataMember]
		public Color color;
		[DataMember]
		public bool isAlone;
		[DataMember]
		public string keysLeft, keysRight;
		[DataMember]
		public string name { get; set; }
		public TTskSet(){
		}

		
		public TTskSet(string _name, string _keysLeft, string _keysRight, int _freq, bool _isAlone, Color _color, int _minLen, int _maxLen)
		{
			keysLeft = _keysLeft;
			keysRight = _keysRight;
			name = _name;
			isAlone = _isAlone;
			minLen = _minLen;
			maxLen = _maxLen;
			color = _color;
			freq = _freq;
		}
	}
}