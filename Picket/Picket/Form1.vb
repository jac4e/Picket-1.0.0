Imports System.Net
Imports System.IO
Imports System.ComponentModel

Public Class Form1
    Dim WithEvents downloader As New WebClient

    Dim status As statuses = statuses.Stopped
    Dim downloading As Boolean = False

    Enum statuses As Integer
        Installing = 1
        Updating = 2
        Running = 3
        Stopped = 4
    End Enum

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click ' Install Button
        InstallHandler()
        UpdateHandler()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click '' Update button
        UpdateHandler()
    End Sub

    Private Sub InstallHandler()
        status = statuses.Installing
        Dim Appdata As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        Dim buildtoolsURL As String = "https://hub.spigotmc.org/jenkins/job/BuildTools/lastSuccessfulBuild/artifact/target/BuildTools.jar"
        Dim buildtoolsfileName As String = Appdata + "\Picket\spigot\BuildTools.jar"
        downloading = True
        downloader.DownloadFileAsync(New Uri(buildtoolsURL), buildtoolsfileName)
        While downloading
            Application.DoEvents()
        End While
        status = statuses.Stopped
    End Sub
    Private Sub UpdateHandler()
        status = statuses.Updating
        If Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\Programs\Git\") = False Then
            Dim result As Integer = MessageBox.Show("Git could not be found on your computer. Would you like to install it now?" + Environment.NewLine + "(17.1MB download, 256MB extracted)", "", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim msysgitURL As String = "http://github.com/msysgit/msysgit/releases/download/Git-1.9.5-preview20150319/Git-1.9.5-preview20150319.exe"
                Dim msysgitDownloadLocation As String = Path.GetTempPath() + "\Picket\git\INSTALL.EXE"
                downloading = True
                downloader.DownloadFileAsync(New Uri(msysgitURL), msysgitDownloadLocation)
                While downloading
                    Application.DoEvents()
                End While
                MessageBox.Show("done")
                Dim installer As Process = Process.Start(Path.GetTempPath + "\Picket\git\INSTALL.EXE", "/SILENT")
            Else
                MessageBox.Show("Your server will not be installed.")
            End If
        End If
        status = statuses.Stopped
    End Sub
    Private Sub StartHandler()
        ' Combo Box could list each server avalible to run (CraftBukkit, Spigot, Bukkit)
        MainTabController.SelectTab(ServerTabPage)
        IntegratedConsole1.startConsole(ComboBox1.SelectedItem)
    End Sub

    Private Sub processProgressChange(sender As Object, e As DownloadProgressChangedEventArgs) Handles downloader.DownloadProgressChanged
        If status = statuses.Installing Then
            ProgressBar1.Value = e.ProgressPercentage
        ElseIf status = statuses.Updating Then
            ProgressBar1.Value = e.ProgressPercentage + 100
        End If
    End Sub
    Private Sub processFinished(sender As Object, e As AsyncCompletedEventArgs) Handles downloader.DownloadFileCompleted
        downloading = False
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub GithubToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GithubToolStripMenuItem.Click
        Process.Start("https://github.com/jaclegonetwork/Picket-1.0.0")
    End Sub

    Private Sub CheckForUpdateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckForUpdateToolStripMenuItem.Click
        ' Check for picket update
        ' If update avalible, create pop up telling user
        ' If user clicks ok, check for UpdateTool.exe, download if not found
        ' Run UpdateTool.exe
        ' Close
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        WebBrowser1.GoBack()
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        WebBrowser1.GoForward()
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        WebBrowser1.Navigate("http://www.spigotmc.org")
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        WebBrowser1.Navigate("https://github.com/jaclegonetwork/Picket-1.0.0")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        StartHandler()
    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub IntegratedConsole1_ServerStart(sender As System.Object, e As Picket.ServerEventArgs) Handles IntegratedConsole1.ServerStart
        StatusLabel.Text = "Running"
        StatusLabel.ForeColor = Color.Green
        ServerTabPage.Text = "Server (Running)"
    End Sub

    Private Sub IntegratedConsole1_ServerStop(sender As System.Object, e As Picket.ServerEventArgs) Handles IntegratedConsole1.ServerStop
        StatusLabel.Text = "Stopped"
        StatusLabel.ForeColor = Color.Red
        ServerTabPage.Text = "Server"
    End Sub
End Class
