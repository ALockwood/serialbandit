using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Diagnostics; //debug



namespace SerialBandit
{

    public enum CleaningModes { Regular, Max, Spot }; //cleaning modes
    public enum CommandModes { Passive, Safe, Direct }; //command modes

    [Flags]
    public enum Day_Of_Week { Mon = 2, Tue = 4, Wed = 8, Thu = 16, Fri = 32, Sat = 64, Sun = 1 };

    //This class will house pretty much everything that deals with the COM port.
    //Rx, Tx, open/Close, Status, etc.
    class RoombaPort
    {
        //CONSTANTS (ideally these  byte arrays would be read in from an XML file... that's on the TODO list)
        private byte[] cmd_ready = new byte[] { 128 }; //this puts roomba into passive mode as well
        private byte[] mode_safe = new byte[] { 131 };
        private byte[] mode_full = new byte[] { 132 };
        private byte[] clean_regular = new byte[] { 135 };
        private byte[] clean_max = new byte[] { 136 };
        private byte[] clean_spot = new byte[] { 134 };
        private byte[] action_seekdock = new byte[] { 143 };
        private byte[] schedule_clear = new byte[] { 167, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private byte[] power_shutdown = new byte[] { 133 };
        private byte[] led_power_red = new byte[] { 139, 0, 255, 255 };
        private byte[] led_power_orange = new byte[] { 139, 0, 100, 255 };
        private byte[] led_power_yellow = new byte[] { 139, 0, 15, 255 };
        private byte[] led_power_green = new byte[] { 139, 0, 0, 255 };
        private byte[] led_dirtdetect_on = new byte[] { 139, 1, 0, 0 };
        private byte[] led_spot_on = new byte[] { 139, 2, 0, 0 };
        private byte[] led_dock_on = new byte[] { 139, 4, 0, 0 };
        private byte[] led_checkbot_on = new byte[] { 139, 8, 0, 0 };
        private byte[] cmd_reset = new byte[] { 7 }; //resets the roomba to factory defaults
        private byte[] cmd_set_daytime = new byte[] { 168 };
        private byte[] cmd_set_schedule = new byte[] { 167 };
        //TODO:let the user program songs (add feature)
        private byte[] test_song = new byte[] { 140, 0, 5, 64, 32, 67, 32, 71, 32, 74, 32, 77, 32 }; 
        private byte[] play_test_song = new byte[] { 141, 0 };

        private string portname = null;
        private string baudrate = null;
        private bool port_tested = false; //set to true if the port has been opened successfully
        private string LastErr = null;

        //EVENTS
        //public event byte[] Data_Received;

        //PROPERTIES
        #region ***PROPERTIES***
        public string serial_port_name
        {//the serial port name property
            get { return portname; }
            set
            {
                portname = value;
                port_tested = false; //reset this as settings have changed
            }
        }

        public string serial_port_baud
        {//the serial port baud rate property
            get { return baudrate; }
            set
            {
                baudrate = value;
                port_tested = false; //reset this as settings have changed
            }
        }

        public bool serial_port_is_tested
        {//lets the parent class check the status of the port
            get { return port_tested; }
        }

        public string last_error
        {
            get { return LastErr; }
        }
        #endregion

        //SUBS
        public RoombaPort(string prtname, string baud)
        {//called by 'new' --setup local vars
            portname = prtname;
            baudrate = baud;
        }

        //private void playthatfunkymusic()
        //{//DEBUG
        //    //start = 128
        //    //safe = 131
        //    //test song = 140,0,4,31,32,45,32,57,32,69,32
        //    //play that song = 141,0
        //    //if (port_tested == true)
        //    //{
        //    //SerialPort sp = new SerialPort(portname, int.Parse(baudrate), Parity.None, 8, StopBits.One);

        //    byte[] start = new byte[] { 128 };
        //    byte[] safemode = new byte[] { 131 };
        //    byte[] song = new byte[] { 140, 0, 4, 31, 32, 45, 32, 57, 32, 69, 32 };
        //    //byte[] song = new byte[] { 140, 0, 9, 67, 32, 67, 32, 67, 32, 65, 32, 65, 32, 67, 32, 65, 32, 67, 32, 67, 60 }; //...would love to figure out imperial march!
        //    byte[] play = new byte[] { 141, 0 };
        //    try
        //    {
        //        direct_command(start);
        //        direct_command(safemode);
        //        direct_command(song);
        //        direct_command(play);
        //    }
        //    catch (Exception ex)
        //    {
        //        LastErr = ex.Message;
        //    }
        //}

        private void Clear_LastError()
        {//clears the lastErr var value
            this.LastErr = string.Empty;
        }

        public void Seek_Dock()
        {//this will send the command to the Roomba that instructs it to dock itself
            Send_Safemode_Command(action_seekdock);
        }

        public void Start_Cleaning(CleaningModes Mode)
        {//start regular cleaning, maxcleaning, or spot cleaning
            switch (Mode)
            {
                case CleaningModes.Regular:
                    Send_Safemode_Command(clean_regular);
                    break;
                case CleaningModes.Max:
                    Send_Safemode_Command(clean_max);
                    break;
                case CleaningModes.Spot:
                    Send_Safemode_Command(clean_spot);
                    break;
            }
        }

        public void Beep()
        {//simply beep the speaker
            //playthatfunkymusic();
            Send_Safemode_Command(test_song, false);
            if(true == direct_command(play_test_song))
                System.Threading.Thread.Sleep(2500); //TODO: write something to determine timing and wait until song shoudl be finished playing before executing next command (5 notes x 0.5s each = 2.5s)
            direct_command(cmd_ready); //get roomba ready for next command
        }

        public void Blink()
        {//sets the Roomba clean light Red, then back to green (ready state)
            Send_Safemode_Command(led_power_red);
        }
       /*TODO: overload this to add functionality for user-defined light colours
        public void Blink(int Red, int Green)
        {//sets the Roomba clean light to whatever colour is created by the integers sent in, then back to green (ready)
            //TODO: Add code for this functionality
        }
        */
        public void Set_Mode(CommandModes Mode)
        {//puts the Roomba into one of its three modes
            if (direct_command(cmd_ready))
            {
                switch (Mode)
                {
                    case CommandModes.Passive:
                        direct_command(cmd_ready);
                        break;
                    case CommandModes.Safe:
                        direct_command(mode_safe);
                        break;
                    case CommandModes.Direct:
                        direct_command(mode_full);
                        break;
                }
            }
        }

        public void Reset_Roomba()
        {//send an undocumented command to the Roomba that will reset it to factory defaults
            Send_Safemode_Command(cmd_reset);
        }
        
        private void Send_Safemode_Command(byte[] CommandBytes)
        {//Sends the ready command, puts the Roomba in 'safemode', then sends the CommandBytes 
         //and finishes with the ready command again
         //Errors will set LastErr.
            try
            {
                bool cmdresult = false;

                cmdresult = direct_command(cmd_ready);
                if (true == cmdresult)
                    cmdresult = direct_command(mode_safe);
                if (true == cmdresult)
                    cmdresult = direct_command(CommandBytes);
                if (true == cmdresult)
                    cmdresult = direct_command(cmd_ready);
                if (false == cmdresult)
                {
                    Exception failedCMD = new Exception("Failed to execute final ready command.");
                    throw failedCMD;
                }
            }
            catch (Exception ex)
            {
                LastErr = "Failed to send commands to Roomba. Err: " + ex.Message;
                return;
            }
        }

        private void Send_Safemode_Command(byte[] CommandBytes, bool EndInReadyMode) //overloaded so I don't have to update old calls to proc ;)
        {//Sends the ready command, puts the Roomba in 'safemode', then sends the CommandBytes 
            //and finishes with the ready command again
            //Errors will set LastErr.
            try
            {
                bool cmdresult = false;

                cmdresult = direct_command(cmd_ready);
                if (true == cmdresult)
                    cmdresult = direct_command(mode_safe);
                if (true == cmdresult)
                    cmdresult = direct_command(CommandBytes);
                if (true == cmdresult & true == EndInReadyMode) //switch to disable re-entering ready mode
                    cmdresult = direct_command(cmd_ready);
                if (false == cmdresult)
                {
                    Exception failedCMD = new Exception("Failed to execute final ready command.");
                    throw failedCMD;
                }
            }
            catch (Exception ex)
            {
                LastErr = "Failed to send commands to Roomba. Err: " + ex.Message;
                return;
            }
        }
        #region "Scheduling"

        public void Clear_Scheduling()
        {//clears the roomba scheduled cleaning time memory
            //check the last_err to see if it succeeded or not
            try
            {
                Clear_LastError();

                if (direct_command(schedule_clear) != true)
                {
                    LastErr = "Failed to clear schedule. CMDErr: " + LastErr;
                }
            }
            catch (Exception ex)
            {
                LastErr = ex.Message;
            }
        }

        public void Set_Current_Time(DateTime inTime)
        {//this will set the current time for the roomba
            //check the last_err to see if it succeeded or not
            Clear_LastError();
            try
            {
                byte[] settime_command = new byte[4];
                settime_command[0] = cmd_set_daytime[0];
                settime_command[1] = (byte)(inTime.DayOfWeek);
                settime_command[2] = (byte)inTime.Hour;
                settime_command[3] = (byte)inTime.Minute;
                
                //TODO: clean this up
                direct_command(cmd_ready);
                direct_command(mode_safe);
                direct_command(cmd_ready);

                if (direct_command(settime_command) != true)
                {//failed to write command
                    LastErr = "Failed to set current time. CMDErr: " + LastErr;
                }
            }
            catch (Exception ex)
            {
                LastErr = ex.Message;
            }
        }

        public void Set_Cleaning_Schedule(List<Roomba_ScheduleBits> Schedules)
        {//responsible for compiling the final 15 byte array and sending it to the Roomba
         //to set the cleaning schedule
            Clear_LastError(); //clear any errors
            byte[] masterSched = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            try //build the 15 byte array
            {
                masterSched[0] += cmd_set_schedule[0];  //command byte
                for (int i = 0; i < Schedules.Count; i++) //loop through each day schedule that was sent and add the values to the "master schedule"
                {//there has GOT to be a better way to do this... :\
                    masterSched[1] += Schedules[i].Schedule[0]; //enabled days bitmap
                    masterSched[2] += Schedules[i].Schedule[1]; //sunday hr
                    masterSched[3] += Schedules[i].Schedule[2]; //sunday min
                    masterSched[4] += Schedules[i].Schedule[3]; //monday hr
                    masterSched[5] += Schedules[i].Schedule[4]; //monday min
                    masterSched[6] += Schedules[i].Schedule[5]; //tuesday hr
                    masterSched[7] += Schedules[i].Schedule[6]; //tuesday min
                    masterSched[8] += Schedules[i].Schedule[7]; //wednesday hr
                    masterSched[9] += Schedules[i].Schedule[8]; //wednesday min
                    masterSched[10] += Schedules[i].Schedule[9]; //thursday hr
                    masterSched[11] += Schedules[i].Schedule[10]; //thursday min
                    masterSched[12] += Schedules[i].Schedule[11]; //friday hr
                    masterSched[13] += Schedules[i].Schedule[12]; //friday min
                    masterSched[14] += Schedules[i].Schedule[13]; //saturday hr
                    masterSched[15] += Schedules[i].Schedule[14]; //saturday min
                }
            }
            catch (Exception ex)
            {
                LastErr = "Failed to build scedule byte arr. Err: " + ex.Message;
                return;
            }

            //if the byte array was built successfully then try to send it to the serial port
            Send_Safemode_Command(masterSched); //send the cleaning schedule
        }
        #endregion

        //FUNCTIONS
        #region ***FUNCTIONS***
        private bool direct_command(byte[] bytes)
        {//this will send the byte string sent in to the roomba. succesful execution returns true
            Clear_LastError();
            try
            {
                if (port_tested == true)
                {
                    SerialPort sp = Make_Port();
                    if (sp != null)
                    {
                        sp.Open();
                        sp.Write(bytes, 0, bytes.Length);
                        System.Threading.Thread.Sleep(50); //delay 50ms as roomba seems to need a delay between commands
                        sp.Close();
                        return true;
                    }
                    else
                    {//something failed...
                        Exception noport = new Exception("Failed to open port. " + LastErr);
                        throw noport;
                    }
                }
                else
                {//port hasn't been tested
                    Exception nottested = new Exception("Serial port hasn't been successfully tested.");
                    throw nottested;
                }
            }
            catch (Exception ex)
            {
                LastErr = "Command failed. Err: " + ex.Message;
                return false;
            }
        }

        private SerialPort Make_Port()
        {//creates a serial port object based on the variables used
            try
            {
                SerialPort sp = new SerialPort(portname, int.Parse(baudrate), Parity.None, 8, StopBits.One);
                return sp;
            }
            catch (Exception portEx)
            {//return null and set the error message
                LastErr = portEx.Message;
                return null;
            }
        }

        public bool Test_Port()
        {//try to open the port return false for failure, true for success & update port_tested either way
            Clear_LastError();

            try
            {
                if (portname != null & baudrate != null)
                {//vars are populated- try to test
                    SerialPort testport = new SerialPort(portname, int.Parse(baudrate), Parity.None, 8, StopBits.One);
                    if (testport.IsOpen == true)
                    {
                        LastErr = "The specified port is already open. Close it before trying again.";
                        port_tested = false; //port not tested successfully
                        return false;
                    }
                    testport.Open();
                    testport.Close();

                    //success if we didn't crap out by this point
                    port_tested = true; //port is tested-- set the flag
                    return true;
                }
                else
                {//something is clearly wrong
                    LastErr = "Unexpected error. Hacking??";
                    port_tested = false; //port not tested successfully
                    return false;
                }
            }
            catch (Exception portOpenEx)
            {
                LastErr = "Unexpected error. " + portOpenEx.Message; //set err text
                port_tested = false; //port not tested successfully
                return false;
            }
        }
        #endregion

    } //end class

