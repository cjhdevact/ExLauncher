::Tips Set the CSIGNCERT as your path.
@echo off
path D:\ProjectsTmp\SignPack;%path%
echo �����ǩ�� ExLauncher...
pause > nul
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0ExLauncher\bin\Release\ExLauncher.exe"
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0ExLauncher\bin\Debug\ExLauncher.exe"
echo.
echo ��ɣ�
echo ������˳�...
pause > nul