Public Class Temp
    ' '\ This function converts a decimal number of inches to a text string like 5'-6 1/2"
    'public Function CFeet(Decimal_Inches, Optional Enter_16_32_64_Etc__To_Round_Inches_To__Fraction_Of_Inch) As string
    '     '\ These variables are used to convert the decimal inches to the number  of fractional units. 
    '     '\ For example 6" would convert to 96 16ths
    '     Dim iNumUnits As Long '\ converted value   = 96 in example
    '     Dim iUnit As Double   '\ unit used to convert  = 1/16 in example

    '     '\ These varibles are used to hold calculated values that will become
    '     '\ part of the text string
    '     Dim iFeet As Integer
    '     Dim iInches As Integer
    '     Dim dFraction As Double
    '     Dim sFtSymbol As String

    '     '\ These variables are used to assign shorter names to input values
    '     Dim vVal As Object
    '     Dim vDenominator As Object

    '     '\ First we assign shorter names
    '     vVal = Decimal_Inches
    '     vDenominator = Enter_16_32_64_Etc__To_Round_Inches_To__Fraction_Of_Inch

    '     '\ If no denominator value was supplied, we will round to 1/9999 of inch
    '     If IsMissing(vDenominator) Then
    '         iUnit = 1 / 9999
    '     Else
    '         iUnit = 1 / vDenominator
    '     End If

    '     '\ Now we calculate the number of fractional units in the input value
    '     '\ Example 6 inches = 96 16ths
    '     iNumUnits = (vVal / iUnit)

    '     '\ We prepare each part of text string
    '     iFeet = Fix(iNumUnits / (12 / iUnit))
    '     iInches = Fix((iNumUnits Mod (12 / iUnit)) * iUnit)
    '     dFraction = (iNumUnits Mod (1 / iUnit)) * iUnit
    '     If iFeet <> 0 Then sFtSymbol = "'-"

    '     '\ Finally we format and return text string
    '     CFeet = Application.Text(iFeet, "##") & sFtSymbol & Application.Text(iInches + dFraction, "# ##/##\""")

    ' End Function
End Class
