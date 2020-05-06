﻿#Region "Microsoft.VisualBasic::4d7eb389619c7bbfb795477151a3444b, vcellkit\Debugger\Debugger.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Module Debugger
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.ModellingEngine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

<Package("vcellkit.debugger", Category:=APICategories.ResearchTools)>
Module Debugger

    <ExportAPI("vcell.summary")>
    Public Sub createDynamicsSummary(inits As Definition, model As CellularModule, dir As String)
        Call Dynamics.Summary.summary(inits, model, dir)
    End Sub
End Module
