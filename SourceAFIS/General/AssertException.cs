using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    public class AssertException
#if !COMPACT_FRAMEWORK
        : ApplicationException
#else
        : Exception
#endif
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
