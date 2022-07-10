# This script generates and updates project configuration files.

# We are assuming that project-config is available in sibling directory.
# Checkout from https://github.com/robertvazan/project-config
import pathlib
project_directory = lambda: pathlib.Path(__file__).parent.parent
config_directory = lambda: project_directory().parent/'project-config'
exec((config_directory()/'src'/'net.py').read_text())

root_namespace = lambda: 'SourceAFIS'
pretty_name = lambda: 'SourceAFIS for .NET'
subdomain = lambda: 'sourceafis'
homepage = lambda: website() + 'net'
inception_year = lambda: 2009
nuget_description = lambda: 'Fingerprint recognition engine that takes a pair of human fingerprint images and returns their similarity score. Supports efficient 1:N search.'
nuget_tags = lambda: 'fingerprint; biometrics; authentication; sourceafis'
test_resources = lambda: ['Resources/*.dat', 'Resources/*.png', 'Resources/*.jpeg', 'Resources/*.bmp']

def documentation_links():
    yield 'SourceAFIS for .NET', homepage()
    yield 'XML doc comments', readme_dir_url(root_namespace())
    yield 'SourceAFIS overview', 'https://sourceafis.machinezoo.com/'
    yield 'Algorithm', 'https://sourceafis.machinezoo.com/algorithm'

def dependencies():
    use('Dahomey.Cbor:1.16.1')
    use('SixLabors.ImageSharp:2.1.3')

generate()
