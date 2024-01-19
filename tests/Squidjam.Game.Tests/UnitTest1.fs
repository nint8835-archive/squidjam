module Squidjam.Game.Tests

open NUnit.Framework
open Squidjam.Game

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ``Example Test`` () =
    Assert.AreEqual(Say.hello "test", "Hello test")