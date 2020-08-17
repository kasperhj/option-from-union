module App

open Microsoft.FSharp.Reflection

open Fable.React
open Fable.React.Props
open OptionsFromUnion

type AnimalSelection =
    | Rat
    | Ape
    | Platypus
    
    with
    override this.ToString() =
        match this with
        | Rat      -> "A rat"
        | Ape      -> "An ape"
        | Platypus -> "A platypus"

type FruitSelection =
    | Banana
    | Kiwi
    | Mango
    | Apple
    | DragonFruit
    | Pear

    with
    override this.ToString() =
        match FSharpValue.GetUnionFields(this, typeof<FruitSelection>) with
        | case, _ -> case.Name.ToLower() + "s"

type Model = 
    { Animal : AnimalSelection option
      Fruit  : FruitSelection }

type Msg = 
    | FruitSelected of string
    | AnimalSelected of string
    | ResetSelections

let initialAnimal = Ape
let initialFruit = Mango

let init () : Model = 
    { Animal = Some initialAnimal
      Fruit = initialFruit }

let inline tryCreateCase<'a> (s:string) =
    FSharpType.GetUnionCases typeof<'a>
    |> Array.map(fun caseInfo -> FSharpValue.MakeUnion(caseInfo,[||]) :?> 'a)
    |> Array.tryFind(fun case -> case.ToString() = s)

let update msg model = 
    match msg with
    | AnimalSelected animalString -> 
        { model with Animal = animalString |> tryCreateCase<AnimalSelection>  }
    | FruitSelected fruitString ->
        { model with Fruit = (fruitString |> tryCreateCase<FruitSelection>).Value }
    | ResetSelections ->
        { model with Animal = Some initialAnimal; Fruit = initialFruit }

let formatSelectedAnimalText =
    (Option.map (fun (animal : AnimalSelection) -> animal.ToString()) >> Option.defaultValue "Unknown")

let view model dispatch = 
    div []
        [
            h1 [] [ ((model.Animal |> formatSelectedAnimalText) + " eats " + model.Fruit.ToString()) |> str ]

            select [
                if model.Animal.IsSome then
                    Value (model.Animal.Value.ToString())
                OnChange(fun ev -> (AnimalSelected ev.Value) |> dispatch) ]
                [
                    OptionsFromUnion.createOptions<AnimalSelection> true
                ]
            select [
                Value (model.Fruit.ToString())
                OnChange(fun ev -> (FruitSelected ev.Value) |> dispatch) ]
                [
                    OptionsFromUnion.createOptions<FruitSelection> false
                ]
            div [] []
            button [ OnClick(fun _ -> ResetSelections |> dispatch) ] [ str "Reset selections" ]
        ]


open Elmish
open Elmish.React

Program.mkSimple init update view
|> Program.withReactSynchronous "root"
|> Program.run