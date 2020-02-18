open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

module Sequence =

    let sequence_comp (list: _ list list) =
        List.foldBack (fun item state -> [ for x in item do for xs in state -> x::xs ]) list [[]]

    let sequence (list: _ list list) =
        List.foldBack (fun item state -> item |> List.collect (fun x -> state |> List.map (fun xs -> x::xs))) list [[]]
        
    let sequencefold (list: _ list list) =
        List.fold (fun state item ->  [for x in item do for xs in state -> x::xs ]) [[]] list

    let sequencefold2 (list: _ list list) =
        List.fold (fun state item -> item |> List.collect (fun x -> state |> List.map (fun xs -> x::xs))) [[]] list

[<MemoryDiagnoser>]
type ParsingBench() =
    let x = [1; 2; 3; 4]
    let y = [10; 20; 30; 40]
    let z = [100; 200; 300; 400]
    let all = [x; y; z]

    [<Benchmark(Baseline=true)>]
    member __.Sequence() =
        Sequence.sequence all

    [<Benchmark>]
    member __.Sequence_Comp() =
        Sequence.sequence_comp all

    [<Benchmark>]
    member __.Sequence_fold() =
        Sequence.sequencefold all

    [<Benchmark>]
    member __.Sequence_fold2() =
        Sequence.sequencefold2 all


[<EntryPoint>]
let main _ =
    let summary = BenchmarkRunner.Run<ParsingBench>()
    printfn "%A" summary
    0 // Return an integer exit code