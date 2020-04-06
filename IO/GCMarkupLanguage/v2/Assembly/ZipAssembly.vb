﻿Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace v2

    ''' <summary>
    ''' 将所有的模型数据都放置在同一个XML文件之中
    ''' 会因为文件过大而难于调试
    ''' 在这里进行元素的分块存储
    ''' </summary>
    Public Class ZipAssembly

        Dim zip As ZipArchive

        Public Function GetText(path As String) As String
            Dim fileKey As String = path.Trim("/"c, "\"c)
            Dim file As ZipArchiveEntry = zip.Entries _
                .Where(Function(f)
                           Return f.Name.Trim("/"c, "\"c) = fileKey
                       End Function) _
                .FirstOrDefault

            If file Is Nothing Then
                Return Nothing
            Else
                Return file _
                    .Decompress _
                    .ToArray _
                    .DoCall(AddressOf Encoding.UTF8.GetString)
            End If
        End Function

        Public Function GetComponent(Of T)(path As String) As T()
            Dim xml As String = GetText(path)

            If xml.StringEmpty Then
                Return Nothing
            Else
                Return xml.LoadFromXml(Of ZipComponent(Of T)).AsEnumerable
            End If
        End Function

        Public Function CreateVirtualCellXml() As VirtualCell
            Return New VirtualCell With {
                .genome = New Genome With {
                    .regulations = GetComponent(Of transcription)($"{NameOf(VirtualCell.genome)}\{NameOf(Genome.regulations)}.Xml"),
                    .replicons = GetComponent(Of replicon)($"{NameOf(VirtualCell.genome)}\{NameOf(Genome.replicons)}.Xml")
                },
                .metabolismStructure = New MetabolismStructure With {
                    .compounds = GetComponent(Of Compound)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.compounds)}.Xml"),
                    .enzymes = GetComponent(Of Enzyme)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.enzymes)}.Xml"),
                    .maps = GetComponent(Of FunctionalCategory)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.maps)}.Xml"),
                    .reactions = New ReactionGroup With {
                        .enzymatic = GetComponent(Of Reaction)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.reactions)}\{NameOf(ReactionGroup.enzymatic)}.Xml"),
                        .etc = GetComponent(Of Reaction)($"{NameOf(VirtualCell.metabolismStructure)}\{NameOf(MetabolismStructure.reactions)}\{NameOf(ReactionGroup.etc)}.Xml")
                    }
                },
                .taxonomy = GetText($"{NameOf(VirtualCell.taxonomy)}.json").LoadJSON(Of Taxonomy),
                .properties = GetText($"{NameOf(VirtualCell.properties)}.json").LoadJSON(Of CompilerServices.[Property])
            }
        End Function

        Public Shared Function WriteZip(vcell As VirtualCell, zip As String) As Boolean
            Return ZipWriter.WriteZip(vcell, zip)
        End Function
    End Class
End Namespace