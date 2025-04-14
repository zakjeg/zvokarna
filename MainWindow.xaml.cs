using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using static xamlPianoRoll.InstrumentViewModel;
using System.Windows.Media.Imaging;
using NAudio.Wave;
using xamlPianoRoll.View;
using System.Text.Json;

namespace xamlPianoRoll;

public partial class MainWindow : Window
{


    private bool isRightMouseButtonPressed = false; 
    private ToggleButton lastHoveredButton = null;
    private int stNotVTaktu = 3;
    private int stTaktov = 4;

    private int playDelay;


    public MainWindow()
    {
       
        InitializeComponent();

        this.Icon = new BitmapImage(new Uri("pack://application:,,,/images/icons/vinyl_record_white.png"));
        

        playDelay = 300;
        MediumButton.IsChecked = true;  
        BpmLabel.Text = "BPM: 266";

    }

    private void ExportTrack(int delay, int repetitions, bool finishOnLastNote)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "MP3 Files (*.mp3)|*.mp3",
            FileName = "repa.mp3"
        };

        if (dialog.ShowDialog() == true)
        {
            ExportBeatToWavWithMixing(dialog.FileName, delay, instrumentTracks, repetitions,1.0f, finishOnLastNote);
            MessageBox.Show("Izvoz datoteke končan!");
        }
    }


    private void SpeedButton_Checked(object sender, RoutedEventArgs e)
    {
        ToggleButton checkedButton = sender as ToggleButton;
        if (checkedButton != null)
        {
          playDelay = Convert.ToInt32(checkedButton.Tag);
            switch(playDelay)
            {
                case 300:
                    BpmLabel.Text = $"BPM: 200";
                    break;
                case 150:
                    BpmLabel.Text = $"BPM: 400";
                    break;
                case 600:
                    BpmLabel.Text = $"BPM: 100";
                    break;
            }   
        }

        if (checkedButton != SlowButton) SlowButton.IsChecked = false;
        if (checkedButton != MediumButton) MediumButton.IsChecked = false;
        if (checkedButton != FastButton) FastButton.IsChecked = false;
    }

    private bool checkIfPlaying()
    {
        bool isPlaying= false;
        foreach (var instrumentViewModel in instrumentTracks)
        {
            if (instrumentViewModel.IsPlaying)
            {
                isPlaying = true;
                break;
            }
        }
        return isPlaying;
    }
    private bool checkIfSoloPlaying()
    {
        bool isSoloPlaying = false;
        foreach (var instrumentViewModel in instrumentTracks)
        {
            if (instrumentViewModel.IsSoloplaying)
            {
                isSoloPlaying = true;
                break;
            }
        }
        return isSoloPlaying;
    }

    private void settingsButton_click(object sender, RoutedEventArgs e)
    {
        if (checkIfPlaying() || checkIfSoloPlaying()){
            MessageBox.Show("Ustavite predvajanjem pred urejanjem nastavitev projekta.");
            return;
        }
        else {
            popupSettings settingsSelector = new popupSettings(stNotVTaktu, stTaktov);
            bool? result = settingsSelector.ShowDialog();

            if (result == true)
            {
                if (settingsSelector.NastavitveniKlic)
                {
                    int novoStNotVTaktu = settingsSelector.NotesPerBeat;
                    int novoStTaktov = settingsSelector.Beats;

                    spremembaTakta(novoStNotVTaktu, novoStTaktov);
                }
                else
                {
                    ExportTrack(settingsSelector.delay, settingsSelector.RepeatCount, settingsSelector.RepeatLastNote);
                }
            }
        }
    }

    private void spremembaTakta(int novoStNotVTaktu, int novoStTaktov)
    {
        if(stNotVTaktu!=novoStNotVTaktu || stTaktov != novoStTaktov)
        {
            MessageBoxResult result = MessageBox.Show("Ste prepričani da želite spremeniti takt?", "Sprememba takta bo izbrisala sledi vseh inštrumentov.", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                stTaktov = novoStTaktov;
                stNotVTaktu = novoStNotVTaktu;

                TrackListPanel.Children.Clear();
                instrumentTracks.Clear();
                trackToInstrumentMap.Clear();
                instrumentCounts.Clear();
                DataContext = null;
            }
        }
        else
        {
            MessageBox.Show("Ni sprememb!");
        }
        UpdateEmptyStateMessage();

    }
    private void AddSoundtrack_Click(object sender, RoutedEventArgs e)
    {

        if (!checkIfPlaying())
        {
            popupMenu instrumentSelector = new popupMenu();
            instrumentSelector.InstrumentSelected += OnInstrumentSelected;
            instrumentSelector.ShowDialog();
        }
        else
        {
            MessageBox.Show("Ustavite predvajanje pred dodajanjem nove sledi.");
        }
        


    }

    private List<InstrumentViewModel> instrumentTracks = new();

    // SEZNAM INSTRUMENTOV DA JIH LAHKO KASNEJE PREŠTEJEMO IN PREVILNO OŠTEVILČIMO SLEDI
    private Dictionary<string, int> instrumentCounts = new Dictionary<string, int>();
    private Dictionary<Button, InstrumentViewModel> trackToInstrumentMap = new();

    private void OnInstrumentSelected(string instrument)
    {
        SimulateLoadingBar();

        InstrumentViewModel instrumentViewModel = new InstrumentViewModel(instrument, (stNotVTaktu*stTaktov));

        instrumentTracks.Add(instrumentViewModel);

        if (!instrumentCounts.ContainsKey(instrument))
        {
            instrumentCounts[instrument] = 1;
        }
        else
        {
            instrumentCounts[instrument]++;
        }   

        string trackName = $"{instrument} Sled {instrumentCounts[instrument]}";


        Image trackImage = new Image
        {
            Source = new BitmapImage(new Uri($"pack://application:,,,/Images/instruments/{instrument}Icon.png")),
            Width = 24,
            Height = 24,
            Margin = new Thickness(5, 0, 5, 0)
        };

        TextBlock trackText = new TextBlock
        {
            Text = trackName,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center
        };

        StackPanel trackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        trackPanel.Children.Add(trackImage);
        trackPanel.Children.Add(trackText);

        Button trackButton = new Button
        {
            Content = trackPanel,
            Width = 230,
            Height = 40,
            Background = Brushes.Gray,
            Foreground = Brushes.White,
            Margin = new Thickness(5, 5, 5, 5),
            Style = (Style)FindResource("RoundedButtonStyle")
        };


        trackToInstrumentMap[trackButton] = instrumentViewModel;

        trackButton.Click += (s, e) =>
        {
            if (!checkIfSoloPlaying())
            {
                if (trackToInstrumentMap.TryGetValue(trackButton, out InstrumentViewModel vm))
                {
                    DataContext = vm;
                }
            }
            else
            {
                MessageBox.Show("Ustavite predvajanje sledi pred dodajanjem nove sledi.");
            }

        };

        ContextMenu contextMenu = new ContextMenu();
        MenuItem removeItem = new MenuItem { Header = "Remove Track" };

        removeItem.Click += (s, e) =>
        {
            if (!checkIfPlaying())
            {

                TrackListPanel.Children.Remove(trackButton);

                instrumentTracks.Remove(instrumentViewModel);
                trackToInstrumentMap.Remove(trackButton);

                if (DataContext == instrumentViewModel)
                {
                    DataContext = instrumentTracks.FirstOrDefault();
                }

                instrumentCounts[instrument]--;
            }
            else
            {
                MessageBox.Show("Stop playback before removing a track!");
            }

            UpdateEmptyStateMessage();

        };

        contextMenu.Items.Add(removeItem);
        trackButton.ContextMenu = contextMenu;

        TrackListPanel.Children.Add(trackButton);



        DataContext = instrumentViewModel;
        if (!checkIfSoloPlaying())
        {

            foreach (var btn in trackToInstrumentMap.Keys)
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#333333");
            }

            // NADZOR BARVE
            trackButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#82ab80");
        }
        // SPREMENI DATA CONTEXT
        DataContext = instrumentViewModel;

        // NADZOR BARVE GUMBOV PO KLIKU
        trackButton.Click += (s, e) =>
        {
            foreach (var btn in trackToInstrumentMap.Keys)
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#333333");

            }
            trackButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#82ab80");
        };

        UpdateEmptyStateMessage();

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
        isRightMouseButtonPressed = false; 
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



    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        if (!checkIfSoloPlaying())
        {

            if (instrumentTracks.Count > 0)
            {
                var playTasks = new List<Task>();

                foreach (var instrumentViewModel in instrumentTracks)
                {
                    if (!instrumentViewModel.IsPlaying)
                    {
                        PlayButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#82ab80");
                        PlayButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/images/icons/stop_playback.png")),
                            Width = 24,
                            Height = 24
                        };
                        playTasks.Add(instrumentViewModel.PlayToggledKeysInOrder(playDelay));
                    }
                    else
                    {
                        instrumentViewModel.StopPlayback();
                        PlayButton.Background = Brushes.Transparent;
                        PlayButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/images/icons/play_button_green.png")),
                            Width = 24,
                            Height = 24
                        };
                    }
                }

                await Task.WhenAll(playTasks);

                foreach (var instrumentViewModel in instrumentTracks)
                {
                    PlayButton.Background = Brushes.Transparent;
                    PlayButton.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/images/icons/play_button_green.png")),
                        Width = 24,
                        Height = 24
                    };
                }
            }
            else
            {
                MessageBox.Show("V projektu ni nobene sledi za predvajati.");
            }
        }
        else
        {
            MessageBox.Show("Ustavite predvajanje posameznega inštrumenta pred predvajanjeem vseh.");
        }
    }


    private async void playSoloButton_click(object sender, RoutedEventArgs e)
    {
        if (instrumentTracks.Count > 0)
        {
            if (!checkIfPlaying())
            {
                if (sender is Button playSoloButton && playSoloButton.DataContext is InstrumentViewModel instrumentViewModel)
                {
                    if (!instrumentViewModel.IsSoloplaying)
                    {
                        playSoloButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#82ab80");
                        playSoloButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/images/icons/stop_solo_playback.png")),
                            Width = 20,
                            Height = 20
                        };

                        await instrumentViewModel.SoloPlayToggledKeysInOrder(playDelay);

                        playSoloButton.Background = Brushes.Transparent;
                        playSoloButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/images/icons/play_solo_button_green.png")),
                            Width = 24,
                            Height = 24
                        };
                    }
                    else
                    {
                        instrumentViewModel.StopPlayback();
                        playSoloButton.Background = Brushes.Transparent;
                        playSoloButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/images/icons/play_solo_button_green.png")),
                            Width = 24,
                            Height = 24
                        };
                    }
                }
            }
            else
            {
                MessageBox.Show("Ustavite predvajanje pred predvajanjem posameznega inštrumenta");
            }
        }
        else
        {
            MessageBox.Show("V projektu ni nobene sledi za predvajati.");
        }
        
    }

    //izvoz .wav datoteke
    public static void ExportBeatToWavWithMixing(string outputPath, int delayMs, List<InstrumentViewModel> instrumentTracks,int repeat, float tailSilenceSeconds, bool finishOnFirstNote ){
        delayMs = 250;
        int sampleRate = 44100;
        int channels = 2;
        var outputFormat = new WaveFormat(sampleRate, channels);

        // izračuna trajanje posamezne note
        int totalColumns = instrumentTracks.SelectMany(t => t.InstrumentKeys).Max(k => k.Column) + 1;
        int singleIterationDurationMs = totalColumns * delayMs;
        int totalDurationMs = singleIterationDurationMs * repeat;

        // na koncu dodamo sekundo tišine da lahko inštrumenti izzvenijo
        totalDurationMs += (int)(tailSilenceSeconds * 1000);

        // če je uporabnik želi da se prva nota ponovi na koncu zvočnega posnetka, podaljšamo čas
        if (finishOnFirstNote)
        {
            totalDurationMs += delayMs;
        }

        int totalSamples = (int)(totalDurationMs / 1000.0 * sampleRate) * channels;
        float[] mixedSamples = new float[totalSamples];

        // Najde prvo noto (najmanjša številka stolpca) iz vseh sledi
        InstrumentNote firstNote = null;
        if (finishOnFirstNote)
        {
            firstNote = instrumentTracks
                .SelectMany(t => t.InstrumentKeys)
                .Where(k => k.IsToggled)
                .OrderBy(k => k.Column)
                .FirstOrDefault();
        }

        for (int iteration = 0; iteration < repeat; iteration++)
        {
            int iterationOffsetMs = iteration * singleIterationDurationMs;

            foreach (var track in instrumentTracks)
            {
                foreach (var key in track.InstrumentKeys.Where(k => k.IsToggled))
                {
                    string path = track.generatePath(key.NoteNumber);
                    AudioFileReader audioFile = null;
                    MediaFoundationResampler resampler = null;

                    try
                    {
                        audioFile = new AudioFileReader(path);
                        ISampleProvider sampleProvider = audioFile;

                        if (audioFile.WaveFormat.SampleRate != sampleRate || audioFile.WaveFormat.Channels != channels)
                        {
                            resampler = new MediaFoundationResampler(audioFile, outputFormat);
                            sampleProvider = resampler.ToSampleProvider();
                            audioFile = null;                         }

                        // Prebere vse zvočne podatke
                        var keySamples = new List<float>();
                        float[] buffer = new float[4096];
                        int samplesRead;
                        while ((samplesRead = sampleProvider.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            for (int i = 0; i < samplesRead; i++)
                            {
                                keySamples.Add(buffer[i]);
                            }
                        }

                        int startTimeMs = key.Column * delayMs + iterationOffsetMs;
                        int startSample = (int)(startTimeMs / 1000.0 * sampleRate) * channels;

                        for (int i = 0; i < keySamples.Count; i++)
                        {
                            int pos = startSample + i;
                            if (pos >= mixedSamples.Length) break;
                            mixedSamples[pos] += keySamples[i];
                        }
                    }
                    finally
                    {
                        resampler?.Dispose();
                        audioFile?.Dispose();
                    }
                }
            }
        }

        // if pogoj če želi uporabnik da se zadnja note ponovi na koncu zvočnega posnetka
        if (finishOnFirstNote && firstNote != null)
        {
            var track = instrumentTracks.FirstOrDefault(t => t.InstrumentKeys.Contains(firstNote));
            if (track != null)
            {
                string path = track.generatePath(firstNote.NoteNumber);
                AudioFileReader audioFile = null;
                MediaFoundationResampler resampler = null;

                try
                {
                    audioFile = new AudioFileReader(path);
                    ISampleProvider sampleProvider = audioFile;

                    if (audioFile.WaveFormat.SampleRate != sampleRate || audioFile.WaveFormat.Channels != channels)
                    {
                        resampler = new MediaFoundationResampler(audioFile, outputFormat);
                        sampleProvider = resampler.ToSampleProvider();
                        audioFile = null; 
                    }

                    // Prebere vse zvočne podatke
                    var keySamples = new List<float>();
                    float[] buffer = new float[4096];
                    int samplesRead;
                    while ((samplesRead = sampleProvider.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < samplesRead; i++)
                        {
                            keySamples.Add(buffer[i]);
                        }
                    }

                    int startTimeMs = (totalColumns * repeat) * delayMs;
                    int startSample = (int)(startTimeMs / 1000.0 * sampleRate) * channels;

                    // meša v glavni pomnilnik
                    for (int i = 0; i < keySamples.Count; i++)
                    {
                        int pos = startSample + i;
                        if (pos >= mixedSamples.Length) break;
                        mixedSamples[pos] += keySamples[i];
                    }
                }
                finally
                {
                    resampler?.Dispose();
                    audioFile?.Dispose();
                }
            }
        }

        float maxPeak = mixedSamples.Max(x => Math.Abs(x));
        float gain = maxPeak > 0.01f ? 0.99f / maxPeak : 1.0f;

        for (int i = 0; i < mixedSamples.Length; i++)
        {
            mixedSamples[i] *= gain;
        }

        // zapiše v .wav datoteko
        using (var writer = new WaveFileWriter(outputPath, outputFormat))
        {
            writer.WriteSamples(mixedSamples, 0, mixedSamples.Length);
        }
    }


    //"počisti sled" gumb
    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("Ste prepričani da želite počistiti izbiro?", "Po izbrisu povrnitev izbire ni možna.", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        if (result == MessageBoxResult.OK)
        {
            if (DataContext is InstrumentViewModel viewModel)
            {
                viewModel.ClearAllKeys();
            }
        }
    }

    // Poskrbi da je sprememba stanja note slišana, če je ta bila "vklopljena"
    private async void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggleButton && toggleButton.DataContext is InstrumentNote note)
        {
            if (note.IsToggled)
            {
                await ((InstrumentViewModel)DataContext).PlayNoteAsync(note);
            }
        }
    }

    //osnovne funkcije okna
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
    public async Task SimulateLoadingBar()
    {
        LoadingBar.Visibility = Visibility.Visible;
        LoadingBar.Value = 0;

        for (int i = 0; i <= 100; i++)
        {
            LoadingBar.Value = i;
            await Task.Delay(10); 
        }

        LoadingBar.Visibility = Visibility.Collapsed;
    }



    public void SaveSoundtrack(SoundtrackData soundtrack)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "REPA Files (*.repa)|*.repa",
            FileName = "moja.repa"
        };

        if (dialog.ShowDialog() == true)
        {
            string filePath = dialog.FileName;
            string json = JsonSerializer.Serialize(soundtrack);
            File.WriteAllText(filePath, json);
            MessageBox.Show("Projekt uspešno shranjen!");
        }
    }

    public SoundtrackData LoadSoundtrack()
    {
        if(checkIfPlaying() || checkIfSoloPlaying())
        {
            MessageBox.Show("Ustavite predvajanje pred nalaganjem novega projekta!");
            return null;
        }
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "REPA Files (*.repa)|*.repa"
        };

        if (dialog.ShowDialog() == true)
        {
            string filePath = dialog.FileName;
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SoundtrackData>(json);
        }

        return null;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if(instrumentTracks.Count == 0)
        {
            MessageBox.Show("Ni sledi za shranjevanje.");
            return;
        }

        if(checkIfPlaying() || checkIfSoloPlaying())
        {
            MessageBox.Show("Ustavite predvajanje pred shranjevanjem novega projekta!");
            return;
        }
        SoundtrackData soundtrack = new SoundtrackData
        {
            Bpm = 200,  
            NotesPerBeat = stNotVTaktu,
            Beats = stTaktov
        };

        foreach (var instrumentViewModel in instrumentTracks)
        {
            var instrumentData = new InstrumentData
            {
                InstrumentName = instrumentViewModel.Instrument,
                Notes = instrumentViewModel.InstrumentKeys
                    .Where(k => k.IsToggled)
                    .Select(k => new NoteData
                    {
                        NoteNumber = k.NoteNumber,
                        Column = k.Column,
                        IsToggled = k.IsToggled
                    })
                    .ToList()
            };

            soundtrack.Instruments.Add(instrumentData);
        }

        SaveSoundtrack(soundtrack);
    }


    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        if (checkIfPlaying() || checkIfSoloPlaying())
        {
            MessageBox.Show("Ustavite predvajanje pred nalaganjem novega projekta!");
            return;
        }if(instrumentTracks.Count > 0)
        {
            MessageBoxResult result = MessageBox.Show("Nalaganje novega projekta bo počistilo vse trenutne sledi. Želite nadaljevati?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.OK)
            {
                return;
            }
        }



        SoundtrackData soundtrack = LoadSoundtrack();
        if (soundtrack != null)
        {

            spremembaTaktaPriNalaganju(soundtrack.NotesPerBeat, soundtrack.Beats);

            TrackListPanel.Children.Clear();
            instrumentTracks.Clear();
            trackToInstrumentMap.Clear();
            instrumentCounts.Clear();
            DataContext = null;

            foreach (var instrumentData in soundtrack.Instruments)
            {
                OnInstrumentSelected(instrumentData.InstrumentName);
                var instrumentViewModel = instrumentTracks.Last();

                foreach (var noteData in instrumentData.Notes)
                {
                    var note = instrumentViewModel.InstrumentKeys.FirstOrDefault(k => k.NoteNumber == noteData.NoteNumber && k.Column == noteData.Column);
                    if (note != null)
                    {
                        note.IsToggled = noteData.IsToggled;
                    }
                }
            }

            BpmLabel.Text = $"BPM: {soundtrack.Bpm}";
            stNotVTaktu = soundtrack.NotesPerBeat;
            stTaktov = soundtrack.Beats;
        }

        UpdateEmptyStateMessage();

    }

    private void spremembaTaktaPriNalaganju(int novoStNotVTaktu, int novoStTaktov)
    {
        stTaktov = novoStTaktov;
        stNotVTaktu = novoStNotVTaktu;

        TrackListPanel.Children.Clear();
        instrumentTracks.Clear();
        trackToInstrumentMap.Clear();
        instrumentCounts.Clear();
        DataContext = null;

    }

    private void UpdateEmptyStateMessage()
    {
        EmptyStateText.Visibility = instrumentTracks.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }


    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Key == Key.T)
            {
                AddSoundtrack_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.N)
            {
                settingsButton_click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.S)
            {
                SaveButton_Click(null, null);
                e.Handled = true;
            }
        }

        base.OnKeyDown(e);
    }



}