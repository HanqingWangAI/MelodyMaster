using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Scripting.Hosting;

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

        public string connectPython(string score)
        {
            ScriptRuntime scriptRuntime = ScriptRuntime.CreateFromConfiguration();
            ScriptEngine rbEng = scriptRuntime.GetEngine("python");
            ScriptSource source = rbEng.CreateScriptSourceFromFile(@"D:\liuchang\Projects\Hackathon\Source\MelodyMaster\src\utils\RNN.py");
            ScriptScope scope = rbEng.CreateScope();

            try
            {
                //设置参数 
                scope.SetVariable("arg1", score);
            }
            catch (Exception)
            {
                Console.WriteLine("Input Error to Python");
            }

            source.Execute(scope);
            return scope.GetVariable("score").ToString();
        }
    }
}
