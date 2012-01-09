using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public class AssertException : ApplicationException
    {
        public AssertException() { }

        public AssertException(string message)
            : base(message)
        {
        }

        public static void Check(bool condition)
        {
            if (!condition)
                Fail();
        }

        public static void Check(bool condition, string message)
        {
            if (!condition)
                Fail(message);
        }

        public static void FailIf(bool condition)
        {
            if (condition)
                Fail();
        }

        public static void Fail()
        {
            throw new AssertException();
        }

        public static void Fail(string message)
        {
            throw new AssertException(message);
        }
    }
}
