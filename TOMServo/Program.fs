// For more information see https://aka.ms/fsharp-console-apps
open Suave
open Fable.Remoting.Server
open Fable.Remoting.Suave

type IGreetingApi = {
  greet : string -> Async<string>
}

type IGoodbyeApi = {
  goodbye : string -> Async<string>
}

// Create the WebPart from the musicStore value
let greetingApi = {
  greet = fun name ->
    async {
      let greeting = sprintf "Hello, %s" name
      return greeting
    }
}

let goodbyeApi = {
  goodbye = fun name ->
    async {
      let greeting = sprintf "Hello, %s" name
      return greeting
    }
}

let webApp1:WebPart =
  Remoting.createApi()
    |> Remoting.fromValue greetingApi
    |> Remoting.buildWebPart

let webApp2:WebPart =
  Remoting.createApi()
    |> Remoting.fromValue greetingApi
    |> Remoting.buildWebPart
let webApps = choose([webApp1; webApp2])

// Expose the implementation as a HTTP service
let webApp:WebPart =
  Remoting.createApi()
    |> Remoting.fromValue greetingApi
    |> Remoting.buildWebPart


startWebServer defaultConfig webApp