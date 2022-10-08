namespace Aspose.NLP.FSharp

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Text.RegularExpressions
open Aspose.NLP.Core.Text

module Core =

    // Perform an asynchronous read of a file using 'async'
    let readDataFromFileAsync (path: string) =
        async {
            let! text = File.ReadAllLinesAsync(path) |> Async.AwaitTask
            return text
        }

    let getGrammarData() =
        let currentDir = Directory.GetCurrentDirectory()
        let path = Path.Combine(currentDir, "grammar.txt")
        let data = readDataFromFileAsync(path) |> Async.RunSynchronously
        data

    let filterGrammar(words_sum : seq<string * int * float * int>) =
        let grammar_words = getGrammarData()
        let wsumf = words_sum |> Seq.filter(fun (w, _, _, _) -> grammar_words |> Seq.exists (fun gw -> not(gw.Contains(w))))
        wsumf

    let excludeGrammarFromText(text : string) =
        let grammar_words = getGrammarData()
        let mutable tresult = text
        let excluded = grammar_words |> Seq.iter (fun gw -> tresult <- tresult.Replace(gw, String.Empty))
        excluded
    
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

    let swords_sum_exclude_grammar (text:string) (exclude_grammar:bool) =
        let swords = swords_count(text)
        if exclude_grammar then
           filterGrammar(swords)
        else
           swords

    let accum = new HashSet<string>()

    let removeSpecial s = 
        let str = Regex.Replace(s, "[^0-9a-zA-Z]+", " ", RegexOptions.IgnoreCase ||| RegexOptions.CultureInvariant ||| RegexOptions.Compiled)
        Regex.Replace(str, "\\s+", " ", RegexOptions.IgnoreCase ||| RegexOptions.Compiled)

    let phrase_gen (sentense:string) (n:int) =
        let words = swords sentense |> Seq.map (fun s -> removeSpecial s) |> Seq.map (fun s -> s.Trim()) |> Seq.filter (fun s -> s.Length <> 0) 
        let rec loop words n =
            if Seq.length words >= n then
               let phrase  = words |> Seq.take n |> String.concat " "
               let current = words |> Seq.skip 1
               accum.Add(phrase) |> ignore
               match Seq.length current with
               | 0 -> 0
               | x when x < n -> 1
               | _ -> loop current n
            else
               1
        loop words n

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

    let dict = new Dictionary<string, int>()
    let applyc (dict:Dictionary<string,int>) key c = if(dict.ContainsKey(key)) then dict.[key] <- c else dict.[key] <- c

    let matches input (phrase:string) =
        let reg_expr = @"\b"+ phrase.Trim() + @"\b"
        Regex.Matches(input, reg_expr) 
        |> Seq.cast<Match>
        |> Seq.groupBy (fun m -> m.Value)
        |> Seq.map (fun (value, groups) -> value, (groups |> Seq.length))

    let phrase_count (text:string) (n:int) =
        let sentenses = sentenses text |> Seq.map (fun s -> s.Trim()) |> Seq.filter (fun s -> s.Length <> 0)
        let words = sentenses |> Seq.collect (fun s -> swords s) |> Seq.map (fun s -> removeSpecial s) |> Seq.map (fun s -> s.Trim()) |> Seq.filter (fun s -> s.Length <> 0) 
        let rec loop words n =
            if Seq.length words >= n then
                let phrase  = words |> Seq.take n |> String.concat " "
                let current = words |> Seq.skip 1
                if not (dict.ContainsKey(phrase)) then
                   matches text phrase |> Seq.iter (fun (word, count) -> applyc dict word count)
                   match Seq.length current with
                   | 0 -> 0
                   | x when x < n -> 1
                   | _ -> loop current n
                else
                   loop current n
            else
               1
        loop words n

    let phrasegen_count (text:string) (n:int) =
        let sentenses = sentenses text
        System.Console.WriteLine($"phrasegen_count sentenses count - {sentenses.Length}")

        let sw = System.Diagnostics.Stopwatch()
        sw.Start()

        accum.Clear()

        // some calculations
        sentenses |> Seq.iter (fun s -> phrase_gen s n |> ignore)
        let words = accum |> Seq.toArray

        System.Console.WriteLine($"phrasegen_count({n}) phrase_gen Words Count - {words.Count()}")

        let words = (sentenses |> Seq.collect (fun s -> phrase_gen_accum s n)).Distinct() |> Seq.toArray 

        System.Console.WriteLine($"phrasegen_count({n}) phrase_gen_accum Words Count - {words.Count()}")

        accum.Clear()

        sw.Stop()
        System.Console.WriteLine("Words gen Time elapsed: {0}", sw.Elapsed)

        let helper = new TextWordCounterHelper();

        //sw.Restart()
        //
        //let cdictr = helper.FastWordOcurrensiesInSentensesCounter(sentenses, words.Distinct().ToArray())
        //sw.Stop()
        //System.Console.WriteLine("WordCounterTextHelper.FastWordOcurrensiesInSentensesCounter Time elapsed: {0} {1}", sw.Elapsed, cdictr.Values.Count)

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

