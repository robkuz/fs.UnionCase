namespace Robkuz.Util

module UnionCase =
    open System.Reflection
    open Microsoft.FSharp.Quotations
    open FSharp.Quotations.Evaluator

    open Microsoft.FSharp.Reflection

    let private flip f a b = f b a
    let private cast x = Expr.Cast x
    let private eval e = QuotationEvaluator.Evaluate e 

    let private makeUnionCaseTests<'a> () =
        let createFn c =  
            let t = typeof<'a>
            let x = Var("x", t)
            Expr.Lambda (x, Expr.IfThenElse (Expr.UnionCaseTest (Expr.Var(x), c), Expr.Value (true), Expr.Value (false)))
        let transform = createFn >> cast >> (fun x -> x :> Expr<'a -> bool>) >> eval
        let cases = FSharpType.GetUnionCases typeof<'a>
        cases
        |> Array.toList
        |> List.map transform

    let makeUnionCaseTest2<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b] -> (a, b)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let makeUnionCaseTest3<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b; c] -> (a, b, c)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let makeUnionCaseTest4<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b; c; d] -> (a, b, c, d)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let makeUnionCaseTest5<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b; c; d; e] -> (a, b, c, d, e)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let makeUnionCaseTest6<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b; c; d; e; f] -> (a, b, c, d, e, f)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let makeUnionCaseTest7<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b; c; d; e; f; g] -> (a, b, c, d, e, f, g)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let makeUnionCaseTest8<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b; c; d; e; f; g; h] -> (a, b, c, d, e, f, g, h)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let makeUnionCaseTest9<'a> () =
        match makeUnionCaseTests<'a>() with
        | [a; b; c; d; e; f; g; h; i] -> (a, b, c, d, e, f, g, h, i)
        | tests -> failwith <| sprintf "please use makeUnionCaseTest%A for the DiscriminateUnion %A" (List.length tests) typeof<'a>

    let unionCaseInfoByName<'a> name = 
        match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = name) with
        |[|case|] -> Some case
        |_ -> None

    let createUnionType<'a> name value =
        let (|>>=) = flip Option.bind
        unionCaseInfoByName<'a> name |>>= fun x -> Some(FSharpValue.MakeUnion(x, value) :?> 'a )