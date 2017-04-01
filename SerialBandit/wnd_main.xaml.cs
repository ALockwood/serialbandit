using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Windows.Shapes;
using SerialBandit.Properties;
using System.Diagnostics; //debug


namespace SerialBandit
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
    

    public partial class WindowMain : Window
    {
       //CONSTANTS
        private const string appname = "SerialBandit"; //application name used for registry stuff
        private const string sb_port_name = "Port Name: ";
        private const string sb_port_baud = "Baud Rate: ";
        private const int day_disabled = -1; //day for schedule is not enabled
        //VARIABLES
        public static string port_name = null;
        public static string port_baud = null;
        private RoombaPort lcl_serialport; //local serial port (roombaport) obj
        private bool SaveOnExit = false; //whether or not to save schedule settings on exit
        private bool isScheduleWipe = false;
        //private bool port_open = false;
        private int last_min = -1; //holds the value of the last minute
        private DispatcherTimer second_timer;
        private double sched_mon = day_disabled; //these are the scheduled start times in # of minutes past 12am for each day
        private double sched_tue = day_disabled; //-1 == disabled
        private double sched_wed = day_disabled;
        private double sched_thu = day_disabled;
        private double sched_fri = day_disabled;
        private double sched_sat = day_disabled;
        private double sched_sun = day_disabled;

        private string LastError = string.Empty; //used in some error handling

        //ENUMS
        enum MenuCommand {Beep, 
                          Blink, 
                          RESET, 
                          ModeSafe, 
                          ModePassive, 
                          ModeDirect, 
                          SeekDock,
                          Clean,
                          CleanMax,
                          CleanSpot
                         };

        #region ***Window Init Code***

        public WindowMain()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {//form loaded
            
            Update_Time(); //set the label text right away
            
            //create instance of a new timer & start it
            second_timer = new DispatcherTimer();
            second_timer.Tick += new EventHandler(second_timer_Tick);
            second_timer.Interval = new TimeSpan(0, 0, 1);
            second_timer.Start();

            Get_App_Settings();
            
            if (Port_Vars_Ready() == false)
            {//if settings weren't updated via appsettings code then load no defaults
                Update_StatusBar_TextAlignment(); //update status bar text with defaults
            }
            else
            {//appsettings had data... try to use those settings
                Update_StatusBar_TextAlignment(port_name,port_baud); //update status bar text with defaults
            }
            
        }

        #endregion
    
        #region ***Events***
        
        private void mnui_file_quit_Click(object sender, RoutedEventArgs e)
        {//user has selected file->quit
            this.Close();
        }

        private void mnui_Settings_com_Click(object sender, RoutedEventArgs e)
        {//user has clicked the COM port settings file menu option
            Show_COM_Settings_Window();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {//window is closing - save any app settings
            Save_App_Settings();
        }

        void second_timer_Tick(object sender, EventArgs e)
        {//handles the second_timer tick event
            Update_Time();
        }

        private void Slider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {//handles the change event of all the slider controls (scheduled cleaning time selector)
            try
            {
                Slider tmpslider = (Slider)e.Source; //temp object reference
                switch (tmpslider.Name)
                {
                    case "sl_mon":
                        this.txt_mon.Text = Convert_To_HumanTime(e.NewValue);
                        break;
                    case "sl_tue":
                        this.txt_tue.Text = Convert_To_HumanTime(e.NewValue);
                        break;
                    case "sl_wed":
                        this.txt_wed.Text = Convert_To_HumanTime(e.NewValue);
                        break;
                    case "sl_thu":
                        this.txt_thu.Text = Convert_To_HumanTime(e.NewValue);
                        break;
                    case "sl_fri":
                        this.txt_fri.Text = Convert_To_HumanTime(e.NewValue);
                        break;
                    case "sl_sat":
                        this.txt_sat.Text = Convert_To_HumanTime(e.NewValue);
                        break;
                    case "sl_sun":
                        this.txt_sun.Text = Convert_To_HumanTime(e.NewValue);
                        break;
                    default: //something went wrong- throw ex
                        Exception BadCase = new Exception("Unexpected case sent to slider eval.");
                        throw BadCase;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Slider changed. Ex - " + ex.Message);
            }
                
        }

        private void Checkbox_Check_Changed(object sender, RoutedEventArgs e)
        {//handles the change event of all the day check boxes
            try
            {
                CheckBox tmpchk = (CheckBox)e.Source; //temp object reference
                switch (tmpchk.Name)
                {
                    case "chk_mon":
                        if (tmpchk.IsChecked == true)
                        {//enable the slider
                            sl_mon.IsEnabled = true;
                            txt_mon.Text = this.Convert_To_HumanTime(sl_mon.Value);
                        }
                        else
                        {//disable the slider and clear the textbox
                            sl_mon.IsEnabled = false;
                            sl_mon.Value = 0;
                            txt_mon.Text = string.Empty;
                        }
                        break;
                    case "chk_tue":
                        if (tmpchk.IsChecked == true)
                        {//enable the slider
                            sl_tue.IsEnabled = true;
                            txt_tue.Text = this.Convert_To_HumanTime(sl_tue.Value);
                        }
                        else
                        {//disable the slider and clear the textbox
                            sl_tue.IsEnabled = false;
                            sl_tue.Value = 0;
                            txt_tue.Text = string.Empty;
                        }
                        break;
                    case "chk_wed":
                        if (tmpchk.IsChecked == true)
                        {//enable the slider
                            sl_wed.IsEnabled = true;
                            txt_wed.Text = this.Convert_To_HumanTime(sl_wed.Value);
                        }
                        else
                        {//disable the slider and clear the textbox
                            sl_wed.IsEnabled = false;
                            sl_wed.Value = 0;
                            txt_wed.Text = string.Empty;
                        }
                        break;
                    case "chk_thu":
                        if (tmpchk.IsChecked == true)
                        {//enable the slider
                            sl_thu.IsEnabled = true;
                            txt_thu.Text = this.Convert_To_HumanTime(sl_thu.Value);
                        }
                        else
                        {//disable the slider and clear the textbox
                            sl_thu.IsEnabled = false;
                            sl_thu.Value = 0;
                            txt_thu.Text = string.Empty;
                        }
                        break;
                    case "chk_fri":
                        if (tmpchk.IsChecked == true)
                        {//enable the slider
                            sl_fri.IsEnabled = true;
                            txt_fri.Text = this.Convert_To_HumanTime(sl_fri.Value);
                        }
                        else
                        {//disable the slider and clear the textbox
                            sl_fri.IsEnabled = false;
                            sl_fri.Value = 0;
                            txt_fri.Text = string.Empty;
                        }
                        break;
                    case "chk_sat":
                        if (tmpchk.IsChecked == true)
                        {//enable the slider
                            sl_sat.IsEnabled = true;
                            txt_sat.Text = this.Convert_To_HumanTime(sl_sat.Value);
                        }
                        else
                        {//disable the slider and clear the textbox
                            sl_sat.IsEnabled = false;
                            sl_sat.Value = 0;
                            txt_sat.Text = string.Empty;
                        }
                        break;
                    case "chk_sun":
                        if (tmpchk.IsChecked == true)
                        {//enable the slider
                            sl_sun.IsEnabled = true;
                            txt_sun.Text = this.Convert_To_HumanTime(sl_sun.Value);
                        }
                        else
                        {//disable the slider and clear the textbox
                            sl_sun.IsEnabled = false;
                            sl_sun.Value = 0;
                            txt_sun.Text = string.Empty;
                        }
                        break;
                    default: //something went wrong- throw ex
                        Exception BadCase = new Exception("Unexpected case sent to checkbox eval.");
                        throw BadCase;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Slider changed. Ex - " + ex.Message);
            }
        }

        private void Enable_Saving_CleaningSchedule(object sender, RoutedEventArgs e)
        {
            SaveOnExit = true;
        }

        private void Disable_Saving_CleaningSchedule(object sender, RoutedEventArgs e)
        {
            SaveOnExit = false;
        }

        private void btn_ResetSchedule_Click(object sender, RoutedEventArgs e)
        {//user clicked the "reset values" button
            try
            {//this should fire the other events that take care of these things
                chk_mon.IsChecked = false;
                chk_tue.IsChecked = false;
                chk_wed.IsChecked = false;
                chk_thu.IsChecked = false;
                chk_fri.IsChecked = false;
                chk_sat.IsChecked = false;
                chk_sun.IsChecked = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to rest properly. Err: " + ex.Message);
            }
        }

        private void btn_SetSchedule_Click(object sender, RoutedEventArgs e)
        {//user clicked the "set schedule" button
            try
            {
                isScheduleWipe = false; //reset values
                LastError = string.Empty;

                //if the port is ready & user doesn't want to cancel sched wipe (if true)...
                if (Roomba_Port_Ready() & Check_Blank_Schedule()) 
                {
                    if (isScheduleWipe == true)
                    {//call the specific method
                        //TODO: perhaps put in an "Are you sure?" prompt...
                        lcl_serialport.Clear_Scheduling();
                    }
                    else
                    {//setup the proper schedule 
                        Set_Clean_Times(); //ensure latest values are stored in vars
                
                        //create the list that'll hold individual day byte arrays (enabled days only)
                        List<Roomba_ScheduleBits> SchedArray = new List<Roomba_ScheduleBits>();
                        
                        if (Get_Checkbox_Status(sched_mon)) //Monday
                        {
                            Roomba_ScheduleBits tmpDay = new Roomba_ScheduleBits(Day_Of_Week.Mon, Convert_Double_To_RoombaTime(sched_mon));
                            SchedArray.Add(tmpDay);
                        }
                        if (Get_Checkbox_Status(sched_tue)) //Tuesday
                        {
                            Roomba_ScheduleBits tmpDay = new Roomba_ScheduleBits(Day_Of_Week.Tue, Convert_Double_To_RoombaTime(sched_tue));
                            SchedArray.Add(tmpDay);
                        }
                        if (Get_Checkbox_Status(sched_wed)) //Wednesday
                        {
                            Roomba_ScheduleBits tmpDay = new Roomba_ScheduleBits(Day_Of_Week.Wed, Convert_Double_To_RoombaTime(sched_wed));
                            SchedArray.Add(tmpDay);
                        }
                        if (Get_Checkbox_Status(sched_thu)) //Thursday
                        {
                            Roomba_ScheduleBits tmpDay = new Roomba_ScheduleBits(Day_Of_Week.Thu, Convert_Double_To_RoombaTime(sched_thu));
                            SchedArray.Add(tmpDay);
                        }
                        if (Get_Checkbox_Status(sched_fri)) //Friday
                        {
                            Roomba_ScheduleBits tmpDay = new Roomba_ScheduleBits(Day_Of_Week.Fri, Convert_Double_To_RoombaTime(sched_fri));
                            SchedArray.Add(tmpDay);
                        }
                        if (Get_Checkbox_Status(sched_sat)) //Saturday
                        {
                            Roomba_ScheduleBits tmpDay = new Roomba_ScheduleBits(Day_Of_Week.Sat, Convert_Double_To_RoombaTime(sched_sat));
                            SchedArray.Add(tmpDay);
                        }
                        if (Get_Checkbox_Status(sched_sun)) //Sunday
                        {
                            Roomba_ScheduleBits tmpDay = new Roomba_ScheduleBits(Day_Of_Week.Sun, Convert_Double_To_RoombaTime(sched_sun));
                            SchedArray.Add(tmpDay);
                        }

                        //Now send the list to the SerialBandit code
                        lcl_serialport.Set_Current_Time(DateTime.Now); //set the date now
                        lcl_serialport.Set_Cleaning_Schedule(SchedArray); //set the schedule
                    }

                    if (lcl_serialport.last_error == string.Empty)
                    {//TODO: show success?
                       // MessageBox.Show("All good!");
                    }
                }
                else
                {//TODO: better error message
                    MessageBox.Show("FAILED!");
                }
            }
            catch (Exception ex)
            {//TODO: better error handling/user alerting?
                MessageBox.Show("An error occured in SetSchedule. Err: " + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mnui_Diagnostics_testcom_beep_Click(object sender, RoutedEventArgs e)
        {//user selected "Beep" from diagnostics menu
            Execute_Roomba_MenuItem_Command(MenuCommand.Beep);
        }
        private void mnui_Diagnostics_testcom_blink_Click(object sender, RoutedEventArgs e)
        {//user selected "Blink" from diagnostics menu
            Execute_Roomba_MenuItem_Command(MenuCommand.Blink);
        }
        
        private void mnui_Diagnostics_passive_Click(object sender, RoutedEventArgs e)
        {//user selected "Passive Mode" from diagnostics menu
            Execute_Roomba_MenuItem_Command(MenuCommand.ModePassive);
        }
        private void mnui_Diagnostics_safe_Click(object sender, RoutedEventArgs e)
        {//user selected safe mode from diagnostics menu
            Execute_Roomba_MenuItem_Command(MenuCommand.ModeSafe);
        }
        private void mnui_Diagnostics_direct_Click(object sender, RoutedEventArgs e)
        {//user selected direct mode from diagnostics menu
            Execute_Roomba_MenuItem_Command(MenuCommand.ModeDirect);
        }
        private void mnui_Diagnostics_RESET_Click(object sender, RoutedEventArgs e)
        {//user selected reset from diagnostics menu
            Execute_Roomba_MenuItem_Command(MenuCommand.RESET);
        }

        private void mnui_Clean_reg_Click(object sender, RoutedEventArgs e)
        {//user selected start clean from clean menu
            Execute_Roomba_MenuItem_Command(MenuCommand.Clean);
        }
        private void mnui_Clean_max_Click(object sender, RoutedEventArgs e)
        {//user selected start cleanMax from clean menu
            Execute_Roomba_MenuItem_Command(MenuCommand.CleanMax);
        }
        private void mnui_Clean_spot_Click(object sender, RoutedEventArgs e)
        {//user selected start spot clean from clean menu
            Execute_Roomba_MenuItem_Command(MenuCommand.CleanSpot);
        }
        private void mnui_Clean_SeekDock_Click(object sender, RoutedEventArgs e)
        {//user selected seek dock from clean menu
            Execute_Roomba_MenuItem_Command(MenuCommand.SeekDock);
        }

        private void mnui_Help_about_Click(object sender, RoutedEventArgs e)
        {//user clicked "About..."
            AboutMe a1 = new AboutMe();
            a1.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            a1.ShowInTaskbar = false;
            a1.Show();
        }
        #endregion


        #region ***Subs***

        #region ***App Settings***

        private void Get_App_Settings()
        {//checks to see if any settings are stored, and if so gets the values and uses them
            try
            {//load class vars with settings if possible
                port_name = Settings.Default.StoredPort.ToString();
                port_baud = Settings.Default.StoredBaud.ToString();
                SaveOnExit = Settings.Default.SaveSchedOnQuit;

                menu_saveCleanSched.IsChecked = SaveOnExit; //show the option

                Load_n_Show_Sched_Settings(); //load saved schedule data + setup checkboxes & sliders
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load appsettings - " + ex.Message);
            }
        }

        private void Load_n_Show_Sched_Settings()
        {//attempts to get the schedule settings
            try
            {
                sched_mon = Settings.Default.SchedMonday;
                sched_tue = Settings.Default.SchedTuesday;
                sched_wed = Settings.Default.SchedWednesday;
                sched_thu = Settings.Default.SchedThursday;
                sched_fri = Settings.Default.SchedFriday;
                sched_sat = Settings.Default.SchedSaturday;
                sched_sun = Settings.Default.SchedSunday;

                //set the checkboxes
                this.chk_mon.IsChecked = Get_Checkbox_Status(sched_mon);
                this.chk_tue.IsChecked = Get_Checkbox_Status(sched_tue);
                this.chk_wed.IsChecked = Get_Checkbox_Status(sched_wed);
                this.chk_thu.IsChecked = Get_Checkbox_Status(sched_thu);
                this.chk_fri.IsChecked = Get_Checkbox_Status(sched_fri);
                this.chk_sat.IsChecked = Get_Checkbox_Status(sched_sat);
                this.chk_sun.IsChecked = Get_Checkbox_Status(sched_sun);

                //set sliders
                if (chk_mon.IsChecked == true)
                { sl_mon.Value = sched_mon; }

                if (chk_tue.IsChecked == true)
                { sl_tue.Value = sched_tue; }

                if (chk_wed.IsChecked == true)
                { sl_wed.Value = sched_wed; }

                if (chk_thu.IsChecked == true)
                { sl_thu.Value = sched_thu; }

                if (chk_fri.IsChecked == true)
                { sl_fri.Value = sched_fri; }

                if (chk_sat.IsChecked == true)
                { sl_sat.Value = sched_sat; }

                if (chk_sun.IsChecked == true)
                { sl_sun.Value = sched_sun; }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to modify schedule controls - " + ex.Message);
            }
        }
        
        private void Save_App_Settings()
        {//stores application settings (called on exit)
            if (sb_port_name != null)
            {
                Settings.Default.StoredPort = port_name;
            }
            if (sb_port_baud != null)
            {
                Settings.Default.StoredBaud = port_baud;
            }

            Settings.Default.SaveSchedOnQuit = SaveOnExit; //save schedule settings or not
            if (SaveOnExit == true)
            {//store the values
                Set_Clean_Times(); //update the global vars for settings

                Settings.Default.SchedMonday = sched_mon;
                Settings.Default.SchedTuesday = sched_tue;
                Settings.Default.SchedWednesday = sched_wed;
                Settings.Default.SchedThursday = sched_thu;
                Settings.Default.SchedFriday = sched_fri;
                Settings.Default.SchedSaturday = sched_sat;
                Settings.Default.SchedSunday = sched_sun;
            }
            Settings.Default.Save();
        }

        #endregion

        #region ***Window Related***
        
        private void Show_COM_Settings_Window()
        {//displays the COM port settings window
            wnd_com comWindow = new wnd_com();
            comWindow.ShowInTaskbar = false; //don't show the window in the taskbar
            comWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; //center the child window
            comWindow.Owner = this;

            bool? show_dialog_result = comWindow.ShowDialog(); //actually show the window and make it modal

            if (show_dialog_result == true)
            {
               Debug.WriteLine("returned TRUE from modal");
               Update_StatusBar_TextAlignment(port_name, port_baud); //update the status bar text for GUI
               Save_App_Settings(); //save the app settings with what the user selected now
            }
            else 
            {
                Debug.WriteLine("returned FALSE or NULL from modal");
            }
        }
        
        #endregion
        
        #region ***Status Bar Text***

        //Update_StatusBar_TextAlignment is overloaded 3 times...
        private void Update_StatusBar_TextAlignment()
        {//simply updates the text in the status bar with the values sent in
            //overloaded version just updates defaults for visual jizzy-jazz
            try
            {//set defaults
                sbi_portName.Content = sb_port_name;
                sbi_baudRate.Content = sb_port_baud;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured setting status bar text!!?" + "\n" + ex.Message, "This isn't right...",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Update_StatusBar_TextAlignment(string portname, string baudrate)
        {//simply updates the text in the status bar with the values sent in
            //overloaded version only updates name and baud
            try
            {//combine const strings with values
                sbi_portName.Content = sb_port_name + portname;
                sbi_baudRate.Content = sb_port_baud + baudrate;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured setting status bar text!!?" + "\n" + ex.Message, "This isn't right...", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Update_StatusBar_TextAlignment(string portname, string baudrate, bool portstatus)
        {//simply updates the text in the status bar with the values sent in
            //overloaded version takes in bool to set port status as well
            try
            {//combine const strings with values
                sbi_portName.Content = sb_port_name + portname;
                sbi_baudRate.Content = sb_port_baud +  baudrate;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured setting status bar text!!?" + "\n" + ex.Message, "This isn't right...", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    #endregion
       
        #region ***Timer Related***
        private void Update_Time()
        {//simply updates the label on the form every time a minute ticks
            if (last_min < DateTime.Now.Minute) //update the label text if it's changed
            {
                last_min = DateTime.Now.Minute;
                sbi_CurrentTime.Content = DateTime.Now.ToShortTimeString();
            }
        }

        #endregion

        private void Set_Clean_Times()
        {//sets the values of the week day clean time variables
            //check each of the days and set the value if not null
            if (chk_mon.IsChecked == true) //monday
            {
                sched_mon = sl_mon.Value;
            }
            else
            {
                sched_mon = day_disabled;
            }

            if (chk_tue.IsChecked == true) //tuesday
            {
                sched_tue = sl_tue.Value;
            }
            else
            {
                sched_tue = day_disabled;
            }

            if (chk_wed.IsChecked == true) //wednesday
            {
                sched_wed = sl_wed.Value;
            }
            else
            {
                sched_wed = day_disabled;
            }

            if (chk_thu.IsChecked == true) //thursday
            {
                sched_thu = sl_thu.Value;
            }
            else
            {
                sched_thu = day_disabled;
            }

            if (chk_fri.IsChecked == true) //friday
            {
                sched_fri = sl_fri.Value;
            }
            else
            {
                sched_fri = day_disabled;
            }

            if (chk_sat.IsChecked == true) //saturday
            {
                sched_sat = sl_sat.Value;
            }
            else
            {
                sched_sat = day_disabled;
            }

            if (chk_sun.IsChecked == true) //sunday
            {
                sched_sun = sl_sun.Value;
            }
            else
            {
                sched_sun = day_disabled;
            }
        }

        private void Execute_Roomba_MenuItem_Command(MenuCommand mnuCmd)
        {//causes the Roomba to execute a specific command the users chooses from the menu items
            try
            {
                if (Roomba_Port_Ready()) //check the port
                {//port is ok -- send the command
                    switch (mnuCmd)
                    {//default handles any entries not caught. This should only be called by specific event handlers thus string not matched == error.
                        case MenuCommand.Beep:
                            lcl_serialport.Beep();
                            break;
                        case MenuCommand.Blink:
                            lcl_serialport.Blink();
                            break;
                        case MenuCommand.Clean:
                            lcl_serialport.Start_Cleaning(CleaningModes.Regular);
                            break;
                        case MenuCommand.CleanMax:
                            lcl_serialport.Start_Cleaning(CleaningModes.Max);
                            break;
                        case MenuCommand.CleanSpot:
                            lcl_serialport.Start_Cleaning(CleaningModes.Spot);
                            break;
                        case MenuCommand.ModeDirect:
                            lcl_serialport.Set_Mode(CommandModes.Direct);
                            break;
                        case MenuCommand.ModePassive:
                            lcl_serialport.Set_Mode(CommandModes.Passive);
                            break;
                        case MenuCommand.ModeSafe:
                            lcl_serialport.Set_Mode(CommandModes.Safe);
                            break;
                        case MenuCommand.RESET:
                            lcl_serialport.Reset_Roomba();
                            break;
                        case MenuCommand.SeekDock:
                            lcl_serialport.Seek_Dock();
                            break;
                        default:
                            Exception badInput = new Exception("Invalid option in ExecMnuItem");
                            throw badInput;
                    }
                    //if any errors occured the lcl_serialport last_error will not be string.empty
                    if (lcl_serialport.last_error != string.Empty)
                    {//a failure occured
                        Exception comErr = new Exception("An error occured in the COM module. Err: " + lcl_serialport.last_error);
                        throw comErr;
                    }
                }//end "if com ready"
                else
                {//raise an error about the port not being ready
                    MessageBox.Show("The COM port is not ready. Make sure you have configured it correctly.", "Command Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception ex)
            {//show the error
                MessageBox.Show("An error has occured. Err: " + ex.Message,"Command Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        #endregion


        #region ***Functions***
        private bool Check_Blank_Schedule()
        {//checks to see if the user is attempting to send a blank schdule to the Roomba and prompts them with a
         //warning to ensure they want to clear the roombas schedule memory
            if (chk_mon.IsChecked == false & chk_tue.IsChecked == false & chk_wed.IsChecked == false & chk_thu.IsChecked == false
                & chk_fri.IsChecked == false & chk_sat.IsChecked == false & chk_sun.IsChecked == false)
            {
                isScheduleWipe = true;

                MessageBoxResult clearit = MessageBox.Show(@"Sending this schedule will clear all schedules from the Roomba. 
                    Is this what you want to do?", "Clear Schedule Memory?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (clearit != MessageBoxResult.OK)
                {
                    LastError = "User Cancelled schedule upload.";
                    return false; //user wants to cancel
                }
            }
            return true; //default return value
        }

        private bool Pre_Communications_Tests()
        {//returns true if all pre-requisite checks are done. Displays prompts to user as well
            try
            {
                if (Port_Vars_Ready())
                {//port is good to go -- attempt to init the Roomba obj
                    try
                    {
                        lcl_serialport = new RoombaPort(port_name, port_baud); //try to init the obj
                        if (lcl_serialport.Test_Port())
                        {
                            return true; //we're ready to communicate with the Roomba
                        }
                        else
                        {//roomba port test failed
                            MessageBox.Show("Roomba port test failed. Err: " + lcl_serialport.last_error,
                                "Roomba Serial Port Test Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    catch (Exception roomba_init_fail)
                    {
                        MessageBox.Show("Failed to init Roomba connection. Err: " + roomba_init_fail.Message,
                            "Roomba COM Init Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else 
                {
                    MessageBox.Show(@"The serial port has not been configured properly. Please configure a serial port for 
                                     communication with your Roomba and try again.",
                "Serial Port Verification Failed", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("problem in pre_comms. Err: " + ex.Message);
                return false;
            }
        }

        private bool Port_Vars_Ready()
        {//simply ensure the vars for portname and baud are defined
            if (port_baud != null & port_name != null) //if either is null we can't talk to the roomba
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Roomba_Port_Ready()
        {//checks to see if the Roomba port obj has been initiliazed properly or not. Attempts to init if not.
            if (lcl_serialport == null)
            {
                return Pre_Communications_Tests();
            }
            //if the roomba port obj is created then check to see if it's tested
            if (lcl_serialport.serial_port_is_tested == true)
            {
                return true; //it's been tested successfully
            }
            else
            {//test it out and return the result
                return lcl_serialport.Test_Port();
            }
        }

        private bool Get_Checkbox_Status(double intval)
        {//return false if intval <0 -- used to check if schedule for the day is enabled
            if (intval < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string Convert_To_HumanTime(double dbl_time)
        {//convert int to human-readable time format (24 hour clock)
            DateTime dt = new DateTime(1978, 12, 28, 0, 0, 0); //default start time
            return dt.AddMinutes(dbl_time).ToShortTimeString();
        }

        private byte[] Convert_Double_To_RoombaTime(double dbl_time)
        {//returns a byte array where byte[0] = integer hours and byte[1] = integer minute
            try
            {
                byte[] rt = new byte[2];
                DateTime dt = new DateTime(1978, 12, 28, 0, 0, 0); //default start time

                Debug.WriteLine(dt.AddMinutes(dbl_time).Hour);
                Debug.WriteLine(dt.AddMinutes(dbl_time).Minute);
                
                rt[0] = (byte)dt.AddMinutes(dbl_time).Hour; //24 clock value of start time + var
                rt[1] = (byte)dt.AddMinutes(dbl_time).Minute;
                return rt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                byte[] err = new byte[2] {0,0};
                return err;
            }
        }

        #endregion

    }
}
