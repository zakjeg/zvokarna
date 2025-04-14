using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace xamlPianoRoll.View
{

    public partial class popupSettings : Window
    {
        public bool RepeatLastNote { get; private set; } = false;
        public int RepeatCount { get; private set; } = 1;
        public int NotesPerBeat { get; private set; }
        public int Beats { get; private set; }
        public int delay { get; private set; } = 300;
        public bool NastavitveniKlic { get; set; } = false;


        public popupSettings(int initialNotesPerBeat, int initialBeats)
        {

            InitializeComponent();


            sliderNotesPerBeat.Value = initialNotesPerBeat;
            sliderBeats.Value = initialBeats;

            sliderNotesPerBeatValue.Text = sliderNotesPerBeat.Value.ToString("0");
            sliderBeatsValue.Text = sliderBeats.Value.ToString("0");

        }


        private int getDelay(int BPM)
        {
            return (int)(60000 / BPM);
        }

        private void nastavi_Click(object sender, RoutedEventArgs e)
        {
            NastavitveniKlic = true;
            NotesPerBeat = (int)sliderNotesPerBeat.Value;
            Beats = (int)sliderBeats.Value;
            RepeatCount = (int)repeatSlider.Value;
            delay=getDelay((int)BPMSlider.Value);
            if (RepeatLastToneButton.IsChecked == true)
            {
                RepeatLastNote = true;
            }
            else
            {
                RepeatLastNote = false;
            }

            this.DialogResult = true;
        }


        private void export_Click(object sender, RoutedEventArgs e)
        {
            NastavitveniKlic = false;
            NotesPerBeat = (int)sliderNotesPerBeat.Value;
            Beats = (int)sliderBeats.Value;
            RepeatCount = (int)repeatSlider.Value;
            delay = getDelay((int)BPMSlider.Value);
            if (RepeatLastToneButton.IsChecked == true)
            {
                RepeatLastNote = true;
            }
            else
            {
                RepeatLastNote = false;
            }

            this.DialogResult = true;
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender == sliderNotesPerBeat)
            {
                if (sliderNotesPerBeatValue != null)
                {
                    sliderNotesPerBeatValue.Text = sliderNotesPerBeat.Value.ToString("0");
                }
            }
            else if (sender == sliderBeats)
            {
                if (sliderBeatsValue != null)
                {
                    sliderBeatsValue.Text = sliderBeats.Value.ToString("0");
                }
            }
            else if(sender == repeatSlider)
            {
                if(repeatSliderValue != null)
                {
                    repeatSliderValue.Text = repeatSlider.Value.ToString("0");
                }
            }
            else if (sender == BPMSlider)
            {
                if (BPMSliderValue != null)
                {
                    BPMSliderValue.Text = BPMSlider.Value.ToString("0");
                }
            }
        }


        private void RepeatLastToneButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RepeatLastNote = false;
        }
        private void RepeatLastTone_Checked(object sender, RoutedEventArgs e)
        {
            RepeatLastNote = true;
        }



        //osnovni gumbi okna (zapri, minimiziraj)
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        //gumb za povečanje okna ni uporabljen
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close(); //zapre okno
                     //Application.Current.Shutdown(); //zapre celotno aplicacijo
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
