namespace BusyBotForm
{
    using System;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        /// <summary>
        /// Button that is used to turn the Bot on and off
        /// </summary>
        private Button StateButton { get; set; }
        
        /// <summary>
        /// Counter of lines written for each pass
        /// </summary>
        private int LineCounter { get; set; }
        
        /// <summary>
        /// Counter of how many passes the Bot has made
        /// </summary>
        private int ExecutionCounter { get; set; }
       
        /// <summary>
        /// Target window for the Busy Bot
        /// </summary>
        private IntPtr TargetWindow => Program.FindWindow(Config.Instance.FindClass, null);


        /// <summary>
        /// Window is loaded and ready
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            LineCounter = 0;
            ExecutionCounter = 1;

            if (!ValidateConfiguration())
            {
                Close();
            }
            else
            {
                // Add the button
                AddButton();
                var sleepInterval = TimeSpan.Parse(Config.Instance.Interval);
                timer1.Interval = (int)sleepInterval.TotalMilliseconds;
                StateChanged(BotState.Stopped);
                base.OnLoad(e);
            }
        }

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns></returns>
        private bool ValidateConfiguration()
        {
            // Validate we have a running process for the Bot 
            if (TargetWindow == IntPtr.Zero)
            {
                MessageBox.Show($"Unable to find any window using the classname {Config.Instance.FindClass}. Double check the spelling and modify appsettings.json as required.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                var sleepInterval = TimeSpan.Parse(Config.Instance.Interval);
            }
            catch(Exception)
            {
                MessageBox.Show($"Unable to determine timing from {Config.Instance.Interval}. Double check the format and make sure to use \"HH:MM:SS\" and modify appsettings.json as required.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Start and Stop the Bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateButton_Click(object sender, EventArgs e)
        {
            if(timer1.Enabled)
            {
                var answer = MessageBox.Show("Do you wish to halt?", "User Request Stop", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (answer == DialogResult.Yes)
                {
                    StateChanged(BotState.Stopped);
                }
            }
            else
            {
                var answer = MessageBox.Show("Do you wish to start?", "User Request Start", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (answer == DialogResult.Yes)
                {
                    StateChanged(BotState.Running);
                }
            }
        }

        private void StateChanged(BotState botState)
        {
            switch(botState)
            {
                case BotState.Running:
                    timer1.Start();
                    StateButton.Text = "Stop";
                    this.Text = "Busy Bot - Running";
                    break;
                case BotState.Stopped:
                    timer1.Stop();
                    StateButton.Text = "Start";
                    this.Text = "Busy Bot - Stopped";
                    break;
                case BotState.Faulted:
                    timer1.Stop();
                    StateButton.Text = "Error";
                    StateButton.Enabled = false;
                    this.Text = "Busy Bot - Faulted";
                    break;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds a button to the Form dynamically
        /// </summary>
        private void AddButton()
        {
            StateButton = new Button()
            {
                Text = "Start",
                Parent = this,
                Left = 0,
                Top = 0,
                Width = 200,
                Height = 100
            };

            StateButton.Click += StateButton_Click;
            this.Controls.Add(StateButton);
        }

        /// <summary>
        /// Fires when the interval expires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Make sure the target window is able to receive input
            if (Program.SetForegroundWindow(TargetWindow) == true)
            {
                var linesToWrite = Config.Instance.LinesToWrite;
                SendKeys.SendWait($"{ExecutionCounter} - {Config.Instance.Message}");
                LineCounter++;
                if (LineCounter % linesToWrite == 0)
                {
                    SendKeys.SendWait($"{{HOME}}{Environment.NewLine}");
                    LineCounter = 0;
                    ExecutionCounter++;
                }
                else
                {
                    SendKeys.SendWait("\t");
                }
            }
            else
            {
                StateChanged(BotState.Faulted);
                MessageBox.Show($"Unable to find a window using class {Config.Instance.FindClass}. Busy bot is stopped and disabled. Please stop it and check the configuration.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
