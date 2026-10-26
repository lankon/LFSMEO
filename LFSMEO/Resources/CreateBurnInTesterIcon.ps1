Add-Type -AssemblyName System.Drawing

$outputDirectory = $PSScriptRoot
$pngPath = Join-Path $outputDirectory 'BurnInTester.png'
$icoPath = Join-Path $outputDirectory 'BurnInTester.ico'
$canvasSize = 512
$bitmap = New-Object System.Drawing.Bitmap($canvasSize, $canvasSize, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
$graphics.Clear([System.Drawing.Color]::Transparent)

function New-Brush([string]$hex) {
    return New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml($hex))
}

function Fill-RoundedRectangle($g, $brush, [float]$x, [float]$y, [float]$width, [float]$height, [float]$radius) {
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $diameter = $radius * 2
    $path.AddArc($x, $y, $diameter, $diameter, 180, 90)
    $path.AddArc($x + $width - $diameter, $y, $diameter, $diameter, 270, 90)
    $path.AddArc($x + $width - $diameter, $y + $height - $diameter, $diameter, $diameter, 0, 90)
    $path.AddArc($x, $y + $height - $diameter, $diameter, $diameter, 90, 90)
    $path.CloseFigure()
    $g.FillPath($brush, $path)
    $path.Dispose()
}

function Draw-HeatWave($g, [float]$startX, [float]$endX, [float]$offset) {
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $path.AddBezier($startX, 98, $startX - 20, 76, $startX + 23, 56, $startX + 5, 32 + $offset)
    $path.AddBezier($startX + 5, 32 + $offset, $startX - 5, 15, $endX, 10 + $offset, $endX, 0 + $offset)
    $pen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 255, 125, 48), 13)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    $g.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
}

$navy = New-Brush '#263341'
$navyLight = New-Brush '#3A4A5B'
$outline = New-Brush '#17212C'
$cavity = New-Brush '#3A211B'
$hotDark = New-Brush '#A83D23'
$hot = New-Brush '#FF7138'
$hotBright = New-Brush '#FFB34F'
$cream = New-Brush '#FFE0A0'
$muted = New-Brush '#8795A2'

# Rising heat waves make the cabinet read as a heater at desktop-icon size.
Draw-HeatWave $graphics 194 205 -1
Draw-HeatWave $graphics 278 289 1
Draw-HeatWave $graphics 360 371 -2

# Single compact rack cabinet silhouette.
Fill-RoundedRectangle $graphics $outline 53 92 406 386 38
Fill-RoundedRectangle $graphics $navy 62 99 388 370 31
Fill-RoundedRectangle $graphics $navyLight 78 109 354 42 12
Fill-RoundedRectangle $graphics $cavity 95 163 309 281 18
Fill-RoundedRectangle $graphics $navy 405 159 29 287 9

# Three broad heated test shelves, each kept to a few bold shapes.
foreach ($y in @(190, 270, 350)) {
    Fill-RoundedRectangle $graphics $outline 111 ($y + 5) 273 61 10
    Fill-RoundedRectangle $graphics $hotDark 120 ($y + 13) 255 42 8
    Fill-RoundedRectangle $graphics $hot 126 ($y + 17) 243 29 6
    Fill-RoundedRectangle $graphics $hotBright 139 ($y + 20) 176 9 4
    Fill-RoundedRectangle $graphics $cream 143 ($y + 22) 91 5 2
    $led = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 211, 91))
    $graphics.FillEllipse($led, 345, ($y + 20), 13, 13)
    $led.Dispose()
    Fill-RoundedRectangle $graphics $muted 112 ($y + 67) 270 7 3
}

# Minimal temperature control: one large hot dial and one status light.
$dialRing = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 20, 30, 40), 8)
$graphics.DrawEllipse($dialRing, 361, 111, 42, 42)
$dialRing.Dispose()
$graphics.FillEllipse($hotBright, 367, 117, 30, 30)
$graphics.FillEllipse($outline, 379, 121, 7, 15)
$graphics.FillEllipse($cream, 385, 117, 7, 7)
$graphics.FillEllipse($hot, 105, 123, 16, 16)

# Small cabinet feet anchor the silhouette without adding busy detail.
Fill-RoundedRectangle $graphics $outline 93 463 42 19 8
Fill-RoundedRectangle $graphics $outline 378 463 42 19 8

# Keep a large transparent PNG source and package common Windows icon sizes.
$bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
$sizes = @(16, 24, 32, 48, 64, 128, 256)
$pngImages = @()
foreach ($size in $sizes) {
    $resized = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $resizeGraphics = [System.Drawing.Graphics]::FromImage($resized)
    $resizeGraphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $resizeGraphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $resizeGraphics.DrawImage($bitmap, 0, 0, $size, $size)
    $stream = New-Object System.IO.MemoryStream
    $resized.Save($stream, [System.Drawing.Imaging.ImageFormat]::Png)
    $pngImages += ,$stream.ToArray()
    $stream.Dispose()
    $resizeGraphics.Dispose()
    $resized.Dispose()
}

$iconStream = New-Object System.IO.FileStream($icoPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write)
$writer = New-Object System.IO.BinaryWriter($iconStream)
$writer.Write([UInt16]0)
$writer.Write([UInt16]1)
$writer.Write([UInt16]$sizes.Count)
$imageOffset = 6 + (16 * $sizes.Count)
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $size = $sizes[$i]
    $writer.Write([byte]$(if ($size -eq 256) { 0 } else { $size }))
    $writer.Write([byte]$(if ($size -eq 256) { 0 } else { $size }))
    $writer.Write([byte]0)
    $writer.Write([byte]0)
    $writer.Write([UInt16]1)
    $writer.Write([UInt16]32)
    $writer.Write([UInt32]$pngImages[$i].Length)
    $writer.Write([UInt32]$imageOffset)
    $imageOffset += $pngImages[$i].Length
}
foreach ($imageBytes in $pngImages) {
    $writer.Write($imageBytes)
}
$writer.Dispose()
$iconStream.Dispose()

$graphics.Dispose()
$bitmap.Dispose()
