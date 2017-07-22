using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace iChord
{
    class Csharp2Python
    {
        public string Python { set; get; }
        public string MyPythonApp { set; get; }

        /// <summary>
        /// Init python intepreter and python script 
        /// </summary>
        /// <param name="python"></param>
        /// <param name="myPythonApp"></param>
        public Csharp2Python(string python = @"D:\Programs\Python27\", string myPythonApp = @"D:\liuchang\Projects\Hackathon\Source\MelodyMaster\src\utils\RNN.py")
        {
            Python = python;
            MyPythonApp = myPythonApp;
        }

        public string run(string score)
        {
            //create new process start info
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(Python);

            //make sure we can read the output from stdout
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;

            //start python app with 3 arguments
            //1st argument is pointer to itself, 2nd and 3rd are actual arguments we want to send
            //sys.argv[0] in python is a pointer to itself.
            score = score.Replace(" ", "\" \"");
            myProcessStartInfo.Arguments = MyPythonApp + " " + score;

            //create a new process and start it
            Process myProcess = new Process();
            myProcess.StartInfo = myProcessStartInfo;

            myProcess.Start();

            StreamReader myStreamReader = myProcess.StandardOutput;
            string myString = myStreamReader.ReadToEnd();

            myProcess.WaitForExit();
            myProcess.Dispose();
            return myString;
        }
    }
}
