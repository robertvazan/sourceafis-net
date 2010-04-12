using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Simple
{
    /// <summary>
    /// Finger position on hand.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Finger position is used to speed up matching by skipping fingerprint pairs
    /// that cannot match due to incompatible position. SourceAFIS will return zero
    /// score for incompatible fingerprint pairs.
    /// </para>
    /// <para>
    /// This feature is optional. It can be disabled by using finger position Any
    /// which is default for new Fingerprint objects.
    /// </para>
    /// <para>
    /// A compatible fingerprint pair consists of two fingerprints with the same
    /// finger position, e.g. RightThumb matches only other RightThumb. Alternatively,
    /// compatible fingerprint pair can be also formed if one of the fingerprints
    /// has Any finger position, e.g. Any can be matched against all other finger
    /// positions and all other finger positions can be matched against Any. Two
    /// fingerprints with Any positions are compatible as well, of course.
    /// </para>
    /// </remarks>
    [Serializable]
    public enum Finger
    {
        /// <summary>
        /// Unspecified finger position.
        /// </summary>
        Any = 0,
        /// <summary>
        /// Thumb finger on the right hand.
        /// </summary>
        RightThumb = 1,
        /// <summary>
        /// Thumb finger on the left hand.
        /// </summary>
        LeftThumb = 2,
        /// <summary>
        /// Index finger on the right hand.
        /// </summary>
        RightIndex = 3,
        /// <summary>
        /// Index finger on the left hand.
        /// </summary>
        LeftIndex = 4,
        /// <summary>
        /// Middle finger on the right hand.
        /// </summary>
        RightMiddle = 5,
        /// <summary>
        /// Middle finger on the left hand.
        /// </summary>
        LeftMiddle = 6,
        /// <summary>
        /// Ring finger on the right hand.
        /// </summary>
        RightRing = 7,
        /// <summary>
        /// Ring finger on the left hand.
        /// </summary>
        LeftRing = 8,
        /// <summary>
        /// Little finger on the right hand.
        /// </summary>
        RightLittle = 9,
        /// <summary>
        /// Little finger on the left hand.
        /// </summary>
        LeftLittle = 10,
    }
}
