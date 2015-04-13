Public Class ServerEventArgs
    Inherits EventArgs
    Enum alertType
        serverStart = 1
        serverStop = 2
    End Enum
    Public logPath As String
    Public eventType As alertType = alertType.serverStart
    ''Add more arguments here

    Public Sub New(logPath As String, eventType As alertType)
        Me.logPath = logPath
        Me.eventType = eventType
    End Sub
End Class
