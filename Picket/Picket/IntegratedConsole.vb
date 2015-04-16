Imports System.ComponentModel
Imports System.IO

''' <summary>
''' Integrated console control class for Picket.
''' Run startConsole() to get up and running.
''' 
''' Author: Ash
''' </summary>

Public Class IntegratedConsole
    Inherits Panel
    '' Constant declaration

    ''' <summary>Amount of time to give CMD to start. Should be more on slower computers.</summary>
    Private Const CMD_WAITTIME As Integer = 100

    Dim cmd As Process

    Dim WithEvents processWatcher As New BackgroundWorker
    Dim WithEvents startupBw As New BackgroundWorker

    Dim textbox As New RichTextBox
    Dim startTime As String
    Public currentLog As String

    Public Event ServerStop As EventHandler(Of ServerEventArgs)
    Public Event ServerStart As EventHandler(Of ServerEventArgs)

    ''Windows API crud (yay!...)
    Declare Auto Function SetParent Lib "user32.dll" (ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As Integer
    Declare Auto Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    Declare Auto Function SetWindowLong Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
    Declare Auto Function GetWindowLong Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nIndex As Integer) As Integer
    Public Declare Auto Function MoveWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal X As Int32, ByVal Y As Int32, ByVal nWidth As Int32, ByVal nHeight As Int32, ByVal bRepaint As Boolean) As Boolean
    Private Const WM_SYSCOMMAND As Integer = 274
    Private Const SC_MAXIMIZE As Integer = 61488
    Private Const GWL_STYLE As Integer = -16
    Private Const WS_BORDER As Integer = &H800000
    Private Const WS_DLGFRAME As Integer = &H400000
    Private Const WS_CAPTION As Integer = WS_BORDER Or WS_DLGFRAME

    Public Sub New()
        ''wtee init has now been multithreaded! Yay!
        startupBw.RunWorkerAsync()
        processWatcher.WorkerReportsProgress = True
        Me.Controls.Add(textbox)
        textbox.Size = New Size(Me.Height, Me.Width)
        textbox.Dock = DockStyle.Fill
        textbox.ReadOnly = True
    End Sub

    ''' <summary>
    ''' Starts up the console and runs the specified command.
    ''' </summary>
    ''' <param name="command">Command to run in console window.</param>
    ''' <remarks>Use "cmd" as command to start a normal command window.</remarks>
    Public Sub startConsole(command As String)
        startTime = Now.ToString("d.M.yyyy H.m.s")
        currentLog = Path.GetTempPath() + "\Picket\logs\" + startTime + ".log"
        Dim startInfo As New ProcessStartInfo(BatchWrap(command, True))
        cmd = Process.Start(startInfo)
        setupWindow(cmd, CMD_WAITTIME)
        processWatcher.RunWorkerAsync(cmd)
        ''Call event for start
        RaiseEvent ServerStart(Me, New ServerEventArgs(currentLog, ServerEventArgs.alertType.serverStart))
    End Sub

    ''' <summary>
    ''' Places window inside panel (hopefully Me), maximises it and makes it fullscreen.
    ''' </summary>
    ''' <param name="window">Command window to process.</param>
    Private Sub setupWindow(window As Process, cmdWaitTime As Integer)
        Threading.Thread.Sleep(cmdWaitTime)
        Dim windowHandle As IntPtr = window.MainWindowHandle
        SetParent(windowHandle, Me.Handle)
        SendMessage(windowHandle, WM_SYSCOMMAND, SC_MAXIMIZE, 0)
        Dim style As Integer = GetWindowLong(windowHandle, GWL_STYLE)
        SetWindowLong(windowHandle, GWL_STYLE, (style And Not WS_CAPTION))
        MoveWindow(windowHandle, 0, 0, Me.Width, Me.Height + 10, True) ''TODO: Make the +10 configurable
    End Sub

    Private Function BatchWrap(command As String, wtee As Boolean)
        Dim n As String = Environment.NewLine
        If wtee = True Then
            File.WriteAllText(Path.GetTempPath() + "\Picket\servers\" + startTime + ".bat", "@echo off" + n + command + " | ""%temp%\Picket\wtee.exe"" """ + currentLog + """")
            Return Path.GetTempPath() + "\Picket\servers\" + startTime + ".bat"
        Else
            File.WriteAllText(Path.GetTempPath() + "\Picket\servers\" + startTime + ".bat", "@echo off" + n + command)
            Return Path.GetTempPath() + "\Picket\servers\" + startTime + ".bat"
        End If
    End Function

    Private Sub watchProcess(sender As Object, args As DoWorkEventArgs) Handles processWatcher.DoWork
        Dim process As Process = CType(args.Argument, Process)
        process.WaitForExit()
        processWatcher.ReportProgress(0, "complete")
    End Sub

    Private Sub processExit(sender As Object, args As ProgressChangedEventArgs) Handles processWatcher.ProgressChanged
        Dim n As String = Environment.NewLine
        textbox.Text = "Server Log" + n + "Time Started: " + startTime + n + "Time Stopped: " + Now.ToString("d.M.yyyy H.m.s") + n + n + "START LOG" + n + File.ReadAllText(currentLog)
        RaiseEvent ServerStop(Me, New ServerEventArgs(currentLog, ServerEventArgs.alertType.serverStop))
    End Sub

    Private Sub startupMultithread(sender As Object, args As DoWorkEventArgs) Handles startupBw.DoWork
        ''Set up good ol' wtee
        If Directory.Exists(Path.GetTempPath() + "\Picket\") = False Then
            Directory.CreateDirectory(Path.GetTempPath() + "\Picket\")
            Directory.CreateDirectory(Path.GetTempPath() + "\Picket\logs\")
            Directory.CreateDirectory(Path.GetTempPath() + "\Picket\servers\")
        End If
        Dim extractPath As String = Path.GetTempPath() + "\Picket\wtee.exe"
        Dim exeExtract As New FileStream(extractPath, FileMode.Create)
        exeExtract.Write(My.Resources.wtee, 0, My.Resources.wtee.Length)
        exeExtract.Dispose()
    End Sub
End Class
