open Microsoft.FSharp.Reflection
open Fable.React
open Fable.React.Props

module OptionsFromUnion =

    let inline createOptions<'a> (includeEmptyOption) =
        let optionsFromUnion = 
            FSharpType.GetUnionCases typeof<'a>
            |> Array.map(fun caseInfo -> 
                let displayName = FSharpValue.MakeUnion(caseInfo,[||]).ToString()
                option [Value displayName] [str displayName])
            |> Array.toList

        if includeEmptyOption then
            option [Value ""] [str ""] :: optionsFromUnion
            |> fragment[]
        else
            optionsFromUnion
            |> fragment[]