# This script generates and updates project configuration files.

# Run this script with rvscaffold in PYTHONPATH
import rvscaffold as scaffold

class Project(scaffold.Net):
    def script_path_text(self): return __file__
    def root_namespace(self): return 'SourceAFIS'
    def pretty_name(self): return 'SourceAFIS for .NET'
    def subdomain(self): return 'sourceafis'
    def homepage(self): return self.website() + 'net'
    def inception_year(self): return 2009
    def nuget_description(self): return 'Fingerprint recognition engine that takes a pair of human fingerprint images and returns their similarity score. Supports efficient 1:N search.'
    def nuget_tags(self): return 'fingerprint; biometrics; authentication; sourceafis'
    def test_resources(self): return ['Resources/*.dat', 'Resources/*.png', 'Resources/*.jpeg', 'Resources/*.bmp']
    def project_status(self): return self.stable_status()

    def documentation_links(self):
        yield 'SourceAFIS for .NET', self.homepage()
        yield 'XML doc comments', self.readme_dir_url(self.root_namespace())
        yield 'SourceAFIS overview', 'https://sourceafis.machinezoo.com/'
        yield 'Algorithm', 'https://sourceafis.machinezoo.com/algorithm'

    def dependencies(self):
        yield from super().dependencies()
        yield self.use('Dahomey.Cbor:1.16.1')
        yield self.use('SixLabors.ImageSharp:2.1.10')

Project().generate()
