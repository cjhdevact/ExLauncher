'****************************************************************************
'    ExLauncher
'    Copyright (C) 2025  CJH
'
'    This program is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    This program is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with this program.  If not, see <http://www.gnu.org/licenses/>.
'****************************************************************************
'/*****************************************************\
'*                                                     *
'*     ExLauncher - LauncherMain.vb                    *
'*                                                     *
'*     Copyright (c) CJH.                              *
'*                                                     *
'*     Launcher Executable.                            *
'*                                                     *
'\*****************************************************/
Imports Microsoft.Win32
Imports System.IO
Imports System.Security.Cryptography

Module LauncherMain
    Sub Main()
        'On Error Resume Next
        Dim mycmd() As String
        Dim ExtStr As String
        '读取目录扩展备用字符串
        Try
            My.Computer.Registry.CurrentUser.CreateSubKey("Software\CJH")
            My.Computer.Registry.CurrentUser.CreateSubKey("Software\CJH\ExLauncher")
        Catch ex As Exception
        End Try
        Dim mykey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\CJH\ExLauncher", True)
        Try
            Dim myv As String
            If (Not mykey Is Nothing) Then
                myv = mykey.GetValue("ExtStr", Chr(13))
                If Not myv = Chr(13) Then
                    ExtStr = myv
                Else
                    ExtStr = ""
                End If
            Else
                ExtStr = ""
            End If
        Catch ex As Exception
            ExtStr = ""
        End Try

        If ExtStr = "" Then
            ExtStr = GenerateRandomString(5)
            Try
                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\CJH\ExLauncher", "ExtStr", ExtStr, RegistryValueKind.String)
            Catch ex As Exception
            End Try
        End If

        If (Not mykey Is Nothing) Then
            mykey.Close()
        End If

        '命令行处理
        mycmd = Environment.GetCommandLineArgs()
        If mycmd.Length < 2 Then
            'Application.Exit()
            End
        End If
        Dim ExeName As String = ""
        For i = 0 To mycmd.Count - 1
            If mycmd(i) = "/?" Then
                '帮助
                MessageBox.Show("ExLauncher Version " & My.Application.Info.Version.ToString & vbCrLf & "Copyright © 2025 CJH. All Rights Reserved." & vbCrLf & "Usage:" & vbCrLf & "ExLauncher [Executable File Path] [Args]", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information)
                'Application.Exit()
                End
            End If
            If mycmd(i).Contains(" ") Then
                mycmd(i) = """" & mycmd(i) & """"
            End If
        Next

        ExeName = Environment.GetCommandLineArgs(1)

        '判断源文件是否存在
        If System.IO.File.Exists(ExeName) = False Then
            'Application.Exit()
            End
        End If

        '获取程序参数
        Dim ExeArgs As String = ""
        If mycmd.Length >= 3 Then
            For i = 2 To mycmd.Count - 1
                If i = mycmd.Count - 1 Then
                    ExeArgs = ExeArgs & mycmd(i)
                Else
                    ExeArgs = ExeArgs & mycmd(i) & " "
                End If
            Next
        End If

        '创建程序总目录（ExLauncher\）或（ExLauncher-XXX\）
        Dim AppDir As String = ""
        Dim LocalDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
        If IO.Directory.Exists(LocalDirectory & "\ExLauncher") = False Then
            If IO.Directory.Exists(LocalDirectory & "\ExLauncher-" & ExtStr) = False Then
                Try
                    My.Computer.FileSystem.CreateDirectory(LocalDirectory & "\ExLauncher")
                    AppDir = LocalDirectory & "\ExLauncher"
                Catch ex As Exception
                    Try
                        My.Computer.FileSystem.CreateDirectory(LocalDirectory & "\ExLauncher-" & ExtStr)
                        AppDir = LocalDirectory & "\ExLauncher-" & ExtStr
                    Catch ee As Exception
                        ExtStr = GenerateRandomString(5)
                        Try
                            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\CJH\ExLauncher", "ExtStr", ExtStr, RegistryValueKind.String)
                        Catch ef As Exception
                        End Try
                        My.Computer.FileSystem.CreateDirectory(LocalDirectory & "\ExLauncher-" & ExtStr)
                        AppDir = LocalDirectory & "\ExLauncher-" & ExtStr
                    End Try
                End Try
            Else
                AppDir = LocalDirectory & "\ExLauncher-" & ExtStr
            End If
        Else
            AppDir = LocalDirectory & "\ExLauncher"
        End If

        '创建程序运行目录（ExLauncher\XXX.exe\）或（ExLauncher\XXX.exe-XXX\）
        Dim AppPath() As String
        AppPath = Split(Environment.GetCommandLineArgs(1), "\")
        Dim AppName As String = ""
        AppName = AppPath(AppPath.Length - 1)

        Dim LauDir As String = ""
        If IO.Directory.Exists(AppDir & "\" & AppName) = False Then
            If IO.Directory.Exists(AppDir & "\" & AppName & "-" & ExtStr) = False Then
                Try
                    My.Computer.FileSystem.CreateDirectory(AppDir & "\" & AppName)
                    LauDir = AppDir & "\" & AppName
                Catch ex As Exception
                    Try
                        My.Computer.FileSystem.CreateDirectory(AppDir & "\" & AppName & "-" & ExtStr)
                        LauDir = AppDir & "\" & AppName & "-" & ExtStr
                    Catch ee As Exception
                        ExtStr = GenerateRandomString(5)
                        Try
                            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\CJH\ExLauncher", "ExtStr", ExtStr, RegistryValueKind.String)
                        Catch ef As Exception
                        End Try
                        My.Computer.FileSystem.CreateDirectory(AppDir & "\" & AppName & "-" & ExtStr)
                        LauDir = AppDir & "\" & AppName & "-" & ExtStr
                    End Try
                End Try
            Else
                LauDir = AppDir & "\" & AppName & "-" & ExtStr
            End If
        Else
            LauDir = AppDir & "\" & AppName
        End If
        '创建程序运行文件（ExLauncher\XXX.exe\XXX.exe）
        If IO.File.Exists(LauDir & "\" & AppName) = True Then
            Dim HashExt As String = ""
            Dim HashDes As String = ""
            HashExt = GetFileHash(LauDir & "\" & AppName, "SHA256")
            HashDes = GetFileHash(ExeName, "SHA256")
            If Not (HashExt = HashDes) Then
                Try
                    IO.File.Delete(LauDir & "\" & AppName)
                    IO.File.Copy(ExeName, LauDir & "\" & AppName)
                Catch ex As Exception
                    'Application.Exit()
                    End
                End Try
            End If
        Else
            Try
                IO.File.Copy(ExeName, LauDir & "\" & AppName)
            Catch ex As Exception
                'Application.Exit()
                End
            End Try
        End If
        '运行程序
        Try
            Process.Start(LauDir & "\" & AppName, ExeArgs)
        Catch ex As Exception
        End Try
    End Sub
    '生成随机字符串函数
    Public Function GenerateRandomString(ByVal length As Integer) As String
        Dim random As New Random()
        Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim result As New List(Of Char)()

        For i As Integer = 1 To length
            result.Add(chars(random.Next(0, chars.Length)))
        Next

        Return New String(result.ToArray())
    End Function
    '生成文件Hash函数
    Public Function GetFileHash(ByVal filePath As String, ByVal hashType As String) As String
        Dim hashAlgorithm As HashAlgorithm = Nothing
        Select Case hashType.ToUpper()
            Case "MD5"
                hashAlgorithm = MD5.Create()
            Case "SHA1"
                hashAlgorithm = SHA1.Create()
            Case "SHA256"
                hashAlgorithm = SHA256.Create()
            Case Else
                Throw New ArgumentException("Unsupported hash type.")
        End Select

        Using fileStream As FileStream = File.OpenRead(filePath)
            Dim hashBytes = hashAlgorithm.ComputeHash(fileStream)
            Return BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
        End Using
    End Function
End Module