    public class Roomba_ScheduleBits
    {//helps to build the byte array that'll be sent to the serial port to setup the Roomba cleaning schedule
        private SerialBandit.Day_Of_Week dayofweek;
        private int start_hour = 0;
        private int start_min = 0;

        private byte[] schedule; //holds the values for the schedule... will be all zeros except for the specific day
        //that way all the values can be added (including [0] which is the flag bits) to get the complete schedule byte array

        public int Scheduled_Start_Hour
        {
            get { return start_hour; }
        }

        public int Scheduled_Start_Minute
        {
            get { return start_min; }
        }

        public SerialBandit.Day_Of_Week Day
        {
            get { return dayofweek; }
        }

        public byte[] Schedule
        {
            get { return schedule; }
        }

        public Roomba_ScheduleBits(SerialBandit.Day_Of_Week wkday, int StartTime_Hour, int StartTime_Min)
        {//builds an object that will have all we need to build a byte array of all enabled/disabled cleaning days
            //and their start times later
            schedule = new byte[15];

            switch (wkday)
            {
                case SerialBandit.Day_Of_Week.Mon:
                    schedule[3] = (byte)StartTime_Hour;
                    schedule[4] = (byte)StartTime_Min;
                    break;
                case SerialBandit.Day_Of_Week.Tue:
                    schedule[5] = (byte)StartTime_Hour;
                    schedule[6] = (byte)StartTime_Min;
                    break;
                case SerialBandit.Day_Of_Week.Wed:
                    schedule[7] = (byte)StartTime_Hour;
                    schedule[8] = (byte)StartTime_Min;
                    break;
                case SerialBandit.Day_Of_Week.Thu:
                    schedule[9] = (byte)StartTime_Hour;
                    schedule[10] = (byte)StartTime_Min;
                    break;
                case SerialBandit.Day_Of_Week.Fri:
                    schedule[11] = (byte)StartTime_Hour;
                    schedule[12] = (byte)StartTime_Min;
                    break;
                case SerialBandit.Day_Of_Week.Sat:
                    schedule[13] = (byte)StartTime_Hour;
                    schedule[14] = (byte)StartTime_Min;
                    break;
                case SerialBandit.Day_Of_Week.Sun:
                    schedule[1] = (byte)StartTime_Hour;
                    schedule[2] = (byte)StartTime_Min;
                    break;
                default:
                    Exception badstuff = new Exception("Unexpected value encountered in Roomba_Builder!");
                    throw badstuff; //throw an exception if values are outside what's expected
            }

            //common to all days
            dayofweek = wkday;
            schedule[0] = (byte)dayofweek;
            start_hour = StartTime_Hour;
            start_min = StartTime_Min;
        }

