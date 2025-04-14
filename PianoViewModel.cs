using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using xamlPianoRoll.Audio;
using System.Diagnostics;


namespace xamlPianoRoll
{

    //instrumentViewModel razred
    public class InstrumentViewModel
    {

        private Dictionary<string, CachedSound> _soundCache = new Dictionary<string, CachedSound>();
        public ObservableCollection<InstrumentNote> SampleButtons { get; set; } = new ObservableCollection<InstrumentNote>();

        private CancellationTokenSource _cancellationTokenSource;
        public ObservableCollection<InstrumentNote> InstrumentKeys { get; set; }

        
        public string[] noteFilePaths;
        public int Rows { get; set; } = 24;
        public int Columns { get; set; } = 36;
        public string basedir;
        public string Instrument;

        public bool IsSoloplaying { get; private set; } = false;
        public bool IsPlaying { get; private set; } = false;


        public ICommand PlaySampleNoteCommand { get; set; }
        public ICommand PlayNoteCommand { get; set; }



        //konstruktor razreda nastavi vrsto inštrumenta (ime ter poti za zvočne datoteke inštrumenta).
        public InstrumentViewModel(string Instrument, int Columns)
        {

            PlaySampleNoteCommand = new RelayCommand(async (parameter) =>
            {
                if (parameter is InstrumentNote note)
                {
                    await PlaySampleNoteAsync(note);
                }
            });

            if (Instrument == "Marimba")
            {
                this.Instrument = "Marimba";
                this.noteFilePaths = new string[]{
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

                this.Rows = 24;
                this.Columns = Columns;

                this.basedir = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "sounds", "marimba") + Path.DirectorySeparatorChar;

                foreach (var noteFile in noteFilePaths)
                {
                    var fullPath = Path.Combine(basedir, noteFile);
                    _soundCache[fullPath] = new CachedSound(fullPath);
                }
            }
            else if (Instrument == "Klavir")
            {
                this.Instrument = "Klavir";
                this.noteFilePaths = new string[] {
                    "key01.wav", "key02.wav", "key03.wav","key04.wav", "key05.wav", "key06.wav", "key07.wav", "key08.wav", "key09.wav", "key10.wav", "key11.wav", "key12.wav","key13.wav", "key14.wav", "key15.wav","key16.wav", "key17.wav", "key18.wav", "key19.wav", "key20.wav", "key21.wav", "key22.wav", "key23.wav", "key24.wav",
                };


                this.Columns = Columns;
                this.Rows = 24;

                this.basedir = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "sounds", "piano_keys_wav") + Path.DirectorySeparatorChar;

                foreach (var noteFile in noteFilePaths)
                {
                    var fullPath = Path.Combine(basedir, noteFile);
                    _soundCache[fullPath] = new CachedSound(fullPath);
                }

            }
            else if (Instrument == "Bobni")
            {
                this.Instrument = "Bobni";
                this.noteFilePaths = new string[] {
                    "kick-heavy.wav","kick-808.wav",  "kick-tight.wav","kick-acoustic01.wav",   
                    "snare-acoustic01.wav","snare-analog.wav","clap-808.wav","snare-lofi01.wav",
                    "tom-acoustic01.wav", "tom-acoustic02.wav", "tom-rototom.wav",
                    "hihat-acoustic01.wav","hihat-acoustic02.wav", "openhat-acoustic01.wav",
                    "crash-acoustic.wav", "ride-acoustic01.wav", 
                    "cowbell-808.wav", "shaker-analog.wav",   "perc-tribal.wav"  
                };

                this.Columns = Columns;
                this.Rows = this.noteFilePaths.Length;

                this.basedir = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "sounds", "drums") + Path.DirectorySeparatorChar;

