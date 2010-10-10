using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Dummy;

namespace SourceAFIS.Extraction.Model
{
    public sealed class SkeletonBuilder : ICloneable
    {
        public sealed class Minutia
        {
            public bool Valid = true;
            public readonly Point Position;
            readonly List<Ridge> AllRidges = new List<Ridge>();
            readonly ReadOnlyCollection<Ridge> ReadOnlyRidges;
            public IList<Ridge> Ridges { get { return ReadOnlyRidges; } }

            public Minutia(Point position)
            {
                Position = position;
                ReadOnlyRidges = new ReadOnlyCollection<Ridge>(AllRidges);
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
                            Minutia detachFrom = StartMinutia;
                            StartMinutia = null;
                            detachFrom.DetachStart(this);
                        }
                        StartMinutia = value;
                        if (StartMinutia != null)
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

            public void Detach()
            {
                Start = null;
                End = null;
            }
        }

        List<Minutia> AllMinutiae = new List<Minutia>();
        public IEnumerable<Minutia> Minutiae { get { return AllMinutiae; } }

        public void AddMinutia(Minutia minutia)
        {
            AllMinutiae.Add(minutia);
        }

        public void RemoveMinutia(Minutia minutia)
        {
            AllMinutiae.Remove(minutia);
        }

        public object Clone()
        {
            SkeletonBuilder clone = new SkeletonBuilder();
            
            Dictionary<Minutia, Minutia> minutiaClones = new Dictionary<Minutia, Minutia>();
            foreach (Minutia minutia in AllMinutiae)
            {
                Minutia minutiaClone = new Minutia(minutia.Position);
                minutiaClone.Valid = minutia.Valid;
                clone.AddMinutia(minutiaClone);
                minutiaClones[minutia] = minutiaClone;
            }

            Dictionary<Ridge, Ridge> ridgeClones = new Dictionary<Ridge, Ridge>();
            foreach (Minutia minutia in AllMinutiae)
            {
                foreach (Ridge ridge in minutia.Ridges)
                {
                    if (!ridgeClones.ContainsKey(ridge))
                    {
                        Ridge ridgeClone = new Ridge();
                        ridgeClone.Start = minutiaClones[ridge.Start];
                        ridgeClone.End = minutiaClones[ridge.End];
                        foreach (Point point in ridge.Points)
                            ridgeClone.Points.Add(point);
                        ridgeClones[ridge] = ridgeClone;
                        ridgeClones[ridge.Reversed] = ridgeClone.Reversed;
                    }
                }
            }

            return clone;
        }
    }
}
