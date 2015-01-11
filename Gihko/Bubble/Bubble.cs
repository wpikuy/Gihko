using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Gihko.Bubble {
    class Bubble {

        public void init(Core.Core core){
            _core = core;
            _intervalTimer = new DispatcherTimer();
            _intervalTimer.Tick += (sender, args) => intervalTick();
            _intervalTimer.Interval = TimeSpan.FromMinutes(_intervalSecs);
            _intervalTimer.Start();
            _checkTimer = new DispatcherTimer();
            _checkTimer.Tick += (sender, args) => checkTimer();
            _checkTimer.Interval = TimeSpan.FromSeconds(5);
            _checkTimer.Start();
            _random = new Random();
            _isTrialTurn = true;
        }

        private void intervalTick(){

            if (_isTrialEnable && _isTCEnable){
                if (_isTrialTurn && _trials.Count > 0) {
                    _isTrialTurn = false;
                    int next = _random.Next(_trials.Count);
                    _core.showText(_trials[next]);
                }
                else if (!_isTrialTurn && _tcs.Count > 0) {
                    _isTrialTurn = true;
                    int next = _random.Next(_tcs.Count);
                    _core.showText(_tcs[next]);
                }
            }
            else if (_isTrialEnable) {
                if (_trials.Count > 0) {
                    int next = _random.Next(_trials.Count);
                    _core.showText(_trials[next]);
                }
            }
            else if (_isTCEnable) {
                if (_tcs.Count > 0) {
                    int next = _random.Next(_tcs.Count);
                    _core.showText(_tcs[next]);
                }
            }
        }

        private void checkTimer(){
            for (int i = 0; i < _reminders.Count; i++) {
                if (_reminders[i].enable && DateTime.Now - _reminders[i].time > TimeSpan.FromSeconds(0)) {
                    _reminders[i].enable = false;
                    _core.showText(_reminders[i].description);
                }
            }
        }

        // Field
        private Core.Core _core;
        private DispatcherTimer _intervalTimer;
        private DispatcherTimer _checkTimer;
        private Random _random;
        private bool _isTrialTurn;

        private bool _isTrialEnable { get { return (bool)_core.Props["Bubble.EnableTrial"]; } }
        private bool _isTCEnable { get { return (bool)_core.Props["Bubble.EnableTC"]; } }
        private double _intervalSecs { get { return (double)_core.Props["Bubble.IntervalMinutes"]; } }
        private List<string> _trials { get { return (List<string>)_core.Props["Bubble.Trial"]; } }
        private List<string> _tcs { get { return (List<string>)_core.Props["Bubble.TC"]; } }
        private List<Reminder> _reminders { get { return (List<Reminder>)_core.Props["Reminder.Reminder"]; } }
    }
}
