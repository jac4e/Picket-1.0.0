Public Class Form1
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub GithubToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GithubToolStripMenuItem.Click
        Process.Start("https://github.com/jaclegonetwork/Picket-1.0.0")
    End Sub

    Private Sub CheckForUpdateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckForUpdateToolStripMenuItem.Click
        ' Check for update
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
        MainTabController.SelectTab(ServerTabPage)
        IntegratedConsole1.startConsole(ComboBox1.SelectedItem)
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
