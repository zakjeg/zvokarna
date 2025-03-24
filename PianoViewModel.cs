using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;


namespace xamlPianoRoll
{
    
	class PianoViewModel 
    {
        public bool IsPlaying { get; private set; } = false;

        private int _bpm = 120; 
        public int BPM
        {
            get => _bpm;
            set
            {
                if (value < 30) value = 30;
                if (value > 240) value = 240;
                _bpm = value;
                OnPropertyChanged(nameof(BPM));
                OnPropertyChanged(nameof(DelayMilliseconds)); 
            }
        }

        public int DelayMilliseconds => 60000 / BPM; 

        private int _currentInstrument=1;

        public int CurrentInstrument
        {
            get => _currentInstrument;
            set
            {
                if (_currentInstrument != value)
                {
                    _currentInstrument = value;
                    OnPropertyChanged(nameof(CurrentInstrument));
                }
            }
        }

        public void ClearAllKeys()
        {
            foreach(var pianoNote in PianoKeys)
            {
                pianoNote.IsToggled = false;
            }
            
        }
        public string generatePath(int noteNumber)
        {
            if (CurrentInstrument == 1)
            {
                var basedir = @"B:\xamlPianoRoll\xamlPianoRoll\sounds\marimba\";

                string[] marimbaNotes = {
                    "marimba-yarn-mf-c2.flac", "marimba-yarn-mf-c#2.flac", "marimba-yarn-mf-d2.flac", "marimba-yarn-mf-d#2.flac",
                    "marimba-yarn-mf-e2.flac", "marimba-yarn-mf-f2.flac", "marimba-yarn-mf-f#2.flac", "marimba-yarn-mf-g2.flac",
                    "marimba-yarn-mf-g#2.flac", "marimba-yarn-mf-a2.flac", "marimba-yarn-mf-a#2.flac", "marimba-yarn-mf-b2.flac",
                    "marimba-yarn-mf-c3.flac", "marimba-yarn-mf-c#3.flac", "marimba-yarn-mf-d3.flac", "marimba-yarn-mf-d#3.flac",
                    "marimba-yarn-mf-e3.flac", "marimba-yarn-mf-f3.flac", "marimba-yarn-mf-f#3.flac", "marimba-yarn-mf-g3.flac",
                    "marimba-yarn-mf-g#3.flac", "marimba-yarn-mf-a3.flac", "marimba-yarn-mf-a#3.flac", "marimba-yarn-mf-b3.flac",
                    "marimba-yarn-mf-c4.flac", "marimba-yarn-mf-c#4.flac", "marimba-yarn-mf-d4.flac", "marimba-yarn-mf-d#4.flac",
                    "marimba-yarn-mf-e4.flac", "marimba-yarn-mf-f4.flac", "marimba-yarn-mf-f#4.flac", "marimba-yarn-mf-g4.flac",
                    "marimba-yarn-mf-g#4.flac", "marimba-yarn-mf-a4.flac", "marimba-yarn-mf-a#4.flac", "marimba-yarn-mf-b4.flac",
                    "marimba-yarn-mf-c5.flac", "marimba-yarn-mf-c#5.flac", "marimba-yarn-mf-d5.flac", "marimba-yarn-mf-d#5.flac",
                    "marimba-yarn-mf-e5.flac", "marimba-yarn-mf-f5.flac", "marimba-yarn-mf-f#5.flac", "marimba-yarn-mf-g5.flac",
                    "marimba-yarn-mf-g#5.flac", "marimba-yarn-mf-a5.flac", "marimba-yarn-mf-a#5.flac", "marimba-yarn-mf-b5.flac",
                    "marimba-yarn-mf-c6.flac", "marimba-yarn-mf-c#6.flac", "marimba-yarn-mf-d6.flac", "marimba-yarn-mf-d#6.flac",
                    "marimba-yarn-mf-e6.flac", "marimba-yarn-mf-f6.flac", "marimba-yarn-mf-f#6.flac", "marimba-yarn-mf-g6.flac",
                    "marimba-yarn-mf-g#6.flac", "marimba-yarn-mf-a6.flac", "marimba-yarn-mf-a#6.flac", "marimba-yarn-mf-b6.flac",
                    "marimba-yarn-mf-c7.flac"
                };


                string noteFilePath = Path.Combine(basedir, marimbaNotes[50-noteNumber]);
                return noteFilePath;

            }
            else if (CurrentInstrument == 2)
            {
                var basedir = @"B:\xamlPianoRoll\xamlPianoRoll\";
                var filePath = Path.Combine("sounds", "piano_keys_wav", $"key{noteNumber:00}.wav");
                var fullPath = Path.Combine(basedir, filePath);
                return fullPath;
            }
            else
            {
                return null;
            }
        }

