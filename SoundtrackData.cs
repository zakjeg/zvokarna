using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xamlPianoRoll
{
    public class SoundtrackData
    {
        public List<InstrumentData> Instruments { get; set; } = new List<InstrumentData>();
        public int Bpm { get; set; }
        public int NotesPerBeat { get; set; }
        public int Beats { get; set; }
    }

    public class InstrumentData
    {
        public string InstrumentName { get; set; }
        public List<NoteData> Notes { get; set; } = new List<NoteData>();
    }

    public class NoteData
    {
        public int NoteNumber { get; set; }
        public int Column { get; set; }
        public bool IsToggled { get; set; }
    }
}
