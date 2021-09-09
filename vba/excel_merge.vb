'检索文件夹下所有aaa-bbb奖金表.xls和aaa-bbb奖金表.xlsx
'每个文件生成bbb奖金列
'在单表仅有一列数据时仍有问题

Public START_R As Integer, nameC As String, dataC As String

Sub ImportMonthWb()

    Application.ScreenUpdating = False

    'START_R = 3 '第一行数据的行号
    'nameC = "c" '姓名列号
    'dataC = "d" '数据列号

    START_R = 5 '第一行数据的行号
    nameC = "c" '姓名列号
    dataC = "e" '数据列号

    Dim thisPath As String, thisName As String, fileName As String
    Dim name As String, value As Integer
    Dim fileCount As Integer, nameCount As Long, index As Integer
    Dim wb As Workbook, sht As Sheets

    Dim names As Object
    Set names = CreateObject("scripting.dictionary")

    thisName = ThisWorkbook.name
    thisPath = ThisWorkbook.Path
    
    fileName = Dir(thisPath & "\" & "*奖金表.xls")
    fileCount = 0
    index = 0
    nameCount = 0
    
    Do While fileName <> ""
        fileCount = fileCount + 1
        fileName = Dir
    Loop
    fileName = Dir(thisPath & "\" & "*奖金表.xls*")

    Dim arr() As Integer
    Dim fileNameArr() As String
    ReDim fileNameArr(fileCount)

    Do While fileName <> ""
        If thisName <> fileName Then
            '遍历奖金表.xls文件
            'Debug.Print fileName; "!"
            fileNameArr(index) = fileName

            Set wb = Workbooks.Open(thisPath & "\" & fileName)

            With ActiveWorkbook.Worksheets(1)
                'Range("c3")
                nameCount = Range(nameC & START_R).End(xlDown).Row
                If Range(nameC & (START_R + 1)).value = "" Then
                    nameCount = START_R
                End If

                For r = START_R To nameCount Step 1
                    ReDim arr(fileCount)
                    '遍历姓名
                    'Cells(r, "c").Value是姓名
                    'Cells(r, "d").Value是奖金
                    'Index代表遍历的第n个文件
                    name = Cells(r, nameC).value
                    'Debug.Print r; nameC
                    value = Cells(r, dataC).value

                    If names.Exists(name) Then
                        arr = names(name)
                        arr(index) = value
                        names.Item(name) = arr
                        'Debug.Print Cells(r, "c").value; Cells(r, "d").value; names.Item(Cells(r, "c").value)(index)
                        'Debug.Print Cells(r, "c").Value; " and"; index; "and"; Cells(r, "d").Value
                    Else
                        arr(index) = value
                        names.Add name, arr
                        'names(Cells(r, "c").Value) = arr
                    End If
                Next
                'Debug.Print .Name
            End With
            index = index + 1
            Workbooks(fileName).Close SaveChanges:=True
        End If
        fileName = Dir
    Loop

    '此时字典names
    '存储着key姓名，value是数组，数组长为目录下文件的个数
    '每个value中数组的每个值是根据遍历到的
    'Debug.Print names("b")(0)
    'Debug.Print names("b")(1)
    'Debug.Print names("b")(2)
    Dim newR As Integer, dotPos As Integer, dashPos As Integer, newColName As String
    newR = 2
    Cells(1, 1).value = "编号"
    Cells(1, 2).value = "姓名"

    With ThisWorkbook.Worksheets(1)
    
        For j = 0 To fileCount - 1
        'fileNameArr(j)是*-奖金表.xsl*格式
            dotPos = InStr(1, fileNameArr(j), ".")
            dashPos = InStr(1, fileNameArr(j), "-")
            Cells(1, 1 + 2 + j).value = Mid(fileNameArr(j), dashPos + 1, dotPos - dashPos - 2) 'Left(fileNameArr(j), dotPos - 2)
        Next
        
        Cells(1, 1 + 2 + fileCount).value = "总计"

        For Each k In names.keys()
            'Debug.Print k
            'K是key，姓名
            'names(K)(i)是对应项的奖金
            Cells(newR, 1).value = newR - 1
            Cells(newR, 2).value = k
            For i = 0 To fileCount - 1
                Cells(newR, 3 + i).value = names(k)(i)
            Next

            '每行总计=SUM(R newR C3:R newR C fileCount - 1) =SUM(R2C3:R2C5)
            Cells(newR, 3 + fileCount).value = "=SUM(R" & newR & "C3:R" & newR & "C" & fileCount + 2 & ")"

            newR = newR + 1
        Next

        Cells(newR, 1).value = "合计"
        For k = 0 To fileCount - 1
            '每列合计=SUM(R2C k + 1: R newR - 1 C k + 1)
            Cells(newR, 3 + k).value = "=SUM(R2C" & 3 + k & ":R" & newR - 1 & "C" & 3 + k & ")"
        Next

        '最终总计=SUM(R newR C3: R newR C fileCount + 2)
        Cells(newR, fileCount + 3).value = "=SUM(R" & newR & "C3:R" & newR & "C" & fileCount + 2 & ")"

    End With
End Sub
