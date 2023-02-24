import SampleToken from 0xf8d6e0586b0a20c7

// Print the balance of Tokens owned by account.
pub fun main(account: Address): UFix64 {
    // Get the public account object
    let owner = getAccount(account)

    // Find the public Receiver capability for their Collection
    let capability = owner.getCapability<&SampleToken.Vault{SampleToken.Receiver, SampleToken.Balance}>(SampleToken.VaultPublicPath)

    // borrow a reference from the capability
    let vaultRef = capability.borrow()
        ?? panic("Could not borrow the receiver reference")

    return vaultRef.balance
}
