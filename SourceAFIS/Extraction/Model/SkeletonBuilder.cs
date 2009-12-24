using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public sealed class SkeletonBuilder
    {
        public sealed class Minutia
        {
            public readonly Point Position;
            List<Ridge> AllRidges = new List<Ridge>();
            public IEnumerable<Ridge> Ridges { get { return AllRidges; } }

            public Minutia(Point position)
            {
                Position = position;
            }

            public void AttachStart(Ridge ridge)
            {
                if (!AllRidges.Contains(ridge))
                {
                    AllRidges.Add(ridge);
                    ridge.Start = this;
                }
            }

            public void DetachStart(Ridge ridge)
            {
                if (AllRidges.Contains(ridge))
                {
                    AllRidges.Remove(ridge);
                    if (ridge.Start == this)
                        ridge.Start = null;
                }
            }
        }

        public sealed class Ridge
        {
            public readonly IList<Point> Points;
            Minutia StartMinutia;
            Minutia EndMinutia;

            public readonly Ridge Reversed;
            public Minutia Start
            {
                get { return StartMinutia; }
                set
                {
                    if (StartMinutia != value)
                    {
                        if (StartMinutia != null)
                        {
                            StartMinutia = null;
                            StartMinutia.DetachStart(this);
                        }
                        StartMinutia = value;
                        StartMinutia.AttachStart(this);
                        Reversed.EndMinutia = value;
                    }
                }
            }
            public Minutia End
            {
                get { return EndMinutia; }
                set
                {
                    if (EndMinutia != value)
                    {
                        EndMinutia = value;
                        Reversed.Start = value;
                    }
                }
            }

            public Ridge()
            {
                Points = new CircularArray<Point>();
                Reversed = new Ridge(this);
            }

            Ridge(Ridge reversed)
            {
                Reversed = reversed;
                Points = new ReversedList<Point>(reversed.Points);
            }
        }

        List<Minutia> AllMinutiae = new List<Minutia>();
        public IEnumerable<Minutia> Minutiae { get { return AllMinutiae; } }

        public void AddMinutia(Minutia minutia)
        {
            AllMinutiae.Add(minutia);
        }
    }
}
