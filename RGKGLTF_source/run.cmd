@echo off
mkdir output
"..\..\RGK_Dist\DLLs\python.exe" build.py
rmdir /s /q temp_build
rmdir /s /q build
rmdir /s /q output
for %%f in (*.pyd) do (
    ren "%%f" "RGKGLTF.pyd"
    goto :done
)
:done
pause