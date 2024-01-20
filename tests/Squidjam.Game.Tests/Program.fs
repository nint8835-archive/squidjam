// Load-bearing unused entrypoint.
// An F# console project without an entrypoint will not initialize module values, leading shared test values to not be
// initialized.
// Source: https://stackoverflow.com/questions/31914381/f-nunit-test-sets-value-as-null

module Program =
    [<EntryPoint>]
    let main _ = 0
