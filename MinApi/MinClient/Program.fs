module App

open Elmish
open Elmish.React
open Feliz
open SharedModels
open Fable.Remoting.Client

let eventStore : IEventStore =
  Remoting.createApi() 
  |> Remoting.withBaseUrl "https://glowing-space-waffle-4vwwwpr94pqh764-8085.app.github.dev/"
  |> Remoting.buildProxy<IEventStore>

type State =
    {
      Cstate : SharedModels.State
    }

type Msg =
    | Submit of Event
    | Update of SharedModels.State

let init() =
    let loadCountCmd = Cmd.OfAsync.perform (fun _ -> eventStore.getState) () (fun e -> Update e)
    { Cstate = Error }, loadCountCmd

let update (msg: Msg) (state: State): State*Cmd<Msg> =
    match msg with
    | Update s -> {state with Cstate=s},Cmd.none
    | Submit e -> 
      {state with Cstate = Present}, Cmd.OfAsync.either
        eventStore.submitEvent
        e
        (fun s -> Update s)
        (fun _ -> Update Error)

let render (state: State) (dispatch: Msg -> unit) =
  Html.div [
    Html.button [
      prop.onClick (fun _ -> dispatch (Submit Start))
      prop.text "Start"
    ]

    Html.button [
      prop.onClick (fun _ -> dispatch (Submit Stop))
      prop.text "Stop"
    ]
    let statestring (x:SharedModels.State) = match x with |Present -> "Present "  | Absent -> "Absent" | Error -> "Error"
    let xx = (statestring state.Cstate)
    Html.h1 xx
  ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.run