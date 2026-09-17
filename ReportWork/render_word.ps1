param([string]$InputFile = 'Report\My Report Completed.docx', [string]$OutputFolder = 'ReportWork\render')
$ErrorActionPreference = 'Stop'
$inputPath = (Resolve-Path -LiteralPath $InputFile).Path
$outputPath = Join-Path (Get-Location) $OutputFolder
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    $doc = $word.Documents.Open($inputPath)
    $doc.Fields.Update() | Out-Null
    foreach ($toc in $doc.TablesOfContents) { $toc.Update() }
    $doc.Repaginate()
    foreach ($toc in $doc.TablesOfContents) { $toc.UpdatePageNumbers() }
    $doc.Save()
    $pdf = Join-Path $outputPath 'report.pdf'
    $doc.ExportAsFixedFormat($pdf,17)
    Write-Output "Pages: $($doc.ComputeStatistics(2))"
    $doc.Close(0)
} finally { $word.Quit() }
& 'C:\Users\Shan\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\poppler\Library\bin\pdftoppm.exe' -png -r 110 $pdf (Join-Path $outputPath 'page')
