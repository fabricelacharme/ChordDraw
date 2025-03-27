using System;

namespace Chorderator
{

    public enum Accidental
    {
        Natural,
        Sharp,
        Flat,
        Unknown
    }

    /// <summary>
    /// Represents a note.
    /// </summary>
    public class Note
    {
        /// <summary>
		/// A number from 0 to 11, where A is 0 and G# is 11.
		/// </summary>
		private int noteNum;
        private Accidental _accidentalType = Accidental.Natural;
        private string _description = "";

        public Note(string noteNameIn)
        {
            this.noteNum = ChordParser.NoteNameToNum(noteNameIn, null);
            this._accidentalType = ChordParser.NoteNameToAccidental(noteNameIn);
        }

        public Note(string noteNameIn, string sharpFlatIn, string description)
        {
            this.noteNum = ChordParser.NoteNameToNum(noteNameIn, sharpFlatIn);
            if ("#+".IndexOf(sharpFlatIn) != -1)
            {
                _accidentalType = Accidental.Sharp;
            }
            else if ("b-".IndexOf(sharpFlatIn) != -1)
            {
                _accidentalType = Accidental.Flat;
            }
            else
            {
                _accidentalType = Accidental.Natural;
            }
            _description = description;
        }

        public Note(int relativeNoteNumIn, int rootNoteNumIn, Accidental accidentalType, string description)
        {
            this.noteNum = (relativeNoteNumIn + rootNoteNumIn) % 12;
            _accidentalType = accidentalType;
            _description = description;
        }

        public Note(int noteNumIn, Accidental accidentalType)
        {
            this.noteNum = noteNumIn;
            _accidentalType = accidentalType;
        }

        public Note()
        {
        }

        public static Note CreateFromStringAndFret(int stringIndex, int fretIndex)
        {
            return new Note((ChordParser.stringNoteNums[stringIndex] + fretIndex) % 12, Accidental.Unknown);
        }

        public int GetFretNum(int stringIndex)
        {
            return (12 + this.noteNum - ChordParser.stringNoteNums[stringIndex]) % 12;
        }

        public override bool Equals(object obj)
        {
            return ((Note)obj).noteNum == this.noteNum;
        }

        public override int GetHashCode()
        {
            return this.noteNum;
        }


        public int GetRelativeNoteNum(int rootNoteNum)
        {
            return (this.noteNum - rootNoteNum + 12) % 12;
        }

        public void SetRelativeNoteNum(int rootNoteNumIn, int relativeNoteNumIn)
        {
            this.noteNum = (relativeNoteNumIn - rootNoteNumIn + 12) % 12;
        }

        public int NoteNum
        {
            get
            {
                return this.noteNum;
            }
            set
            {
                this.noteNum = value;
            }
        }

        public string NoteName
        {
            get
            {
                return ChordParser.NoteNumToName(noteNum, _accidentalType);
            }
            set
            {
                this.noteNum = ChordParser.NoteNameToNum(value);
                this._accidentalType = ChordParser.NoteNameToAccidental(value);
            }
        }

        public override string ToString()
        {
            return NoteName;
        }

        public string ToLongString()
        {
            return String.Format("{0} - {1}", NoteName, _description);
        }

        /// <summary>
        /// Whether this should be represented as a sharp or a flat.
        /// </summary>
        public Accidental AccidentalType
        {
            get
            {
                return _accidentalType;
            }
            set
            {
                _accidentalType = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


    }
}
