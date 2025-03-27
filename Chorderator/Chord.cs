using System.Collections;

namespace Chorderator
{
    /// <summary>
	/// Represents a chord (note: not a chord fingering)
	/// Contains an ArrayList of Notes.
	/// </summary>
	public class Chord
    {
        ArrayList notes = new ArrayList();
        ArrayList requiredNotes = new ArrayList();
        Note rootNote, slashNote = null;

        public Chord(Note rootNoteIn)
        {
            rootNote = rootNoteIn;
        }

        public void AddNote(Note note, bool required)
        {
            addUniqueNote(notes, note);
            if (required)
            {
                addUniqueNote(RequiredNotes, note);
            }
        }

        public Note RootNote
        {
            get
            {
                return rootNote;
            }
            set
            {
                rootNote = value;
            }
        }

        public Note SlashNote
        {
            get { return slashNote; }

            set
            {
                slashNote = value;
                AddNote(slashNote, true);
            }
        }

        /// <summary>
        /// Adds a note.
        /// </summary>
        /// <param name="noteNum"></param>
        /// <param name="required"></param>
        public void AddRelativeNote(int relativeNoteNum, bool required, Accidental accidental, string description)
        {
            Note note = new Note(relativeNoteNum, this.rootNote.NoteNum, accidental, description);
            addUniqueNote(notes, note);
            if (required)
            {
                addUniqueNote(requiredNotes, note);
            }
        }



        /// <summary>
        /// Removes a note.
        /// </summary>
        /// <param name="noteNum"></param>
        /// <returns></returns>
        public bool RemoveRelativeNote(int relativeNoteNum)
        {
            removeNote(notes, relativeNoteNum);
            removeNote(requiredNotes, relativeNoteNum);
            return true;
        }

        public ArrayList Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
            }
        }

        public ArrayList RequiredNotes
        {
            get
            {
                return requiredNotes;
            }
            set
            {
                requiredNotes = value;
            }
        }

        /// <summary>
        /// Adds num to array, only if it's not already there.
        /// </summary>
        private void addUniqueNote(ArrayList array, Note noteIn)
        {
            if (array.IndexOf(noteIn) == -1)
            {
                array.Add(noteIn);
            }
        }

        public bool ContainsNote(Note noteIn)
        {
            return notes.IndexOf(noteIn) != -1;
        }

        public Note GetNoteAtStringAndFret(int stringNum, int fretNum)
        {
            Note note = Note.CreateFromStringAndFret(stringNum, fretNum);
            int i = notes.IndexOf(note);
            return (i != -1) ? (Note)notes[i] : (Note)null;
        }

        private bool removeNote(ArrayList array, int relativeNoteNum)
        {
            foreach (object noteObj in array)
            {
                Note note = (Note)noteObj;

                if (note.GetRelativeNoteNum(rootNote.NoteNum) == relativeNoteNum)
                {
                    array.Remove(noteObj);
                    return true;
                }
            }
            return false;
        }


    }
}
