using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace application_watch_dog_timer
{
    enum TimeOutState{ WakeUp = 3, Sleeping = 2, Warning = 1, Exit = 0 }
    public partial class Form1 : Form, IMessageFilter
    {
        public Form1()
        {
            InitializeComponent();
            _wdt = new Timer();            
            _wdt.Interval = 5000; // Use a very short time-out for this demo
            _wdt.Tick += _wdt_Tick;
        }

        Timer _wdt; // Watch-Dog Timer

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_MOUSEMOVE:
                    TimeOutState = TimeOutState.WakeUp;
                    break;
            }
            return false; // Do not suppress downstream message
        }
        const int WM_MOUSEMOVE = 0x0200; // WinOS Message

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Application.AddMessageFilter(this);
            // Wait for Form1 window handle to exist before starting WDT
            TimeOutState = TimeOutState.WakeUp;
        }

        private void _wdt_Tick(object sender, System.EventArgs e)
        {
            // A tick reduces the TimeOutState by 1
            TimeOutState = (TimeOutState)(TimeOutState - 1);
        }

        // Time out state machine
        TimeOutState TimeOutState
        {
            get => _timeOutState;
            set
            {
                switch (TimeOutState)
                {
                    case TimeOutState.WakeUp:
                        _wdt.Stop();
                        _wdt.Start();
                        break;
                    case TimeOutState.Exit:
                        _wdt.Stop();
                        Application.Exit();
                        return;
                }
                if (value != _timeOutState)  // If state changes, write message
                {
                    // In a timer callback that changes the UI, it's
                    // best to post the action in the message queue.
                    BeginInvoke((MethodInvoker)delegate
                    {
                        textBox1.AppendText(_timeOutState.ToString());
                        if (TimeOutState == TimeOutState.Warning)
                        {
                            textBox1.AppendText(
                                ": Closing in " + (_wdt.Interval / 1000).ToString() + " seconds.");
                        }
                        textBox1.AppendText(Environment.NewLine);
                        textBox1.Select(textBox1.TextLength, 0);
                    });
                }
                _timeOutState = value;
            }
        }
        TimeOutState _timeOutState = (TimeOutState)(-1);    // Initialize to invalid state
    }
}
