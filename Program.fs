// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System.IO
open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats

type Pixel = {
    R:byte
    G:byte
    B:byte
    A:byte
} 
with override this.ToString() = sprintf "%i %i %i %i" this.R this.G this.B this.A
let pixel (r,g,b,a) = {R = r;G = g;B = b;A = a}
let pixelFromRgba (rgba:Rgba32) = {R = rgba.R;G = rgba.G;B = rgba.B;A = rgba.A}
let loadFileStream file = file |> File.ReadAllBytes
let loadImage filePath = Image.Load(loadFileStream filePath)
let getRowSpan y (image:Image<Rgba32>) = [|
    let span = image.GetPixelRowSpan(y)
    for i in span.ToArray() do
        yield pixelFromRgba i
|]
let pixelMatrix (image:Image<Rgba32>) = [|    
    for y in [|0..image.Height - 1|] do
        yield getRowSpan y image
        
|]
let pixelArray (image:Image<Rgba32>) = [|
    for y in [0..image.Height - 1] do
        for span in image.GetPixelRowSpan(y).ToArray() do
             yield pixelFromRgba span
|]
let max arr = arr |> Array.max
let min arr = arr |> Array.min
let averagePixel pixel = (pixel.R + pixel.G + pixel.B) / (byte 3)
let lightness pixel = (max [|pixel.R;pixel.G;pixel.B|]) + (min [|pixel.R;pixel.G;pixel.B|]) / (byte 2)
let luminosity pixel = (0.21 * (float pixel.R)) + (0.72 * (float pixel.G)) + (0.07 * (float pixel.B))
let transformMatrix transformation matrix = matrix |> Array.map transformation
let alphabet = """`^\",:;Il!i~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"""
let pixelToAscii pixel = int (((pixel / 255.0) * 100.0) % (float alphabet.Length))
let asciiImage arr = arr |> Array.map (fun pixel -> alphabet |> Seq.item (pixelToAscii pixel))
    
[<EntryPoint>]
let main argv =    
    let image = loadImage "imagem_teste.png"    
    let arr = pixelArray image
    let averagedPixels = arr |> Array.map luminosity  
    let asciiPixelsIndex = averagedPixels |> Array.map pixelToAscii
    // printfn "%A" asciiPixelsIndex
    let ascii = (asciiImage (arr |> Array.map luminosity)) |> Array.map string |> String.concat ""
    printfn "%s" ascii
    // for chr in ascii do
    //     printfn "%c" chr
    0 // return an integer exit code