        public class PianoNote : INotifyPropertyChanged
        {
            public int NoteNumber { get; set; }
            public string DisplayName { get; set; }
            public bool IsBlackKey { get; set; }
            public int Column { get; set; } // Position in the piano roll
            public int Row { get; set; }    // Vertical position (0 = highest note, 23 = lowest)
            
            private bool _isToggled;
            private Brush _buttonColor = Brushes.White; // Default color

            public bool IsToggled
            {
                get => _isToggled;
                set
                {
                    _isToggled = value;
                    OnPropertyChanged(nameof(IsToggled));
                    OnPropertyChanged(nameof(DisplayColor)); // Update UI when toggled
                }
            }

            public Brush DisplayColor => IsToggled ? Brushes.LightBlue : (IsBlackKey ? Brushes.DarkGray : Brushes.White);


            public Brush ButtonColor
            {
                get => _buttonColor;
                set
                {
                    if (_buttonColor != value)
                    {
                        _buttonColor = value;
                        OnPropertyChanged(nameof(ButtonColor));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public ObservableCollection<PianoNote> PianoKeys { get; set; }
        public int Rows { get; set; } = 24;   
        public int Columns { get; set; } = 12; 

        public ICommand PlayNoteCommand { get; set; }

        public PianoViewModel()
        {
            PianoKeys = new ObservableCollection<PianoNote>();

            for (int row = 0; row < Rows; row++)
            {
                string noteName = GetNoteName(row);
                bool isBlack = IsBlackKey(noteName);

                for (int col = 0; col < Columns; col++)
                {
                    PianoKeys.Add(new PianoNote
                    {
                        NoteNumber = 24 - row,
                        DisplayName = noteName,
                        IsBlackKey = isBlack,
                        Column = col,
                        Row = Rows - row - 1,
                        ButtonColor = isBlack ? Brushes.DarkGray : Brushes.White
                    });
                }
            }

            PlayNoteCommand = new RelayCommand(async (param) => await PlayNoteAsync(param));
        }

        public async Task ChangeKeyColorsInOrder()
        {
            Random random = new Random();

            for (int i = 0; i < PianoKeys.Count; i++)
            {
                PianoKeys[i].ButtonColor = new SolidColorBrush(Color.FromRgb(
                    (byte)random.Next(256),
                    (byte)random.Next(256),
                    (byte)random.Next(256))); 

                await Task.Delay(500);
            }
        }

        private CancellationTokenSource _cancellationTokenSource;

        public async Task PlayToggledKeysInOrder()
        {
            var notesToPlay = PianoKeys.Where(k => k.IsToggled).ToList();
            if (notesToPlay.Count == 0)
                return;

            _cancellationTokenSource?.Cancel();  // Cancel any previous playback
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            IsPlaying = true;

            int maxColumns = PianoKeys.Max(k => k.Column);

            while (IsPlaying && !token.IsCancellationRequested)
            {
                for (int column = 0; column <= maxColumns; column++)
                {
                    if (!IsPlaying || token.IsCancellationRequested)  // Stop if playback is canceled
                        return;

                    var keysInColumn = PianoKeys.Where(k => k.Column == column && k.IsToggled).ToList();

                    var tasks = keysInColumn.Select(key => Task.Run(() => AudioPlaybackEngine.Instance.PlaySound(generatePath(key.NoteNumber))));

                    await Task.WhenAll(tasks);
                    await Task.Delay(300);
                }
            }

            IsPlaying = false; 
        }

        public void StopPlayback()
        {
            _cancellationTokenSource?.Cancel();
            IsPlaying = false; 
        }




        private string GetNoteName(int row)
        {
            string[] noteNames = { "c", "c#", "d", "d#", "e", "f", "f#", "g", "g#", "a", "a#", "b" };
            return noteNames[row % 12];
        }

        private bool IsBlackKey(string noteName)
        {
            return noteName.Contains("#");
        }

        public async Task PlayNoteAsync(object parameter)
        {
            if (parameter is PianoNote note)
            {
                if (note.IsToggled)
                {
                var fullPath = generatePath(note.NoteNumber);

                AudioPlaybackEngine.Instance.PlaySound(fullPath);
                await Task.CompletedTask;
                }
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class AudioPlayer
	{
        public static string generatePath(int noteNumber)
        {

            var basedir = @"B:\xamlPianoRoll\xamlPianoRoll\sounds\marimba\";

            string[] marimbaNotes = {
                "marimba-yarn-mf-c2.flac", "marimba-yarn-mf-c#2.flac", "marimba-yarn-mf-d2.flac", "marimba-yarn-mf-d#2.flac",
                "marimba-yarn-mf-e2.flac", "marimba-yarn-mf-f2.flac", "marimba-yarn-mf-f#2.flac", "marimba-yarn-mf-g2.flac",
                "marimba-yarn-mf-g#2.flac", "marimba-yarn-mf-a2.flac", "marimba-yarn-mf-a#2.flac", "marimba-yarn-mf-b2.flac",
                "marimba-yarn-mf-c3.flac", "marimba-yarn-mf-c#3.flac", "marimba-yarn-mf-d3.flac", "marimba-yarn-mf-d#3.flac",
                "marimba-yarn-mf-e3.flac", "marimba-yarn-mf-f3.flac", "marimba-yarn-mf-f#3.flac", "marimba-yarn-mf-g3.flac",
                "marimba-yarn-mf-g#3.flac", "marimba-yarn-mf-a3.flac", "marimba-yarn-mf-a#3.flac", "marimba-yarn-mf-b3.flac",
                "marimba-yarn-mf-c4.flac", "marimba-yarn-mf-c#4.flac", "marimba-yarn-mf-d4.flac", "marimba-yarn-mf-d#4.flac",
                "marimba-yarn-mf-e4.flac", "marimba-yarn-mf-f4.flac", "marimba-yarn-mf-f#4.flac", "marimba-yarn-mf-g4.flac",
                "marimba-yarn-mf-g#4.flac", "marimba-yarn-mf-a4.flac", "marimba-yarn-mf-a#4.flac", "marimba-yarn-mf-b4.flac",
                "marimba-yarn-mf-c5.flac", "marimba-yarn-mf-c#5.flac", "marimba-yarn-mf-d5.flac", "marimba-yarn-mf-d#5.flac",
                "marimba-yarn-mf-e5.flac", "marimba-yarn-mf-f5.flac", "marimba-yarn-mf-f#5.flac", "marimba-yarn-mf-g5.flac",
                "marimba-yarn-mf-g#5.flac", "marimba-yarn-mf-a5.flac", "marimba-yarn-mf-a#5.flac", "marimba-yarn-mf-b5.flac",
                "marimba-yarn-mf-c6.flac", "marimba-yarn-mf-c#6.flac", "marimba-yarn-mf-d6.flac", "marimba-yarn-mf-d#6.flac",
                "marimba-yarn-mf-e6.flac", "marimba-yarn-mf-f6.flac", "marimba-yarn-mf-f#6.flac", "marimba-yarn-mf-g6.flac",
                "marimba-yarn-mf-g#6.flac", "marimba-yarn-mf-a6.flac", "marimba-yarn-mf-a#6.flac", "marimba-yarn-mf-b6.flac",
                "marimba-yarn-mf-c7.flac"
            };


            string noteFilePath = Path.Combine(basedir, marimbaNotes[noteNumber-1]);
            return noteFilePath;


        }
        
		public static async Task PlayNoteAsync(int noteNumber)
		{
 
            var fullPath = generatePath(noteNumber);
            

            await Task.Run(() =>
			{
				using (var audioFile = new AudioFileReader(fullPath))
				using (var outputDevice = new WaveOutEvent())
				{
					outputDevice.Init(audioFile);
					outputDevice.Play();
					while (outputDevice.PlaybackState == PlaybackState.Playing)
					{
					}
				}
			});
		}
	}
    

    
    
}
