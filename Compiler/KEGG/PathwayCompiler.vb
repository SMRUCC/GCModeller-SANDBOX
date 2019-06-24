﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' Create virtual cell xml file model from KEGG pathway data
''' </summary>
Public Module PathwayCompiler

    <Extension>
    Public Function CompileOrganism(replicons As Dictionary(Of String, GBFF.File), keggModel As OrganismModel) As VirtualCell
        Dim taxonomy As Taxonomy = replicons.getTaxonomy
        Dim Kofunction As Dictionary(Of String, String) = keggModel.KoFunction
        Dim genotype As New Genotype With {
            .centralDogmas = replicons _
                .GetCentralDogmas(Kofunction) _
                .ToArray
        }
        Dim cell As New CellularModule With {
            .Taxonomy = taxonomy,
            .Genotype = genotype
        }

        Return cell.ToMarkup(replicons, keggModel)
    End Function

    <Extension>
    Private Function ToMarkup(cell As CellularModule, genomes As Dictionary(Of String, GBFF.File), kegg As OrganismModel) As VirtualCell
        Dim KOgenes As Dictionary(Of String, CentralDogma) = cell _
            .Genotype _
            .centralDogmas _
            .Where(Function(process)
                       Return Not process.IsRNAGene AndAlso Not process.orthology.StringEmpty
                   End Function) _
            .ToDictionary(Function(term) term.geneID)
        Dim pathwayCategory = BriteHEntry.Pathway.LoadFromResource
        Dim pathwayIndex = kegg.genome.ToDictionary(Function(map) map.briteID)
        Dim maps As FunctionalCategory() = pathwayCategory _
            .GroupBy(Function(pathway) pathway.class) _
            .Select(Function(category)
                        Dim pathways = category _
                            .Where(Function(entry) pathwayIndex.ContainsKey(entry.EntryId)) _
                            .Select(Function(entry)
                                        Dim map = pathwayIndex(entry.EntryId)

                                        Return New Pathway With {
                                            .ID = map.EntryId,
                                            .name = map.name,
                                            .enzymes = map.genes _
                                                .Select(Function(gene)
                                                            Return New [Property] With {
                                                                .name = gene.name.GetTagValue(":", trim:=True).Value,
                                                                .comment = gene.name,
                                                                .value = gene.text.Split.First
                                                            }
                                                        End Function) _
                                                .ToArray
                                        }
                                    End Function) _
                            .ToArray

                        Return New FunctionalCategory With {
                            .category = category.Key,
                            .pathways = pathways
                        }
                    End Function) _
            .ToArray

        Return New VirtualCell With {
            .taxonomy = cell.Taxonomy,
            .genome = New Genome With {
                .replicons = cell _
                    .populateReplicons(genomes) _
                    .ToArray
            },
            .MetabolismStructure = New MetabolismStructure With {
                .maps = maps
            }
        }
    End Function
End Module
