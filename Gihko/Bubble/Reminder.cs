using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gihko.Bubble {
    [Serializable]
    class Reminder{
        public Reminder(){
            time = time.AddHours(-time.Hour);
            time = time.AddMinutes(-time.Minute);
        }
        public string description = "";
        public DateTime time = DateTime.Now;
        public bool enable = false;
    }
}
