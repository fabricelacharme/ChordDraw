using System;
using System.Collections;
using System.Text;

namespace ChordLib
{

    /// <summary>
    /// Represents a fret as part of a Fingering.
    /// </summary>
    public struct Fret
    {
        /// <summary>
        /// The note this fret is playing.
        /// </summary>
        public Note note;

        /// <summary>
        /// The fret number, where 0 is open.
        /// </summary>
        public int num;
        /// <summary>
        /// Whether this fret is a required part of the fingering.
        /// </summary>
        public bool required;
        /// <summary>
        /// Whether this fret is played or not.  If this is false, the other
        /// member variables are meaningless.
        /// </summary>
        public bool played;

        public override bool Equals(object obj)
        {
            Fret other = (Fret)obj;

            if (!this.played && !other.played) return true; // Nothing else matters if they're not played.

            return (this.played == other.played && this.required == other.required && this.num == other.num);
        }

        public override int GetHashCode()
        {
            if (!played)
            {
                return 0;
            }
            else if (required)
            {
                return num;
            }
            else
            {
                return -num;
            }
        }


        public static bool operator ==(Fret one, Fret other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(Fret one, Fret other)
        {
            return !one.Equals(other);
        }
    };


    /// <summary>
	/// Represents one way of fingering a chord.
	/// </summary>
    public class Fingering : IComparable
    {
        private int numStrings;
        private Fret[] frets; // Indexed by string number
        private double difficulty = -1.0;
        private Chord chord;

        public Fingering(Chord chordIn, int numStringsIn)
        {
            numStrings = numStringsIn;
            frets = new Fret[numStrings];
            for (int i = 0; i < numStrings; i++)
            {
                frets[i].num = -1;
                frets[i].required = true;
                frets[i].played = false;
            }
            chord = chordIn;
        }

        /// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		public Fingering(Fingering other)
        {
            this.numStrings = other.numStrings;
            this.frets = (Fret[])other.frets.Clone();
            this.chord = other.chord;
        }

        /// <summary>
        /// ES: This is probably not an appropriate use of the overloaded [] operator.
        /// TODO: Change this to a method call interface.
        /// </summary>
        public int this[int stringIndex]
        {
            get
            {
                return frets[stringIndex].num;
            }
        }

        public int GetFretNum(int stringIndex)
        {
            return frets[stringIndex].num;
        }

        public void SetFret(int stringIndex, int num, Note note)
        {
            frets[stringIndex].num = num;
            frets[stringIndex].note = note;
            frets[stringIndex].played = true;
        }

        public bool IsPlayed(int stringIndex)
        {
            return frets[stringIndex].played;
        }

        public void SetPlayed(int stringIndex, bool played)
        {
            frets[stringIndex].played = played;
        }

        public bool IsRequired(int stringIndex)
        {
            return frets[stringIndex].required;
        }

        public void SetRequired(int stringIndex, bool required)
        {
            frets[stringIndex].required = required;
        }

        public override bool Equals(object obj)
        {
            Fingering other = (Fingering)obj;

            if (this.numStrings != other.numStrings) return false;

            for (int i = 0; i < numStrings; i++)
            {
                if (frets[i] != other.frets[i]) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Fingering first, Fingering other)
        {
            if ((object)first == null) return ((object)other == null);
            if ((object)other == null) return false;
            return first.Equals(other);
        }

        public static bool operator !=(Fingering first, Fingering other)
        {
            if ((object)first == null) return ((object)other != null);
            if ((object)other == null) return true;
            return !first.Equals(other);
        }

        public Fret GetFret(int stringIndex)
        {
            return this.frets[stringIndex];
        }

        public void SetFret(int stringIndex, Fret fretIn)
        {
            frets[stringIndex] = fretIn;
        }
        #region IComparable Members

        public int CompareTo(object obj)
        {
            Fingering other = (Fingering)obj;
            if (this.Difficulty > other.Difficulty) return 1;
            if (this.Difficulty < other.Difficulty) return -1;
            return 0;
        }

        #endregion

        // Comes up with a metric for a chord's difficult.
        // fretNums = an array of length six, with the fret numbers.
        public double Difficulty
        {
            get
            {
                if (difficulty >= 0) return difficulty;

                // Compute the number of unique frets and the range of frets
                double numFrets = 0;
                int minFret = 9999;
                int maxFret = 0;
                ArrayList fretsSoFar = new ArrayList(6);
                int numStrings = 6;
                bool foundOpenString = false;
                bool barre = false;

                // First find the minimum and maximum frets.
                foreach (Fret fret in frets)
                {
                    if (fret.played && fret.num != 0)
                    { // Don't count non-played or open frets.
                        minFret = Math.Min(minFret, fret.num);
                        maxFret = Math.Max(maxFret, fret.num);
                    }
                }
                int fretRange = maxFret - minFret;
                // Now count how many unique frets are played.
                foreach (Fret fret in frets)
                {
                    if (fret.played && fret.num != 0)
                    {
                        if (fretsSoFar.IndexOf(fret.num) == -1)
                        {
                            if (fret.num == 0)
                            {
                                foundOpenString = true;
                            }

                            fretsSoFar.Add(fret.num);
                            // Optional strings count less.
                            if (fret.required)
                            {
                                numFrets++;
                            }
                            else if (fret.num != 0)
                            {
                                numFrets += 0.25;
                            }
                        }
                        else if (fret.num != minFret || foundOpenString)
                        {
                            // Don't count
                            // frets that are equal to minFret, unless we've had an open string
                            // in the middle.  This is to give a boost to barre chords.
                            // Optional strings count less.
                            if (fret.required)
                            {
                                numFrets++;
                            }
                            else
                            {
                                numFrets += 0.25;
                            }
                        }
                        else
                        {
                            barre = true;
                        }
                    }
                    else
                    {
                        if (fret.played)
                        {
                            foundOpenString = true;
                            if (barre)
                            {
                                // Penalty for finding an open string after we've started a barre.
                                numFrets++;
                            }
                        }
                        else
                        {
                            numStrings--;
                        }
                        fretsSoFar.Clear();
                    }
                }
                difficulty = fretRange * 2 + numFrets + numStrings / 2.0;
                // Penalty for lots of frets.
                if (numFrets > 4) difficulty *= 2;
                // Bonus for open position.
                if (maxFret <= 4 && foundOpenString) difficulty /= 2;

                return difficulty;
            }
        }

        /// <summary>
		/// Gets the first string which is played.
		/// </summary>
		public int RootString
        {
            get
            {
                for (int i = 0; i < this.numStrings; i++)
                {
                    if (this.frets[i].played) return i;
                }
                return -1;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Fret fret in frets)
            {
                if (!fret.played)
                {
                    sb.Append("- ");
                }
                else if (fret.required)
                {
                    sb.AppendFormat("{0} ", fret.num);
                }
                else
                {
                    sb.AppendFormat("({0}) ", fret.num);
                }
            }
            sb.Append(" == ");
            for (int i = 0; i < this.numStrings; i++)
            {
                Fret fret = frets[i];

                if (!fret.played)
                {
                    sb.Append("- ");
                }
                else
                {
                    sb.AppendFormat("{0} ", fret.note);
                }
            }
            sb.AppendFormat("Difficulty: {0}", this.Difficulty);
            return sb.ToString();
        }

        public Note GetNote(int index)
        {
            return frets[index].note;
        }

        public Chord PlayedChord
        {
            get
            {
                return chord;
            }
        }

        public int NumStrings
        {
            get
            {
                return this.numStrings;
            }
        }

    }
}
