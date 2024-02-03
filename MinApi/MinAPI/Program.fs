module Server

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore
open Giraffe
open SharedModels
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.Extensions.DependencyInjection

let eventStore : IEventStore = {
    submitEvent = fun event -> 
            match event with |Start -> async{return Present} |Stop -> async{return Absent}
    getEvent = async{return Start}
    getState = async{return Absent}
}

let webApp=
    Remoting.createApi() 
    |> Remoting.fromValue eventStore
    |> Remoting.buildHttpHandler

let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe().AddCors()|>ignore

let app = builder.Build()
app.UseCors(fun (b: Cors.Infrastructure.CorsPolicyBuilder) -> 
                                        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()|>ignore
                                        ) |>ignore
app.UseGiraffe webApp
app.Run("http://localhost:8085")
