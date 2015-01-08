using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Gihko.Bubble;

namespace Gihko.QuickFunc {

    class QuickFunc {

        public void init(Core.Core core){
            _core = core;
        }

        public void execute(int index){
            if (index > 6 || index < 1) return;
            if (_qfElems[index - 1].Cmd == "") return;
            runCmd(_qfElems[index - 1].Cmd);
        }

        private void runCmd(string cmd){
            Process p = new Process();
            p.StartInfo.FileName = "CMD.EXE";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine("start " + cmd);
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
        }

        // Field
        private List<QuickFuncElem> _qfElems{get { return _core.Props["QuickFunc.Funcs"] as List<QuickFuncElem>; }}

        private Core.Core _core;
    }
}
