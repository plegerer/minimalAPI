module Server

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Giraffe
open SharedModels
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

let eventStore : IEventStore = {
    submitEvent = fun event -> async{return Present}
    getEvent = async{return Start}
    getState = async{return Absent}
}

let webApp=
    Remoting.createApi() 
    |> Remoting.fromValue eventStore
    |> Remoting.buildHttpHandler
let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe()|>ignore
let app = builder.Build()
app.UseGiraffe webApp

app.Run("http://localhost:8076")
