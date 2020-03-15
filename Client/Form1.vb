Imports System.IO
Imports System.Net.Sockets

Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Public Class Form1
    'Dim game As New Form2
    Dim Client As TcpClient
    Dim RX As StreamReader
    Dim TX As StreamWriter

    '***************************************************
    Dim random As Byte
    Dim scoreP1 As String
    Dim scoreP2 As String

    Dim player1 As Boolean = True
    Dim player2 As Boolean = True

    Dim flag As Boolean = False ' if false, player 1 turn

    Dim firstTurnFlag As Boolean = False

    '********************** player 1 ***************
    Dim x1 = -56, y1 = 501
    'Dim x2 = 36, y2 = 501
    Dim p = 0
    Dim dice As Integer = 0
    Dim pos(101) As Integer

    '*********************player 2 *****************
    Dim x2 = -56 + 32, y2 = 501
    Dim q = 0
    '******************* randomizer ******************
    Dim rand As Random

    Dim loadingscreen As New loadingScreen
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        REM Connect Button
        Try
            rollBtnP1.Enabled = True
            REM IP, Port
            REM If port is in a textbox, use: integer.parse(textbox1.text)  instead of the port number vvv
            Client = New TcpClient("192.168.254.111", 63861)
            If Client.GetStream.CanRead = True Then
                RX = New StreamReader(Client.GetStream)
                TX = New StreamWriter(Client.GetStream)
                Threading.ThreadPool.QueueUserWorkItem(AddressOf Connected)
            End If
        Catch ex As Exception
            RichTextBox1.Text += "Failed to connect, E: " + ex.Message + vbNewLine
            rollBtnP2.Enabled = False
        End Try
    End Sub
    Function Connected()
        REM Has connected to server and now listening for data from the server
        If RX.BaseStream.CanRead = True Then
            Try
                Button1.Text = "Connected"
                Button2.Text = "Disconnect"
                RichTextBox1.Text = ""
                While RX.BaseStream.CanRead = True
                    Dim RawData As String = RX.ReadLine
                    If RawData.ToUpper = "/MSG" Then
                        Threading.ThreadPool.QueueUserWorkItem(AddressOf MSG1, "Hello World.")
                    Else
                        'RichTextBox1.Text += "Server>>" + RawData + vbNewLine

                        'received data from server
                        diceP1.Image = My.Resources.ResourceManager.GetObject("_" & RawData)
                        diceLabel.Text = RawData
                        dice = RawData

                        TextBox2.Text = dice

                        diceP1.SizeMode = PictureBoxSizeMode.StretchImage
                        prevVal.Text = positionVal.Text
                        'diceTimer.Start()

                        'rollDiceP1()

                        'scoreP1 = random
                        '  playerTwo.Visible = True
                        'rollBtnP1.Enabled = False
                        'rollBtnP2.Enabled = True
                        'Label1.Visible = False
                        'Label2.Visible = True

                        If (player1 = True) Then
                            'x1 += 60
                            'pieceP1.Location = New Point(x1, y1)
                            'stepLimiter()

                            Mover()

                        End If
                        If (diceLabel.Text = "6" AndAlso player1 = False) Then
                            pieceP1.Visible = True
                            '    playerOne.Visible = False

                            'pieceP1.Location = New Point(x1, y1)

                            player1 = True
                            p += 1
                            'pos(p) = 1
                            pieceP1.Visible = True
                        End If
                        snakeBites()
                        ladder()

                    End If
                End While
            Catch ex As Exception
                Client.Close()
                RichTextBox1.Text += "Disconnected" + vbNewLine

                x1 = -56
                y1 = 501
                pieceP1.Location = New Point(x1, y1)
                p = 0
                dice = 0

                x2 = -56 + 32
                y2 = 501
                pieceP2.Location = New Point(x2, y2)
                q = 0

                positionVal.Text = 0
                positionVal2.Text = 0
                prevVal.Text = ""
                prevVal2.Text = ""
                Client.Close()
                Button1.Text = "Connect"
                Button2.Text = "Disconnect"

                wait(500)
                Panel4.Visible = True


            End Try
        End If
        Return True
    End Function
    Function MSG1(ByVal Data As String)
        REM Creates a messageBox for new threads to stop freezing
        MsgBox(Data)
        Return True
    End Function
    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        REM When you press enter on the textbox to send the message
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If TextBox1.Text.Length > 0 Then
                SendToServer(TextBox1.Text)
                TextBox1.Clear()
            End If
        End If
    End Sub
    Function SendToServer(ByVal Data As String)
        REM Send a message to the server
        Try
            TX.WriteLine(Data)
            TX.Flush()
        Catch ex As Exception

        End Try
        Return True
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        REM Stops crossthreadingIssues
        CheckForIllegalCrossThreadCalls = False
        loadingscreen.ShowDialog()

        rollBtnP1.Enabled = False
        rollBtnP2.Enabled = False
        'rollBtnP2.Enabled = False
        'rollDiceP1()
        'Timer1.Start()
        pieceP1.Visible = True
        pieceP2.Visible = True
        snakeBites()
        ladder()
        ' snakeBites(q, x2, y2, pieceP2)
        'ladder(q, x2, y2, pieceP2)
        snakeBites2()
        ladder2()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Button1.Text = "Connect"
        Button2.Text = "Disconnected"
        REM Disconnect Button
        Try
            Client.Close()
            RichTextBox1.Text += "Connection Ended" + vbNewLine
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Threading.ThreadPool.QueueUserWorkItem(AddressOf SendToServer, TextBox1.Text)
    End Sub

    '*******************************************************************************


    Private Sub rollBtnP1_Click(sender As Object, e As EventArgs) Handles rollBtnP1.Click
        ''diceTimer.Start()
        rollBtnP2.Enabled = True
        'rollDiceP1()

        ''scoreP1 = random
        ''  playerTwo.Visible = True
        ''rollBtnP1.Enabled = False
        ''rollBtnP2.Enabled = True
        ''Label1.Visible = False
        ''Label2.Visible = True

        'If (player1 = True) Then
        '    'x1 += 60
        '    'pieceP1.Location = New Point(x1, y1)
        '    'stepLimiter()
        '    Mover()

        'End If
        'If (diceLabel.Text = "6" AndAlso player1 = False) Then
        '    pieceP1.Visible = True
        '    '    playerOne.Visible = False

        '    'pieceP1.Location = New Point(x1, y1)

        '    player1 = True
        '    'p += 1
        '    'pos(p) = 1
        'End If
        'snakeBites()
        'ladder()
    End Sub

    Private Sub rollBtnP2_Click(sender As Object, e As EventArgs) Handles rollBtnP2.Click
        rollDiceP2()

        TextBox1.Text = diceLabel.Text
        Threading.ThreadPool.QueueUserWorkItem(AddressOf SendToServer, TextBox1.Text)



        rollBtnP1.Enabled = True
        rollBtnP2.Enabled = False

        Label4.Visible = False
        'scoreP2 = random
        'pieceP2.Visible = True

        'playerOne.Visible = True
        '  playerTwo.Visible = False
        'rollBtnP1.Enabled = True
        'rollBtnP2.Enabled = False
        ' Label1.Visible = True
        'Label2.Visible = False
        'pieceP2.Location = New Point(x2, y2)

        'firstMove()
        '********************************

        If (player2 = True) Then
            'x1 += 60
            'pieceP1.Location = New Point(x1, y1)
            'stepLimiter()
            ' Mover(q, x2, y2, pieceP2)
            Mover2()

        End If
        If (diceLabel.Text = "6" AndAlso player2 = False) Then
            pieceP2.Visible = True
            '    playerTwo.Visible = False

            'pieceP1.Location = New Point(x1, y1)

            player2 = True
            'p += 1
            'pos(p) = 1
        End If
        'snakeBites(q, x2, y2, pieceP2)
        'ladder(q, x2, y2, pieceP2)
        snakeBites2()
        ladder2()

    End Sub
    Private Sub rollDiceP1()
        Randomize()
        random = (Rnd() * 5) + 1
        dice = random
        diceP1.Image = My.Resources.ResourceManager.GetObject("_" & random)
        diceP1.SizeMode = PictureBoxSizeMode.StretchImage
        diceLabel.Text = random.ToString
        'diceTimer.Start()

        prevVal.Text = positionVal.Text

    End Sub
    Private Sub rollDiceP2()
        Randomize()
        random = (Rnd() * 5) + 1
        dice = random
        diceP1.Image = My.Resources.ResourceManager.GetObject("_" & random)
        diceP1.SizeMode = PictureBoxSizeMode.StretchImage
        diceLabel.Text = random.ToString
        'diceTimer.Start()

        prevVal2.Text = positionVal2.Text

    End Sub

    Private Sub Mover()

        If ((dice + p) > 100) Then

            Return
        End If

        If (dice + p) = 100 Then

            x1 += 60 * dice
            pieceP1.Location = New Point(x1, y1)
            p += 1 * dice
            pos(p) = 1
            positionVal.Text = p.ToString
            MsgBox("You Lost", MsgBoxStyle.OkOnly, "Defeated")
            rollBtnP1.Enabled = False
            rollBtnP2.Enabled = False

            Return
        End If

        For i As Integer = 1 To dice

            If p = 10 Then
                wait(500)
                x1 = 4
                y1 = 447
            ElseIf p = 20 Then
                wait(500)
                x1 = 4
                y1 = 393
            ElseIf p = 30 Then
                wait(500)
                x1 = 4
                y1 = 339
            ElseIf p = 40 Then
                wait(500)
                x1 = 4
                y1 = 285
            ElseIf p = 50 Then
                wait(500)
                x1 = 4
                y1 = 231
            ElseIf p = 60 Then
                wait(500)
                x1 = 4
                y1 = 177
            ElseIf p = 70 Then
                wait(500)
                x1 = 4
                y1 = 123
            ElseIf p = 80 Then
                wait(500)
                x1 = 4
                y1 = 69
            ElseIf p = 90 Then
                wait(500)
                x1 = 4
                y1 = 15

            Else
                wait(500)
                x1 = x1 + 60

            End If
            pieceP1.Location = New Point(x1, y1)
            p += 1
            pos(p) = 1
            positionVal.Text = p.ToString

        Next
    End Sub

    ' ****************************** FOR SNAKES **************************************
    Private Sub snakeBites()

        If p = 25 Then
            wait(500)
            x1 = 244
            y1 = 501
            p = 5

        ElseIf p = 34 Then
            wait(500)
            x1 = 4
            y1 = 501
            p = 1
        ElseIf p = 47 Then
            wait(500)
            x1 = 484
            y1 = 447
            p = 19
        ElseIf p = 65 Then
            wait(500)
            x1 = 64
            y1 = 231
            p = 52
        ElseIf p = 87 Then
            wait(500)
            x1 = 364
            y1 = 231
            p = 57
        ElseIf p = 91 Then
            wait(500)
            x1 = 4
            y1 = 177
            p = 61
        ElseIf p = 99 Then
            wait(500)
            x1 = 484
            y1 = 177
            p = 69
        End If
        pieceP1.Location = New Point(x1, y1)
        positionVal.Text = p.ToString
        '*********************************************************************************
    End Sub

    Private Sub ladder()

        ' ****************************** FOR LADDER **************************************
        If p = 3 Then
            wait(500)
            x1 = 4
            y1 = 231
            p = 51
        ElseIf p = 6 Then
            wait(500)
            x1 = 364
            y1 = 393
            p = 27
        ElseIf p = 20 Then
            wait(500)
            x1 = 544
            y1 = 177
            p = 70
        ElseIf p = 36 Then
            wait(500)
            x1 = 244
            y1 = 231
            p = 55
        ElseIf p = 63 Then
            wait(500)
            x1 = 244
            y1 = 20
            p = 95
        ElseIf p = 68 Then
            wait(500)
            x1 = 424
            y1 = 20
            p = 98
        End If
        pieceP1.Location = New Point(x1, y1)
        positionVal.Text = p.ToString
        '*********************************************************************************

    End Sub

    Private Sub Mover2()

        If ((dice + q) > 100) Then

            Return
        End If

        If (dice + q) = 100 Then

            x2 += 60 * dice
            pieceP2.Location = New Point(x2, y2)
            q += 1 * dice
            pos(q) = 1
            positionVal2.Text = q.ToString
            MsgBox("You win", MsgBoxStyle.OkOnly, "Congratulations")
            rollBtnP1.Enabled = False
            rollBtnP2.Enabled = False

            Return
        End If

        For i As Integer = 1 To dice

            If q = 10 Then
                wait(500)
                x2 = 4 + 32
                y2 = 447
            ElseIf q = 20 Then
                wait(500)
                x2 = 4 + 32
                y2 = 393
            ElseIf q = 30 Then
                wait(500)
                x2 = 4 + 32
                y2 = 339
            ElseIf q = 40 Then
                wait(500)
                x2 = 4 + 32
                y2 = 285
            ElseIf q = 50 Then
                wait(500)
                x2 = 4 + 32
                y2 = 231
            ElseIf q = 60 Then
                wait(500)
                x2 = 4 + 32
                y2 = 177
            ElseIf q = 70 Then
                wait(500)
                x2 = 4 + 32
                y2 = 123
            ElseIf q = 80 Then
                wait(500)
                x2 = 4 + 32
                y2 = 69
            ElseIf q = 90 Then
                wait(500)
                x2 = 4 + 32
                y2 = 15

            Else
                wait(500)
                x2 = x2 + 60

            End If
            pieceP2.Location = New Point(x2, y2)
            q += 1
            'pos(q) = 1
            positionVal2.Text = q.ToString



        Next
    End Sub
    ' ****************************** FOR SNAKES **************************************
    Private Sub snakeBites2()

        If q = 25 Then
            wait(500)
            x2 = 244 + 32
            y2 = 501
            p = 5

        ElseIf q = 34 Then
            wait(500)
            x2 = 4 + 32
            y2 = 501
            q = 1
        ElseIf q = 47 Then
            wait(500)
            x2 = 484 + 32
            y2 = 447
            q = 19
        ElseIf q = 65 Then
            wait(500)
            x2 = 64 + 32
            y2 = 231
            q = 52
        ElseIf q = 87 Then
            wait(500)
            x2 = 364 + 32
            y2 = 231
            q = 57
        ElseIf q = 91 Then
            wait(500)
            x2 = 4 + 32
            y2 = 177
            q = 61
        ElseIf q = 99 Then
            wait(500)
            x2 = 484 + 32
            y2 = 177
            q = 69
        End If
        pieceP1.Location = New Point(x1, y1)
        positionVal.Text = p.ToString
        '*********************************************************************************
    End Sub

    Private Sub positionVal_TextChanged(sender As Object, e As EventArgs)
        rollBtnP1.Enabled = False
        rollBtnP2.Enabled = True
    End Sub

    Private Sub ladder2()

        ' ****************************** FOR LADDER **************************************
        If q = 3 Then
            wait(500)
            x2 = 4 + 32
            y2 = 231
            q = 51
        ElseIf q = 6 Then
            wait(500)
            x2 = 364 + 32
            y2 = 393
            q = 27
        ElseIf q = 20 Then
            wait(500)
            x2 = 544 + 32
            y2 = 177
            q = 70
        ElseIf q = 36 Then
            wait(500)
            x2 = 244 + 32
            y2 = 231
            q = 55
        ElseIf q = 63 Then
            wait(500)
            x2 = 244 + 32
            y2 = 20
            q = 95
        ElseIf q = 68 Then
            wait(500)
            x2 = 424 + 32
            y2 = 20
            q = 98
        End If
        pieceP2.Location = New Point(x2, y2)
        positionVal2.Text = q.ToString
        '*********************************************************************************

    End Sub

    Private Sub receiveMove()

    End Sub

    Private Sub wait(ByVal interval As Integer)
        Dim sw As New Stopwatch
        sw.Start()
        Do While sw.ElapsedMilliseconds < interval

            Application.DoEvents()
        Loop
        sw.Stop()
    End Sub


    '*********************************************************************************************

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        Panel4.Visible = False
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs)
        GroupBox1.Visible = True
        RichTextBox2.Visible = False

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs)
        RichTextBox2.Visible = True
        GroupBox1.Visible = False
    End Sub
    Private Sub Button7_Click(sender As Object, e As EventArgs)
        Me.Dispose()
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        Panel4.Visible = False
    End Sub

    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        RichTextBox2.Visible = True
        GroupBox1.Visible = False
    End Sub

    Private Sub Button6_Click_1(sender As Object, e As EventArgs) Handles Button6.Click
        GroupBox1.Visible = True
        RichTextBox2.Visible = False
    End Sub

    Private Sub Button7_Click_1(sender As Object, e As EventArgs) Handles Button7.Click
        Me.Dispose()
    End Sub

    Private Sub positionVal2_TextChanged(sender As Object, e As EventArgs)
        Label4.Visible = False
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        rollBtnP1.Enabled = False
        rollBtnP2.Enabled = True
        'Label4.Visible = True
    End Sub


    Private Sub positionVal_TextChanged_1(sender As Object, e As EventArgs) Handles positionVal.TextChanged
        rollBtnP1.Enabled = False
        rollBtnP2.Enabled = True
        Label4.Visible = True
    End Sub
End Class