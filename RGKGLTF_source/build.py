import sys
sys.path.append('..\\..\\RGK_Dist\\site-packages')
import re
from pathlib import Path
from Cython.Build import cythonize
from setuptools import setup, Extension

SOURCE_DIR = Path("source")
OUTPUT_DIR = Path("output")
MERGED_FILE = OUTPUT_DIR / "merged.pyx"

def resolve_imports(filepath, seen=None):
    if seen is None:
        seen = set()

    code = []
    full_path = SOURCE_DIR / filepath
    if full_path in seen:
        return []  # избегаем повторов
    seen.add(full_path)

    with open(full_path, "r", encoding="utf-8") as f:
        lines = f.readlines()

    for line in lines:
        match = re.match(r"from (\w+) import .*|import (\w+)", line)
        mod = match.group(1) or match.group(2) if match else None
        if mod and (SOURCE_DIR / f"{mod}.py").exists():
            code += resolve_imports(f"{mod}.py", seen)
        else:
            code.append(line)

    return code

def merge_and_write():
    code = resolve_imports("main.py")
    OUTPUT_DIR.mkdir(exist_ok=True)
    with open(MERGED_FILE, "w", encoding="utf-8") as f:
        f.writelines(code)
    print(f"Сформирован файл: {MERGED_FILE}")

def build_pyd():
    setup(
        name="RGKGLTF",
        ext_modules=cythonize(
            [Extension("RGKGLTF", [str(MERGED_FILE)])],
            compiler_directives={"language_level": "3"},
            build_dir="temp_build"
        ),
        script_args=["build_ext", "--inplace"],
    )

if __name__ == "__main__":
    merge_and_write()
    build_pyd()
    print("Сборка завершена!")
