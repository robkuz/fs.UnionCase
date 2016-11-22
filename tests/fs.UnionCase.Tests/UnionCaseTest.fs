namespace Test.Robkuz.Util

module UnionCase =
    open NUnit.Framework
    open Robkuz.Util.UnionCase

    type SomeDU1 = 
        | Zero 
        | Negative of string 

    type SomeDU2<'a> = 
        | One of 'a
        | Two of 'a * int

    type SomeDU3<'a,'b, 'c> = 
        | Three of 'a
        | Four of 'a * 'b
        | Five of 'a * 'b * 'c

    [<Test>]
    let ``fails if union case count != used fun count``() =
        try
            let a, b, c = makeUnionCaseTest3<SomeDU1>()
            Assert.Fail "Shouldnt be here"
        with
        | _ -> Assert.Pass()


    [<Test>]
    let ``check simple DU``() =
        let isZero, isNegative = makeUnionCaseTest2<SomeDU1>()
        Zero            |> isZero       |> Assert.IsTrue
        Negative "1"    |> isNegative   |> Assert.IsTrue
        Negative "1"    |> isZero       |> Assert.IsFalse


    [<Test>]
    let ``check parametrized DU<_,_>``() =
        let isOne, isTwo = makeUnionCaseTest2<SomeDU2<_>>()
        One 1       |> isOne |> Assert.IsTrue
        Two (1,2)   |> isTwo |> Assert.IsTrue
        One 1       |> isTwo |> Assert.IsFalse

    [<Test>]
    let ``check parametrized DU<int,string,bool>``() =
        let isThree, isFour, isFive = makeUnionCaseTest3<SomeDU3<_, _, _>>()
        Three 1             |> isThree  |> Assert.IsTrue
        Four(1, "1")        |> isFour   |> Assert.IsTrue
        Five(1,"2", false)  |> isFive   |> Assert.IsTrue

        Three 1             |> isFour   |> Assert.IsFalse
        Four(1, "1")        |> isFive   |> Assert.IsFalse
        Five(1,"2", false)  |> isThree  |> Assert.IsFalse

        //wont compile
        //Five(1,"2", "false")|> isThree  |> Assert.IsFalse

    [<Test>]
    let ``get UnionCase Name``() =
        let uci = unionCaseInfoByName<SomeDU3<_,_,_>> "Five"
        uci.Value.Name = "Five" |> Assert.IsTrue

    [<Test>]
    let ``create a UnionCase Value``() =
        let uc = createUnionType<SomeDU3<_,_,_>> "Four" [|box 1; box "foo"|] 
        uc.Value 
        |> fun x -> 
            match x with 
            | Four(x,y) -> x = 1 && y = "foo" 
            | _ -> false
        |> Assert.IsTrue

    open FSharp.Quotations.Evaluator
    let trx a = (a, "foo")
    let expra () =
        let e = <@ fun x -> trx x @>   
        QuotationEvaluator.Evaluate <@ fun x -> trx x @>

    let z = expra() 1
    let y = expra() "s"
