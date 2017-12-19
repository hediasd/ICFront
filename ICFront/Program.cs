using System;
using Gtk;
using System.Diagnostics;
using System.Timers;

namespace ICFront
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();


			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();

			//Debug.Log ("this?");
			//Console.WriteLine ("WORK");
		}


	}
}
