using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace application_watch_dog_timer
{
    enum TimeOutState
    {
        WakeUp,
        Sleeping
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _wdt = new Timer();
            
            _wdt.Interval = 10000; // Use a very short time-out for this demo
            _wdt.Tick += _wdt_Tick;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        private void _wdt_Tick(object sender, System.EventArgs e)
        { 
        }
        
        Timer _wdt; // Watch-Dog Timer
        TimeOutState TimeOutState
        {
            get => _timeOutState;
            set
            {
                if(value != _timeOutState)  // Check if property has changed
                {
                    _timeOutState = value;
                    // In a timer callback that changes the UI, it's
                    // best to post the action in the message queue.
                    BeginInvoke((MethodInvoker)delegate
                    {
                        textBox1.AppendText(_timeOutState.ToString() + Environment.NewLine);
                    });
                }
            }
        }

        TimeOutState _timeOutState = (TimeOutState)(-1);    // Initialize to invalid state

        // Just override this method. No need to subscribe with += to a MouseMove event.
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if(!_wdt.Enabled)
            {

            }
        }
    }
}
