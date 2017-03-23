using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using CommandMessenger.TransportLayer;
using CommandMessenger;

namespace ArduinoMotorControl_BSC
{
    enum Command
    {
        setLED,
        rotateAngle, // Command to request led to be set in specific state
    };

    enum Motors
    {
        motorA,
        motorB
    };

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialTransport Arduino;
        private CmdMessenger _cmdMessenger;
        private bool controllerReady = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Connection()
        {
            // Create Serial Port object
            Arduino = new SerialTransport();
            Arduino.CurrentSerialSettings.PortName = Cmb_portName.Text;   // Set com port
            Arduino.CurrentSerialSettings.BaudRate = Convert.ToInt32(Cmb_baudrate.Text);  //Int16 is from -32768 ~32767, not enough

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(Arduino);

            // Tell CmdMessenger if it is communicating with a 16 or 32 bit Arduino board
            _cmdMessenger.BoardType = BoardType.Bit16;

            // Start listening
            _cmdMessenger.StartListening();
        }
        
        public void Exit()
        {
            // Stop listening
            _cmdMessenger.StopListening();

            // Dispose Command Messenger
            _cmdMessenger.Dispose();

            // Dispose Serial Port object
            Arduino.Dispose();
        }

        private void Btn_connect_click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection();
                lbl_status.Content = "Connected!";
            }
            catch (Exception)
            {
                MessageBox.Show("Please give a valid port number or check your connection");
            }
        }

        private void Btn_rotate_Click(object sender, RoutedEventArgs e)
        {
            if (TB_angle.Text != "")
                Rotate((int)Motors.motorA, Convert.ToDouble(TB_angle.Text));
        }

        private void Rotate(int motorIndex, double angle)
        {
            // Create command
            var command = new SendCommand((int)Command.rotateAngle);
            // Send command
            command.AddArgument(motorIndex);
            command.AddArgument(angle);
            _cmdMessenger.SendCommand(command);
        }

        public void ToggleLED(int index, bool signal)
        {
            // Create command
            var command = new SendCommand((int)Command.setLED);
            // Send command
            command.AddArgument(index);
            command.AddArgument(signal);

            _cmdMessenger.SendCommand(command);

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleLED(13, true);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleLED(13, false);
        }
    }
}
