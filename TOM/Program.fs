// For more information see https://aka.ms/fsharp-console-apps
open Fable.Remoting.DotnetClient
type IGreetingApi = {
  greet : string -> Async<string>
}

type IGoodbyeApi = {
  goodbye : string -> Async<string>
}

let server =
    // Also note the base URI is no longer optional.
    Remoting.createApi "https:/localhost:8080" 
    |> Remoting.buildProxy<IGreetingApi>

let unitAsync = 
    async {
        let! greeting = server.greet "Alfred"
        printfn $"{greeting}"
    }