        //overloaded because I just want to send a byte array for start hour & min
        public Roomba_ScheduleBits(SerialBandit.Day_Of_Week wkday, byte[] sched_starttimes)
        {//builds an object that will have all we need to build a byte array of all enabled/disabled cleaning days
            //and their start times later
            schedule = new byte[15];

            switch (wkday)
            {
                case SerialBandit.Day_Of_Week.Mon:
                    schedule[3] = sched_starttimes[0];
                    schedule[4] = sched_starttimes[1];
                    break;
                case SerialBandit.Day_Of_Week.Tue:
                    schedule[5] = sched_starttimes[0];
                    schedule[6] = sched_starttimes[1];
                    break;
                case SerialBandit.Day_Of_Week.Wed:
                    schedule[7] = sched_starttimes[0];
                    schedule[8] = sched_starttimes[1];
                    break;
                case SerialBandit.Day_Of_Week.Thu:
                    schedule[9] = sched_starttimes[0];
                    schedule[10] = sched_starttimes[1];
                    break;
                case SerialBandit.Day_Of_Week.Fri:
                    schedule[11] = sched_starttimes[0];
                    schedule[12] = sched_starttimes[1];
                    break;
                case SerialBandit.Day_Of_Week.Sat:
                    schedule[13] = sched_starttimes[0];
                    schedule[14] = sched_starttimes[1];
                    break;
                case SerialBandit.Day_Of_Week.Sun:
                    schedule[1] = sched_starttimes[0];
                    schedule[2] = sched_starttimes[1];
                    break;
                default:
                    Exception badstuff = new Exception("Unexpected value encountered in Roomba_Builder!");
                    throw badstuff; //throw an exception if values are outside what's expected
            }

            //common to all days
            dayofweek = wkday;
            schedule[0] = (byte)dayofweek;
            start_hour = (int)sched_starttimes[0];
            start_min = (int)sched_starttimes[1];
        }
    }
}


