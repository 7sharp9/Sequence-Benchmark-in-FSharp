open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open System.Collections
open System.Collections.Generic
open System.Linq

module Sequence =

    let sequence_comp (list: _ list list) =
        List.foldBack (fun item state -> [ for x in item do for xs in state -> x::xs ]) list [[]]

    let sequence (list: _ list list) =
        List.foldBack (fun item state -> item |> List.collect (fun x -> state |> List.map (fun xs -> x::xs))) list [[]]

    let sequencefold_comp (list: _ list list) =
        List.fold (fun state item -> 
            [for x in item do for xs in state -> x::xs ]) [[]] list

    let sequencefold (lists: _ list list) =
        lists
        |> List.fold (fun acc item -> item |> List.collect (fun x -> acc |> List.map (fun xs -> x::xs))) [[]]

    let sequenceRecursive (list: _ list list) =
        let rec sequence' list =
            match list with
            | [] -> seq { [] }
            | x :: xs -> seq { for sub in sequence' xs do for item in x -> item :: sub }
        sequence' list |> Seq.toList

    let sequenceRecursiveL (list: 'a list list) =
        let rec sequence' list =
            match list with
            | [] -> [[]]
            | x :: xs -> [for sub in sequence' xs do for item in x -> item :: sub ]
        sequence' list 

[<MemoryDiagnoser>]
type ParsingBench() =
    let x = [1; 2; 3; 4]
    let y = [10; 20; 30; 40]
    let z = [100; 200; 300; 400]
    let w = [10000; 2000; 3000; 4000]
    let a = [100000; 20000; 300000; 400000]
    let b = [100000; 20000; 300000; 400000]
    let c = [100000; 20000; 300000; 400000]
    let d = [100000; 20000; 300000; 400000]
    let e = [100000; 20000; 300000; 400000]
    let f = [100000; 20000; 300000; 400000]
    let all6 = [x; y; z; w; a; b; c; d; e; f]

    [<Benchmark(Baseline=true)>]
    member __.Sequence() =
        Sequence.sequence all6

    [<Benchmark>]
    member __.Sequence_Comp() =
        Sequence.sequence_comp all6

    [<Benchmark>]
    member __.Sequence_fold() =
        Sequence.sequencefold all6

    [<Benchmark>]
    member __.sequencefold_comp() =
        Sequence.sequencefold_comp all6

    [<Benchmark>]
    member __.SequenceRecursive() =
        Sequence.sequenceRecursive all6

    [<Benchmark>]
    member __.SequenceRecursiveL() =
        Sequence.sequenceRecursiveL all6



[<EntryPoint>]
let main _ =
    let summary = BenchmarkRunner.Run<ParsingBench>()
    printfn "%A" summary
    0 // Return an integer exit code