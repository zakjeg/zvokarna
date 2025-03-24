using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using System.Diagnostics;
using static xamlPianoRoll.PianoViewModel;
using System.Windows.Media.Imaging;
using xamlPianoRoll.View;
namespace xamlPianoRoll;
using xamlPianoRoll.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private const int Rows = 24;
	private const int Columns = 12;
	private ToggleButton[,] buttons = new ToggleButton[Rows, Columns];
	private static readonly int[] BlackKeys = { 1, 3, 6, 8, 10 };
	private List<MediaPlayer> activePlayers = new List<MediaPlayer>();
	private bool isRightMouseButtonPressed = false; // To track if the right mouse button is held down
	private ToggleButton lastHoveredButton = null; 
	
	private DispatcherTimer playTimer;
	private int currentColumn = 0;
	private bool isPlaying = false;


	public MainWindow()
	{
		InitializeComponent();
		DataContext = new PianoViewModel();
        this.Icon = new BitmapImage(new Uri("B:\\xamlPianoRoll\\xamlPianoRoll\\images\\ZvokarnaWhiteLogo2.png"));

    }

    private void RandomizeColorsButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PianoViewModel viewModel)
        {
			viewModel.ChangeKeyColorsInOrder();
        }
    }


    private void ToggleButton_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (sender is ToggleButton btn)
		{
			isRightMouseButtonPressed = true;
			ToggleButtonState(btn);
			lastHoveredButton = btn; 
		}
	}

	private void ToggleButton_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		if (isRightMouseButtonPressed && sender is ToggleButton btn && btn != lastHoveredButton)
		{
			lastHoveredButton = btn; 
			ToggleButtonState(btn);  
		}
	}


	private void ToggleButton_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
	{
		isRightMouseButtonPressed = false; // Stop dragging
	}

	private void ToggleButtonState(ToggleButton button)
	{
		if (button.IsChecked == null || button.IsChecked == false)
		{
			button.IsChecked = true;  
		}
		else
		{
			button.IsChecked = false; 
		}

		var command = button.Command;
		var parameter = button.CommandParameter;

		if (command != null && command.CanExecute(parameter))
		{
			command.Execute(parameter);  
		}
	}
	
	private void PianoButton_Click(object sender, RoutedEventArgs e)
	{
		if (DataContext is PianoViewModel viewModel)
		{
			viewModel.CurrentInstrument = 1; 
		}
	}

    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PianoViewModel viewModel)
        {
            if (viewModel.IsPlaying)
            {
                viewModel.StopPlayback();
                PlayButton.Content = "Play";  // Ensure button updates immediately when stopping
            }
            else
            {
                PlayButton.Content = "Stop"; 
                await viewModel.PlayToggledKeysInOrder();
                PlayButton.Content = "Play";  
            }
        }
    }




    private void MarimbaButton_Click(object sender, RoutedEventArgs e)
	{
		if (DataContext is PianoViewModel viewModel)
		{
			viewModel.CurrentInstrument = 2;  
		}
	}
	
	private void ClearButton_Click(object sender, RoutedEventArgs e)
	{
		MessageBoxResult result = MessageBox.Show("Ste prepričani da želite počistiti izbiro?","Po izbrisu povrnitev izbire ni možna.",MessageBoxButton.OKCancel, MessageBoxImage.Question);

		if (result == MessageBoxResult.OK)
		{
			if (DataContext is PianoViewModel viewModel)
			{
				viewModel.ClearAllKeys();
			}
		}
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggleButton && toggleButton.DataContext is PianoNote note)
        {
            if (note.IsToggled) 
            {
                await ((PianoViewModel)DataContext).PlayNoteAsync(note);
            }
        }
    }

    private void AddSoundtrack_Click(object sender, RoutedEventArgs e)
    {
		popupMenu instrumentSelector = new popupMenu();
		instrumentSelector.ShowDialog();
    }

    //OSNOVNE FUNKCIJE OKNA
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
    private void btnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
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
}

