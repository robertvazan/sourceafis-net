using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public class AssertException : ApplicationException
    {
        public static void Check(bool condition)
        {
            if (!condition)
                Fail();
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
    }
}
