using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.MyGameConsole
{
    /// <summary>
	/// A la responsabilité de contenir le text de l'évenement et de le rendre accessible

	/// </summary>
	public class GenerateTextEventArgs : EventArgs
	{
		private string myEventText = null;

        public GenerateTextEventArgs(string theEventText)
		{			
			if (theEventText == null) throw new NullReferenceException();
			myEventText = theEventText; 
		}
									
		public string EventText
		{
			get { return this.myEventText; }
		}	
	}
}
