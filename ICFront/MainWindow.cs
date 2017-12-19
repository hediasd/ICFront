using System;
using Gtk;
using System.Diagnostics;
using System.Management;
using System.Collections.Generic;
using System.Timers;
using System.IO;


public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		InitTimer ();
		//fileChooser1. = "/home/grv/Cerebros/ANATeFLAIR_V1/ANATeFLAIR001/ESCM001.FLAIR.1x1x1.nii.gz";
		//fileChooser2. = "/home/grv/Cerebros/ANATeFLAIR_V1/ANATeFLAIR001/ESCM001.ANAT.1x1x1.nii.gz";
	}
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}



	public Timer timer_ProcessFinding; 
	int a = 0;
	static string activeANTsProcess = null;



	public void InitTimer()
	{
		timer_ProcessFinding = new Timer();
		timer_ProcessFinding.Elapsed += timer_ProcessFinding_Tick;
		timer_ProcessFinding.Interval = 1000; // in miliseconds
		timer_ProcessFinding.Start();
	}

	private void timer_ProcessFinding_Tick(object sender, EventArgs e)
	{
		// check if registration is running, disable button
		
		if (activeANTsProcess == null) {
			foreach (var process in Process.GetProcessesByName("antsRegistration"))
			{
				activeANTsProcess = "antsRegistration";
				Debug.WriteLine ("Got new process ");
				break;
			}
		}

		if (activeANTsProcess != null) {
			
		}


	}

	protected void OnRegistrationButtonReleased (object sender, EventArgs e)
	{
		string f1 = fixedImage_fileChooser.Filename;
		string f2 = movingImage_fileChooser.Filename;		//registrationButton.Label = "antsRegistrationSyNQuick.sh -d 3 -f \"+self.TextEntry1.get()+\" -m \"+self.TextEntry2.get()+\" -o output\", stdout=subprocess.PIPE)";
		string outputPref = registration_outputPrefix_entry.Text;

		string numberOfThreads = " -n " + numberOfThreads_comboBox.ActiveText;
		//string initialTransform;
		//string collapseOutputTransforms;
		string histogramBins = " -r " + histogramBins_entry.Text;
		string splineDistance = " -s " + splineDistance_entry.Text;
		string fixedImageMask = (fixedImageMask_fileChooser.Filename == null ? "" : " -x " + fixedImageMask_fileChooser.Filename);
		string precisionType = " -p " + (precisionType_comboBox.ActiveText == "Double" ? "d" : "f");
		string histogramMatching = " -j " + (histogramMatching_comboBox.ActiveText == "True" ? "1" : "0");

		string transformType = " -t ";
		switch (transformType_comboBox.ActiveText) {
			case "translation":
			transformType += "t";
			break;
			case "rigid":
			transformType += "r";
			break;
			case "rigid + affine":
			transformType += "a";
			break;
			case "rigid + affine + deformable syn":
			transformType += "s";
			break;
			case "rigid + deformable syn":
			transformType += "sr";
			break;
			case "deformable syn only":
			transformType += "so";
			break;
			case "rigid + affine + deformable b-spline syn":
			transformType += "b";
			break;
			case "rigid + deformable b-spline syn":
			transformType += "br";
			break;
			case "deformable b-spline syn only":
			transformType += "bo";
			break;
		}

		string compulsoryCommand = "gnome-terminal -x bash -ic 'cd $HOME; antsRegistrationSyNQuick.sh -d 3 -f " + f1 + " -m " + f1 + " -o " + outputPref;
		string optionalCommand = numberOfThreads + transformType + histogramBins + splineDistance + fixedImageMask + precisionType + histogramMatching;
		string finalCommand = compulsoryCommand + optionalCommand + "; bash'"; 



		if (a == 0) {
			string res = ExecuteANTsCommand (finalCommand);
			Debug.WriteLine ("Executed ");
			a = 1;
		} else {
			bool b = KillANTsCommand ("antsRegistration");
			a = 0;
		}
	}

	protected void OnAntroposButtonReleased (object sender, EventArgs e)
	{
		string inputImage = " -a " + inputImage_fileChooser.Filename;
		string maskImage = " -x " + maskImage_fileChooser.Filename;
		string segmentationClasses = " -c " + segmentationClasses_Entry.Text;//registrationButton.Label = "antsRegistrationSyNQuick.sh -d 3 -f \"+self.TextEntry1.get()+\" -m \"+self.TextEntry2.get()+\" -o output\", stdout=subprocess.PIPE)";
		string outputPrefix = " -o " + atropos_outputPrefix_entry.Text;

		string maxN4AtroposIterations = (maxN4AtroposIterations_entry.Text == "" ? "" : " -m " + maxN4AtroposIterations_entry.Text);
		string maxAtroposIterations = (maxAtroposIterations_entry.Text == "" ? "" : " -n " + maxAtroposIterations_entry.Text);
		string imageFileSuffix = (imageFileSuffix_entry.Text == "" ? "" : " -s " + imageFileSuffix_entry.Text);

		string denoiseAnatomicalImages = " -g " + (denoiseAnatomicalImages_comboBox.ActiveText == "True" ? "1" : "0");
		string keepTempFiles = " -k " + (keepTempFiles_comboBox.ActiveText == "True" ? "1" : "0");
		string priorSegmentationWeight = " -w " + (priorSegmentationWeight_comboBox.ActiveText == "True" ? "1" : "0");

		string compulsoryCommand = "gnome-terminal -x bash -ic 'cd $HOME; antsAtroposN4.sh -d 3" + inputImage + maskImage + segmentationClasses + outputPrefix;
		string optionalCommand = maxN4AtroposIterations + maxAtroposIterations + imageFileSuffix + denoiseAnatomicalImages + keepTempFiles + priorSegmentationWeight;
		string finalCommand = compulsoryCommand + optionalCommand + "; bash'"; 



		if (a == 0) {
			string res = ExecuteANTsCommand (finalCommand);
			Debug.WriteLine ("Executed ");
			a = 1;
		} else {
			bool b = KillANTsCommand ("antsAtroposN4");
			a = 0;
		}
	}

	public bool KillANTsCommand(string st){

		Process[] AliveProcessList = Process.GetProcessesByName (st);
		try {
			
			AliveProcessList[0].CloseMainWindow();
			AliveProcessList[0].WaitForExit();
			activeANTsProcess = null;
			a = 0;
			Debug.WriteLine ("Process ended");
			return true;

		} catch(Exception) {
			
			Debug.WriteLine ("Couldnt end process ");
			return false;

		}

	}

	public string ExecuteANTsCommand(string command)
	{
		//if (activeANTsProcess == null) {

		Debug.WriteLine (command);

			Process newProcess = new Process ();
			newProcess.StartInfo.FileName = "/bin/bash";
			newProcess.StartInfo.Arguments = "-c \" " + command + " \"";
			newProcess.StartInfo.UseShellExecute = false; 

			newProcess.Start ();
		//	activeANTsProcess = newProcess;

		//}
			
		return " ";
	}

	protected void OnAntroposButtonReleased (object o, ButtonReleaseEventArgs args)
	{
		throw new NotImplementedException ();
	}
}
