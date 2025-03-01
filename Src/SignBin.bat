::Tips Set the CSIGNCERT as your path.
@echo off
path D:\ProjectsTmp\SignPack;%path%
echo 任意键签名 ExLauncher...
pause > nul
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0ExLauncher\bin\Release\ExLauncher.exe"
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0ExLauncher\bin\Debug\ExLauncher.exe"
echo.
echo 完成！
echo 任意键退出...
pause > nul