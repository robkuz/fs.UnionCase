#UnionCase-Library
A library of small helper functions to work with Discriminate Unions without the boilerplate code yet still with native speed.

##Available Functions

###makeUnionCaseTest2<'a> to makeUnionCaseTest9<'a>
create test functions for a each case of the given Discriminate Union

###unionCaseInfoByName<'a>
Get the UnionCaseInfo by name.

###createUnionType<'a>
Create a UnionCase by name and value(s)

##Example Usage
###makeUnionCaseTest2<'a> to makeUnionCaseTest9<'a>
These are by far the most interesting functions as they take away a lot of boilerplate code when working with DUs and testing these. Usually you would write code like this

    type SomeDU3<'a,'b, 'c> = 
    | Three of 'a
    | Four of 'a * 'b
    | Five of 'a * 'b * 'c

    let isThree x =
        match x with
        | Three _ -> true
        | _ 	  -> false
        
    let isFour x =
        match x with
        | Four _  -> true
        | _ 	  -> false
	
    let isFive x =
        match x with
        | Five _  -> true
        | _ 	  -> false

and then write code like this

    let processSomeDU3s xs:SomeDU3<int,string,bool> list =
        xs
        |> List.filter isFour
        |> List.map (isFour >> not) // yeah its a somehow made up example
        
Now you simply call the appropriate `makeUnionCaseTest` function. That is the function name should match the number of union cases you have in that Discriminate Union and destruct the returned tuple into the proper names.

    let isThree, isFour, isFive = makeUnionCaseTest3<SomeDU3<_, _, _>>()
    
After you have done this you can easily use those functions as seen below.

    Three 1             |> isThree  |> Assert.IsTrue
    Four(1, "1")        |> isFour   |> Assert.IsTrue
    Five(1,"2", false)  |> isFive   |> Assert.IsTrue

    Three 1             |> isFour   |> Assert.IsFalse
    Four(1, "1")        |> isFive   |> Assert.IsFalse
    Five(1,"2", false)  |> isThree  |> Assert.IsFalse
        

###unionCaseInfoByName<'a>

    type SomeDU3<'a,'b, 'c> = 
        | Three of 'a
        | Four of 'a * 'b
        | Five of 'a * 'b * 'c

	let uci = unionCaseInfoByName<SomeDU3<_,_,_>> "Five"
    uci.Value.Name = "Five" |> Assert.IsTrue

Please note that you dont need fully parametrize the target type

###createUnionType<'a>

    type SomeDU3<'a,'b, 'c> = 
    | Three of 'a
    | Four of 'a * 'b
    | Five of 'a * 'b * 'c

    let uc = createUnionType<SomeDU3<int,string,bool>> "Four" [|box 1; box "foo"|] 
    uc.Value 
    |> fun x -> 
       match x with 
       | Four(x,y) -> x = 1 && y = "foo" 
       | _ -> false
    |> Assert.IsTrue

Please be aware, that the name is case sensitive and that all parameters need to be boxed. Again this type doesnt need to be parametrized.

##Installation
On MacOSX and Linux execute the following commands

    mono .packet/paket.exe install
    ./fake.sh
    
Sadly (I mean luckily ;-) ) I dont work on Windows so I cant help you on that.

##FAQ
### Why do you fail instead of returning an Option on the `makeUnionCaseTest*` functions?
The idea behind that is that you should instantiate those functions pretty close to the top of your program. Sort of closer to the compile time step instead of the runtime step. And as such I want your program to crash and burn if you do anything wrong instead of pretending that you have some test functions (wrapped in Options) that you can use later on.
### Type Inference 
If you are using parametrized DUs you might have to instantiate those functions multiple times. For each resolved combination of parametric DUs you work with. The F# inference algorithm is a bit to eager here and doesnt allow for this kind of code.

    let isThree, isFour, isFive = makeUnionCaseTest3<SomeDU3<_, _, _>>()
    
    Four(1, "1")        |> isFour   |> Assert.IsTrue  // this will work
    Four("1", 1)        |> isFour   |> Assert.IsTrue  // this wont
    
As soon as the type inferrer finds the first call site of `isFour` it will infere the type `isFour: SomeDU3<int,string,'c> -> bool'`. Therefore a construction of `Four<string,int,_>` will fail. Do the following to overcome this behaviour

    let isThreeA, isFourA, isFiveA = makeUnionCaseTest3<SomeDU3<int,string, bool>>()
    let isThreeB, isFourB, isFiveB = makeUnionCaseTest3<SomeDU3<string, int, bool>>()
    
    Four(1, "1")        |> isFourA   |> Assert.IsTrue  // this will work
    Four("1", 1)        |> isFourB   |> Assert.IsTrue  // this wont

### Need for Speed!
This certainly slow because you are using reflection or runtime evaluation!  
Nope! The functions are instantiated only once and the compiled (during runtime).
The below code does all the magic including the compilation during runtime and should be as fast as native code.

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

Obviously if you recreate the function over and over again you will be hit with the runtime penalty of compiling those functions.

### What about a NuGet package?
I need to learn on how to create one. The I will build one. For now its Github

### License
MIT

### Famous last words
Go in peace and prosper!