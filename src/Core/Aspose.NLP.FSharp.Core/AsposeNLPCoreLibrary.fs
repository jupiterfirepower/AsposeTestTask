namespace Aspose.NLP.FSharp

open System
open System.Linq
open System.Collections.Generic
open System.Text.RegularExpressions
open Aspose.NLP.Core.Text

module Core =

    let removePunctuations sentense = 
        let text = Regex.Replace(sentense, @"[^\w\d\s]", " ", RegexOptions.IgnoreCase ||| RegexOptions.Compiled)
        Regex.Replace(text, "\\s+", " ", RegexOptions.IgnoreCase ||| RegexOptions.Compiled)
   
    let sentenses (text:string) = 
        text.Split([| '.' ; '?' ; '!' |], StringSplitOptions.None) 
        |> Array.map (fun s -> s.Trim())

    let swords (sentense:string) = 
        sentense.Split([| ',' ; ' ' ; ';' ; ':' |], StringSplitOptions.None) 
        |> Array.map (fun s -> s.Trim())

    let swords_count(text:string) = 
        let sentenses = sentenses text |> Seq.map (fun s -> s.Trim()) |> Seq.filter (fun s -> s.Length <> 0)
        let words = sentenses |> Seq.collect (fun s -> swords s)

        let result =
          words
          |> Seq.groupBy id
          |> Seq.map (fun (w, ws) -> (w, Seq.length ws))
          |> Seq.sortBy (snd >> (~-))

        let result = result |> Seq.map (fun (w, c) -> (w, c , Math.Round((float c)/(float (Seq.length result)), 2), 1))

        result

    let removeSpecial s = 
        let str = Regex.Replace(s, "[^0-9a-zA-Z]+", " ", RegexOptions.IgnoreCase ||| RegexOptions.CultureInvariant ||| RegexOptions.Compiled)
        Regex.Replace(str, "\\s+", " ", RegexOptions.IgnoreCase ||| RegexOptions.Compiled)

    let phrase_gen_accum (sentense:string) (n:int) =
        let words = swords sentense |> Seq.map (fun s -> removeSpecial s) |> Seq.map (fun s -> s.Trim()) |> Seq.filter (fun s -> s.Length <> 0) 
        let rec loop words accum n =
            if Seq.length words >= n then
               let phrase  = words |> Seq.take n |> String.concat " "
               let current = words |> Seq.skip 1
               let accum = accum @ [phrase]
               match Seq.length current with
               | 0 -> accum
               | x when x < n -> accum
               | _ -> loop current accum n
            else
               accum
        loop words [] n

    let phrasegen_count (text:string) (n:int) =
        let sentenses = sentenses text
        System.Console.WriteLine($"phrasegen_count sentenses count - {sentenses.Length}")

        let sw = System.Diagnostics.Stopwatch()
        sw.Start()

        let words = (sentenses |> Seq.collect (fun s -> phrase_gen_accum s n)).Distinct() |> Seq.toArray 

        System.Console.WriteLine($"phrasegen_count({n}) phrase_gen_accum Words Count - {words.Count()}")

        sw.Stop()
        System.Console.WriteLine("Words gen Time elapsed: {0}", sw.Elapsed)

        let helper = new TextWordCounterHelper();

        sw.Restart()

        let cdictrp = helper.FastWordCounterParallel(sentenses, words)

        sw.Stop()
        System.Console.WriteLine("WordCounterTextHelper.FastWordCounterParallel Time elapsed: {0} {1}", sw.Elapsed, cdictrp.Values.Count)

        cdictrp

    let pairs (d:Dictionary<'a, 'b>) =
        seq {
            for kv in d do
                yield (kv.Key, kv.Value)
        }

    let numWords text =
        let wordRegex = new Regex(@"\b(\w+)\b", RegexOptions.Compiled)
        wordRegex.Matches(text) 
        |> Seq.cast<Match>
        |> Seq.length

    let numSentences text =     
        let mutable count = 1
        for c in text do
           if c = '.' || c = '!' || c = '?' then
              count <- count + 1
        count

