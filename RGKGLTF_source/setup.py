from setuptools import setup, Extension
from Cython.Build import cythonize

extensions = [
    Extension("RGKGLTF", ["GLTF.py"]),
]

setup(
    ext_modules=cythonize(extensions, language_level="3"),
)