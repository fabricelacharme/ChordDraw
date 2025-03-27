using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Chorderator
{
    /// <summary>
	/// Summary description for ChordParser.
	/// </summary>
	public class ChordParser
    {
        /// <summary>
        /// String specifying the last error.
        /// </summary>
        public string err;

        /// <summary>
        /// Array of strings for converting notes from numbers to strings.
        /// </summary>
        public static string[] noteList = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };

        /// <summary>
        /// Array of strings for converting notes from numbers to strings, using flats.
        /// </summary>
        public static string[] flatNoteList = { "A", "Bb", "B", "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab" };

        /// <summary>
        /// Starting with the low sixth string, an array of ints, representing note numbers.
        /// Standard tuning:   E A D G B E
        /// </summary>
        public static int[] stringNoteNums = { 7, 0, 5, 10, 2, 7 };

        public static int NumStrings = 6;

        /// <summary>
        /// ArrayList of all chords we have found.
        /// </summary>
        private ArrayList foundChords = null; // of Fingerings
        public ArrayList sortedChords = null, mergedChords = null;

        private Chord chord = null;

        private string rootString, sharpFlat, majorMinor, number, susNumber, addNumber, alteredSharpFlat,
            alteredNumber, slashString, slashSharpFlat;
        private Note rootNote = null;
        private Chorderator.Accidental accidentalType = Accidental.Sharp;

        private static Regex regex = new Regex("(?<root>[A-G]) (?<sharpFlat>[-+#b])? " +
            "(?<majorMinor>(?:Maj)|(?:maj)|(?:mMaj)|m|(?:dim))? " +
            "(?<number>[0-9]*) (?:sus(?<susNumber>[0-9]+))? (?:add(?<addNumber>[0-9]+))? " +
            "(?:(?<alteredSharpFlat>[-+b#])(?<alteredNumber>[0-9]+))? " +
            "(?:/(?<slashNote>[A-G]))? (?<slashSharpFlat>[-+#b])?", RegexOptions.IgnorePatternWhitespace);

        public ChordParser()
        {
        }

        /// <summary>
		/// Tuning is a string like "EADBE".  Capitalization only matters for B, to
		/// distinguish it from the flat symbol.
		/// </summary>
		/// <param name="tuning"></param>
		public static void SetTuning(string tuning, int capo)
        {
            ArrayList noteNums = new ArrayList();

            foreach (char c in tuning.ToCharArray())
            {
                if ("ABCDEFGacdefg".IndexOf(c) != -1)
                {
                    noteNums.Add((NoteNameToNum(c.ToString().ToUpper()) + capo) % 12);
                }
                else if ("b-".IndexOf(c) != -1)
                {
                    // Flat the last note.
                    if (noteNums.Count >= 1)
                    {
                        // Subtract one, wrapping at zero.
                        noteNums[noteNums.Count - 1] = ((int)noteNums[noteNums.Count - 1] + 12 - 1) % 12;
                    }
                }
                else if ("#+".IndexOf(c) != -1)
                {
                    // Sharp the last note.
                    if (noteNums.Count >= 1)
                    {
                        // Add one, wrapping at 12.
                        noteNums[noteNums.Count - 1] = ((int)noteNums[noteNums.Count - 1] + 1) % 12;
                    }
                }
            }
            if (noteNums.Count >= 1)
            {
                stringNoteNums = new int[noteNums.Count];
                noteNums.CopyTo(stringNoteNums);
                NumStrings = stringNoteNums.Length;
            }
        }

        public void parse(string chordString)
        {
            string err = "";

            foundChords = new ArrayList(); // Of fingerings.

            // Capitalize the first letter, since it's always the chord name.
            if (Char.IsLower(chordString, 0))
            {
                chordString = Char.ToUpper(chordString[0]) + chordString.Substring(1);
            }
            // Breake the chord into its consituent parts, as strings, stored in
            // member variables.
            tokenizeChord(chordString);

            // Find the required notes.
            if (err == "")
            {
                err = this.findNotes();
            }
            // Find all possible fingerings.
            if (err == "")
            {
                err = this.findFingerings();
            }
            if (err == "")
            {
                // Sort by difficulty.  The Fingering class is comparable by
                // difficulty, so we can use the built-in Sort method of ArrayList.
                //sortedChords = (ArrayList)foundChords.Clone();
                //sortedChords.Sort();

                // Merge any chords we can.
                this.mergedChords = mergeFingerings(foundChords);
                this.mergedChords.Sort();
            }
        }

        public string GetListing()
        {
            StringBuilder sb = new StringBuilder();

            // Is there no "join" function???
            foreach (Fingering fingering in this.mergedChords)
            {
                sb.Append(fingering);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            if (rootNote == null) return "";
            StringBuilder sb = new StringBuilder(rootString);
            if (sharpFlat != "")
            {
                sb.Append(sharpFlat);
            }
            switch (majorMinor)
            {
                case "m":
                    sb.Append(" minor");
                    break;
                case "dim":
                    sb.Append(" diminished");
                    break;
                case "mMaj":
                    sb.Append(" minor major");
                    break;
                case "maj":
                case "Maj":
                    sb.Append(" major");
                    break;
                default:
                    if (number == "")
                    {
                        sb.Append(" major");
                    }
                    break;
            }
            if (number != "")
            {
                sb.AppendFormat(" {0}th", number);
            }
            if (susNumber != "")
            {
                sb.AppendFormat(" suspended {0}{1}", susNumber, (susNumber == "2") ? "nd" : "th");
            }
            if (addNumber != "")
            {
                sb.AppendFormat(" add {0}", addNumber);
            }
            if (alteredSharpFlat != "")
            {
                switch (alteredSharpFlat)
                {
                    case "+":
                    case "#":
                        sb.Append(" sharp ");
                        break;
                    default:
                        sb.Append(" flat ");
                        break;
                }
                sb.Append(alteredNumber);
            }
            if (slashString != "")
            {
                sb.AppendFormat(" over {0}{1}", slashString, slashSharpFlat);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Break a chord into it's constituent parts.
        /// </summary>
        /// <param name="chordString"></param>
        /// <returns></returns>
        public void tokenizeChord(string chordString)
        {
            Match match = regex.Match(chordString);
            GroupCollection groups = match.Groups;

            if (!match.Success)
            {
                throw new Exception("Could not tokenize chord.");
            }
            // Store the extracted values into member variables.
            rootString = groups["root"].Value;
            sharpFlat = groups["sharpFlat"].Value;
            majorMinor = groups["majorMinor"].Value;
            number = groups["number"].Value;
            susNumber = groups["susNumber"].Value;
            addNumber = groups["addNumber"].Value;
            alteredSharpFlat = groups["alteredSharpFlat"].Value;
            alteredNumber = groups["alteredNumber"].Value;
            slashString = groups["slashNote"].Value;
            slashSharpFlat = groups["slashSharpFlat"].Value;
        }

        // This takes a list of chords and merges any chords it can.
        private ArrayList mergeFingerings(ArrayList /* of Fingerings */ fingList)
        {
            ArrayList newFingList = new ArrayList();
            foreach (Fingering fing1 in fingList)
            {
                ArrayList fingsToMerge = new ArrayList();
                fingsToMerge.Add(fing1);
                foreach (Fingering fing2 in fingList)
                {
                    if (fing1 == fing2) continue; // Don't merge with itself.
                                                  // Only merge chords that have the same root string.
                    if (fing1.RootString != fing2.RootString)
                    {
                        continue;
                    }
                    bool match = true;
                    for (int index = 0; index < NumStrings; index++)
                    {
                        if (fing1.IsPlayed(index) && fing2.IsPlayed(index) &&
                            fing1[index] != fing2[index])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        fingsToMerge.Add(fing2);
                    }
                }

                if (fingsToMerge.Count > 1)
                {
                    // Actually merge the fings.
                    Fingering newFing = new Fingering(this.chord, NumStrings);
                    for (int index = 0; index < NumStrings; index++)
                    {
                        Fret fret = new Fret();
                        fret.num = -1;
                        foreach (Fingering fing in fingsToMerge)
                        {
                            if (fret.num == -1)
                            {
                                fret = fing.GetFret(index);
                            }
                            else if (!fret.played && fing.IsPlayed(index))
                            {
                                fret = fing.GetFret(index);
                                fret.required = false;
                            }
                            else if (fret.played && !fing.IsPlayed(index) && fret.required)
                            {
                                fret.required = false;
                            }
                        }
                        newFing.SetFret(index, fret);
                    }
                    addUniqueFingering(newFingList, newFing);
                }
                else
                {
                    addUniqueFingering(newFingList, fing1);
                }
            }
            return newFingList;
        }

        private void addUniqueFingering(ArrayList fingList, Fingering fingering)
        {
            foreach (Fingering fingeringIt in fingList)
            {
                if (fingeringIt == fingering) return; // Dupe.
            }
            // No dupe found.  Add the new chord.
            fingList.Add(fingering);
        }

        private string findNotes()
        {
            // Find the root note, as a number.
            this.rootNote = new Note(this.rootString, this.sharpFlat, "Root");

            this.accidentalType = Accidental.Sharp;

            if (this.sharpFlat == "b" || this.sharpFlat == "-" ||
                (this.sharpFlat == "" && this.rootString == "F"))
            {
                this.accidentalType = Accidental.Flat;
            }


            this.chord = new Chord(this.rootNote);

            // Add the root to the list of relative note numbers.
            chord.AddRelativeNote(0, true, this.accidentalType, "Root");
            if (this.slashString != "")
            {
                chord.SlashNote = new Note(this.slashString, this.slashSharpFlat, "Slash note");
            }

            // Add the third, if this is not a 5 chord or a sus chord
            if (this.number != "5" && this.susNumber == "")
            {
                if (this.majorMinor == "m" || this.majorMinor == "mMaj" || this.majorMinor == "dim")
                {
                    // Minor third.
                    chord.AddRelativeNote(3, true, Accidental.Flat, "minor 3rd");
                }
                else
                {
                    // Major third.
                    chord.AddRelativeNote(4, true, this.accidentalType, "major 3rd");
                }
            }

            // Process any "sus" numbers
            if (this.susNumber != "")
            {
                chord.AddRelativeNote(halfTonesIn(int.Parse(susNumber)), true, this.accidentalType, "suspended note");
            }

            // Add the fifth
            if (this.majorMinor == "dim")
            {
                // Diminished 5th.
                chord.AddRelativeNote(6, true, Accidental.Flat, "diminished 5th");
            }
            else
            {
                chord.AddRelativeNote(7,
                    // Is the fifth note required?
                    (this.number == "" || int.Parse(this.number) <= 7 ||
                    (this.alteredNumber != "" && int.Parse(this.alteredNumber) == 5)),
                    this.accidentalType, "5th");
            }

            // Process the number, if necessary (as in a seventh or a ninth chord).
            // todo: This could be more efficient, I think.
            if (this.number != "")
            {
                bool need7 = true;
                switch (int.Parse(this.number))
                {
                    case 5:
                        need7 = false;
                        // Nothing to add here.
                        break;
                    case 6:
                        chord.AddRelativeNote(9, true, this.accidentalType, "6th");
                        need7 = false;
                        break;
                    case 9:
                        chord.AddRelativeNote(14, true, this.accidentalType, "9th");
                        break;
                    case 11:
                        chord.AddRelativeNote(14, false, this.accidentalType, "9th");
                        chord.AddRelativeNote(17, true, this.accidentalType, "11th");
                        break;
                    case 13:
                        chord.AddRelativeNote(14, false, this.accidentalType, "9th");
                        chord.AddRelativeNote(17, false, this.accidentalType, "11th");
                        chord.AddRelativeNote(21, true, this.accidentalType, "13th");
                        break;
                }
                if (need7)
                {
                    if (this.majorMinor == "Maj" || this.majorMinor == "mMaj" || this.majorMinor == "maj")
                    {
                        // Major seventh
                        chord.AddRelativeNote(11, true, this.accidentalType, "major 7th");
                    }
                    else if (this.majorMinor == "dim")
                    {
                        // Double-flat seventh
                        chord.AddRelativeNote(9, true, Accidental.Flat, "double-flat 7th");
                    }
                    else
                    {
                        // Diminished seventh
                        chord.AddRelativeNote(10, true, Accidental.Flat, "diminished 7th");
                    }
                }
            }

            // Process any "add" numbers
            if (this.addNumber != "")
            {
                chord.AddRelativeNote(halfTonesIn(int.Parse(this.addNumber)), true, this.accidentalType, String.Format("{0}th", this.addNumber));
            }

            // Process any alterations, like b5 or #9
            if (this.alteredNumber != "")
            {
                int offset = -1;
                Accidental alteredAccidental = Accidental.Flat;

                if (this.alteredSharpFlat == "+" || this.alteredSharpFlat == "#")
                {
                    offset = 1;
                    alteredAccidental = Accidental.Sharp;
                }

                int alteredRelativeNoteNum = halfTonesIn(int.Parse(this.alteredNumber));
                int newRelativeNoteNum = (12 + alteredRelativeNoteNum + offset) % 12;

                // Remove the original note (if present).  Add the altered version.
                chord.RemoveRelativeNote(alteredRelativeNoteNum);
                chord.AddRelativeNote(newRelativeNoteNum, true, alteredAccidental, String.Format("{0}{1}", this.alteredSharpFlat, this.alteredNumber));
            }

            Debug.Write("All notes: ");
            foreach (Note note in this.chord.Notes)
            {
                Debug.Write(note.ToLongString() + ", ");
            }
            Debug.Write("\n");
            Debug.Write("Required notes: ");
            foreach (Note note in this.chord.RequiredNotes)
            {
                Debug.Write(note.ToLongString() + ", ");
            }
            Debug.Write("\n");
            return "";
        }

        private string findFingerings()
        {
            Fingering start = new Fingering(this.chord, NumStrings);

            for (int stringIndex = 0; stringIndex < NumStrings; stringIndex++)
            {
                // Make sure we have enough strings to get all required notes
                if (this.chord.RequiredNotes.Count <= NumStrings - stringIndex)
                {
                    int rootFret;
                    if (chord.SlashNote != null)
                    {
                        rootFret = chord.SlashNote.GetFretNum(stringIndex);
                        start.SetFret(stringIndex, rootFret, chord.SlashNote);
                    }
                    else
                    {
                        rootFret = chord.RootNote.GetFretNum(stringIndex);
                        start.SetFret(stringIndex, rootFret, chord.RootNote);
                    }
                    this.findFingerings(rootFret, start, stringIndex + 1);
                    start.SetPlayed(stringIndex, false);
                }
            }
            return "";
        }

        private void findFingerings(int rootFret, Fingering soFarIn, int currentString)
        {
            Note matchingNote = null;

            if (currentString == NumStrings)
            {
                foundChord(soFarIn);
                return;
            }

            int lowestFret = rootFret;
            int highestFret = rootFret;
            for (int i = 0; i < NumStrings; i++)
            {
                if (soFarIn.IsPlayed(i))
                {
                    lowestFret = Math.Min(lowestFret, soFarIn[i]);
                    highestFret = Math.Max(highestFret, soFarIn[i]);
                }
            }
            int lowerBound = Math.Max(highestFret - 4, 0);
            int upperBound = lowestFret + 4;
            // Make a copy of the fingering, so we can modify it.
            Fingering soFar = new Fingering(soFarIn);

            soFar.SetPlayed(currentString, true);

            for (int i = lowerBound; i <= upperBound; i++)
            {
                matchingNote = this.chord.GetNoteAtStringAndFret(currentString, i);
                if (matchingNote != null)
                {
                    soFar.SetFret(currentString, i, matchingNote);
                    findFingerings(rootFret, soFar, currentString + 1);
                }
            }
            // Try it with an open string
            if (lowerBound > 0 &&
                (matchingNote = this.chord.GetNoteAtStringAndFret(currentString, 0)) != null)
            {
                soFar.SetFret(currentString, 0, matchingNote);
                findFingerings(rootFret, soFar, currentString + 1);
            }
            // Try it with the rest of the strings muted.
            for (int i = currentString; i < NumStrings; i++)
            {
                soFar.SetPlayed(i, false);
            }
            foundChord(soFar);
        }

        /// <summary>
        /// Takes chord as an array of fretNumbers.
        /// If it's what we're looking for, stores it.
        /// </summary>
        /// <param name="chord"></param>
        /// <returns></returns>
        public bool foundChord(Fingering fingering)
        {
            // Convert to Notes
            ArrayList chordNotes = new ArrayList();

            for (int stringIndex = 0; stringIndex < NumStrings; stringIndex++)
            {
                if (fingering.IsPlayed(stringIndex))
                {
                    chordNotes.Add(Note.CreateFromStringAndFret(stringIndex, fingering[stringIndex]));
                }
            }
            // Make sure each required note is present.
            bool good = true;
            foreach (object noteObj in this.chord.RequiredNotes)
            {
                Note note = (Note)noteObj;

                if (chordNotes.IndexOf(note) == -1)
                {
                    good = false;
                    break;
                }
            }
            if (good)
            {
                foundChords.Add(new Fingering(fingering));
            }
            return good;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Utility functions
        ////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Converts a note as an integer to a note name as a string.
        /// </summary>
        /// <param name="noteNum"></param>
        /// <returns></returns>
        public static string NoteNumToName(int noteNum, Accidental preferredAccidental)
        {
            if (preferredAccidental == Accidental.Flat)
            {
                return flatNoteList[noteNum];
            }
            else
            {
                return noteList[noteNum];
            }
        }

        // Convert a note name (A-G) with a sharp or flat sign into a note number (0-11).
        public static int NoteNameToNum(string noteName, string sharpFlat)
        {
            int num;

            for (num = noteList.Length - 1; num >= 0; num--)
            {
                if (noteList[num] == noteName.ToUpper()) break;
            }
            if (num < 0) return -1;
            if (sharpFlat != null)
            {
                if ("+#".IndexOf(sharpFlat) != -1)
                {
                    num = (num + 1) % 12;
                }
                if ("-b".IndexOf(sharpFlat) != -1)
                {
                    num = (num + 11) % 12; // Same as subtracting one.
                }
            }

            return num;
        }

        // Convert a note name (A-G) with a sharp or flat sign into a note number (0-11).
        public static int NoteNameToNum(string noteName)
        {
            int num;
            string sharpFlat = null;

            if (noteName.Length > 1)
            {
                sharpFlat = noteName[1].ToString();
                noteName = noteName[0].ToString();
            }
            for (num = noteList.Length - 1; num >= 0; num--)
            {
                if (noteList[num] == noteName.ToUpper()) break;
            }
            if (num < 0) return -1;
            if (sharpFlat != null)
            {
                if ("+#".IndexOf(sharpFlat) != -1)
                {
                    num = (num + 1) % 12;
                }
                if ("-b".IndexOf(sharpFlat) != -1)
                {
                    num = (num + 11) % 12; // Same as subtracting one.
                }
            }

            return num;
        }

        public static Accidental NoteNameToAccidental(string noteName)
        {
            if (noteName.Length > 1)
            {
                string sharpFlat = noteName[1].ToString();
                if ("#+".IndexOf(sharpFlat) != -1)
                {
                    return Accidental.Sharp;
                }
                else if ("b-".IndexOf(sharpFlat) != -1)
                {
                    return Accidental.Flat;
                }
                else
                {
                    // This shouldn't ever happen
                    System.Diagnostics.Debug.Assert(false, "Unknown accidental");
                    return Accidental.Unknown;
                }
            }
            else
            {
                return Accidental.Natural;
            }
        }

        // Takes an interval, like a sixth, or a ninth, and returns how
        // many half-tones are in it.
        private int halfTonesIn(int interval)
        {
            int[] halfTonesMapping = new int[] { 0, 0, 2, 4, 5, 7, 9, 11 };
            // Deal with intervals greater than an octave.
            while (interval > 7) interval -= 7;
            return halfTonesMapping[interval];
        }

        private int stringAndFretToNoteNum(int stringIndex, int fret)
        {
            return (stringNoteNums[stringIndex] + fret) % 12;
        }

        private int[] appendToArray(int[] array, int i)
        {
            int[] ret = new int[array.Length + 1];

            array.CopyTo(ret, 0);
            ret[array.Length] = i;
            return ret;
        }

        public Chord ParsedChord
        {
            get { return chord; }
            set { chord = value; }
        }





    }
}