                foreach (var noteFile in noteFilePaths)
                {
                    var fullPath = Path.Combine(basedir, noteFile);
                    _soundCache[fullPath] = new CachedSound(fullPath);
                }
            }
            else if (Instrument == "AkusticniBobni")
            {
                this.Instrument = "AkusticniBobni";
                this.noteFilePaths = new string[] {
                    "kick-808.wav", "kick-electro01.wav", "kick-tron.wav", "kick-newwave.wav",
                    "snare-analog.wav", "snare-electro.wav", "clap-analog.wav", "snare-dist01.wav",
                    "tom-808.wav", "tom-analog.wav", "tom-fm.wav",
                    "hihat-808.wav", "hihat-digital.wav", "openhat-analog.wav",
                    "crash-808.wav", "crash-noise.wav",
                    "cowbell-808.wav", "perc-laser.wav", "perc-808.wav"
                };

                this.Columns = Columns;
                this.Rows = this.noteFilePaths.Length;

                this.basedir = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "sounds", "drums") + Path.DirectorySeparatorChar;

                foreach (var noteFile in noteFilePaths)
                {
                    var fullPath = Path.Combine(basedir, noteFile);
                    _soundCache[fullPath] = new CachedSound(fullPath);
                }
            }


            InstrumentKeys = new ObservableCollection<InstrumentNote>();


            // Ustvarnjanje seznama za probne tipke

            for (int row = 0; row < Rows; row++)
            {
                string baseDisplayName;
                bool isBlack = false;

                if (Instrument == "Bobni" || Instrument == "AkusticniBobni")
                {
                    string fullName = Path.GetFileNameWithoutExtension(noteFilePaths[(noteFilePaths.Length) - row - 1]);
                    int hyphenIndex = fullName.IndexOf('-');
                    int spaceIndex = fullName.IndexOf(' ');

                    int separatorIndex = (hyphenIndex == -1) ? spaceIndex :
                                        (spaceIndex == -1) ? hyphenIndex :
                                        Math.Min(hyphenIndex, spaceIndex);

                    baseDisplayName = separatorIndex == -1 ? fullName : fullName.Substring(0, separatorIndex);
                }
                else
                {
                    baseDisplayName = GetNoteName(row);
                    isBlack = IsBlackKey(baseDisplayName);
                }

                SampleButtons.Add(new InstrumentNote
                {
                    NoteNumber = Rows - row - 1,
                    DisplayName = baseDisplayName,
                    Column = 0,
                    Row = Rows - row - 1,
                    ButtonColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#82ab80")
                });

                // Ustvarnjanje seznama za tipke
                for (int col = 0; col < Columns; col++)
                {
                    InstrumentKeys.Add(new InstrumentNote
                    {
                        NoteNumber = Rows - row - 1,
                        //DisplayName = displayName,
                        IsBlackKey = isBlack,
                        Column = col,
                        Row = Rows - row - 1,
                        ButtonColor = isBlack ? Brushes.DarkGray : Brushes.White
                    });
                }
            }





            //Povezava med tipkami in ukazi
            PlayNoteCommand = new RelayCommand(async (param) => await PlayNoteAsync(param));
        }

        public void ClearAllKeys()
        {
            foreach (var pianoNote in InstrumentKeys)
            {
                pianoNote.IsToggled = false;
            }

        }


        public string generatePath(int noteNumber)
        {
            if (Instrument == "Marimba")
            {
                return Path.Combine(basedir, noteFilePaths[20 + noteNumber]);

            }
            string fullPath = Path.Combine(basedir, noteFilePaths[noteNumber]);
            return fullPath;


        }


        //razred za posamezne note
        public class InstrumentNote : INotifyPropertyChanged
        {

            public int NoteNumber { get; set; }
            public string DisplayName { get; set; }
            public bool IsBlackKey { get; set; }
            public int Column { get; set; } 
            public int Row { get; set; }    //(0 = najvisja nota, 23 = najnizja)

            private bool _isToggled;

            private Brush _buttonColor = Brushes.White; 

            public bool IsToggled
            {
                get => _isToggled;
                set
                {
                    _isToggled = value;
                    OnPropertyChanged(nameof(IsToggled));
                    OnPropertyChanged(nameof(DisplayColor)); 
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




        //metode instrumentViewModel razreda
        
        //predvajanje zvočne sledi

        public async Task PlayToggledKeysInOrder(int delay)
        {
            var notesToPlay = InstrumentKeys.Where(k => k.IsToggled).ToList();
            if (notesToPlay.Count == 0)
                return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            IsPlaying = true;

            int maxColumns = InstrumentKeys.Max(k => k.Column);

            while (IsPlaying && !token.IsCancellationRequested)
            {
                for (int column = 0; column <= maxColumns; column++)
                {
                    if (!IsPlaying || token.IsCancellationRequested)
                        return;

                    var keysInColumn = InstrumentKeys.Where(k => k.Column == column && k.IsToggled).ToList();

                    var tasks = keysInColumn.Select(key => Task.Run(() =>
                    {
                        var path = generatePath(key.NoteNumber);
                        if (_soundCache.TryGetValue(path, out var cachedSound))
                        {
                            AudioPlaybackEngine.Instance.PlaySound(cachedSound);
                        }
                        else
                        {
                            AudioPlaybackEngine.Instance.PlaySound(path);
                        }
                    }));

                    await Task.WhenAll(tasks);

                    Debug.WriteLine($"igra ton -  {DateTime.Now:HH:mm:ss.fff}, Delay: {delay}");
                    await Task.Delay(delay);

                }
            }

            IsPlaying = false;
            IsSoloplaying = false;
        }

        public async Task SoloPlayToggledKeysInOrder(int delay)
        {

            var notesToPlay = InstrumentKeys.Where(k => k.IsToggled).ToList();
            if (notesToPlay.Count == 0)
                return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            IsSoloplaying = true;
            IsPlaying = false;

            int maxColumns = InstrumentKeys.Max(k => k.Column);

            while (IsSoloplaying && !token.IsCancellationRequested)
            {
                for (int column = 0; column <= maxColumns; column++)
                {
                    if (!IsSoloplaying || token.IsCancellationRequested)
                        return; // prekinitev ponavljanja

                    var keysInColumn = InstrumentKeys.Where(k => k.Column == column && k.IsToggled).ToList();

                    var tasks = keysInColumn.Select(key => Task.Run(() =>
                    {
                        var path = generatePath(key.NoteNumber);
                        if (_soundCache.TryGetValue(path, out var cachedSound))
                        {
                            AudioPlaybackEngine.Instance.PlaySound(cachedSound);
                        }
                        else
                        {
                            AudioPlaybackEngine.Instance.PlaySound(path);
                        }
                    }));

                    await Task.WhenAll(tasks);
                    await Task.Delay(delay);
                }
            }
            IsSoloplaying = false;
            IsPlaying = false;
        }


        public void StopPlayback()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource(); // ponastavi token
            IsPlaying = false;
            IsSoloplaying = false;


        }



        private string GetNoteName(int row)
        {
            string[] noteNames = { "c", "c#", "d", "d#", "e", "f", "f#", "g", "g#", "a", "a#", "b" };
            return noteNames[11-(row % 12)];
        }


        //Preveri če je črna tipka
        private bool IsBlackKey(string noteName)
        {
            return noteName.Contains("#");
        }


        //IGRA NOTE "ON TOGGLE"
        public async Task PlaySampleNoteAsync(object parameter)
        {
            if (parameter is InstrumentNote note)
            {
                if (note != null)
                {
                    var fullPath = generatePath(note.NoteNumber);

                    if (_soundCache.TryGetValue(fullPath, out var cachedSound))
                    {
                        AudioPlaybackEngine.Instance.PlaySound(cachedSound);
                    }
                }
                await Task.CompletedTask;

            }
        }

        public async Task PlayNoteAsync(object parameter)
        {
            if (parameter is InstrumentNote note)
            {
                if (note.IsToggled)
                {
                    var fullPath = generatePath(note.NoteNumber);   

                    //AudioPlaybackEngine.Instance.PlaySound(fullPath);
                    //await Task.CompletedTask;

                    if (_soundCache.TryGetValue(fullPath, out var cachedSound))
                    {
                        AudioPlaybackEngine.Instance.PlaySound(cachedSound);
                    }
                    await Task.CompletedTask;
                }
            }
        }



        //EVENT HANDLERI
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }




}