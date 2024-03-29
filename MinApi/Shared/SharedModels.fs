module SharedModels

open System

type Event = 
    |Start
    |Stop

type State =
    |Present
    |Absent
    |Error

type IEventStore = {
    submitEvent : Event -> Async<State>
    getEvent : Async<Event>
    getState : Async<State>
}
