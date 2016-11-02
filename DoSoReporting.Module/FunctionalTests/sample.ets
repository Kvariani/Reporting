;#DropDB DoSoReportingEasyTest

#Application DoSoReporting.Win
#Application DoSoReporting.Web
#IfDef DoSoReporting.Win

*Action About...
 
*Action OK

#EndIf

#IfDef DoSoReporting.Web
!Action Navigation(My Details)
#EndIf
