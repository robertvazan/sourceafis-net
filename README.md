# SourceAFIS for .NET #

[![Nuget](https://img.shields.io/nuget/v/SourceAFIS)](https://www.nuget.org/packages/SourceAFIS/)

SourceAFIS is a fingerprint recognition engine that takes a pair of human fingerprint images and returns their similarity score.
It can do 1:1 comparisons as well as efficient 1:N search. This is the .NET implementation of the SourceAFIS algorithm.

* Documentation: [SourceAFIS for .NET](https://sourceafis.machinezoo.com/net), [SourceAFIS overview](https://sourceafis.machinezoo.com/), [Algorithm](https://sourceafis.machinezoo.com/algorithm)
* Download: see [SourceAFIS for .NET](https://sourceafis.machinezoo.com/net) page
* Sources: [GitHub](https://github.com/robertvazan/sourceafis-net), [Bitbucket](https://bitbucket.org/robertvazan/sourceafis-net)
* Issues: [GitHub](https://github.com/robertvazan/sourceafis-net/issues), [Bitbucket](https://bitbucket.org/robertvazan/sourceafis-net/issues)
* License: [Apache License 2.0](LICENSE)

```csharp
var options = new FingerprintImageOptions { Dpi = 500 };
var probe = new FingerprintTemplate(
    new FingerprintImage(
        332, 533, File.ReadAllBytes("probe.dat"), options));
var candidate = new FingerprintTemplate(
    new FingerprintImage(
        320, 407, File.ReadAllBytes("candidate.dat"), options));
double score = new FingerprintMatcher(probe)
    .Match(candidate);
bool matches = score >= 40;
```

