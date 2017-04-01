using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Diagnostics; //debug

namespace SerialBandit
{
    /// <summary>
    /// Interaction logic for wnd_com.xaml
    /// </summary>
    public partial class wnd_com : Window
    {
        public wnd_com()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {//window was loaded
            Enumerate_COM_Ports(); //attempt to list all the serial ports on the local machine
            if (WindowMain.port_baud == "19200") //stupid way of setting the drop down... tired of gui coding!
            {
                this.cmb_baudRate.SelectedIndex = 1;
            }
        }
                
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {//user has clicked the save button
            WindowMain.port_name = this.cmb_comports.Text;
            WindowMain.port_baud = this.cmb_baudRate.Text;
            Close_COM_Window(true);
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {//user has clicked the cancel button
            Close_COM_Window(false);
        }

        private void Enumerate_COM_Ports()
        {//this will attempt to list all the serial ports on the computer and add them to the combobox 
            try
            {
                Debug.WriteLine(this.cmb_comports.Items.Count.ToString());
                foreach (string tmpPort_name in SerialPort.GetPortNames()) //this will try to list the 
                {
                    this.cmb_comports.Items.Add(TrimEndToNumber(tmpPort_name)); //add the serial port name
                }
                if (this.cmb_comports.Items.Count > 0)
                {//there were serial ports found
                    this.cmb_comports.SelectedIndex = 0; //0 based index and there is at least one item...
                }
                else
                {//no serial ports found -- raise an ex
                    Exception noCOM = new Exception("Unable to locate any serial ports on localhost (" 
                        + System.Environment.MachineName + ")");
                    throw noCOM;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Err: " + ex.Message, "Serial Port Enumeration Failure!", MessageBoxButton.OK, MessageBoxImage.Error);
                Close_COM_Window(false); //close this window and head back to the main window
            }
        }

        //function and use of function added from patch 13687 by slaumets
        private string TrimEndToNumber(string input)
        {
            char[] letters = input.ToCharArray();

            for (int i = letters.Length - 1; i > 0; i--)
            {
                if (!char.IsDigit(letters[i]))
                {
                    input = input.Remove(i);
                }
                else
                {
                    break;
                }
            }
            return input;
        }


        private void Close_COM_Window(bool isOK)
        {//close the COM port config window
            this.DialogResult = isOK;
            this.Close();
        }
      
      


    }
}
